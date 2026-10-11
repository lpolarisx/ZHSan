using System;
using System.Threading.Tasks;
using GameManager;
using GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Platforms;

namespace WorldOfTheThreeKingdoms.GameScreens.ScreenLayers
{
    /// <summary>
    /// 势力边界：格子按 TileData.OwnerArchitecture 归属建筑，相邻格子所属势力不同处画边界线。
    /// 边界以“两个相邻格子之间的那条格边”为单位绘制：每条格边对应一小段曲线，
    /// 曲线两端的走向由格边两头的格子归属决定（直行、拐弯），拐弯处是弧线，连续的阶梯则连成斜直线。
    /// 三个以上势力交汇处、以及对角相接处不做平滑，边界直接交于格角，避免留下无主的空隙。
    /// 两势力相邻处只画较强一方（Faction.Power）的线。
    /// 边界线每帧只对屏幕内的格子现算，所以建筑易主、势力灭亡后无需刷新缓存。
    /// 领地另铺一层均匀的半透明势力色，小地图也用同一张贴图画色块。
    /// </summary>
    public class FactionBorderLayer
    {
        private const float Depth = 0.86f;
        private const float TerritoryDepth = 0.87f;

        // 实线宽度相对格子宽度的比例
        private const float LineWidthRate = 3f / 45f;
        private const float LineAlpha = 0.8f;

        // 领地底色的不透明度
        private const float TerritoryAlpha = 0.25f;

        // 实线是否画成虚线：每条格边上画DashCount段，DashRate是每段中实的部分所占比例
        private const bool Dashed = true;
        private const int DashCount = 2;
        private const float DashRate = 0.6f;

        // 图集：4种朝向 x 两端各4种走向 = 64个图块，按8x8排布，图块之间留透明间隔防止采样串色
        private const int OrientationCount = 4;
        private const int EndKindCount = 4;
        private const int AtlasColumns = 8;
        private const int AtlasPadding = 1;

        // 一段边界在某一端的走向
        private enum EndKind
        {
            // 沿格边方向直行到格角
            Straight = 0,
            // 朝本势力一侧拐弯
            TurnIn = 1,
            // 朝对方一侧拐弯
            TurnOut = 2,
            // 交汇处：直行到格角后在格角处直角转向本势力一侧
            JunctionTurn = 3
        }

        // 朝向：格边是横是竖，以及本势力在格边的哪一侧
        private const int HorizontalInAbove = 0;
        private const int HorizontalInBelow = 1;
        private const int VerticalInLeft = 2;
        private const int VerticalInRight = 3;

        private Texture2D atlas;
        private int atlasTileWidth;
        private int atlasTileHeight;

        // 后台正在生成的图集像素，以及它对应的格子大小
        private Task<Color[]> pendingAtlas;
        private int pendingTileWidth;
        private int pendingTileHeight;

        // 领地底色贴图：每个地图格子一个像素（不透明的势力色），绘制时拉伸到目标区域
        private Texture2D territory;
        private TileData[,] territoryTiles;
        private int territorySignature;

        public void Draw()
        {
            var screen = Session.MainGame.mainGameScreen;
            if (!screen.ShowFactionBorder) return;

            var tiles = Session.Current.Scenario.MapTileData;
            if (tiles == null) return;

            var mapLayer = screen.mainMapLayer;
            this.DrawTerritory(new Rectangle(mapLayer.LeftEdge, mapLayer.TopEdge, mapLayer.TotalTileWidth, mapLayer.TotalTileHeight), TerritoryAlpha, TerritoryDepth);

            int tileWidth = mapLayer.TileWidth;
            int tileHeight = mapLayer.TileHeight;
            this.PrepareAtlas(tileWidth, tileHeight);

            // 每个格子检查它右边和下边的格边，从-1开始以包含地图左、上边缘
            int left = Math.Max(screen.TopLeftPosition.X - 1, -1);
            int top = Math.Max(screen.TopLeftPosition.Y - 1, -1);
            int right = Math.Min(screen.BottomRightPosition.X, tiles.GetLength(0) - 1);
            int bottom = Math.Min(screen.BottomRightPosition.Y, tiles.GetLength(1) - 1);

            for (int x = left; x <= right; x++)
            {
                for (int y = top; y <= bottom; y++)
                {
                    Faction current = GetFaction(tiles, x, y);

                    Faction rightFaction = GetFaction(tiles, x + 1, y);
                    if (current != rightFaction)
                    {
                        // 图块以格边中点为中心
                        var destination = new Rectangle(mapLayer.LeftEdge + (x + 1) * tileWidth - tileWidth / 2, mapLayer.TopEdge + y * tileHeight, tileWidth, tileHeight);
                        if (DrawsBorder(current, rightFaction)) this.DrawPiece(tiles, VerticalInLeft, current, rightFaction, x, y, x + 1, y, destination);
                        else this.DrawPiece(tiles, VerticalInRight, rightFaction, current, x + 1, y, x, y, destination);
                    }

                    Faction belowFaction = GetFaction(tiles, x, y + 1);
                    if (current != belowFaction)
                    {
                        var destination = new Rectangle(mapLayer.LeftEdge + x * tileWidth, mapLayer.TopEdge + (y + 1) * tileHeight - tileHeight / 2, tileWidth, tileHeight);
                        if (DrawsBorder(current, belowFaction)) this.DrawPiece(tiles, HorizontalInAbove, current, belowFaction, x, y, x, y + 1, destination);
                        else this.DrawPiece(tiles, HorizontalInBelow, belowFaction, current, x, y + 1, x, y, destination);
                    }
                }
            }
        }

        // 把各势力的领地色块画到destination（整张地图对应的屏幕区域），主地图和小地图共用
        public void DrawTerritory(Rectangle destination, float alpha, float depth)
        {
            var tiles = Session.Current.Scenario.MapTileData;
            if (tiles == null) return;

            // 建筑的归属势力或势力颜色变了才重新生成贴图
            var signature = new HashCode();
            foreach (var architecture in Session.Current.Scenario.Architectures.Values)
            {
                signature.Add(architecture.ID);
                signature.Add(architecture.BelongedFaction?.ID ?? -1);
                signature.Add(architecture.BelongedFaction?.FactionColor.PackedValue ?? 0u);
            }
            int currentSignature = signature.ToHashCode();

            if (this.territory == null || this.territory.IsDisposed || this.territoryTiles != tiles || this.territorySignature != currentSignature)
            {
                int width = tiles.GetLength(0);
                int height = tiles.GetLength(1);
                var data = new Color[width * height];
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        Faction faction = tiles[x, y].OwnerArchitecture?.BelongedFaction;
                        if (faction != null)
                        {
                            data[y * width + x] = faction.FactionColor;
                        }
                    }
                }

                if (this.territory == null || this.territory.IsDisposed || this.territory.Width != width || this.territory.Height != height)
                {
                    this.territory?.Dispose();
                    this.territory = new Texture2D(Platform.GraphicsDevice, width, height);
                }
                this.territory.SetData(data);
                this.territoryTiles = tiles;
                this.territorySignature = currentSignature;
            }

            if (CacheManager.Scale != Vector2.One)
            {
                destination = new Rectangle((int)(destination.X * CacheManager.Scale.X), (int)(destination.Y * CacheManager.Scale.Y), (int)(destination.Width * CacheManager.Scale.X), (int)(destination.Height * CacheManager.Scale.Y));
            }
            Session.Current.SpriteBatch.Draw(this.territory, destination, null, Color.White * alpha, 0f, Vector2.Zero, SpriteEffects.None, depth);
        }

        // 两个不同势力（或一方无势力）相邻时，边界线只由较强的一方画，对面是空城、无主地时由有势力的一方画
        private static bool DrawsBorder(Faction faction, Faction other)
        {
            if (faction == null) return false;
            if (other == null) return true;

            int power = faction.Power;
            int otherPower = other.Power;
            return power != otherPower ? power > otherPower : faction.ID < other.ID;
        }

        // 画势力faction在一条格边上的那段边界，(inX, inY)是格边本势力一侧的格子，(outX, outY)是另一侧的格子
        private void DrawPiece(TileData[,] tiles, int orientation, Faction faction, Faction outFaction, int inX, int inY, int outX, int outY, Rectangle destination)
        {
            // 沿格边方向前后各看一格
            int stepX = orientation < VerticalInLeft ? 1 : 0;
            int stepY = 1 - stepX;
            EndKind negative = GetEndKind(faction, outFaction, GetFaction(tiles, inX - stepX, inY - stepY), GetFaction(tiles, outX - stepX, outY - stepY));
            EndKind positive = GetEndKind(faction, outFaction, GetFaction(tiles, inX + stepX, inY + stepY), GetFaction(tiles, outX + stepX, outY + stepY));

            if (CacheManager.Scale != Vector2.One)
            {
                destination = new Rectangle(Convert.ToInt16(destination.X * CacheManager.Scale.X), Convert.ToInt16(destination.Y * CacheManager.Scale.Y), Convert.ToInt16(destination.Width * CacheManager.Scale.X), Convert.ToInt16(destination.Height * CacheManager.Scale.Y));
            }
            Session.Current.SpriteBatch.Draw(this.atlas, destination, GetAtlasSource(this.atlasTileWidth, this.atlasTileHeight, orientation, negative, positive), faction.FactionColor, 0f, Vector2.Zero, SpriteEffects.None, Depth);
        }

        // farIn、farOut是格边某一头再过去的两个格子（分别挨着本势力一侧和对方一侧的格子）所属的势力
        private static EndKind GetEndKind(Faction faction, Faction outFaction, Faction farIn, Faction farOut)
        {
            if (farIn == faction)
            {
                return farOut == faction ? EndKind.TurnOut : EndKind.Straight;
            }
            // 只有这四格恰好分属两方、且不是对角相接时才平滑拐弯
            return farOut != faction && farIn == outFaction && farOut == outFaction ? EndKind.TurnIn : EndKind.JunctionTurn;
        }

        private static Faction GetFaction(TileData[,] tiles, int x, int y)
        {
            if (x < 0 || y < 0 || x >= tiles.GetLength(0) || y >= tiles.GetLength(1)) return null;

            return tiles[x, y].OwnerArchitecture?.BelongedFaction;
        }

        private static Rectangle GetAtlasSource(int tileWidth, int tileHeight, int orientation, EndKind negative, EndKind positive)
        {
            int index = (orientation * EndKindCount + (int)negative) * EndKindCount + (int)positive;
            return new Rectangle(
                (index % AtlasColumns) * (tileWidth + AtlasPadding * 2) + AtlasPadding,
                (index / AtlasColumns) * (tileHeight + AtlasPadding * 2) + AtlasPadding,
                tileWidth, tileHeight);
        }

        private static Point GetAtlasSize(int tileWidth, int tileHeight)
        {
            return new Point(
                (tileWidth + AtlasPadding * 2) * AtlasColumns,
                (tileHeight + AtlasPadding * 2) * (OrientationCount * EndKindCount * EndKindCount / AtlasColumns));
        }

        // 图块按格子大小1:1生成，这样线宽始终是清晰的像素宽度。
        // 缩放地图后新图集在后台线程生成，生成好之前先把旧图集拉伸着用，避免缩放时卡顿
        private void PrepareAtlas(int tileWidth, int tileHeight)
        {
            if (this.atlas != null && !this.atlas.IsDisposed && this.atlasTileWidth == tileWidth && this.atlasTileHeight == tileHeight) return;

            if (this.atlas == null || this.atlas.IsDisposed)
            {
                this.SetAtlas(GenerateAtlasData(tileWidth, tileHeight), tileWidth, tileHeight);
                return;
            }

            if (this.pendingAtlas != null && this.pendingAtlas.IsCompleted)
            {
                Task<Color[]> finished = this.pendingAtlas;
                this.pendingAtlas = null;
                if (finished.IsCompletedSuccessfully && this.pendingTileWidth == tileWidth && this.pendingTileHeight == tileHeight)
                {
                    this.SetAtlas(finished.Result, tileWidth, tileHeight);
                    return;
                }
            }

            if (this.pendingAtlas == null)
            {
                this.pendingTileWidth = tileWidth;
                this.pendingTileHeight = tileHeight;
                this.pendingAtlas = Task.Run(() => GenerateAtlasData(tileWidth, tileHeight));
            }
        }

        private void SetAtlas(Color[] data, int tileWidth, int tileHeight)
        {
            this.atlas?.Dispose();

            Point size = GetAtlasSize(tileWidth, tileHeight);
            this.atlas = new Texture2D(Platform.GraphicsDevice, size.X, size.Y);
            this.atlas.SetData(data);
            this.atlasTileWidth = tileWidth;
            this.atlasTileHeight = tileHeight;
        }

        private static Color[] GenerateAtlasData(int tileWidth, int tileHeight)
        {
            Point atlasSize = GetAtlasSize(tileWidth, tileHeight);
            int atlasWidth = atlasSize.X;
            var data = new Color[atlasWidth * atlasSize.Y];

            float lineWidth = Math.Max(1f, tileWidth * LineWidthRate);
            int centerX = tileWidth / 2;
            int centerY = tileHeight / 2;

            for (int orientation = 0; orientation < OrientationCount; orientation++)
            {
                // 换算到统一的坐标：along沿格边方向，perp垂直格边且本势力一侧为负，原点在格边中点。
                // alongNegative/alongPositive是中点到格边两头的距离，perpIn/perpOut是格边到两侧格子中心的距离
                bool horizontal = orientation < VerticalInLeft;
                bool inOnNegativeSide = orientation == HorizontalInAbove || orientation == VerticalInLeft;
                int alongSize = horizontal ? tileWidth : tileHeight;
                int alongCenter = horizontal ? centerX : centerY;
                int perpSize = horizontal ? tileHeight : tileWidth;
                int perpCenter = horizontal ? centerY : centerX;
                float alongNegative = alongCenter;
                float alongPositive = alongSize - alongCenter;
                float perpIn = inOnNegativeSide ? perpSize - perpCenter : perpCenter;
                float perpOut = perpSize - perpIn;

                for (int negative = 0; negative < EndKindCount; negative++)
                {
                    for (int positive = 0; positive < EndKindCount; positive++)
                    {
                        Vector2 start = GetEndPoint((EndKind)negative, -alongNegative, perpIn, perpOut);
                        Vector2 end = GetEndPoint((EndKind)positive, alongPositive, perpIn, perpOut);
                        Rectangle source = GetAtlasSource(tileWidth, tileHeight, orientation, (EndKind)negative, (EndKind)positive);

                        for (int px = 0; px < tileWidth; px++)
                        {
                            for (int py = 0; py < tileHeight; py++)
                            {
                                float along = (horizontal ? px - centerX : py - centerY) + 0.5f;
                                float perp = (horizontal ? py - centerY : px - centerX) + 0.5f;
                                if (!inOnNegativeSide) perp = -perp;

                                float alpha = GetAlpha(along, perp, start, end, (EndKind)negative, (EndKind)positive, horizontal, lineWidth);
                                if (alpha > 0f)
                                {
                                    data[(source.Y + py) * atlasWidth + source.X + px] = new Color(alpha, alpha, alpha, alpha);
                                }
                            }
                        }
                    }
                }
            }

            return data;
        }

        // 一段边界在某一端的终点。相邻两条格边中点的连线构成折线，每段曲线取到折线线段的中点为止，
        // 直行时终点就是格角，拐弯时终点是斜线段的中点
        private static Vector2 GetEndPoint(EndKind kind, float along, float perpIn, float perpOut)
        {
            switch (kind)
            {
                case EndKind.TurnIn:
                    return new Vector2(along / 2f, -perpIn / 2f);
                case EndKind.TurnOut:
                    return new Vector2(along / 2f, perpOut / 2f);
                default:
                    return new Vector2(along, 0f);
            }
        }

        // 曲线是以格边中点（原点）为控制点、从start到end的二次贝塞尔曲线，和相邻各段在端点处相切
        private static float GetAlpha(float along, float perp, Vector2 start, Vector2 end, EndKind startKind, EndKind endKind, bool tieInclusive, float lineWidth)
        {
            // 每个像素只归一段曲线管，分界线是端点处的法线；恰好落在分界线上的像素归横向格边的那一段
            if (!InRange(along, perp, start, startKind, tieInclusive) || !InRange(along, perp, end, endKind, tieInclusive)) return 0f;

            // 先按along估算曲线参数t，离得远的像素直接跳过
            float sum = start.X + end.X;
            float t;
            if (Math.Abs(sum) < 0.001f)
            {
                t = (along - start.X) / (-2f * start.X);
            }
            else
            {
                t = (start.X + MathF.Sqrt(Math.Max(0f, start.X * start.X - sum * (start.X - along)))) / sum;
            }
            float curvePerp = (1f - t) * (1f - t) * start.Y + t * t * end.Y;
            if (Math.Abs(curvePerp - perp) > (lineWidth + 2f) * 1.5f) return 0f;

            // 牛顿迭代求曲线上的最近点
            Vector2 point = new Vector2(along, perp);
            Vector2 second = 2f * (start + end);
            Vector2 position = Vector2.Zero;
            Vector2 tangent = Vector2.UnitX;
            for (int i = 0; i < 4; i++)
            {
                position = (1f - t) * (1f - t) * start + t * t * end;
                tangent = -2f * (1f - t) * start + 2f * t * end;
                if (i == 3) break;

                Vector2 offset = position - point;
                float derivative = Vector2.Dot(tangent, tangent) + Vector2.Dot(offset, second);
                if (derivative < 0.0001f) break;

                t -= Vector2.Dot(offset, tangent) / derivative;
            }

            // 到曲线的距离，本势力一侧为正
            float distance = (tangent.Y * (along - position.X) - tangent.X * (perp - position.Y)) / tangent.Length();

            float inside = Math.Clamp(distance + 0.5f, 0f, 1f);
            if (inside <= 0f) return 0f;

            float line = inside * Math.Clamp(lineWidth - distance + 0.5f, 0f, 1f) * LineAlpha;
            if (Dashed)
            {
                // 按曲线参数t分段，空隙居于每段两头，所以相邻两条格边的虚线能接上
                float phase = t * DashCount;
                phase -= MathF.Floor(phase);
                float gap = (1f - DashRate) / 2f;
                float periodLength = tangent.Length() / DashCount;
                line *= Math.Clamp(Math.Min(phase - gap, 1f - gap - phase) * periodLength + 0.5f, 0f, 1f);
            }
            return line;
        }

        private static bool InRange(float along, float perp, Vector2 endPoint, EndKind kind, bool tieInclusive)
        {
            if (kind == EndKind.TurnIn || kind == EndKind.TurnOut)
            {
                // 端点处的切线方向就是原点指向端点的方向
                float side = (along - endPoint.X) * endPoint.X + (perp - endPoint.Y) * endPoint.Y;
                return side < 0f || (side == 0f && tieInclusive);
            }

            float gap = endPoint.X > 0f ? endPoint.X - along : along - endPoint.X;
            if (gap < 0f) return false;

            if (kind == EndKind.JunctionTurn)
            {
                // 格角处直角转弯，和转过去的那一段以角平分线为界
                return gap > -perp || (gap == -perp && tieInclusive);
            }
            return true;
        }
    }
}

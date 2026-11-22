using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameManager;
using GameObjects;
using GameObjects.MapDetail;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Platforms;

namespace WorldOfTheThreeKingdoms.GameScreens.ScreenLayers
{
    /// <summary>
    /// 地图边界显示模式
    /// </summary>
    public enum MapBorderMode
    {
        None = 0,
        Faction = 1,
        Province = 2,
        Commandery = 3
    }

    /// <summary>
    /// 势力边界与历史州郡边界绘制层：
    /// 边界以“两个相邻格子之间的那条格边”为单位绘制：每条格边对应一小段二次贝塞尔曲线，
    /// 曲线两端的走向由格边两头的格子归属决定（直行、拐弯），拐弯处是弧线，连续的阶梯则连成斜直线。
    /// 三个以上区域交汇处、以及对角相接处不做平滑，边界直接交于格角，避免留下无主的空隙。
    /// - 势力边界模式：按 TileData.OwnerArchitecture 归属势力绘制，两势力相邻处只画较强一方（Faction.Power）的虚线。
    /// - 历史州界/州郡界模式：按 ProvinceMap（来自可配置 JSON）绘制静态州界实线、州内郡界细虚线，并标注州名与郡名。
    /// 领地/州郡另铺一层半透明底色贴图，主地图与小地图共用。
    /// </summary>
    public class FactionBorderLayer
    {
        private const float Depth = 0.86f;
        private const float CommanderyDepth = 0.865f;
        private const float TerritoryDepth = 0.87f;
        // 州郡名称层级高于城池(0.8)、旗帜与城名(0.7999~0.79998)，低于选中框(0.72)与部队(0.7)
        private const float ProvinceLabelDepth = 0.74f;
        private const float CommanderyLabelDepth = 0.745f;

        // 领地与州郡底色的不透明度
        private const float TerritoryAlpha = 0.25f;
        private const float ProvinceTerritoryAlpha = 0.22f;
        private const float CommanderyTerritoryAlpha = 0.26f;

        // 朝向：格边是横是竖，以及本侧在格边的哪一边
        private const int HorizontalInAbove = 0;
        private const int HorizontalInBelow = 1;
        private const int VerticalInLeft = 2;
        private const int VerticalInRight = 3;

        // 一段边界在某一端的走向
        private enum EndKind
        {
            // 沿格边方向直行到格角
            Straight = 0,
            // 朝本侧拐弯
            TurnIn = 1,
            // 朝对方一侧拐弯
            TurnOut = 2,
            // 交汇处：直行到格角后在格角处直角转向本侧
            JunctionTurn = 3
        }

        // 三套不同样式的 8x8 曲线图集（按需懒加载）
        private readonly BorderAtlas factionAtlas = new(3f / 45f, 0.8f, dashed: true, dashCount: 2, dashRate: 0.6f);
        private readonly BorderAtlas provinceAtlas = new(3.0f / 45f, 0.94f, dashed: false, dashCount: 1, dashRate: 1f);
        private readonly BorderAtlas commanderyAtlas = new(2.4f / 45f, 0.88f, dashed: true, dashCount: 2, dashRate: 0.62f);

        // 势力领地底色贴图：每个地图格子一个像素，绘制时拉伸到目标区域
        private Texture2D territory;
        private TileData[,] territoryTiles;
        private int territorySignature;

        // 历史州郡底色贴图：按州纯色或按郡微调明暗
        private Texture2D provinceTerritory;
        private int provinceTerritoryVersion = -1;
        private bool provinceTerritoryWithCommanderyShade;

        // 避开城池/旗帜/城名后的州郡标注位置缓存
        private GameScenario labelCacheScenario;
        private int labelCacheVersion = -1;
        private bool labelCacheSimpleArch;
        private Dictionary<int, Vector2> adjustedProvinceLabels = new();
        private Dictionary<int, Vector2> adjustedCommanderyLabels = new();

        public void Draw()
        {
            var screen = Session.MainGame.mainGameScreen;
            MapBorderMode mode = screen.BorderMode;
            if (mode == MapBorderMode.None) return;

            var scenario = Session.Current.Scenario;
            var tiles = scenario?.MapTileData;
            if (tiles == null) return;

            var mapLayer = screen.mainMapLayer;
            var mapRect = new Rectangle(mapLayer.LeftEdge, mapLayer.TopEdge, mapLayer.TotalTileWidth, mapLayer.TotalTileHeight);

            if (mode == MapBorderMode.Faction)
            {
                this.DrawFactionTerritory(mapRect, TerritoryAlpha, TerritoryDepth);
                this.DrawFactionBorders(screen, mapLayer, tiles);
                return;
            }

            ProvinceMap.EnsureInitialized(scenario);
            if (ProvinceMap.Width <= 0 || ProvinceMap.Height <= 0) return;

            bool showCommandery = mode == MapBorderMode.Commandery;
            this.DrawProvinceTerritory(mapRect, showCommandery ? CommanderyTerritoryAlpha : ProvinceTerritoryAlpha, TerritoryDepth, showCommandery);
            this.DrawProvinceBorders(screen, mapLayer, showCommandery);
            this.DrawProvinceLabels(screen, mapLayer, showCommandery);
        }

        // 把当前模式对应的色块画到 destination（整张地图对应的屏幕区域），主地图和小地图共用
        public void DrawTerritory(Rectangle destination, float alpha, float depth)
        {
            var screen = Session.MainGame.mainGameScreen;
            if (screen.BorderMode == MapBorderMode.Province || screen.BorderMode == MapBorderMode.Commandery)
            {
                ProvinceMap.EnsureInitialized(Session.Current.Scenario);
                this.DrawProvinceTerritory(destination, alpha, depth, screen.BorderMode == MapBorderMode.Commandery);
                return;
            }

            this.DrawFactionTerritory(destination, alpha, depth);
        }

        // 在小地图上绘制各州名称（仅在州界/州郡界模式下显示）
        public void DrawMinimapProvinceLabels(Rectangle destination, float alpha, float depth)
        {
            var screen = Session.MainGame.mainGameScreen;
            if (screen.BorderMode != MapBorderMode.Province && screen.BorderMode != MapBorderMode.Commandery) return;
            if (CacheManager.FontPair.Size <= 0 || destination.Width <= 0 || destination.Height <= 0) return;

            ProvinceMap.EnsureInitialized(Session.Current.Scenario);
            if (ProvinceMap.Width <= 0 || ProvinceMap.Height <= 0) return;

            float fontPx = Math.Clamp(Math.Min(destination.Width, destination.Height) * 0.042f, 11f, 16f);
            float fontScale = fontPx / CacheManager.FontPair.Size;
            var textColor = new Color(255, 248, 224) * (0.95f * alpha);
            var outlineColor = new Color(12, 10, 8) * (0.9f * alpha);

            foreach (var prov in ProvinceMap.Provinces)
            {
                if (!prov.HasLabel || string.IsNullOrEmpty(prov.Name)) continue;

                float sx = destination.X + prov.LabelTile.X / ProvinceMap.Width * destination.Width;
                float sy = destination.Y + prov.LabelTile.Y / ProvinceMap.Height * destination.Height;

                DrawCenteredOutlineText(
                    prov.Name,
                    new Vector2(sx, sy),
                    fontScale,
                    textColor,
                    outlineColor,
                    1f,
                    depth,
                    0.000005f,
                    destination);
            }
        }

        private void DrawFactionBorders(MainGameScreen screen, MainMapLayer mapLayer, TileData[,] tiles)
        {
            int tileWidth = mapLayer.TileWidth;
            int tileHeight = mapLayer.TileHeight;
            this.factionAtlas.Prepare(tileWidth, tileHeight);

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
                        if (DrawsBorder(current, rightFaction)) this.DrawFactionPiece(tiles, VerticalInLeft, current, rightFaction, x, y, x + 1, y, destination);
                        else this.DrawFactionPiece(tiles, VerticalInRight, rightFaction, current, x + 1, y, x, y, destination);
                    }

                    Faction belowFaction = GetFaction(tiles, x, y + 1);
                    if (current != belowFaction)
                    {
                        var destination = new Rectangle(mapLayer.LeftEdge + x * tileWidth, mapLayer.TopEdge + (y + 1) * tileHeight - tileHeight / 2, tileWidth, tileHeight);
                        if (DrawsBorder(current, belowFaction)) this.DrawFactionPiece(tiles, HorizontalInAbove, current, belowFaction, x, y, x, y + 1, destination);
                        else this.DrawFactionPiece(tiles, HorizontalInBelow, belowFaction, current, x, y + 1, x, y, destination);
                    }
                }
            }
        }

        private void DrawProvinceBorders(MainGameScreen screen, MainMapLayer mapLayer, bool showCommandery)
        {
            int tileWidth = mapLayer.TileWidth;
            int tileHeight = mapLayer.TileHeight;
            this.provinceAtlas.Prepare(tileWidth, tileHeight);
            if (showCommandery)
            {
                this.commanderyAtlas.Prepare(tileWidth, tileHeight);
            }

            int left = Math.Max(screen.TopLeftPosition.X - 1, -1);
            int top = Math.Max(screen.TopLeftPosition.Y - 1, -1);
            int right = Math.Min(screen.BottomRightPosition.X, ProvinceMap.Width - 1);
            int bottom = Math.Min(screen.BottomRightPosition.Y, ProvinceMap.Height - 1);

            for (int x = left; x <= right; x++)
            {
                for (int y = top; y <= bottom; y++)
                {
                    var currentProv = ProvinceMap.GetProvince(x, y);
                    var rightProv = ProvinceMap.GetProvince(x + 1, y);

                    if (currentProv != rightProv)
                    {
                        var destination = new Rectangle(mapLayer.LeftEdge + (x + 1) * tileWidth - tileWidth / 2, mapLayer.TopEdge + y * tileHeight, tileWidth, tileHeight);
                        // 相邻两州各自向本州内侧画实线，贴合无缝、层次清晰
                        if (currentProv != null)
                        {
                            this.DrawProvincePiece(VerticalInLeft, currentProv, rightProv, x, y, x + 1, y, destination);
                        }
                        if (rightProv != null)
                        {
                            this.DrawProvincePiece(VerticalInRight, rightProv, currentProv, x + 1, y, x, y, destination);
                        }
                    }
                    else if (showCommandery && currentProv != null)
                    {
                        var currentCmd = ProvinceMap.GetCommandery(x, y);
                        var rightCmd = ProvinceMap.GetCommandery(x + 1, y);
                        if (currentCmd != rightCmd && currentCmd != null && rightCmd != null)
                        {
                            var destination = new Rectangle(mapLayer.LeftEdge + (x + 1) * tileWidth - tileWidth / 2, mapLayer.TopEdge + y * tileHeight, tileWidth, tileHeight);
                            if (currentCmd.ID < rightCmd.ID)
                                this.DrawCommanderyPiece(VerticalInLeft, currentCmd, rightCmd, x, y, x + 1, y, destination);
                            else
                                this.DrawCommanderyPiece(VerticalInRight, rightCmd, currentCmd, x + 1, y, x, y, destination);
                        }
                    }

                    var belowProv = ProvinceMap.GetProvince(x, y + 1);
                    if (currentProv != belowProv)
                    {
                        var destination = new Rectangle(mapLayer.LeftEdge + x * tileWidth, mapLayer.TopEdge + (y + 1) * tileHeight - tileHeight / 2, tileWidth, tileHeight);
                        if (currentProv != null)
                        {
                            this.DrawProvincePiece(HorizontalInAbove, currentProv, belowProv, x, y, x, y + 1, destination);
                        }
                        if (belowProv != null)
                        {
                            this.DrawProvincePiece(HorizontalInBelow, belowProv, currentProv, x, y + 1, x, y, destination);
                        }
                    }
                    else if (showCommandery && currentProv != null)
                    {
                        var currentCmd = ProvinceMap.GetCommandery(x, y);
                        var belowCmd = ProvinceMap.GetCommandery(x, y + 1);
                        if (currentCmd != belowCmd && currentCmd != null && belowCmd != null)
                        {
                            var destination = new Rectangle(mapLayer.LeftEdge + x * tileWidth, mapLayer.TopEdge + (y + 1) * tileHeight - tileHeight / 2, tileWidth, tileHeight);
                            if (currentCmd.ID < belowCmd.ID)
                                this.DrawCommanderyPiece(HorizontalInAbove, currentCmd, belowCmd, x, y, x, y + 1, destination);
                            else
                                this.DrawCommanderyPiece(HorizontalInBelow, belowCmd, currentCmd, x, y + 1, x, y, destination);
                        }
                    }
                }
            }
        }

        private void DrawProvinceLabels(MainGameScreen screen, MainMapLayer mapLayer, bool showCommandery)
        {
            if (CacheManager.FontPair.Size <= 0) return;

            var scenario = Session.Current.Scenario;
            this.EnsureAdjustedLabels(scenario);

            int tileWidth = mapLayer.TileWidth;
            int tileHeight = mapLayer.TileHeight;
            int viewWidth = screen.viewportSize.X;
            int viewHeight = screen.viewportSize.Y;

            // 先画郡名（较低层级、较小字号），再画州名（较高层级、较大字号）
            if (showCommandery)
            {
                float cmdPx = Math.Clamp(tileWidth * 0.34f, 13f, 22f);
                float cmdScale = cmdPx / CacheManager.FontPair.Size;
                var cmdTextColor = new Color(248, 245, 234) * 0.92f;
                var cmdOutlineColor = new Color(20, 16, 12) * 0.78f;

                foreach (var cmd in ProvinceMap.Commanderies)
                {
                    if (!cmd.HasLabel || string.IsNullOrEmpty(cmd.Name)) continue;

                    Vector2 pos = this.adjustedCommanderyLabels.TryGetValue(cmd.ID, out var adjusted) ? adjusted : cmd.LabelTile;
                    float sx = mapLayer.LeftEdge + pos.X * tileWidth;
                    float sy = mapLayer.TopEdge + pos.Y * tileHeight;
                    if (sx < -120 || sy < -60 || sx > viewWidth + 120 || sy > viewHeight + 60) continue;

                    DrawCenteredOutlineText(cmd.Name, new Vector2(sx, sy), cmdScale, cmdTextColor, cmdOutlineColor, 1.5f, CommanderyLabelDepth);
                }
            }

            float provPx = Math.Clamp(tileWidth * 0.62f, 22f, 44f);
            float provScale = provPx / CacheManager.FontPair.Size;
            var provTextColor = new Color(255, 246, 214) * 0.94f;
            var provOutlineColor = new Color(18, 12, 8) * 0.82f;

            foreach (var prov in ProvinceMap.Provinces)
            {
                if (!prov.HasLabel || string.IsNullOrEmpty(prov.Name)) continue;

                Vector2 pos = this.adjustedProvinceLabels.TryGetValue(prov.ID, out var adjusted) ? adjusted : prov.LabelTile;
                float sx = mapLayer.LeftEdge + pos.X * tileWidth;
                float sy = mapLayer.TopEdge + pos.Y * tileHeight;
                if (sx < -160 || sy < -80 || sx > viewWidth + 160 || sy > viewHeight + 80) continue;

                string label = prov.Name.Length == 2 ? $"{prov.Name[0]} {prov.Name[1]}" : prov.Name;
                DrawCenteredOutlineText(label, new Vector2(sx, sy), provScale, provTextColor, provOutlineColor, 2f, ProvinceLabelDepth);
            }
        }

        private readonly struct TileRect
        {
            public readonly float MinX;
            public readonly float MinY;
            public readonly float MaxX;
            public readonly float MaxY;

            public TileRect(float minX, float minY, float maxX, float maxY)
            {
                this.MinX = minX;
                this.MinY = minY;
                this.MaxX = maxX;
                this.MaxY = maxY;
            }

            public bool Overlaps(float x, float y, float halfW, float halfH)
            {
                return x + halfW > this.MinX && x - halfW < this.MaxX
                    && y + halfH > this.MinY && y - halfH < this.MaxY;
            }
        }

        private void EnsureAdjustedLabels(GameScenario scenario)
        {
            bool simpleArch = scenario?.ScenarioMap?.UseSimpleArchImages ?? false;
            if (ReferenceEquals(this.labelCacheScenario, scenario)
                && this.labelCacheVersion == ProvinceMap.Version
                && this.labelCacheSimpleArch == simpleArch)
            {
                return;
            }

            this.labelCacheScenario = scenario;
            this.labelCacheVersion = ProvinceMap.Version;
            this.labelCacheSimpleArch = simpleArch;
            this.adjustedProvinceLabels.Clear();
            this.adjustedCommanderyLabels.Clear();

            var archRects = BuildArchitectureVisualRects(scenario, simpleArch);

            foreach (var cmd in ProvinceMap.Commanderies)
            {
                if (!cmd.HasLabel || string.IsNullOrEmpty(cmd.Name)) continue;
                float halfW = Math.Clamp(cmd.Name.Length * 0.22f, 0.5f, 1.2f);
                float halfH = 0.28f;
                this.adjustedCommanderyLabels[cmd.ID] = AvoidArchitectures(
                    cmd.LabelTile,
                    halfW,
                    halfH,
                    archRects,
                    (tx, ty) => ReferenceEquals(ProvinceMap.GetCommandery(tx, ty), cmd));
            }

            foreach (var prov in ProvinceMap.Provinces)
            {
                if (!prov.HasLabel || string.IsNullOrEmpty(prov.Name)) continue;
                float halfW = Math.Clamp(prov.Name.Length * 0.42f, 0.8f, 1.8f);
                float halfH = 0.45f;
                this.adjustedProvinceLabels[prov.ID] = AvoidArchitectures(
                    prov.LabelTile,
                    halfW,
                    halfH,
                    archRects,
                    (tx, ty) => ReferenceEquals(ProvinceMap.GetProvince(tx, ty), prov));
            }
        }

        private static List<TileRect> BuildArchitectureVisualRects(GameScenario scenario, bool simpleArch)
        {
            var rects = new List<TileRect>();
            if (scenario?.Architectures == null) return rects;

            foreach (var arch in scenario.Architectures.Values)
            {
                Point center = arch.zhongxindian;
                int guimo = arch.JianzhuGuimo;
                int kindId = arch.Kind?.ID ?? 0;

                float minX = center.X;
                float minY = center.Y;
                float maxX = center.X + 1f;
                float maxY = center.Y + 1f;

                if (!simpleArch)
                {
                    if (kindId != 2 && kindId != 3)
                    {
                        if (guimo == 13)
                        {
                            minX = center.X - 2f;
                            minY = center.Y - 2f;
                            maxX = center.X + 3f;
                            maxY = center.Y + 3f;
                        }
                        else if ((guimo >= 4 && guimo <= 9) || (kindId == 1 && guimo == 1))
                        {
                            minX = center.X - 1f;
                            minY = center.Y - 1f;
                            maxX = center.X + 2f;
                            maxY = center.Y + 2f;
                        }
                    }
                    else if (kindId == 2 && guimo > 1 && arch.ArchitectureArea?.Area != null && arch.ArchitectureArea.Area.Count >= 2)
                    {
                        bool vertical = arch.ArchitectureArea.Area[0].X == arch.ArchitectureArea.Area[1].X;
                        if (guimo == 5)
                        {
                            minX = center.X - (vertical ? 2f : 4f);
                            minY = center.Y - (vertical ? 3f : 1f);
                            maxX = center.X + (vertical ? 3f : 5f);
                            maxY = center.Y + (vertical ? 4f : 2f);
                        }
                        else if (guimo == 3)
                        {
                            minX = center.X - 2f;
                            minY = center.Y - (vertical ? 2f : 1f);
                            maxX = center.X + 3f;
                            maxY = center.Y + (vertical ? 3f : 2f);
                        }
                    }
                }

                // 合并城名标题（位于 dingdian）和旗帜（位于 jianzhuqizi.qizipoint）占用的格域
                Point top = arch.dingdian;
                if (top.Y > 0)
                {
                    minX = Math.Min(minX, top.X - 1.5f);
                    maxX = Math.Max(maxX, top.X + 2.5f);
                    minY = Math.Min(minY, top.Y);
                    maxY = Math.Max(maxY, top.Y + 1f);

                    if (arch.jianzhuqizi != null)
                    {
                        Point flag = arch.jianzhuqizi.qizipoint;
                        minX = Math.Min(minX, flag.X);
                        maxX = Math.Max(maxX, flag.X + 1f);
                        minY = Math.Min(minY, flag.Y);
                        maxY = Math.Max(maxY, flag.Y + 1f);
                    }
                }

                rects.Add(new TileRect(minX, minY, maxX, maxY));
            }

            return rects;
        }

        private static Vector2 AvoidArchitectures(Vector2 origin, float halfW, float halfH, List<TileRect> archRects, Func<int, int, bool> inRegion)
        {
            TileRect? blocker = FindFirstOverlap(origin.X, origin.Y, halfW, halfH, archRects);
            if (!blocker.HasValue) return origin;

            var b = blocker.Value;
            const float margin = 0.25f;
            Span<Vector2> candidates = stackalloc Vector2[]
            {
                new Vector2(origin.X, b.MaxY + halfH + margin),
                new Vector2(origin.X, b.MinY - halfH - margin),
                new Vector2(b.MaxX + halfW + margin, origin.Y),
                new Vector2(b.MinX - halfW - margin, origin.Y),
                new Vector2(b.MaxX + halfW + margin, b.MaxY + halfH + margin),
                new Vector2(b.MinX - halfW - margin, b.MaxY + halfH + margin)
            };

            // 优先选在本州/本郡内且不压住任何城池/旗帜的候选位置
            for (int i = 0; i < candidates.Length; i++)
            {
                var c = candidates[i];
                if (inRegion((int)c.X, (int)c.Y) && !FindFirstOverlap(c.X, c.Y, halfW, halfH, archRects).HasValue)
                {
                    return c;
                }
            }

            // 次选不压住任何城池/旗帜的最近候选位置
            for (int i = 0; i < candidates.Length; i++)
            {
                var c = candidates[i];
                if (c.X >= 0 && c.Y >= 0 && c.X < ProvinceMap.Width && c.Y < ProvinceMap.Height
                    && !FindFirstOverlap(c.X, c.Y, halfW, halfH, archRects).HasValue)
                {
                    return c;
                }
            }

            return candidates[0];
        }

        private static TileRect? FindFirstOverlap(float x, float y, float halfW, float halfH, List<TileRect> archRects)
        {
            for (int i = 0; i < archRects.Count; i++)
            {
                if (archRects[i].Overlaps(x, y, halfW, halfH))
                {
                    return archRects[i];
                }
            }
            return null;
        }

        private static void DrawCenteredOutlineText(string text, Vector2 center, float fontScale, Color color, Color outlineColor, float outlineOffset, float depth, float outlineDepthDelta = 0.0005f, Rectangle? clampRect = null)
        {
            var bounds = CacheManager.CalculateTextBounds(null, text, Vector2.Zero, fontScale);
            float width = 0f;
            float height = 0f;
            if (bounds != null && bounds.Count > 0)
            {
                width = bounds.Max(b => b.X2) - bounds.Min(b => b.X);
                height = bounds.Max(b => b.Y2) - bounds.Min(b => b.Y);
            }

            float left = center.X - width * 0.5f;
            float top = center.Y - height * 0.5f;
            if (clampRect.HasValue)
            {
                var r = clampRect.Value;
                const float pad = 2f;
                float minLeft = r.Left + pad;
                float maxLeft = Math.Max(minLeft, r.Right - width - pad);
                float minTop = r.Top + pad;
                float maxTop = Math.Max(minTop, r.Bottom - height - pad);
                left = Math.Clamp(left, minLeft, maxLeft);
                top = Math.Clamp(top, minTop, maxTop);
            }

            var topLeft = new Vector2(left, top);
            float outlineDepth = depth + outlineDepthDelta;
            CacheManager.DrawString(null, text, new Vector2(topLeft.X - outlineOffset, topLeft.Y), outlineColor, 0f, Vector2.Zero, fontScale, SpriteEffects.None, outlineDepth);
            CacheManager.DrawString(null, text, new Vector2(topLeft.X + outlineOffset, topLeft.Y), outlineColor, 0f, Vector2.Zero, fontScale, SpriteEffects.None, outlineDepth);
            CacheManager.DrawString(null, text, new Vector2(topLeft.X, topLeft.Y - outlineOffset), outlineColor, 0f, Vector2.Zero, fontScale, SpriteEffects.None, outlineDepth);
            CacheManager.DrawString(null, text, new Vector2(topLeft.X, topLeft.Y + outlineOffset), outlineColor, 0f, Vector2.Zero, fontScale, SpriteEffects.None, outlineDepth);
            CacheManager.DrawString(null, text, topLeft, color, 0f, Vector2.Zero, fontScale, SpriteEffects.None, depth);
        }

        private void DrawFactionTerritory(Rectangle destination, float alpha, float depth)
        {
            var tiles = Session.Current.Scenario.MapTileData;
            if (tiles == null) return;

            // 辖区重算过，或者建筑的归属势力、势力颜色变了才重新生成贴图
            var signature = new HashCode();
            signature.Add(TerritoryCalculator.Version);
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

        private void DrawProvinceTerritory(Rectangle destination, float alpha, float depth, bool withCommanderyShade)
        {
            int width = ProvinceMap.Width;
            int height = ProvinceMap.Height;
            if (width <= 0 || height <= 0) return;

            if (this.provinceTerritory == null
                || this.provinceTerritory.IsDisposed
                || this.provinceTerritory.Width != width
                || this.provinceTerritory.Height != height
                || this.provinceTerritoryVersion != ProvinceMap.Version
                || this.provinceTerritoryWithCommanderyShade != withCommanderyShade)
            {
                var data = new Color[width * height];
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        if (withCommanderyShade)
                        {
                            var cmd = ProvinceMap.GetCommandery(x, y);
                            if (cmd != null)
                            {
                                data[y * width + x] = cmd.FillColor;
                            }
                        }
                        else
                        {
                            var prov = ProvinceMap.GetProvince(x, y);
                            if (prov != null)
                            {
                                data[y * width + x] = prov.Color;
                            }
                        }
                    }
                }

                if (this.provinceTerritory == null || this.provinceTerritory.IsDisposed || this.provinceTerritory.Width != width || this.provinceTerritory.Height != height)
                {
                    this.provinceTerritory?.Dispose();
                    this.provinceTerritory = new Texture2D(Platform.GraphicsDevice, width, height);
                }
                this.provinceTerritory.SetData(data);
                this.provinceTerritoryVersion = ProvinceMap.Version;
                this.provinceTerritoryWithCommanderyShade = withCommanderyShade;
            }

            if (CacheManager.Scale != Vector2.One)
            {
                destination = new Rectangle((int)(destination.X * CacheManager.Scale.X), (int)(destination.Y * CacheManager.Scale.Y), (int)(destination.Width * CacheManager.Scale.X), (int)(destination.Height * CacheManager.Scale.Y));
            }
            Session.Current.SpriteBatch.Draw(this.provinceTerritory, destination, null, Color.White * alpha, 0f, Vector2.Zero, SpriteEffects.None, depth);
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

        private void DrawFactionPiece(TileData[,] tiles, int orientation, Faction faction, Faction outFaction, int inX, int inY, int outX, int outY, Rectangle destination)
        {
            int stepX = orientation < VerticalInLeft ? 1 : 0;
            int stepY = 1 - stepX;
            EndKind negative = GetEndKind(faction, outFaction, GetFaction(tiles, inX - stepX, inY - stepY), GetFaction(tiles, outX - stepX, outY - stepY));
            EndKind positive = GetEndKind(faction, outFaction, GetFaction(tiles, inX + stepX, inY + stepY), GetFaction(tiles, outX + stepX, outY + stepY));

            this.factionAtlas.Draw(orientation, negative, positive, faction.FactionColor, destination, Depth);
        }

        private void DrawProvincePiece(int orientation, ProvinceMap.ProvinceInfo province, ProvinceMap.ProvinceInfo outProvince, int inX, int inY, int outX, int outY, Rectangle destination)
        {
            int stepX = orientation < VerticalInLeft ? 1 : 0;
            int stepY = 1 - stepX;
            EndKind negative = GetEndKind(province, outProvince, ProvinceMap.GetProvince(inX - stepX, inY - stepY), ProvinceMap.GetProvince(outX - stepX, outY - stepY));
            EndKind positive = GetEndKind(province, outProvince, ProvinceMap.GetProvince(inX + stepX, inY + stepY), ProvinceMap.GetProvince(outX + stepX, outY + stepY));

            this.provinceAtlas.Draw(orientation, negative, positive, province.BorderColor, destination, Depth);
        }

        private void DrawCommanderyPiece(int orientation, ProvinceMap.CommanderyInfo cmd, ProvinceMap.CommanderyInfo outCmd, int inX, int inY, int outX, int outY, Rectangle destination)
        {
            int stepX = orientation < VerticalInLeft ? 1 : 0;
            int stepY = 1 - stepX;
            EndKind negative = GetEndKind(cmd, outCmd, ProvinceMap.GetCommandery(inX - stepX, inY - stepY), ProvinceMap.GetCommandery(outX - stepX, outY - stepY));
            EndKind positive = GetEndKind(cmd, outCmd, ProvinceMap.GetCommandery(inX + stepX, inY + stepY), ProvinceMap.GetCommandery(outX + stepX, outY + stepY));

            this.commanderyAtlas.Draw(orientation, negative, positive, cmd.BorderColor, destination, CommanderyDepth);
        }

        // farIn、farOut 是格边某一头再过去的两个格子（分别挨着本侧和对方一侧）的归属
        private static EndKind GetEndKind<T>(T owner, T outOwner, T farIn, T farOut) where T : class
        {
            if (ReferenceEquals(farIn, owner))
            {
                return ReferenceEquals(farOut, owner) ? EndKind.TurnOut : EndKind.Straight;
            }
            // 只有这四格恰好分属两方、且不是对角相接时才平滑拐弯
            return !ReferenceEquals(farOut, owner) && ReferenceEquals(farIn, outOwner) && ReferenceEquals(farOut, outOwner) ? EndKind.TurnIn : EndKind.JunctionTurn;
        }

        private static Faction GetFaction(TileData[,] tiles, int x, int y)
        {
            if (x < 0 || y < 0 || x >= tiles.GetLength(0) || y >= tiles.GetLength(1)) return null;

            return tiles[x, y].OwnerArchitecture?.BelongedFaction;
        }

        /// <summary>
        /// 8x8 平滑边界曲线图集：4 种朝向 x 两端各 4 种走向 = 64 个图块，按当前格宽 1:1 异步重建。
        /// </summary>
        private sealed class BorderAtlas
        {
            private const int OrientationCount = 4;
            private const int EndKindCount = 4;
            private const int AtlasColumns = 8;
            private const int AtlasPadding = 1;

            private readonly float lineWidthRate;
            private readonly float lineAlpha;
            private readonly bool dashed;
            private readonly int dashCount;
            private readonly float dashRate;

            private Texture2D atlas;
            private int atlasTileWidth;
            private int atlasTileHeight;

            private Task<Color[]> pendingAtlas;
            private int pendingTileWidth;
            private int pendingTileHeight;

            public BorderAtlas(float lineWidthRate, float lineAlpha, bool dashed, int dashCount, float dashRate)
            {
                this.lineWidthRate = lineWidthRate;
                this.lineAlpha = lineAlpha;
                this.dashed = dashed;
                this.dashCount = dashCount;
                this.dashRate = dashRate;
            }

            public void Prepare(int tileWidth, int tileHeight)
            {
                if (this.atlas != null && !this.atlas.IsDisposed && this.atlasTileWidth == tileWidth && this.atlasTileHeight == tileHeight) return;

                if (this.atlas == null || this.atlas.IsDisposed)
                {
                    this.SetAtlas(this.GenerateAtlasData(tileWidth, tileHeight), tileWidth, tileHeight);
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
                    this.pendingAtlas = Task.Run(() => this.GenerateAtlasData(tileWidth, tileHeight));
                }
            }

            public void Draw(int orientation, EndKind negative, EndKind positive, Color color, Rectangle destination, float depth)
            {
                if (this.atlas == null || this.atlas.IsDisposed) return;

                if (CacheManager.Scale != Vector2.One)
                {
                    destination = new Rectangle(
                        Convert.ToInt16(destination.X * CacheManager.Scale.X),
                        Convert.ToInt16(destination.Y * CacheManager.Scale.Y),
                        Convert.ToInt16(destination.Width * CacheManager.Scale.X),
                        Convert.ToInt16(destination.Height * CacheManager.Scale.Y));
                }
                Session.Current.SpriteBatch.Draw(
                    this.atlas,
                    destination,
                    GetAtlasSource(this.atlasTileWidth, this.atlasTileHeight, orientation, negative, positive),
                    color,
                    0f,
                    Vector2.Zero,
                    SpriteEffects.None,
                    depth);
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

            private Color[] GenerateAtlasData(int tileWidth, int tileHeight)
            {
                Point atlasSize = GetAtlasSize(tileWidth, tileHeight);
                int atlasWidth = atlasSize.X;
                var data = new Color[atlasWidth * atlasSize.Y];

                float lineWidth = Math.Max(1f, tileWidth * this.lineWidthRate);
                int centerX = tileWidth / 2;
                int centerY = tileHeight / 2;

                for (int orientation = 0; orientation < OrientationCount; orientation++)
                {
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

                                    float alpha = this.GetAlpha(along, perp, start, end, (EndKind)negative, (EndKind)positive, horizontal, lineWidth);
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

            private float GetAlpha(float along, float perp, Vector2 start, Vector2 end, EndKind startKind, EndKind endKind, bool tieInclusive, float lineWidth)
            {
                if (!InRange(along, perp, start, startKind, tieInclusive) || !InRange(along, perp, end, endKind, tieInclusive)) return 0f;

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

                float distance = (tangent.Y * (along - position.X) - tangent.X * (perp - position.Y)) / tangent.Length();

                float inside = Math.Clamp(distance + 0.5f, 0f, 1f);
                if (inside <= 0f) return 0f;

                float line = inside * Math.Clamp(lineWidth - distance + 0.5f, 0f, 1f) * this.lineAlpha;
                if (this.dashed)
                {
                    float phase = t * this.dashCount;
                    phase -= MathF.Floor(phase);
                    float gap = (1f - this.dashRate) / 2f;
                    float periodLength = tangent.Length() / this.dashCount;
                    line *= Math.Clamp(Math.Min(phase - gap, 1f - gap - phase) * periodLength + 0.5f, 0f, 1f);
                }
                return line;
            }

            private static bool InRange(float along, float perp, Vector2 endPoint, EndKind kind, bool tieInclusive)
            {
                if (kind == EndKind.TurnIn || kind == EndKind.TurnOut)
                {
                    float side = (along - endPoint.X) * endPoint.X + (perp - endPoint.Y) * endPoint.Y;
                    return side < 0f || (side == 0f && tieInclusive);
                }

                float gap = endPoint.X > 0f ? endPoint.X - along : along - endPoint.X;
                if (gap < 0f) return false;

                if (kind == EndKind.JunctionTurn)
                {
                    return gap > -perp || (gap == -perp && tieInclusive);
                }
                return true;
            }
        }
    }
}

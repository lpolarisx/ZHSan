using GameDatas;
using GameEnums;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace GameObjects.MapDetail
{
    /// <summary>
    /// 历史州郡地图数据：按现有数据框架读取州郡配置（优先剧本/存档 .dat 内的 Provinces.json，
    /// 否则使用 CommonData.dat 内的 Provinces.json），通过静态地形多源 Dijkstra 扩散计算每个地图格子的
    /// 归属郡与归属州，结果缓存供边界绘制与提示查询。
    /// 州郡归属与势力动态辖区（TerritoryCalculator）独立，不会随人口变化或城池易主漂移。
    /// </summary>
    public static class ProvinceMap
    {
        private const int StraightStep = 100;
        private const int DiagonalStep = 141;
        private const int DefaultMaxCost = 4500;
        private const int MinWeight = 20;
        private const int MaxWeight = 400;

        public sealed class ProvinceInfo
        {
            public int ID { get; init; }
            public string Name { get; init; }
            public Color Color { get; init; }
            public Color BorderColor { get; init; }
            public Vector2 LabelTile { get; internal set; }
            public bool HasLabel { get; internal set; }
            public List<CommanderyInfo> Commanderies { get; } = new();
        }

        public sealed class CommanderyInfo
        {
            public int ID { get; init; }
            public string Name { get; init; }
            public ProvinceInfo Province { get; init; }
            public Color FillColor { get; init; }
            public Color BorderColor { get; init; }
            public Vector2 LabelTile { get; internal set; }
            public bool HasLabel { get; internal set; }
        }

        private static readonly Color[] FallbackPalette =
        {
            new(230, 180, 56),  new(201, 80, 70),   new(74, 166, 114),
            new(126, 98, 184),  new(59, 156, 156),  new(214, 122, 60),
            new(120, 86, 196),  new(122, 168, 62),  new(199, 86, 146),
            new(217, 164, 62),  new(66, 132, 198),  new(78, 158, 94),
            new(138, 98, 184),  new(204, 120, 64),  new(102, 142, 188),
            new(178, 138, 86),  new(96, 156, 132),  new(168, 96, 126),
            new(146, 154, 82)
        };

        /// <summary>
        /// 每次加载或重算州郡地图后递增，供渲染层判断贴图缓存是否失效
        /// </summary>
        public static int Version { get; private set; }

        public static int Width { get; private set; }
        public static int Height { get; private set; }
        public static IReadOnlyList<ProvinceInfo> Provinces => provinces;
        public static IReadOnlyList<CommanderyInfo> Commanderies => commanderies;

        private static ProvinceInfo[,] tileProvinces;
        private static CommanderyInfo[,] tileCommanderies;
        private static List<ProvinceInfo> provinces = new();
        private static List<CommanderyInfo> commanderies = new();

        private static GameScenario loadedScenario;
        private static ProvinceMapConfig loadedConfig;

        /// <summary>
        /// 载入剧本或存档后调用：优先使用剧本自定义 Provinces.json，其次使用 CommonData.dat 中的默认配置。
        /// </summary>
        public static void Initialize(GameScenario scenario)
        {
            if (scenario?.ScenarioMap?.MapData == null) return;

            ProvinceMapConfig config = ResolveConfig(scenario);
            loadedScenario = scenario;
            loadedConfig = config;

            if (config?.Provinces != null && config.Provinces.Count > 0)
            {
                BuildFromConfig(scenario.ScenarioMap, config);
            }
            else
            {
                BuildFallbackFromScenario(scenario);
            }
        }

        /// <summary>
        /// 确保当前剧本的州郡缓存已就绪（若剧本或配置引用发生变化则自动重建）。
        /// </summary>
        public static void EnsureInitialized(GameScenario scenario)
        {
            if (scenario?.ScenarioMap?.MapData == null) return;

            ProvinceMapConfig config = ResolveConfig(scenario);
            if (tileProvinces == null
                || !ReferenceEquals(loadedScenario, scenario)
                || !ReferenceEquals(loadedConfig, config)
                || Width != scenario.ScenarioMap.MapDimensions.X
                || Height != scenario.ScenarioMap.MapDimensions.Y)
            {
                Initialize(scenario);
            }
        }

        public static ProvinceInfo GetProvince(int x, int y)
        {
            if (tileProvinces == null || x < 0 || y < 0 || x >= Width || y >= Height) return null;
            return tileProvinces[x, y];
        }

        public static CommanderyInfo GetCommandery(int x, int y)
        {
            if (tileCommanderies == null || x < 0 || y < 0 || x >= Width || y >= Height) return null;
            return tileCommanderies[x, y];
        }

        public static string GetDisplayName(int x, int y, bool includeCommandery)
        {
            var province = GetProvince(x, y);
            if (province == null) return "";

            if (!includeCommandery) return province.Name;

            var commandery = GetCommandery(x, y);
            if (commandery == null || string.IsNullOrEmpty(commandery.Name) || commandery.Name == province.Name)
            {
                return province.Name;
            }
            return province.Name + "·" + commandery.Name;
        }

        private static ProvinceMapConfig ResolveConfig(GameScenario scenario)
        {
            if (scenario.CustomProvinceMapConfig?.Provinces is { Count: > 0 })
            {
                return scenario.CustomProvinceMapConfig;
            }
            if (scenario.GameCommonData?.ProvinceMapConfig?.Provinces is { Count: > 0 })
            {
                return scenario.GameCommonData.ProvinceMapConfig;
            }
            if (CommonData.Current?.ProvinceMapConfig?.Provinces is { Count: > 0 })
            {
                return CommonData.Current.ProvinceMapConfig;
            }
            return null;
        }

        private static void BuildFromConfig(Map map, ProvinceMapConfig config)
        {
            int width = map.MapDimensions.X;
            int height = map.MapDimensions.Y;

            var terrainCosts = BuildTerrainCostTable(config.TerrainCosts);
            int globalMaxCost = config.MaxCost > 0 ? config.MaxCost : DefaultMaxCost;

            var newProvinces = new List<ProvinceInfo>();
            var newCommanderies = new List<CommanderyInfo>();
            var commanderyById = new Dictionary<int, CommanderyInfo>();
            var provinceExplicitLabels = new Dictionary<int, Vector2>();
            var commanderyExplicitLabels = new Dictionary<int, Vector2>();

            var owners = new CommanderyInfo[width, height];
            var costs = new int[width, height];
            var locked = new bool[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    costs[x, y] = int.MaxValue;
                }
            }

            var queue = new PriorityQueue<(int X, int Y, CommanderyInfo Owner, int Weight, int MaxCost), int>();

            int fallbackIndex = 0;
            foreach (var pCfg in config.Provinces)
            {
                if (pCfg == null || string.IsNullOrWhiteSpace(pCfg.Name)) continue;

                Color baseColor = ParseColor(pCfg.Color, FallbackPalette[fallbackIndex % FallbackPalette.Length]);
                fallbackIndex++;

                var province = new ProvinceInfo
                {
                    ID = pCfg.Id,
                    Name = pCfg.Name,
                    Color = baseColor,
                    BorderColor = AdjustColor(baseColor, 0.72f, 1.15f)
                };
                newProvinces.Add(province);

                if (pCfg.LabelPos is { Length: >= 2 })
                {
                    provinceExplicitLabels[province.ID] = new Vector2(pCfg.LabelPos[0], pCfg.LabelPos[1]);
                }

                if (pCfg.Commanderies == null) continue;

                for (int i = 0; i < pCfg.Commanderies.Count; i++)
                {
                    var cCfg = pCfg.Commanderies[i];
                    if (cCfg == null || string.IsNullOrWhiteSpace(cCfg.Name) || commanderyById.ContainsKey(cCfg.Id)) continue;

                    // 同州内各郡底色在州色基础上做明暗交替，既能看出属同一州，也能清晰区分相邻郡域
                    float shade = (i % 3) switch
                    {
                        0 => 1.0f,
                        1 => 0.80f,
                        _ => 1.20f
                    };
                    var commandery = new CommanderyInfo
                    {
                        ID = cCfg.Id,
                        Name = cCfg.Name,
                        Province = province,
                        FillColor = AdjustColor(baseColor, shade, 1.0f),
                        // 郡界虚线用浅米亮色调，与深色粗实线的州界形成明显层级对比
                        BorderColor = Color.Lerp(AdjustColor(baseColor, 1.35f, 0.45f), new Color(255, 250, 235), 0.55f)
                    };
                    province.Commanderies.Add(commandery);
                    newCommanderies.Add(commandery);
                    commanderyById[commandery.ID] = commandery;

                    if (cCfg.LabelPos is { Length: >= 2 })
                    {
                        commanderyExplicitLabels[commandery.ID] = new Vector2(cCfg.LabelPos[0], cCfg.LabelPos[1]);
                    }

                    int cmdWeight = Math.Clamp(cCfg.Weight > 0 ? cCfg.Weight : 100, MinWeight, MaxWeight);
                    int cmdMaxCost = cCfg.MaxCost > 0 ? cCfg.MaxCost : globalMaxCost;

                    if (cCfg.Anchors == null) continue;
                    foreach (var anchor in cCfg.Anchors)
                    {
                        if (anchor == null) continue;
                        if (anchor.X < 0 || anchor.Y < 0 || anchor.X >= width || anchor.Y >= height) continue;

                        int startCost = Math.Max(0, anchor.StartCost);
                        int anchorWeight = Math.Clamp(anchor.Weight > 0 ? anchor.Weight : cmdWeight, MinWeight, MaxWeight);
                        if (startCost < costs[anchor.X, anchor.Y])
                        {
                            costs[anchor.X, anchor.Y] = startCost;
                            owners[anchor.X, anchor.Y] = commandery;
                            locked[anchor.X, anchor.Y] = true;
                            queue.Enqueue((anchor.X, anchor.Y, commandery, anchorWeight, cmdMaxCost), startCost);
                        }
                    }
                }
            }

            // 多源 Dijkstra：按静态地形代价向外扩散
            while (queue.TryDequeue(out var node, out int cost))
            {
                if (cost > costs[node.X, node.Y] || owners[node.X, node.Y] != node.Owner) continue;

                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        if (dx == 0 && dy == 0) continue;

                        int nx = node.X + dx;
                        int ny = node.Y + dy;
                        if (nx < 0 || ny < 0 || nx >= width || ny >= height) continue;

                        int terrainId = map.MapData[nx, ny];
                        int terrainCost = terrainId >= 0 && terrainId < terrainCosts.Length ? terrainCosts[terrainId] : 2;
                        if (terrainCost <= 0) continue;

                        int step = dx != 0 && dy != 0 ? DiagonalStep : StraightStep;
                        int newCost = cost + terrainCost * step * 100 / node.Weight;
                        if (newCost > node.MaxCost || newCost >= costs[nx, ny]) continue;

                        costs[nx, ny] = newCost;
                        owners[nx, ny] = node.Owner;
                        queue.Enqueue((nx, ny, node.Owner, node.Weight, node.MaxCost), newCost);
                    }
                }
            }

            // 手工覆盖区域（用于配置精细修边，CommanderyId <= 0 表示清空该区域）
            if (config.Overrides != null)
            {
                foreach (var ov in config.Overrides)
                {
                    if (ov == null) continue;
                    CommanderyInfo target = ov.CommanderyId > 0 && commanderyById.TryGetValue(ov.CommanderyId, out var found) ? found : null;
                    int x0 = Math.Max(0, ov.X);
                    int y0 = Math.Max(0, ov.Y);
                    int x1 = Math.Min(width - 1, ov.X + Math.Max(1, ov.Width) - 1);
                    int y1 = Math.Min(height - 1, ov.Y + Math.Max(1, ov.Height) - 1);
                    for (int x = x0; x <= x1; x++)
                    {
                        for (int y = y0; y <= y1; y++)
                        {
                            owners[x, y] = target;
                            locked[x, y] = true;
                        }
                    }
                }
            }

            // 清理因斜向扩散产生的 1 格孤立噪点
            RemoveIsolatedSpeckles(owners, locked, width, height);

            var provGrid = new ProvinceInfo[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    provGrid[x, y] = owners[x, y]?.Province;
                }
            }

            ComputeProvinceLabels(provGrid, newProvinces, provinceExplicitLabels, width, height);
            ComputeCommanderyLabels(owners, newCommanderies, commanderyExplicitLabels, width, height);

            Width = width;
            Height = height;
            tileProvinces = provGrid;
            tileCommanderies = owners;
            provinces = newProvinces;
            commanderies = newCommanderies;
            Version++;
        }

        /// <summary>
        /// 旧版数据包/MOD 未包含 Provinces.json 时的兜底方案：基于剧本已有 State 与 Architecture 按等权生成州界。
        /// </summary>
        private static void BuildFallbackFromScenario(GameScenario scenario)
        {
            var config = new ProvinceMapConfig { MaxCost = DefaultMaxCost };
            int idx = 0;
            foreach (var state in scenario.States.Values.OrderBy(s => s.ID))
            {
                var pItem = new ProvinceConfig
                {
                    Id = state.ID + 1,
                    Name = state.Name,
                    Color = ColorToHex(FallbackPalette[idx % FallbackPalette.Length])
                };
                idx++;

                var anchors = new List<CommanderyAnchorConfig>();
                foreach (var arch in state.Architectures)
                {
                    anchors.Add(new CommanderyAnchorConfig
                    {
                        Name = arch.Name,
                        X = arch.zhongxindian.X,
                        Y = arch.zhongxindian.Y,
                        StartCost = arch.Kind.CountToMerit ? 0 : 800
                    });
                }

                if (anchors.Count > 0)
                {
                    pItem.Commanderies.Add(new CommanderyConfig
                    {
                        Id = (state.ID + 1) * 100 + 1,
                        Name = state.Name,
                        Weight = 100,
                        Anchors = anchors
                    });
                    config.Provinces.Add(pItem);
                }
            }

            BuildFromConfig(scenario.ScenarioMap, config);
        }

        private static void RemoveIsolatedSpeckles(CommanderyInfo[,] owners, bool[,] locked, int width, int height)
        {
            var fixes = new List<(int X, int Y, CommanderyInfo Owner)>();
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var cur = owners[x, y];
                    if (cur == null || locked[x, y]) continue;

                    CommanderyInfo n1 = x > 0 ? owners[x - 1, y] : null;
                    CommanderyInfo n2 = x + 1 < width ? owners[x + 1, y] : null;
                    CommanderyInfo n3 = y > 0 ? owners[x, y - 1] : null;
                    CommanderyInfo n4 = y + 1 < height ? owners[x, y + 1] : null;

                    if (n1 == cur || n2 == cur || n3 == cur || n4 == cur) continue;

                    CommanderyInfo best = PickMajority(n1, n2, n3, n4);
                    if (best != null)
                    {
                        fixes.Add((x, y, best));
                    }
                }
            }

            foreach (var fix in fixes)
            {
                owners[fix.X, fix.Y] = fix.Owner;
            }
        }

        private static CommanderyInfo PickMajority(CommanderyInfo a, CommanderyInfo b, CommanderyInfo c, CommanderyInfo d)
        {
            CommanderyInfo[] list = { a, b, c, d };
            CommanderyInfo best = null;
            int bestCount = 0;
            for (int i = 0; i < 4; i++)
            {
                if (list[i] == null) continue;
                int count = 0;
                for (int j = 0; j < 4; j++)
                {
                    if (list[j] == list[i]) count++;
                }
                if (count > bestCount)
                {
                    bestCount = count;
                    best = list[i];
                }
            }
            return best;
        }

        private static void ComputeProvinceLabels(ProvinceInfo[,] grid, List<ProvinceInfo> list, Dictionary<int, Vector2> explicitLabels, int width, int height)
        {
            var visited = new bool[width, height];
            var largestComponent = new Dictionary<int, List<Point>>();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var p = grid[x, y];
                    if (p == null || visited[x, y]) continue;

                    var comp = CollectComponent(grid, visited, x, y, p, width, height);
                    if (!largestComponent.TryGetValue(p.ID, out var existing) || comp.Count > existing.Count)
                    {
                        largestComponent[p.ID] = comp;
                    }
                }
            }

            foreach (var p in list)
            {
                if (explicitLabels.TryGetValue(p.ID, out var pos))
                {
                    p.LabelTile = pos;
                    p.HasLabel = true;
                }
                else if (largestComponent.TryGetValue(p.ID, out var comp) && comp.Count > 0)
                {
                    p.LabelTile = FindInteriorCenter(grid, comp, p, width, height);
                    p.HasLabel = true;
                }
            }
        }

        private static void ComputeCommanderyLabels(CommanderyInfo[,] grid, List<CommanderyInfo> list, Dictionary<int, Vector2> explicitLabels, int width, int height)
        {
            var visited = new bool[width, height];
            var largestComponent = new Dictionary<int, List<Point>>();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var c = grid[x, y];
                    if (c == null || visited[x, y]) continue;

                    var comp = CollectComponent(grid, visited, x, y, c, width, height);
                    if (!largestComponent.TryGetValue(c.ID, out var existing) || comp.Count > existing.Count)
                    {
                        largestComponent[c.ID] = comp;
                    }
                }
            }

            foreach (var c in list)
            {
                if (explicitLabels.TryGetValue(c.ID, out var pos))
                {
                    c.LabelTile = pos;
                    c.HasLabel = true;
                }
                else if (largestComponent.TryGetValue(c.ID, out var comp) && comp.Count > 0)
                {
                    c.LabelTile = FindInteriorCenter(grid, comp, c, width, height);
                    c.HasLabel = true;
                }
            }
        }

        private static List<Point> CollectComponent<T>(T[,] grid, bool[,] visited, int startX, int startY, T target, int width, int height) where T : class
        {
            var result = new List<Point>();
            var q = new Queue<Point>();
            visited[startX, startY] = true;
            q.Enqueue(new Point(startX, startY));

            while (q.Count > 0)
            {
                var cur = q.Dequeue();
                result.Add(cur);

                TryEnqueue(cur.X + 1, cur.Y);
                TryEnqueue(cur.X - 1, cur.Y);
                TryEnqueue(cur.X, cur.Y + 1);
                TryEnqueue(cur.X, cur.Y - 1);
            }
            return result;

            void TryEnqueue(int nx, int ny)
            {
                if (nx < 0 || ny < 0 || nx >= width || ny >= height || visited[nx, ny]) return;
                if (!ReferenceEquals(grid[nx, ny], target)) return;
                visited[nx, ny] = true;
                q.Enqueue(new Point(nx, ny));
            }
        }

        // 用多源 BFS 距离场求连通块内部最深且靠近质心的点，避免弯曲或狭长州郡的名称落到边界外
        private static Vector2 FindInteriorCenter<T>(T[,] grid, List<Point> component, T target, int width, int height) where T : class
        {
            double sumX = 0, sumY = 0;
            var dist = new Dictionary<Point, int>(component.Count);
            var q = new Queue<Point>();

            foreach (var pt in component)
            {
                sumX += pt.X;
                sumY += pt.Y;

                bool isBoundary = pt.X == 0 || pt.Y == 0 || pt.X == width - 1 || pt.Y == height - 1
                    || !ReferenceEquals(grid[pt.X - 1, pt.Y], target)
                    || !ReferenceEquals(grid[pt.X + 1, pt.Y], target)
                    || !ReferenceEquals(grid[pt.X, pt.Y - 1], target)
                    || !ReferenceEquals(grid[pt.X, pt.Y + 1], target);

                if (isBoundary)
                {
                    dist[pt] = 0;
                    q.Enqueue(pt);
                }
            }

            float cx = (float)(sumX / component.Count);
            float cy = (float)(sumY / component.Count);

            int maxDist = 0;
            while (q.Count > 0)
            {
                var cur = q.Dequeue();
                int nextDist = dist[cur] + 1;

                Expand(cur.X + 1, cur.Y, nextDist);
                Expand(cur.X - 1, cur.Y, nextDist);
                Expand(cur.X, cur.Y + 1, nextDist);
                Expand(cur.X, cur.Y - 1, nextDist);
            }

            Point best = component[0];
            float bestScore = float.NegativeInfinity;
            // 在靠近最深处（>= 80% 最大内距）的点中优先挑离质心最近的，视觉最稳
            int threshold = Math.Max(0, maxDist * 4 / 5);
            foreach (var pt in component)
            {
                int d = dist.TryGetValue(pt, out int val) ? val : 0;
                if (d < threshold) continue;

                float dx = pt.X - cx;
                float dy = pt.Y - cy;
                float score = d * 2f - MathF.Sqrt(dx * dx + dy * dy) * 0.35f;
                if (score > bestScore)
                {
                    bestScore = score;
                    best = pt;
                }
            }

            return new Vector2(best.X + 0.5f, best.Y + 0.5f);

            void Expand(int nx, int ny, int d)
            {
                if (nx < 0 || ny < 0 || nx >= width || ny >= height) return;
                if (!ReferenceEquals(grid[nx, ny], target)) return;
                var npt = new Point(nx, ny);
                if (dist.ContainsKey(npt)) return;
                dist[npt] = d;
                if (d > maxDist) maxDist = d;
                q.Enqueue(npt);
            }
        }

        private static int[] BuildTerrainCostTable(Dictionary<string, int> custom)
        {
            var table = new int[16];
            table[(int)TerrainKind.None] = 0;
            table[(int)TerrainKind.Plain] = 1;
            table[(int)TerrainKind.Grassland] = 1;
            table[(int)TerrainKind.Forrest] = 2;
            table[(int)TerrainKind.Marsh] = 2;
            table[(int)TerrainKind.Mountain] = 3;
            table[(int)TerrainKind.Water] = 3;
            table[(int)TerrainKind.Ridge] = 6;
            table[(int)TerrainKind.Wasteland] = 2;
            table[(int)TerrainKind.Desert] = 2;
            table[(int)TerrainKind.Cliff] = 2;

            if (custom != null)
            {
                foreach (var kv in custom)
                {
                    if (Enum.TryParse<TerrainKind>(kv.Key, true, out var kind) && (int)kind < table.Length)
                    {
                        table[(int)kind] = Math.Max(0, kv.Value);
                    }
                }
            }
            return table;
        }

        private static Color ParseColor(string hex, Color fallback)
        {
            if (string.IsNullOrWhiteSpace(hex)) return fallback;
            string s = hex.Trim().TrimStart('#');
            if (s.Length == 6 && int.TryParse(s, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int rgb))
            {
                return new Color((rgb >> 16) & 0xFF, (rgb >> 8) & 0xFF, rgb & 0xFF);
            }
            return fallback;
        }

        private static string ColorToHex(Color c) => $"#{c.R:X2}{c.G:X2}{c.B:X2}";

        private static Color AdjustColor(Color c, float brightness, float saturation)
        {
            float r = c.R / 255f;
            float g = c.G / 255f;
            float b = c.B / 255f;
            float gray = r * 0.299f + g * 0.587f + b * 0.114f;

            r = Math.Clamp((gray + (r - gray) * saturation) * brightness, 0f, 1f);
            g = Math.Clamp((gray + (g - gray) * saturation) * brightness, 0f, 1f);
            b = Math.Clamp((gray + (b - gray) * saturation) * brightness, 0f, 1f);
            return new Color(r, g, b);
        }
    }
}

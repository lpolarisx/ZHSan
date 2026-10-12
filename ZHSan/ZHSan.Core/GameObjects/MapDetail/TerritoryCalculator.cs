using GameEnums;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GameObjects.MapDetail
{
    /// <summary>
    /// 计算每个地图格子归属的建筑（辖区），结果写入 TileData.OwnerArchitecture。
    /// 所有建筑同时向外扩散，扩散的快慢由建筑的影响力（Architecture.TerritoryInfluence）决定：
    /// 城池看人口、统治、民心和是否都城，关隘港口等没有人口的建筑看驻军。
    /// 影响力只在换季和建筑易主（含都城变动）时更新，并且分档取值，避免边界频繁抖动。
    /// </summary>
    public static class TerritoryCalculator
    {
        // 直走、斜走一格的距离系数
        private const int StraightStep = 100;
        private const int DiagonalStep = 141;

        // 辖区最远能延伸到的代价，超出的格子为无主地（影响力100%时约等于40格平原）
        private const int MaxCost = 4000;

        // 关隘、港口等非城池建筑的起始代价，使其辖区小于城池
        private const int MinorArchitectureStartCost = 800;

        // 影响力是百分比，按InfluenceStep分档，并限制在[MinInfluence, MaxInfluence]内，
        // 上下限拉得太开的话弱城会被相邻的强城整个包住
        private const int InfluenceStep = 10;
        private const int MinInfluence = 70;
        private const int MaxInfluence = 150;

        // 统治、民心为0时影响力打的折扣，满值时不打折
        private const float MinDominationRate = 0.7f;
        private const float MinMoraleRate = 0.9f;

        // 都城的加成
        private const float CapitalRate = 1.15f;

        // 没有人口的建筑：无驻军时的影响力，以及驻军规模达到GarrisonFullScale时的影响力
        private const float GarrisonMinRate = 0.7f;
        private const float GarrisonMaxRate = 1.3f;
        private const int GarrisonFullScale = 40;

        /// <summary>
        /// 每次重算辖区后加一，供界面判断缓存是否过期
        /// </summary>
        public static int Version { get; private set; }

        // 上次更新影响力时的季节，以及各建筑当时的归属
        private static int lastSeasonKey;
        private static readonly Dictionary<int, int> ownerKeys = new();

        // 辖区向该地形扩散一格的代价，0表示不可归属
        private static int GetTerrainCost(TerrainKind kind)
        {
            switch (kind)
            {
                case TerrainKind.None:
                    return 0;
                case TerrainKind.Plain:
                case TerrainKind.Grassland:
                    return 1;
                case TerrainKind.Mountain:
                case TerrainKind.Water:
                    return 3;
                case TerrainKind.Ridge:
                    return 6;
                default:
                    return 2;
            }
        }

        /// <summary>
        /// 载入剧本或存档后调用。存档里已有的影响力沿用，这样读档前后边界不变；没有的现算。
        /// </summary>
        public static void Initialize(GameScenario scenario)
        {
            lastSeasonKey = GetSeasonKey(scenario);
            ownerKeys.Clear();

            int averagePopulation = GetAveragePopulation(scenario);
            foreach (var architecture in scenario.Architectures.Values)
            {
                ownerKeys[architecture.ID] = GetOwnerKey(architecture);
                if (architecture.TerritoryInfluence <= 0)
                {
                    architecture.TerritoryInfluence = ToStep(GetRawInfluence(architecture, averagePopulation));
                }
            }

            Calculate(scenario);
        }

        /// <summary>
        /// 每天调用。换季时更新所有建筑的影响力，平时只更新易主的建筑，影响力有变化才重算辖区。
        /// </summary>
        public static void Update(GameScenario scenario)
        {
            int seasonKey = GetSeasonKey(scenario);
            bool seasonChanged = seasonKey != lastSeasonKey;
            lastSeasonKey = seasonKey;

            int averagePopulation = -1;
            bool changed = false;
            foreach (var architecture in scenario.Architectures.Values)
            {
                int ownerKey = GetOwnerKey(architecture);
                bool ownerChanged = !ownerKeys.TryGetValue(architecture.ID, out int oldOwnerKey) || oldOwnerKey != ownerKey;
                ownerKeys[architecture.ID] = ownerKey;
                if (!seasonChanged && !ownerChanged) continue;

                if (averagePopulation < 0) averagePopulation = GetAveragePopulation(scenario);

                float raw = GetRawInfluence(architecture, averagePopulation);
                // 换季时变化不足一档的不动，免得数值在两档之间徘徊时边界来回跳
                if (!ownerChanged && Math.Abs(raw - architecture.TerritoryInfluence) < InfluenceStep) continue;

                int influence = ToStep(raw);
                if (influence != architecture.TerritoryInfluence)
                {
                    architecture.TerritoryInfluence = influence;
                    changed = true;
                }
            }

            if (changed) Calculate(scenario);
        }

        private static int GetSeasonKey(GameScenario scenario)
        {
            return scenario.Date.Year * 4 + (int)scenario.Date.Season;
        }

        private static int GetOwnerKey(Architecture architecture)
        {
            return (architecture.BelongedFaction?.ID ?? -1) * 2 + (architecture.IsCapital ? 1 : 0);
        }

        // 有人口的建筑的平均人口，作为人口多少的基准
        private static int GetAveragePopulation(GameScenario scenario)
        {
            long total = 0;
            int count = 0;
            foreach (var architecture in scenario.Architectures.Values)
            {
                if (!architecture.Kind.HasPopulation) continue;

                total += architecture.Population;
                count++;
            }
            return count > 0 ? (int)Math.Max(1, total / count) : 1;
        }

        // 未分档的影响力（百分比）
        private static float GetRawInfluence(Architecture architecture, int averagePopulation)
        {
            float rate;
            if (architecture.Kind.HasPopulation)
            {
                // 人口取平方根，否则大城和小城差距过大
                rate = MathF.Sqrt((float)Math.Max(0, architecture.Population) / averagePopulation);
            }
            else
            {
                int scale = architecture.BelongedFaction != null ? Math.Min(architecture.ArmyScale, GarrisonFullScale) : 0;
                rate = GarrisonMinRate + (GarrisonMaxRate - GarrisonMinRate) * scale / GarrisonFullScale;
            }

            if (architecture.Kind.HasDomination)
            {
                rate *= GetCeilingRate(architecture.Domination, architecture.DominationCeiling, MinDominationRate);
            }
            if (architecture.Kind.HasMorale)
            {
                rate *= GetCeilingRate(architecture.Morale, architecture.MoraleCeiling, MinMoraleRate);
            }
            if (architecture.IsCapital)
            {
                rate *= CapitalRate;
            }

            return Math.Clamp(rate * 100f, MinInfluence, MaxInfluence);
        }

        // value从0到ceiling时，折扣从minRate线性增加到1
        private static float GetCeilingRate(int value, int ceiling, float minRate)
        {
            if (ceiling <= 0) return 1f;

            return minRate + (1f - minRate) * Math.Clamp((float)value / ceiling, 0f, 1f);
        }

        private static int ToStep(float raw)
        {
            int influence = (int)MathF.Round(raw / InfluenceStep) * InfluenceStep;
            return Math.Clamp(influence, MinInfluence, MaxInfluence);
        }

        public static void Calculate(GameScenario scenario)
        {
            var map = scenario.ScenarioMap;
            var tiles = scenario.MapTileData;
            if (tiles == null || map.MapData == null) return;

            int width = map.MapDimensions.X;
            int height = map.MapDimensions.Y;

            // 先算到owners里，算完再一次写回，免得界面在计算过程中读到算了一半的结果
            var owners = new Architecture[width, height];
            var costs = new int[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    costs[x, y] = int.MaxValue;
                }
            }

            // 多源Dijkstra：所有建筑同时向外扩散，格子归最先到达的建筑
            var queue = new PriorityQueue<Point, int>();
            foreach (var architecture in scenario.Architectures.Values.OrderBy(a => a.ID))
            {
                int startCost = architecture.Kind.CountToMerit ? 0 : MinorArchitectureStartCost;
                foreach (var point in architecture.ArchitectureArea.Area)
                {
                    if (map.PositionOutOfRange(point)) continue;

                    owners[point.X, point.Y] = architecture;
                    costs[point.X, point.Y] = startCost;
                    queue.Enqueue(point, startCost);
                }
            }

            while (queue.TryDequeue(out var current, out var cost))
            {
                if (cost > costs[current.X, current.Y]) continue;

                var owner = owners[current.X, current.Y];
                // 影响力越大，扩散同样的距离代价越小
                int influence = Math.Clamp(owner.TerritoryInfluence, MinInfluence, MaxInfluence);
                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        if (dx == 0 && dy == 0) continue;

                        int x = current.X + dx;
                        int y = current.Y + dy;
                        if (x < 0 || y < 0 || x >= width || y >= height) continue;

                        // 建筑自身占的格子不会被别的建筑划走
                        if (tiles[x, y].TileArchitecture != null) continue;

                        int terrainCost = GetTerrainCost((TerrainKind)map.MapData[x, y]);
                        if (terrainCost == 0) continue;

                        int newCost = cost + terrainCost * (dx != 0 && dy != 0 ? DiagonalStep : StraightStep) * 100 / influence;
                        if (newCost > MaxCost || newCost >= costs[x, y]) continue;

                        costs[x, y] = newCost;
                        owners[x, y] = owner;
                        queue.Enqueue(new Point(x, y), newCost);
                    }
                }
            }

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    tiles[x, y].OwnerArchitecture = owners[x, y];
                }
            }
            Version++;
        }
    }
}

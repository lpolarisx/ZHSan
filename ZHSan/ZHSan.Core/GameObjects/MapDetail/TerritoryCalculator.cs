using GameEnums;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace GameObjects.MapDetail
{
    /// <summary>
    /// 计算每个地图格子归属的建筑（辖区），结果写入 TileData.OwnerArchitecture。
    /// 只和地图、建筑位置有关，和势力无关，载入剧本或存档后算一次即可。
    /// </summary>
    public static class TerritoryCalculator
    {
        // 直走、斜走一格的距离系数
        private const int StraightStep = 10;
        private const int DiagonalStep = 14;

        // 辖区最远能延伸到的代价，超出的格子为无主地（约等于40格平原）
        private const int MaxCost = 400;

        // 关隘、港口等非城池建筑的起始代价，使其辖区小于城池
        private const int MinorArchitectureStartCost = 80;

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

        public static void Calculate(GameScenario scenario)
        {
            var map = scenario.ScenarioMap;
            var tiles = scenario.MapTileData;
            if (tiles == null || map.MapData == null) return;

            int width = map.MapDimensions.X;
            int height = map.MapDimensions.Y;

            var costs = new int[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    costs[x, y] = int.MaxValue;
                    tiles[x, y].OwnerArchitecture = null;
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

                    tiles[point.X, point.Y].OwnerArchitecture = architecture;
                    costs[point.X, point.Y] = startCost;
                    queue.Enqueue(point, startCost);
                }
            }

            while (queue.TryDequeue(out var current, out var cost))
            {
                if (cost > costs[current.X, current.Y]) continue;

                var owner = tiles[current.X, current.Y].OwnerArchitecture;
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

                        int newCost = cost + terrainCost * (dx != 0 && dy != 0 ? DiagonalStep : StraightStep);
                        if (newCost > MaxCost || newCost >= costs[x, y]) continue;

                        costs[x, y] = newCost;
                        tiles[x, y].OwnerArchitecture = owner;
                        queue.Enqueue(new Point(x, y), newCost);
                    }
                }
            }
        }
    }
}

using GameManager;
using GameObjects;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;



namespace GameObjects.TroopDetail
{
    public class TierPathFinder
    {
        private Dictionary<Point, GameSquare> openDictionary = new Dictionary<Point, GameSquare>();
        // 同一位置被更优路径替换时，旧节点留在队列里，出队时与openDictionary比对后丢弃
        private PriorityQueue<GameSquare, int> openList = new PriorityQueue<GameSquare, int>();
        private HashSet<Point> closeSet = new HashSet<Point>();
        private GameSquare lastClosedSquare;
        private List<Point> lastPath = new List<Point>();

        public event GetCost OnGetCost;

        public event GetPenalizedCost OnGetPenalizedCost;

        private int distance(Point a, Point b)
        {
            return (Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y)) * 30; // 25 = 5 * 5 (5 for the map cost and 5 for Movability multiplier) (a little overestimate is better, use 30)
        }

        private void ClearLists()
        {
            openDictionary.Clear();
            openList.Clear();
            closeSet.Clear();
        }

        private GameSquare AddToCloseList()
        {
            GameSquare square = this.RemoveFromOpenList();
            if (square != null)
            {
                this.AddToCloseList(square);
            }
            return square;
        }

        private void AddToCloseList(GameSquare square)
        {
            closeSet.Add(square.Position);
            lastClosedSquare = square;
        }

        private void AddToOpenList(GameSquare square, bool useAStar)
        {
            int key = 0;
            if (useAStar) // A* search
                key = square.F * 160000 + (square.Position.X * 400 + square.Position.Y); // 加上位置使相同F值的节点有确定的先后顺序
            else          // least cost first search
                key = square.G * 160000 + (square.Position.X * 400 + square.Position.Y);
            openList.Enqueue(square, key);
            openDictionary[square.Position] = square;
        }

        private void CheckAdjacentSquares(GameSquare currentSquare, Point end, bool useAStar, MilitaryKind kind)
        {
            int leftSquareCost = this.MakeSquare(currentSquare, false, new Point(currentSquare.Position.X - 1, currentSquare.Position.Y), end, -1, useAStar, kind);
            int topSquareCost = this.MakeSquare(currentSquare, false, new Point(currentSquare.Position.X, currentSquare.Position.Y - 1), end, -1, useAStar, kind);
            int rightSquareCost = this.MakeSquare(currentSquare, false, new Point(currentSquare.Position.X + 1, currentSquare.Position.Y), end, -1, useAStar, kind);
            int bottomSquareCost = this.MakeSquare(currentSquare, false, new Point(currentSquare.Position.X, currentSquare.Position.Y + 1), end, -1, useAStar, kind);
            this.MakeSquare(currentSquare, true, new Point(currentSquare.Position.X - 1, currentSquare.Position.Y - 1), end, (topSquareCost > leftSquareCost) ? topSquareCost : leftSquareCost, useAStar, kind);
            this.MakeSquare(currentSquare, true, new Point(currentSquare.Position.X - 1, currentSquare.Position.Y + 1), end, (bottomSquareCost > leftSquareCost) ? bottomSquareCost : leftSquareCost, useAStar, kind);
            this.MakeSquare(currentSquare, true, new Point(currentSquare.Position.X + 1, currentSquare.Position.Y - 1), end, (topSquareCost > rightSquareCost) ? topSquareCost : rightSquareCost, useAStar, kind);
            this.MakeSquare(currentSquare, true, new Point(currentSquare.Position.X + 1, currentSquare.Position.Y + 1), end, (bottomSquareCost > rightSquareCost) ? bottomSquareCost : rightSquareCost, useAStar, kind);
        }

        private int GetCostByPosition(Point position, bool oblique, int DirectionCost, MilitaryKind kind)
        {
            if (this.OnGetCost != null)
            {
                return this.OnGetCost(position, oblique, DirectionCost, kind);
            }
            return 0xdac;
        }

        private void saveLastPath() {
            lastPath.Clear();
            GameSquare parent = lastClosedSquare;
            do
            {
                lastPath.Add(parent.Position);
                parent = parent.Parent;
            }
            while (parent != null);
            lastPath.Reverse();
        }

        public GameArea GetDayArea(Troop troop, int Days)
        {
            GameArea area = new GameArea();
            ClearLists();
            GameSquare square = new GameSquare();
            square.Position = troop.Position;
            this.AddToCloseList(square);
            int num = troop.Movability * Days;
            int movabilityLeft = troop.MovabilityLeft;
            //int num3 = troop.RealMovability * Days;
            troop.MovabilityLeft = num;
            MilitaryKind kind = troop.Army.Kind;
            do
            {
                CheckAdjacentSquares(square, troop.Position, false, kind);
                if (this.openDictionary.Count == 0)
                {
                    break;
                }
                square = this.AddToCloseList();
                if (square == null)
                {
                    break;
                }
                if (num >= square.G)
                {
                    if (!Session.Current.Scenario.PositionIsTroop(square.Position))
                    {
                        area.AddPoint(square.Position);
                    }
                }
                else
                {
                    break;
                }
                if (closeSet.Count > 2500) break;
            } while (true);
            troop.MovabilityLeft = movabilityLeft;
            saveLastPath();
            ClearLists();
            return area;
        }

        public bool GetPath(Point start, Point end, MilitaryKind kind)
        {
            bool flag = false;
            ClearLists();
            GameSquare square = new GameSquare();
            square.Position = start;
            this.AddToCloseList(square);
            if (start == end)
            {
                lastPath = new List<Point>();
                lastPath.Add(start);
                return true;
            }
            do
            {
                CheckAdjacentSquares(square, end, true, kind);
                if (this.openDictionary.Count == 0)
                {
                    break;
                }
                square = this.AddToCloseList();
                if (square == null)
                {
                    break;
                }
                flag = square.Position == end;
                if (closeSet.Count > 2500) break;
            }
            while (!flag && (square.RealG < 0xdac));
            saveLastPath();
            ClearLists();
            return flag;
        }

        private int GetPenalizedCostByPosition(Point position, MilitaryKind kind)
        {
            if (this.OnGetPenalizedCost != null)
            {
                return this.OnGetPenalizedCost(position, kind);
            }
            return 0;
        }

        private int MakeSquare(GameSquare currentSquare, bool oblique, Point position, Point end, int DirectionCost, bool useAStar, MilitaryKind kind)
        {
            // 斜向格子的返回值不会被使用，已在关闭列表中时不必再计算代价
            if (oblique && closeSet.Contains(position))
            {
                return 0xdac;
            }
            int num = this.GetCostByPosition(position, oblique, DirectionCost, kind);
            if ((num < 0xdac) && !closeSet.Contains(position))
            {
                int num2;
                if (oblique)
                {
                    num2 = currentSquare.RealG + (7 * num);
                }
                else
                {
                    num2 = currentSquare.RealG + (5 * num);
                }
                openDictionary.TryGetValue(position, out GameSquare squareFromOpenList);
                if (squareFromOpenList == null || num2 < squareFromOpenList.RealG)
                {
                    GameSquare square = new GameSquare();
                    square.Parent = currentSquare;
                    square.Position = position;
                    square.PenalizedCost = this.GetPenalizedCostByPosition(position, kind);
                    if (useAStar)
                        square.H = distance(position, end);
                    square.RealG = num2;
                    this.AddToOpenList(square, useAStar);
                }
            }
            return num;
        }

        private GameSquare RemoveFromOpenList()
        {
            while (openList.TryDequeue(out GameSquare square, out _))
            {
                if (openDictionary.TryGetValue(square.Position, out GameSquare current) && current == square)
                {
                    openDictionary.Remove(square.Position);
                    return square;
                }
            }
            return null;
        }

        public void SetPath(List<Point> path)
        {
            path.Clear();
            path.AddRange(lastPath);
        }

        public delegate int GetCost(Point position, bool Oblique, int DirectionCost, MilitaryKind kind);

        public delegate int GetPenalizedCost(Point position, MilitaryKind kind);
    }
}

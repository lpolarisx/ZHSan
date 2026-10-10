using GameGlobal;
using GameManager;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace GameObjects.Animations
{
    public class CombatNumberItemList
    {
        #region  常量

        private const int MinTileWidth = 40;
        private const int FullScaleTileWidth = 100;
        private const float MinScale = 0.6f;
        private const double BaseDurationMs = 300;

        #endregion

        private readonly CombatNumberDirection direction;
        private readonly List<CombatNumberItem> numbers = new();

        private double startTime;
        private bool startDrawing;

        public CombatNumberItemList(CombatNumberDirection direction)
        {
            this.direction = direction;
        }

        public bool IsEmpty => numbers.Count == 0;

        public void AddNumber(int number, CombatNumberKind kind, Point position)
        {
            numbers.Add(new CombatNumberItem
            {
                Number = number,
                Kind = kind,
                Position = position,
            });

            startDrawing = false;
        }

        public void Clear() => numbers.Clear();

        public void Draw(CombatNumberGenerator generator, GetDisplayRectangle positionMethod, int tileWidth, GameTime gameTime)
        {
            if (Session.MainGame.mainGameScreen.PeekUndoneWork().Kind != UndoneWorkKind.None)
            {
                startDrawing = false;
                return;
            }

            double now = gameTime.TotalGameTime.TotalMilliseconds;
            if (!startDrawing)
            {
                startDrawing = true;
                startTime = now;
            }

            if (tileWidth < MinTileWidth)
            {
                Clear();
                return;
            }

            DrawNumbers(generator, positionMethod, GetScale(tileWidth));

            if (now - startTime >= GetDurationMs())
            {
                Clear();
            }
        }

        // 多个数字从格子中线开始，向上（或向下）逐行堆叠
        private void DrawNumbers(CombatNumberGenerator generator, GetDisplayRectangle positionMethod, float scale)
        {
            bool up = direction == CombatNumberDirection.上;

            for (int i = 0; i < numbers.Count; i++)
            {
                Rectangle rect = positionMethod(numbers[i].Position);
                int offset = (int)(generator.DigitHeight * i * scale);
                var anchor = new Point(
                    up ? rect.Left : rect.Right,
                    rect.Top + rect.Height / 2 + (up ? -offset : offset));

                numbers[i].Draw(generator, direction, anchor, scale);
            }
        }

        private static float GetScale(int tileWidth)
        {
            return tileWidth < FullScaleTileWidth
                ? Math.Max(tileWidth / (float)FullScaleTileWidth, MinScale)
                : 1f;
        }

        private static double GetDurationMs()
        {
            var vars = Setting.Current.GlobalVariables;
            return BaseDurationMs * vars.TroopMoveSpeed + vars.ShowNumberAddTime * 1000;
        }
    }
}

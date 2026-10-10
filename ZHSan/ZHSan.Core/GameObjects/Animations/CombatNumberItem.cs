using Microsoft.Xna.Framework;

namespace GameObjects.Animations
{
    public class CombatNumberItem
    {
        #region  常量

        private const float NumberDepth = 0.649f;
        private const float TroopQuantityDepth = 0.449f;

        #endregion

        public CombatNumberKind Kind;

        public int Number;

        public Point Position;

        public void Draw(CombatNumberGenerator generator, CombatNumberDirection direction, Point anchor, float scale)
        {
            if (direction == CombatNumberDirection.上)
            {
                DrawUp(generator, anchor, scale);
            }
            else
            {
                DrawDown(generator, anchor, scale);
            }
        }

        // 箭头在左，数字向右排列，整体位于锚点上方
        private void DrawUp(CombatNumberGenerator generator, Point anchor, float scale)
        {
            int step = (int)(generator.DigitWidth * scale);
            float y = anchor.Y - generator.DigitHeight * scale;

            generator.Draw(generator.GetArrowRectangle(Kind, CombatNumberDirection.上), new Vector2(anchor.X, y), scale, NumberDepth);

            int right = anchor.X + CountDigits(Number) * step;
            DrawDigits(generator, Kind, CombatNumberDirection.上, right, y, step, scale, NumberDepth);
        }

        // 箭头在右，数字向左排列，整体位于锚点下方
        private void DrawDown(CombatNumberGenerator generator, Point anchor, float scale)
        {
            int arrowX = anchor.X - (int)(generator.DigitWidth * scale);
            generator.Draw(generator.GetArrowRectangle(Kind, CombatNumberDirection.下), new Vector2(arrowX, anchor.Y), scale, NumberDepth);

            CombatNumberKind digitKind = Kind;
            float digitScale = scale;
            float depth = NumberDepth;
            if (Kind == CombatNumberKind.人数)
            {
                (digitKind, float enlarge) = GetTroopQuantityStyle(Number);
                digitScale = scale * enlarge;
                depth = TroopQuantityDepth;
            }

            int step = (int)(generator.DigitWidth * digitScale);
            DrawDigits(generator, digitKind, CombatNumberDirection.下, arrowX - step, anchor.Y, step, digitScale, depth);
        }

        // 从个位开始由右向左绘制，right为个位的x坐标
        private void DrawDigits(CombatNumberGenerator generator, CombatNumberKind kind, CombatNumberDirection direction, int right, float y, int step, float scale, float depth)
        {
            int x = right;
            int number = Number;
            do
            {
                generator.Draw(generator.GetDigitRectangle(kind, direction, number % 10), new Vector2(x, y), scale, depth);
                x -= step;
                number /= 10;
            }
            while (number > 0);
        }

        // 人数减少越多，数字越大，并借用其他类型所在行的颜色
        private static (CombatNumberKind Kind, float Enlarge) GetTroopQuantityStyle(int number)
        {
            return number switch
            {
                < 1000 => (CombatNumberKind.人数, 1f),
                < 3000 => (CombatNumberKind.士气, 1.3f),
                < 5000 => (CombatNumberKind.战意, 1.5f),
                _ => (CombatNumberKind.资金, 1.8f),
            };
        }

        private static int CountDigits(int number)
        {
            int count = 1;
            while ((number /= 10) > 0)
            {
                count++;
            }
            return count;
        }
    }
}

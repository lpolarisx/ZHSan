using GameManager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace GameObjects.Animations
{
    public class CombatNumberGenerator
    {
        #region  常量

        // 贴图布局：每行12列（10个数字+上下两个箭头），每种类型占上下两行
        private const int ColumnCount = 12;
        private const int ArrowColumn = 10;
        private const int RowsPerKind = 2;
        private const int DefaultTextureWidth = 144;
        private const int DefaultTextureHeight = 200;

        private static readonly int KindCount = Enum.GetValues(typeof(CombatNumberKind)).Length;

        #endregion

        public string TextureFileName;

        private PlatformTexture texture;
        private int digitWidth = DefaultTextureWidth / ColumnCount;
        private int digitHeight = DefaultTextureHeight / KindCount / RowsPerKind;

        public int DigitWidth
        {
            get
            {
                EnsureLoaded();
                return digitWidth;
            }
        }

        public int DigitHeight
        {
            get
            {
                EnsureLoaded();
                return digitHeight;
            }
        }

        public PlatformTexture Texture
        {
            get
            {
                EnsureLoaded();
                return texture;
            }
        }

        public Rectangle GetArrowRectangle(CombatNumberKind kind, CombatNumberDirection direction)
        {
            return GetCellRectangle(ArrowColumn + (int)direction, kind, direction);
        }

        public Rectangle GetDigitRectangle(CombatNumberKind kind, CombatNumberDirection direction, int digit)
        {
            return GetCellRectangle(digit, kind, direction);
        }

        public void Draw(Rectangle source, Vector2 position, float scale, float depth)
        {
            CacheManager.Draw(Texture, position, source, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, depth);
        }

        private Rectangle GetCellRectangle(int column, CombatNumberKind kind, CombatNumberDirection direction)
        {
            int row = (int)kind * RowsPerKind + (int)direction;
            return new Rectangle(DigitWidth * column, DigitHeight * row, DigitWidth, DigitHeight);
        }

        private void EnsureLoaded()
        {
            if (texture != null || string.IsNullOrEmpty(TextureFileName))
            {
                return;
            }

            texture = CacheManager.GetTempTexture(TextureFileName);

            // 按贴图实际尺寸计算单字大小
            Texture2D tex = CacheManager.LoadTexture(TextureFileName);
            if (tex != null)
            {
                digitWidth = tex.Width / ColumnCount;
                digitHeight = tex.Height / KindCount / RowsPerKind;
            }
        }
    }
}

using GameGlobal;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;


namespace GameFreeText
{

    public class SimpleText
    {
        public Font Builder = new Font()
        {
            Size = 14
        };

        public bool NewLine;
        public int Row;
        public string Text;
        public Color TextColor;
        public Rectangle TextPosition;
        //public Texture2D TextTexture;
                
        static Vector2 OneWidthHeight = new Vector2(28, 30);

        // public int Height
        // {
        //     get
        //     {
        //         return Convert.ToInt32(OneWidthHeight.Y * Builder.Scale);  // ((this.TextTexture != null) ? this.TextTexture.Height : 0);
        //     }
        // }

        // public int Width
        // {
        //     get
        //     {
        //         if (string.IsNullOrEmpty(Text)) return 0;

        //         return Convert.ToInt32(OneWidthHeight.X * Text.Length * Builder.Scale);  // ((this.TextTexture != null) ? this.TextTexture.Width : 0);
        //     }
        // }

        public int Width => string.IsNullOrEmpty(Text) ? 0 : (int)Math.Ceiling(GameManager.TextManager.MeasureText(Text, Builder.Scale).X);

        public int Height => (int)Math.Ceiling(GameManager.TextManager.MeasureText("国", Builder.Scale).Y);
    }
}
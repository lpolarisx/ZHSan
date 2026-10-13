using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FontStashSharp;
using GameManager;
using Tools;

namespace GamePanels.Scrollbar
{
    class TextContent : IFrameContent
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public Vector2 OffsetPos { get; set; }
        public float Scale { get; set; }
        public float Depth { get; set; }
        public List<Bounds> bounds { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public Frame baseFrame { get; set; }
        /// <summary>
        /// 要绘制的文本
        /// </summary>
        public string Text;
        /// <summary>
        /// 文本的颜色
        /// </summary>
        public Color color { get; set; }
        public float Alpha { get; set; }
        public SpriteFont Font;
        /// <summary>
        /// 是否自动换行
        /// </summary>
        public bool AutoWrap = false;
        public void DrawToCanvas(SpriteBatch batch)
        {
            CacheManager.DrawStringReturnBounds(batch, Font, Text, OffsetPos, color * Alpha, 0f, Vector2.Zero, Scale, SpriteEffects.None, Depth);

        }
        public void CalculateControlSize()
        {
            bounds = CacheManager.CalculateTextBounds(Font, Text, OffsetPos, Scale);
            if (bounds.Count == 0)//文本为空
            {
                Width = Height = 0;
                return;
            }
            //宽高是从控件位置到所有行最右、最下边界的距离，框架据此计算画布大小
            Width = bounds.Max(b => b.X2) - OffsetPos.X;
            Height = bounds.Max(b => b.Y2) - OffsetPos.Y;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="offsetPos">文字在画布内的偏移量坐标</param>
        /// <param name="text">文字文本</param>
        /// <param name="baseframe">包含文本控件的框架</param>
        /// <param name="textColor">文本的颜色</param>
        /// <param name="scale">文本的缩放倍数</param>
        /// <param name="depth">深度</param>
        public TextContent(Vector2 offsetPos, string text, Frame baseframe, Color? textColor,bool autoWrap=false, float scale = 1f, float depth = 0, string id = null, string name = null,SpriteFont font=null)
        {
            ID = id;
            Name = name;
            OffsetPos = offsetPos;
            Text = text;
            baseFrame = baseframe;
            Scale = scale;
            Depth = depth;
            Alpha = 1f;
            color = textColor ?? Color.White;
            Font = font ?? Session.Current.Font;
            AutoWrap = autoWrap;
            if (AutoWrap)
                AutoWrapText();
        }

        public void UpdateCanvas()
        {

        }

        public void AutoWrapText()
        {
            if (baseFrame != null && OffsetPos.X < baseFrame.VisualFrame.X + baseFrame.VisualFrame.Width)//自动换行以当前可视框架为范围，超过框架范围则无法自动换行
                Text=CacheManager.AutoWrap(Font, Text, baseFrame.VisualFrame.X + baseFrame.VisualFrame.Width - OffsetPos.X, Scale);
        }
    }
}

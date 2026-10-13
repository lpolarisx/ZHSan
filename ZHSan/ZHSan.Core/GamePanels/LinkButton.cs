using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using GameManager;
using Platforms;

namespace GamePanels
{
    /// <summary>
    /// 鏈接按鈕
    /// </summary>
    public class LinkButton
    {
        //全角字符的宽度，半角字符按一半计算
        int PerCharacterWidth = 28;
        int PerCharacterHeight = 30;
        public string Text { get; set; }
        public float TextScale = 1f;
        /// <summary>
        /// 按文字内容估算的宽度（未乘TextScale），点击范围和下划线都以它为准
        /// </summary>
        int TextWidth
        {
            get
            {
                int width = 0;
                foreach (char ch in Text ?? "")
                {
                    width += ch > 128 ? PerCharacterWidth : PerCharacterWidth / 2;
                }
                return width;
            }
        }
        public bool FireEventWhenUnEnable = false;
        public bool MouseOver { get; set; }
        public bool Visible { get; set; }
        public bool Enable { get; set; }
        public Vector2 Position { get; set; }
        public Color Color = Color.White;
        public bool PreMouseOver { get; set; }
        public event ButtonPressEventHandler OnButtonPress;
        DateTime? lastClickTime;
        public LinkButton(string name)
        {
            Text = name;
            Visible = Enable = true; MouseOver = false;
        }
        public void ClearOnButtonPress()
        {
            OnButtonPress = null;
        }
        public void OnClick()
        {
            if (OnButtonPress != null)
            {
                OnButtonPress.Invoke(this, null);
            }
        }
        public void Update(Vector2? basePos, bool sound)
        {
            MouseOver = IsInTexture(InputManager.PoX, InputManager.PoY, basePos);
            if (MouseOver)
            {
                if (!PreMouseOver && sound) Platform.Current.PlayEffect(@"Content\Sound\Move");
                if (InputManager.IsPressed)
                {
                    if (!Platform.IsActive)
                    {
                        return;
                    }
                    if (lastClickTime != null)
                    {
                        TimeSpan ts = DateTime.Now - (DateTime)lastClickTime;
                        if (ts.TotalSeconds < 0.3f)
                        {
                            return;
                        }
                    }
                    lastClickTime = DateTime.Now;
                    OnClick();
                }
            }
        }
        public bool IsInTexture(int poX, int poY, Vector2? basePos)
        {
            if (Visible && (Enable || FireEventWhenUnEnable))
            {
                PreMouseOver = MouseOver;
                Vector2 pos = Position + (basePos ?? Vector2.Zero);
                MouseOver = pos.X <= poX && poX <= pos.X + TextWidth * TextScale
                    && pos.Y <= poY && poY <= pos.Y + PerCharacterHeight * TextScale;
            }
            else
            {
                MouseOver = false;
            }
            return MouseOver;
        }
        public void Draw()
        {
            Draw(1f);
        }
        public void Draw(float alpha)
        {
            if (Visible)
            {
                CacheManager.DrawString(Session.Current.Font, Text, Position, (Enable ? Color : Color.Silver) * alpha, 0f, Vector2.Zero, TextScale, Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0f);
                if (MouseOver)
                {
                    //下划线贴图会再按TextScale放大，所以源矩形用缩放前的宽度
                    Rectangle rec = new Rectangle(0, 0, TextWidth, 1);
                    CacheManager.Draw(@"Content\Textures\Resources\Start\Line-Red", Position + new Vector2(0, (PerCharacterHeight - 5) * TextScale), rec, Color.White * alpha, Microsoft.Xna.Framework.Graphics.SpriteEffects.None, TextScale);
                }
            }
        }
    }
}

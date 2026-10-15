using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;
using Platforms;

namespace GameManager
{
    /// <summary>
    /// 按钮的三种外观，各对应一张基础图
    /// </summary>
    public enum MyraButtonLook
    {
        /// <summary>
        /// 平时蓝色，鼠标移上去变绿
        /// </summary>
        Normal,

        /// <summary>
        /// 选中，一直是绿色
        /// </summary>
        Selected,

        /// <summary>
        /// 灰色，表示不可用
        /// </summary>
        Disabled
    }

    /// <summary>
    /// Myra界面共用的基础图：一张窗口底板、三种状态的按钮，都不带文字，按九宫格拉伸到需要的大小。
    /// 新做的Myra窗口尽量从这里取底板和按钮，外观才统一。
    /// </summary>
    public static class MyraSkin
    {
        private const string TexturePath = "Content/Textures/GameComponents/Common/Data/";

        /// <summary>
        /// 底板四周花纹的宽度，窗口内容要留出这么宽的边
        /// </summary>
        public const int PanelBorder = 20;

        /// <summary>
        /// 底板上方标题栏的高度，标题栏下沿有一条横线，标题写在横线上面，内容从横线下面开始
        /// </summary>
        public const int PanelTitleHeight = 41;

        // 与按钮图的原始高度一致，纵向不用拉伸，花纹最清楚
        public const int ButtonHeight = 30;

        // 文字配色：降低饱和度，避免纯白和荧光色，接近古典策略游戏的氛围

        /// <summary>
        /// 主要文字：君主名称、正文
        /// </summary>
        public static readonly Color TextPrimary = new Color(0xDD, 0xD9, 0xCB);

        /// <summary>
        /// 次要文字：列表数值、辅助信息
        /// </summary>
        public static readonly Color TextSecondary = new Color(0xA1, 0xB0, 0xB0);

        /// <summary>
        /// 标题金色：窗口标题、区域标题
        /// </summary>
        public static readonly Color TextTitle = new Color(0xC9, 0xAA, 0x68);

        /// <summary>
        /// 重点金色：重要数值、资金，也用作选中项的边框
        /// </summary>
        public static readonly Color TextHighlight = new Color(0xE1, 0xBE, 0x74);

        /// <summary>
        /// 友好青绿：同盟、正向关系
        /// </summary>
        public static readonly Color TextFriendly = new Color(0x69, 0xBD, 0x87);

        /// <summary>
        /// 敌对赭红：敌对、负向关系
        /// </summary>
        public static readonly Color TextHostile = new Color(0xC7, 0x76, 0x68);

        /// <summary>
        /// 警告橙黄：条件不足、消耗提示
        /// </summary>
        public static readonly Color TextWarning = new Color(0xDD, 0xA6, 0x5A);

        /// <summary>
        /// 禁用灰色：不可点击的操作。灰色按钮底上太暗看不清，按钮文字用TextSecondary
        /// </summary>
        public static readonly Color TextDisabled = new Color(0x77, 0x85, 0x89);

        /// <summary>
        /// 列表行、卡片的底色：深青灰
        /// </summary>
        public static readonly Color ListRowColor = new Color(26, 38, 46);

        /// <summary>
        /// 列表行、卡片鼠标移上去的底色
        /// </summary>
        public static readonly Color ListRowOverColor = new Color(44, 64, 76);

        /// <summary>
        /// 列表行选中的底色，取自按钮图的蓝
        /// </summary>
        public static readonly Color ListRowSelectedColor = new Color(6, 86, 124);

        // 按钮图四角花纹的大小。图高30，上下各切14，中间只剩2行纯色用来拉伸
        private const int ButtonCorner = 14;

        private static readonly IBrush ButtonNormal = new TextureBackground(TexturePath + "Button.png", new Thickness(ButtonCorner), new Color(6, 86, 124));
        private static readonly IBrush ButtonSelected = new TextureBackground(TexturePath + "ButtonSelected.png", new Thickness(ButtonCorner), new Color(21, 117, 2));
        private static readonly IBrush ButtonDisabled = new TextureBackground(TexturePath + "ButtonDisabled.png", new Thickness(ButtonCorner), new Color(95, 95, 97));
        private static readonly IBrush Panel = new TextureBackground(TexturePath + "Panel.png", new Thickness(PanelBorder, PanelTitleHeight, PanelBorder, PanelBorder), new Color(38, 54, 64));

        /// <summary>
        /// 小图标的大小，与16号字的行高相配，图片按原大小显示
        /// </summary>
        public const int IconSize = 16;

        /// <summary>
        /// 创建警告图标，放在条件不足等提示文字前面。图片Warning.png不存在时返回null，调用方自己用文字代替
        /// </summary>
        public static Image CreateWarningIcon()
        {
            string name = TexturePath + "Warning.png";
            if (!Platform.Current.FileExists(name))
            {
                return null;
            }
            Texture2D texture = CacheManager.LoadTexture(name);
            if (texture == null)
            {
                return null;
            }
            return new Image
            {
                Renderable = new TextureRegion(texture),
                Width = IconSize,
                Height = IconSize
            };
        }

        /// <summary>
        /// 窗口底板，带标题栏横线。窗口里的分区也可以用
        /// </summary>
        public static IBrush PanelBackground
        {
            get
            {
                return Panel;
            }
        }

        /// <summary>
        /// 创建一个带文字的按钮
        /// </summary>
        /// <param name="text">按钮上的文字</param>
        /// <param name="width">按钮宽度，高度固定为ButtonHeight</param>
        /// <param name="label">按钮上的文字控件，需要改文字颜色时用</param>
        public static Button CreateButton(string text, int width, out Label label)
        {
            label = new Label
            {
                Text = text,
                Font = TextManager.GetFont(16),
                TextColor = TextPrimary,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            var button = new Button
            {
                Content = label,
                Width = width,
                Height = ButtonHeight,
                // 用Enabled禁用的按钮固定显示灰色图
                DisabledBackground = ButtonDisabled
            };
            SetButtonLook(button, MyraButtonLook.Normal);
            return button;
        }

        /// <summary>
        /// 切换按钮的外观。Disabled只是换成灰色图，按钮仍然可以点，要彻底禁用请设置按钮的Enabled
        /// </summary>
        public static void SetButtonLook(Button button, MyraButtonLook look)
        {
            IBrush background = look == MyraButtonLook.Selected ? ButtonSelected : look == MyraButtonLook.Disabled ? ButtonDisabled : ButtonNormal;
            IBrush over = look == MyraButtonLook.Normal ? ButtonSelected : background;
            button.Background = background;
            button.FocusedBackground = background;
            button.OverBackground = over;
            button.PressedBackground = over;
        }

        // 用一张图片做底，按九宫格拉伸到任意大小：四角和四边保持原样，只拉伸中间。
        // 材质可能被游戏的缓存清理掉，所以每次画之前检查一下，失效了就重新取
        private class TextureBackground : IBrush
        {
            private readonly string textureName;
            private readonly Thickness border;
            private readonly Color fallbackColor;
            private Texture2D texture;
            private NinePatchRegion region;

            public TextureBackground(string textureName, Thickness border, Color fallbackColor)
            {
                this.textureName = textureName;
                this.border = border;
                this.fallbackColor = fallbackColor;
            }

            public void Draw(RenderContext context, Rectangle dest, Color color)
            {
                if ((this.texture == null || this.texture.IsDisposed) && Platform.Current.FileExists(this.textureName))
                {
                    this.texture = CacheManager.LoadTexture(this.textureName);
                    this.region = this.texture == null ? null : new NinePatchRegion(this.texture, this.texture.Bounds, this.border);
                }
                if (this.texture == null || this.texture.IsDisposed)
                {
                    // 找不到图片时退回纯色
                    context.FillRectangle(dest, this.fallbackColor);
                }
                else
                {
                    this.region.Draw(context, dest, color);
                }
            }
        }
    }
}

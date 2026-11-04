using GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Myra;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.UI;
using Platforms;
using System;
using System.Collections.Generic;

namespace GameManager
{
    /// <summary>
    /// 基于Myra的UI层，画在游戏原有界面之上，目前只用来承载模态弹窗。
    /// 弹窗可以一层层叠起来，例如在人物详情里打开赏赐宝物，关掉上层就回到下层。
    /// 原有界面在弹窗显示期间应通过IsBlockingInput判断并停止响应鼠标。
    /// 弹窗打开之后，原有界面又弹出自己的窗口（例如赏赐宝物后人物说话的图片文字）时，Myra暂时让开，
    /// 不画也不接收输入，等那个窗口关掉再回来。
    /// </summary>
    public static class MyraUI
    {
        private const string MouseArrowTexture = @"Content\Textures\Resources\MouseArrow\Normal.png";

        // 叠在下层弹窗上面的遮罩颜色，与MyraPicker的遮罩一致
        private static readonly Color CoverColor = new Color(0, 0, 0, 120);

        private static Desktop desktop;
        // 所有弹窗都放在这个铺满窗口的容器里，后加的盖在上面
        private static Panel layerHost;
        private static readonly List<Layer> layers = new List<Layer>();
        private static Action afterCloseAction;
        // 弹窗关闭时鼠标键可能还按着，等松开之后再把输入还给原有界面，避免点穿
        private static bool waitingForRelease;
        private static bool rightButtonWasDown;

        public static Desktop Desktop
        {
            get
            {
                if (desktop == null)
                {
                    desktop = new Desktop
                    {
                        // Myra默认以排版区域的中心为原点缩放，放大后左上角会移出窗口。改成从左上角放大，和游戏画面的缩放一致
                        TransformOrigin = Vector2.Zero
                    };
                    // Myra的Scale只放大绘制，排版仍按窗口的真实像素。这里给出除以缩放倍数后的大小，
                    // 铺满窗口的遮罩放大后正好盖住整个窗口，居中的弹窗也还在正中
                    desktop.BoundsFetcher = () =>
                    {
                        Viewport viewport = Platform.GraphicsDevice.Viewport;
                        Vector2 scale = desktop.Scale;
                        return new Rectangle(0, 0, (int)Math.Round(viewport.Width / scale.X), (int)Math.Round(viewport.Height / scale.Y));
                    };
                    layerHost = new Panel();
                    desktop.Root = layerHost;
                }
                return desktop;
            }
        }

        /// <summary>
        /// 弹窗可用的排版大小，即窗口大小除以缩放倍数。需要随窗口大小调整布局的弹窗在创建时取这个值
        /// </summary>
        public static Point LayoutSize
        {
            get
            {
                Viewport viewport = Platform.GraphicsDevice.Viewport;
                Vector2 scale = GetScale();
                return new Point((int)Math.Round(viewport.Width / scale.X), (int)Math.Round(viewport.Height / scale.Y));
            }
        }

        /// <summary>
        /// 弹窗的缩放倍数，排版的1个单位画出来是这么多个屏幕像素。自己生成图片的控件按这个倍数生成，放大后才清楚
        /// </summary>
        public static Vector2 Scale => GetScale();

        // 手机上游戏画面整体缩放过，弹窗跟着用同样的倍数
        private static Vector2 GetScale()
        {
            // 主菜单上Desktop不缩放，Desktop.Scale是把画好的图拉伸，字会发虚；弹窗自己按MenuScale放大字号和尺寸
            if (IsMenuScreen)
            {
                return Vector2.One;
            }
            Matrix scale = Session.MainGame.SpriteScale2;
            return Session.MainGame.disScale ? new Vector2(scale.M11, scale.M22) : Vector2.One;
        }

        /// <summary>
        /// 游戏初始化时调用。Myra创建控件时会加载默认样式，必须在此之前设置好Game
        /// </summary>
        public static void Initialize(Game game)
        {
            MyraEnvironment.Game = game;
            // 字形已经按缩放倍数生成，放大后和屏幕像素一一对应。缩放1.5倍时文字常落在半个像素上，
            // 线性插值会把每个字形像素和旁边的混在一起，字发虚；取最近的像素就清楚
            MyraEnvironment.TextTextureFiltering = Myra.Graphics2D.TextureFiltering.Nearest;
        }

        /// <summary>
        /// 是否有弹窗正在显示
        /// </summary>
        public static bool IsShowing
        {
            get
            {
                return layers.Count > 0;
            }
        }

        private static bool IsMenuScreen => Session.MainGame.mainGameScreen == null || Session.MainGame.loadingScreen != null;

        /// <summary>
        /// 主菜单上弹窗应放大的倍数。主菜单按1280*720设计后整体缩放到窗口，弹窗的字号和尺寸乘上这个倍数才跟主菜单一样大
        /// </summary>
        public static float MenuScale
        {
            get
            {
                Matrix scale = Session.MainGame.SpriteScale1;
                return Math.Max(0.5f, Math.Min(scale.M11, scale.M22));
            }
        }

        /// <summary>
        /// 原有界面此时是否应当忽略鼠标操作
        /// </summary>
        public static bool IsBlockingInput
        {
            get
            {
                return (IsShowing && !IsSuspended) || waitingForRelease;
            }
        }

        // 最上层弹窗打开之后，原有界面又压了新的未完成工作，说明它弹出了自己的窗口，Myra要让开
        private static bool IsSuspended
        {
            get
            {
                return IsShowing && GetUndoneWorkCount() > layers[layers.Count - 1].UndoneWorkCount;
            }
        }

        private static int GetUndoneWorkCount()
        {
            Screen screen = Session.MainGame?.mainGameScreen;
            return screen == null ? 0 : screen.UndoneWorks.Count;
        }

        /// <summary>
        /// 显示一个模态弹窗。已有弹窗时叠在最上层，下层弹窗被遮罩盖住，不再响应鼠标
        /// </summary>
        /// <param name="root">弹窗的根控件，放在铺满整个窗口的容器里，按自己的对齐方式摆放</param>
        /// <param name="onCancel">点击鼠标右键时调用，与游戏里右键关闭窗口的习惯一致</param>
        public static void Show(Widget root, Action onCancel)
        {
            // 第一次显示时才创建Desktop和容器
            _ = Desktop;
            Widget widget = root;
            if (layers.Count > 0)
            {
                // 遮罩带底色才会接住鼠标，点不到下层弹窗
                var cover = new Panel
                {
                    Background = new SolidBrush(CoverColor)
                };
                cover.Widgets.Add(root);
                widget = cover;
            }
            // 弹窗在Show之前就压好了自己的未完成工作，这时的深度算作它本身的
            layers.Add(new Layer(root, widget, onCancel, GetUndoneWorkCount()));
            layerHost.Widgets.Add(widget);
            // 打开弹窗的那次右键不算作取消
            rightButtonWasDown = false;
        }

        /// <summary>
        /// 关闭最上层的弹窗，下层弹窗重新响应鼠标
        /// </summary>
        /// <param name="afterClose">等鼠标键松开的时候调用。关闭后要接着打开原有界面的窗口时放在这里，避免关闭弹窗的那次点击落到新窗口上</param>
        public static void Close(Action afterClose = null)
        {
            if (IsShowing)
            {
                Layer top = layers[layers.Count - 1];
                layers.RemoveAt(layers.Count - 1);
                layerHost.Widgets.Remove(top.Widget);
                waitingForRelease = true;
                afterCloseAction += afterClose;
            }
        }

        /// <summary>
        /// 弹窗是否在最上层。Desktop的触摸事件各层弹窗都会收到，下层弹窗要用这个判断，不去处理上层弹窗上的拖动
        /// </summary>
        /// <param name="root">弹窗的根控件，即传给Show的那个</param>
        public static bool IsTop(Widget root)
        {
            return layers.Count > 0 && layers[layers.Count - 1].Root == root;
        }

        /// <summary>
        /// 在游戏原有界面画完之后调用。Myra在这里同时处理输入和绘制
        /// </summary>
        public static void Draw()
        {
            MouseState mouse = Mouse.GetState();
            bool anyButtonDown = mouse.LeftButton == ButtonState.Pressed || mouse.RightButton == ButtonState.Pressed;
            if (waitingForRelease && !anyButtonDown)
            {
                waitingForRelease = false;
                Action afterClose = afterCloseAction;
                afterCloseAction = null;
                afterClose?.Invoke();
            }
            bool rightButtonDown = mouse.RightButton == ButtonState.Pressed;
            if (!IsShowing || IsSuspended)
            {
                // 让开期间也记下右键状态，回来时不会把之前的右键当成取消
                rightButtonWasDown = rightButtonDown;
                return;
            }

            desktop.Scale = GetScale();
            desktop.Render();

            // 右键只取消最上层的弹窗
            if (rightButtonWasDown && !rightButtonDown && IsShowing)
            {
                layers[layers.Count - 1].Cancel?.Invoke();
            }
            rightButtonWasDown = rightButtonDown;

            // 电脑上游戏的鼠标指针是自己画的，会被弹窗盖住，这里在最上层补画一次
            if (Platform.PlatFormType == PlatFormType.Win || Platform.PlatFormType == PlatFormType.Desktop)
            {
                // 鼠标位置是除以InputManager.Scale得到的逻辑坐标，这里乘回同样的倍数。
                // 开始菜单按1280x720换算，和弹窗的缩放倍数不是一回事，不能用弹窗的
                Vector2 mouseScale = InputManager.Scale;
                Session.Current.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Matrix.CreateScale(mouseScale.X, mouseScale.Y, 1));
                CacheManager.Draw(MouseArrowTexture, InputManager.Position, Color.White);
                Session.Current.SpriteBatch.End();
            }
        }

        private class Layer
        {
            // 弹窗自己的根控件
            public readonly Widget Root;
            // 实际放进容器的控件，上层弹窗是套在外面的遮罩
            public readonly Widget Widget;
            public readonly Action Cancel;
            // 打开时游戏界面未完成工作的数量
            public readonly int UndoneWorkCount;

            public Layer(Widget root, Widget widget, Action cancel, int undoneWorkCount)
            {
                this.Root = root;
                this.Widget = widget;
                this.Cancel = cancel;
                this.UndoneWorkCount = undoneWorkCount;
            }
        }
    }
}

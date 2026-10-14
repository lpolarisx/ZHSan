using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Myra;
using Myra.Graphics2D.UI;
using Platforms;
using System;

namespace GameManager
{
    /// <summary>
    /// 基于Myra的UI层，画在游戏原有界面之上，目前只用来承载模态弹窗。
    /// 原有界面在弹窗显示期间应通过IsBlockingInput判断并停止响应鼠标。
    /// </summary>
    public static class MyraUI
    {
        private const string MouseArrowTexture = @"Content\Textures\Resources\MouseArrow\Normal.png";

        private static Desktop desktop;
        private static Action cancelAction;
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
                    desktop = new Desktop();
                }
                return desktop;
            }
        }

        /// <summary>
        /// 游戏初始化时调用。Myra创建控件时会加载默认样式，必须在此之前设置好Game
        /// </summary>
        public static void Initialize(Game game)
        {
            MyraEnvironment.Game = game;
        }

        /// <summary>
        /// 是否有弹窗正在显示
        /// </summary>
        public static bool IsShowing
        {
            get
            {
                return desktop != null && desktop.Root != null;
            }
        }

        /// <summary>
        /// 原有界面此时是否应当忽略鼠标操作
        /// </summary>
        public static bool IsBlockingInput
        {
            get
            {
                return IsShowing || waitingForRelease;
            }
        }

        /// <summary>
        /// 显示一个模态弹窗
        /// </summary>
        /// <param name="root">弹窗的根控件，会铺满整个窗口</param>
        /// <param name="onCancel">点击鼠标右键时调用，与游戏里右键关闭窗口的习惯一致</param>
        public static void Show(Widget root, Action onCancel)
        {
            cancelAction = onCancel;
            // 打开弹窗的那次右键不算作取消
            rightButtonWasDown = false;
            Desktop.Root = root;
        }

        /// <summary>
        /// 关闭弹窗
        /// </summary>
        /// <param name="afterClose">等鼠标键松开、输入还给原有界面的时候调用。关闭后要接着打开原有界面的窗口时放在这里，避免关闭弹窗的那次点击落到新窗口上</param>
        public static void Close(Action afterClose = null)
        {
            if (IsShowing)
            {
                desktop.Root = null;
                cancelAction = null;
                waitingForRelease = true;
                afterCloseAction = afterClose;
            }
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
            if (!IsShowing)
            {
                return;
            }

            // 手机上游戏画面整体缩放过，弹窗跟着用同样的倍数
            Matrix scale = Session.MainGame.SpriteScale2;
            desktop.Scale = Session.MainGame.disScale ? new Vector2(scale.M11, scale.M22) : Vector2.One;
            desktop.Render();

            bool rightButtonDown = mouse.RightButton == ButtonState.Pressed;
            if (rightButtonWasDown && !rightButtonDown && cancelAction != null)
            {
                cancelAction();
            }
            rightButtonWasDown = rightButtonDown;

            // 电脑上游戏的鼠标指针是自己画的，会被弹窗盖住，这里在最上层补画一次
            if (Platform.PlatFormType == PlatFormType.Win || Platform.PlatFormType == PlatFormType.Desktop)
            {
                Session.Current.SpriteBatch.Begin();
                CacheManager.Draw(MouseArrowTexture, InputManager.Position, Color.White);
                Session.Current.SpriteBatch.End();
            }
        }
    }
}

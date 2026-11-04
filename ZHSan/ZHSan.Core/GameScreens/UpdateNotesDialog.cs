using GameManager;
using Microsoft.Xna.Framework;
using Myra.Graphics2D;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.UI;
using Platforms;
using System;
using System.IO;
using Tools;

namespace WorldOfTheThreeKingdoms.GameScreens
{
    /// <summary>
    /// 更新说明：版本号（程序目录下的version.txt）和上次看过的不一样时，进主菜单弹一次程序目录下changelog.txt的内容。
    /// 玩家解压覆盖更新只会覆盖程序目录，看过的版本记在用户目录下，不会被覆盖掉
    /// </summary>
    internal class UpdateNotesDialog
    {
        private const string VersionFile = "version.txt";
        private const string ChangelogFile = "changelog.txt";
        private const string SeenVersionFile = "seenVersion.txt";

        private const int DialogWidth = 640;
        private const int DialogPadding = 28;
        private const int CloseButtonSize = 32;
        private const int ContentWidth = DialogWidth - 2 * DialogPadding;
        private const int MaxContentHeight = 420;

        private readonly string version;

        private UpdateNotesDialog(string version)
        {
            this.version = version;
        }

        /// <summary>
        /// 读取程序目录下version.txt里的版本号，没有时返回空字符串
        /// </summary>
        public static string ReadVersion()
        {
            return ReadAppText(VersionFile).Trim();
        }

        /// <summary>
        /// 版本更新后第一次进游戏时显示更新说明
        /// </summary>
        public static void ShowIfNewVersion()
        {
            try
            {
                string version = ReadVersion();
                if (version == "") return;

                string seen = Platform.Current.UserFileExist(SeenVersionFile) ? Platform.Current.GetUserText(SeenVersionFile).Trim() : "";
                if (seen == version) return;

                string notes = ReadAppText(ChangelogFile).Trim();
                if (notes == "")
                {
                    // 这一版没写更新说明，直接记为看过
                    Platform.Current.SaveUserFile(SeenVersionFile, version);
                    return;
                }

                var dialog = new UpdateNotesDialog(version);
                MyraUI.Show(dialog.Build(notes), dialog.Close);
            }
            catch (Exception ex)
            {
                WebTools.TakeWarnMsg("显示更新说明失败", "UpdateNotesDialog:", ex);
            }
        }

        private static string ReadAppText(string file)
        {
            string path = Path.Combine(AppContext.BaseDirectory, file);
            return File.Exists(path) ? File.ReadAllText(path) : "";
        }

        private void Close()
        {
            // 关掉才算看过，中途退出游戏下次还会再弹
            Platform.Current.SaveUserFile(SeenVersionFile, this.version);
            MyraUI.Close();
        }

        private Widget Build(string notes)
        {
            // 主菜单按1280*720整体缩放到窗口，弹窗按同样倍数放大字号和尺寸（直接用大字号画，字才不会发虚）。
            // 主菜单上Desktop不放大，字体也不能按界面缩放生成字形，要用GetUnscaledFont
            float s = MyraUI.MenuScale;
            int Scaled(int size) => (int)Math.Round(size * s);

            var dialog = new VerticalStackPanel
            {
                Width = Scaled(DialogWidth),
                Spacing = Scaled(12),
                Padding = new Thickness(Scaled(DialogPadding), Scaled(7), Scaled(DialogPadding), Scaled(DialogPadding)),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Background = MyraSkin.PanelBackground
            };
            var titleRow = new Panel();
            titleRow.Widgets.Add(new Label
            {
                Text = "更新说明 " + this.version,
                Font = TextManager.GetUnscaledFont(24 * s),
                TextColor = MyraSkin.TextTitle,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            });
            Button close = MyraWidgets.CreateCloseButton(this.Close);
            close.Width = close.Height = Scaled(CloseButtonSize);
            close.VerticalAlignment = VerticalAlignment.Center;
            ((Label)close.Content).Font = TextManager.GetUnscaledFont(28 * s);
            titleRow.Widgets.Add(close);
            dialog.Widgets.Add(titleRow);

            var content = new Label
            {
                // 更新说明是纯文本，不解析Myra的/c、/n之类的格式命令
                SupportsCommands = false,
                Text = notes.Replace("\r\n", "\n"),
                Font = TextManager.GetUnscaledFont(16 * s),
                TextColor = MyraSkin.TextPrimary,
                Wrap = true,
                Width = Scaled(ContentWidth - 20)
            };
            dialog.Widgets.Add(new ScrollViewer
            {
                Content = content,
                ShowHorizontalScrollBar = false,
                Width = Scaled(ContentWidth),
                // 内容短时跟着内容高度走，超出最大高度就滚动
                MaxHeight = Scaled(MaxContentHeight)
            });

            // 半透明遮罩铺满窗口，压暗后面的主菜单
            var mask = new Panel
            {
                Background = new SolidBrush(new Color(0, 0, 0, 150))
            };
            mask.Widgets.Add(dialog);
            return mask;
        }
    }
}

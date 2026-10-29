using GameObjects.Conditions;
using GameObjects.Influences;
using Microsoft.Xna.Framework;
using Myra.Events;
using Myra.Graphics2D;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GameManager
{
    /// <summary>
    /// 人物详情、部队详情等Myra弹窗共用的小控件和配色。用的地方写using static GameManager.MyraWidgets，直接调用
    /// </summary>
    public static class MyraWidgets
    {
        /// <summary>
        /// 分区标题行的高度
        /// </summary>
        public const int SectionTitleHeight = 30;

        // 卡片、边框、进度条的配色，文字颜色用MyraSkin的
        public static readonly Color CardColor = MyraSkin.ListRowColor;
        public static readonly Color HoverColor = MyraSkin.ListRowOverColor;
        public static readonly Color LineColor = new Color(90, 110, 120);
        public static readonly Color BarTrackColor = new Color(16, 24, 30);
        public static readonly Color GoldDim = new Color(0x8A, 0x74, 0x48);
        public static readonly Color PopoverColor = new Color(30, 44, 54);
        public static readonly IBrush Transparent = new SolidBrush(Color.Transparent);

        // 这几种影响本身不加数值，而是限定后面的效果何时生效：仅君主、仅部队主将、满足条件、限定兵种类型、限定兵种。
        // 说明里把它们写在“效果”标题后面
        private static readonly int[] RestrictionInfluenceKinds = { 280, 281, 285, 290, 300 };

        // 竖向间隔。Myra里固定了高度的控件，外边距算在高度里面，所以分区之间的空隙单独用一个空控件
        public static Widget CreateSpacer(int height)
        {
            return new Panel
            {
                Height = height
            };
        }

        public static Label CreateLabel(string text, int fontSize, Color color)
        {
            return new Label
            {
                Text = text,
                Font = TextManager.GetFont(fontSize),
                TextColor = color,
                HorizontalAlignment = HorizontalAlignment.Left
            };
        }

        public static Label CreateWrappedLabel(string text, int fontSize, Color color, int width)
        {
            return new Label
            {
                Text = text,
                Font = TextManager.GetFont(fontSize),
                TextColor = color,
                Wrap = true,
                Width = width
            };
        }

        // 分区标题：大字标题，后面可以跟一个小字的数量
        public static Widget CreateSectionTitle(string title, string count, out Label countLabel)
        {
            var row = new HorizontalStackPanel
            {
                Spacing = 8,
                Height = SectionTitleHeight
            };
            Label label = CreateLabel(title, 18, MyraSkin.TextPrimary);
            label.VerticalAlignment = VerticalAlignment.Top;
            row.Widgets.Add(label);
            countLabel = null;
            if (count != null)
            {
                countLabel = CreateLabel(count, 13, MyraSkin.TextSecondary);
                countLabel.VerticalAlignment = VerticalAlignment.Top;
                countLabel.Margin = new Thickness(0, 5, 0, 0);
                row.Widgets.Add(countLabel);
            }
            return row;
        }

        // 格子里的小字名称在上、数值在下，两行都居中，整体在格子中间
        public static Widget CreateCellContent(string title, string value, int valueFontSize, Color valueColor)
        {
            var content = new VerticalStackPanel
            {
                Spacing = 2,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            Label label = CreateLabel(title, 12, MyraSkin.TextSecondary);
            label.HorizontalAlignment = HorizontalAlignment.Center;
            content.Widgets.Add(label);
            Label number = CreateLabel(value, valueFontSize, valueColor);
            number.HorizontalAlignment = HorizontalAlignment.Center;
            content.Widgets.Add(number);
            return content;
        }

        // 小标签：同色细边框，底色是同色的淡色。游戏按预乘透明度混合，所以淡色用颜色乘系数得到
        public static Widget CreateTag(string text, Color color)
        {
            var tag = new Panel
            {
                Padding = new Thickness(8, 1, 8, 1),
                Background = new FrameBrush(color * 0.16f, color, false)
            };
            tag.Widgets.Add(CreateLabel(text, 13, color));
            return tag;
        }

        public static Widget CreateBar(int width, int height, int value, int max, Color color)
        {
            var bar = new Panel
            {
                Width = width,
                Height = height,
                HorizontalAlignment = HorizontalAlignment.Left,
                Background = new SolidBrush(BarTrackColor)
            };
            int filled = max <= 0 ? 0 : (int)Math.Round((double)width * MathHelper.Clamp(value, 0, max) / max);
            if (filled > 0)
            {
                bar.Widgets.Add(new Panel
                {
                    Width = filled,
                    Height = height,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Background = new SolidBrush(color)
                });
            }
            return bar;
        }

        // 扁平按钮：平时用给定的底，鼠标移上去整块稍微变亮
        public static Button CreateFlatButton(Widget content, int width, int height, IBrush normal)
        {
            var button = new Button
            {
                Content = content,
                Width = width,
                Height = height
            };
            SetFlatBackground(button, normal);
            return button;
        }

        public static void SetFlatBackground(Button button, IBrush brush)
        {
            button.Background = brush;
            button.FocusedBackground = brush;
            IBrush over = brush is FrameBrush frame ? frame.WithFill(HoverColor) : new SolidBrush(HoverColor);
            button.OverBackground = over;
            button.PressedBackground = over;
        }

        // 关闭按钮只画一个“×”，鼠标移上去变成金色
        public static Button CreateCloseButton(Action onClick)
        {
            var label = new Label
            {
                Text = "×",
                Font = TextManager.GetFont(28),
                TextColor = MyraSkin.TextSecondary,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            var close = new Button
            {
                Content = label,
                Width = 32,
                Height = 32,
                HorizontalAlignment = HorizontalAlignment.Right,
                Background = Transparent,
                FocusedBackground = Transparent,
                PressedBackground = Transparent,
                OverBackground = Transparent
            };
            close.MouseEntered += (sender, args) => label.TextColor = MyraSkin.TextHighlight;
            close.MouseLeft += (sender, args) => label.TextColor = MyraSkin.TextSecondary;
            close.Click += (sender, args) => onClick();
            return close;
        }

        // 小标题和上一段之间的空隙放在外层容器的内边距里，固定了宽度的文字自己不带外边距
        public static Widget CreateDetailHeader(string title, int width)
        {
            var holder = new Panel
            {
                Padding = new Thickness(0, 10, 0, 0)
            };
            holder.Widgets.Add(CreateWrappedLabel(title, 12, MyraSkin.TextSecondary, width));
            return holder;
        }

        // 限定生效时机的影响写在“效果”标题后面，其余的逐条列出
        public static void AddInfluences(VerticalStackPanel panel, IEnumerable<Influence> influences, int width)
        {
            List<Influence> list = influences.ToList();
            if (list.Count == 0)
            {
                return;
            }
            List<string> restrictions = list.Where(x => RestrictionInfluenceKinds.Contains(x.Kind.ID)).Select(x => x.Description).ToList();
            string title = restrictions.Count == 0 ? "效果" : "效果 · " + string.Join(" · ", restrictions);
            panel.Widgets.Add(CreateDetailHeader(title, width));
            foreach (Influence influence in list.Where(x => !RestrictionInfluenceKinds.Contains(x.Kind.ID)))
            {
                panel.Widgets.Add(CreateWrappedLabel("· " + influence.Description, 14, MyraSkin.TextPrimary, width));
            }
        }

        public static KeyValuePair<string, bool?> Check(Condition condition, bool satisfied)
        {
            return new KeyValuePair<string, bool?>(condition.Name, satisfied);
        }

        // 条件前面一个菱形，满足的绿色，不满足的红色，无法判断的灰色
        public static void AddConditions(VerticalStackPanel panel, string title, IEnumerable<KeyValuePair<string, bool?>> conditions, int width)
        {
            List<KeyValuePair<string, bool?>> list = conditions.ToList();
            panel.Widgets.Add(CreateDetailHeader(title, width));
            if (list.Count == 0)
            {
                panel.Widgets.Add(CreateLabel("无", 13, MyraSkin.TextSecondary));
                return;
            }
            foreach (var condition in list)
            {
                Color color = condition.Value == null ? MyraSkin.TextDisabled : condition.Value.Value ? MyraSkin.TextFriendly : MyraSkin.TextHostile;
                var row = new HorizontalStackPanel
                {
                    Spacing = 6
                };
                row.Widgets.Add(CreateLabel("◇", 13, color));
                row.Widgets.Add(CreateWrappedLabel(condition.Key, 13, condition.Value == false ? MyraSkin.TextSecondary : MyraSkin.TextPrimary, width - 20));
                panel.Widgets.Add(row);
            }
        }

        // 控件和它的上级都可见才算显示出来，隐藏的页里的控件也在原位置
        public static bool IsShown(Widget widget)
        {
            for (; widget != null; widget = widget.Parent)
            {
                if (!widget.Visible)
                {
                    return false;
                }
            }
            return true;
        }

        // 带1像素边框的底，边框可以是虚线
        public class FrameBrush : IBrush
        {
            private const int Dash = 4;
            private const int Gap = 3;

            private readonly Color fill;
            private readonly Color line;
            private readonly bool dashed;

            public FrameBrush(Color fill, Color line, bool dashed)
            {
                this.fill = fill;
                this.line = line;
                this.dashed = dashed;
            }

            // 边框不变，换一个底色，用作鼠标移上去的样子
            public FrameBrush WithFill(Color newFill)
            {
                return new FrameBrush(newFill, this.line, this.dashed);
            }

            public void Draw(RenderContext context, Rectangle dest, Color color)
            {
                if (this.fill.A > 0)
                {
                    context.FillRectangle(dest, this.fill);
                }
                if (this.line.A == 0)
                {
                    return;
                }
                if (!this.dashed)
                {
                    context.FillRectangle(new Rectangle(dest.X, dest.Y, dest.Width, 1), this.line);
                    context.FillRectangle(new Rectangle(dest.X, dest.Bottom - 1, dest.Width, 1), this.line);
                    context.FillRectangle(new Rectangle(dest.X, dest.Y, 1, dest.Height), this.line);
                    context.FillRectangle(new Rectangle(dest.Right - 1, dest.Y, 1, dest.Height), this.line);
                    return;
                }
                for (int x = dest.X; x < dest.Right; x += Dash + Gap)
                {
                    int width = Math.Min(Dash, dest.Right - x);
                    context.FillRectangle(new Rectangle(x, dest.Y, width, 1), this.line);
                    context.FillRectangle(new Rectangle(x, dest.Bottom - 1, width, 1), this.line);
                }
                for (int y = dest.Y; y < dest.Bottom; y += Dash + Gap)
                {
                    int height = Math.Min(Dash, dest.Bottom - y);
                    context.FillRectangle(new Rectangle(dest.X, y, 1, height), this.line);
                    context.FillRectangle(new Rectangle(dest.Right - 1, y, 1, height), this.line);
                }
            }
        }

        // 横向虚线
        public class DashLineBrush : IBrush
        {
            private const int Dash = 3;
            private const int Gap = 3;
            private readonly Color line;

            public DashLineBrush(Color line)
            {
                this.line = line;
            }

            public void Draw(RenderContext context, Rectangle dest, Color color)
            {
                for (int x = dest.X; x < dest.Right; x += Dash + Gap)
                {
                    context.FillRectangle(new Rectangle(x, dest.Y, Math.Min(Dash, dest.Right - x), 1), this.line);
                }
            }
        }
    }

    /// <summary>
    /// 说明浮层：鼠标移到某一项上时显示说明，移开就收起。触屏没有悬停，点一下也能显示，点别处收起。
    /// 浮层放在弹窗最外层的遮罩里，盖在弹窗内容上面，位置不超出弹窗
    /// </summary>
    public class MyraPopover
    {
        public const int Width = 300;
        public const int Padding = 18;
        private const int Gap = 8;

        /// <summary>
        /// 浮层里文字的宽度
        /// </summary>
        public const int TextWidth = Width - 2 * Padding;

        private readonly Widget container;
        private readonly Widget dialog;
        private readonly Panel panel;
        private readonly VerticalStackPanel content;
        private Widget anchor;

        /// <param name="container">最外层铺满窗口的遮罩，浮层要加到它里面，并且最后加</param>
        /// <param name="dialog">弹窗本体，浮层不超出它的范围</param>
        public MyraPopover(Widget container, Widget dialog)
        {
            this.container = container;
            this.dialog = dialog;
            this.content = new VerticalStackPanel
            {
                Spacing = 6
            };
            // 位置用Left、Top从左上角算
            this.panel = new Panel
            {
                Width = Width,
                Padding = new Thickness(Padding),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Background = new MyraWidgets.FrameBrush(MyraWidgets.PopoverColor, MyraWidgets.GoldDim, false),
                Visible = false
            };
            this.panel.Widgets.Add(this.content);
        }

        /// <summary>
        /// 浮层本身，加到遮罩里
        /// </summary>
        public Widget Widget => this.panel;

        /// <summary>
        /// 浮层放在这一项的下面，默认放在右边
        /// </summary>
        public bool PlaceBelow { get; set; }

        public void Attach(Widget anchor, Action<VerticalStackPanel, int> fill)
        {
            anchor.MouseEntered += (sender, args) => this.Show(anchor, fill);
            anchor.MouseLeft += (sender, args) => this.Hide(anchor);
            if (anchor is Button button)
            {
                button.Click += (sender, args) => this.Show(anchor, fill);
            }
        }

        public void Show(Widget anchor, Action<VerticalStackPanel, int> fill)
        {
            if (this.anchor == anchor && this.panel.Visible)
            {
                return;
            }
            this.anchor = anchor;
            this.content.Widgets.Clear();
            fill(this.content, TextWidth);

            Point size = this.panel.Measure(new Point(Width, int.MaxValue));
            Point anchorTopLeft = this.container.ToLocal(anchor.ToGlobal(Point.Zero));
            Point dialogTopLeft = this.container.ToLocal(this.dialog.ToGlobal(Point.Zero));
            var dialogBounds = new Rectangle(dialogTopLeft.X, dialogTopLeft.Y, this.dialog.Bounds.Width, this.dialog.Bounds.Height);
            int x;
            int y;
            if (this.PlaceBelow)
            {
                // 放在这一项的下面、左边对齐，下面放不下就放上面；左右不超出弹窗
                x = MathHelper.Clamp(anchorTopLeft.X, dialogBounds.X + Gap, Math.Max(dialogBounds.X + Gap, dialogBounds.Right - Gap - size.X));
                y = anchorTopLeft.Y + anchor.Bounds.Height + Gap;
                if (y + size.Y > dialogBounds.Bottom - Gap)
                {
                    y = Math.Max(dialogBounds.Y + Gap, anchorTopLeft.Y - Gap - size.Y);
                }
            }
            else
            {
                // 放在这一项的右边，右边放不下就放左边；上下不超出弹窗
                x = anchorTopLeft.X + anchor.Bounds.Width + Gap;
                if (x + size.X > dialogBounds.Right - Gap)
                {
                    x = anchorTopLeft.X - Gap - size.X;
                }
                x = Math.Max(dialogBounds.X + Gap, x);
                y = MathHelper.Clamp(anchorTopLeft.Y, dialogBounds.Y + Gap, Math.Max(dialogBounds.Y + Gap, dialogBounds.Bottom - Gap - size.Y));
            }
            this.panel.Left = x;
            this.panel.Top = y;
            this.panel.Visible = true;
        }

        /// <summary>
        /// 收起浮层。给了anchor时，只有浮层正属于它才收起
        /// </summary>
        public void Hide(Widget anchor = null)
        {
            if (anchor == null || anchor == this.anchor)
            {
                this.panel.Visible = false;
                this.anchor = null;
            }
        }

        /// <summary>
        /// 触屏上点到浮层所属的那一项以外的地方，收起浮层
        /// </summary>
        public void HideIfOutside(Point globalPosition)
        {
            if (this.anchor != null && !this.anchor.ContainsGlobalPoint(globalPosition))
            {
                this.Hide();
            }
        }
    }

    /// <summary>
    /// 按住列表上下拖动来滑动，触屏上没有滚轮，主要靠这个。弹窗打开时Attach，关闭时Detach
    /// </summary>
    public class MyraDragScroll
    {
        // 按住拖动超过这个距离才算滑动列表，否则算点击
        private const int DragThreshold = 6;

        private readonly List<ScrollViewer> viewers = new List<ScrollViewer>();
        private Widget root;
        private MyraPopover popover;
        private ScrollViewer viewer;
        private int? lastY;
        private int distance;

        public void Add(ScrollViewer viewer)
        {
            this.viewers.Add(viewer);
        }

        /// <param name="root">传给MyraUI的根控件，用来判断弹窗是否在最上层</param>
        /// <param name="popover">按下时顺便收起说明浮层，没有可以不给</param>
        public void Attach(Widget root, MyraPopover popover)
        {
            this.root = root;
            this.popover = popover;
            Desktop desktop = MyraUI.Desktop;
            desktop.TouchDown += this.Desktop_TouchDown;
            desktop.TouchMoved += this.Desktop_TouchMoved;
            desktop.TouchUp += this.Desktop_TouchUp;
        }

        public void Detach()
        {
            Desktop desktop = MyraUI.Desktop;
            desktop.TouchDown -= this.Desktop_TouchDown;
            desktop.TouchMoved -= this.Desktop_TouchMoved;
            desktop.TouchUp -= this.Desktop_TouchUp;
        }

        private void Desktop_TouchDown(object sender, MyraEventArgs args)
        {
            this.distance = 0;
            this.lastY = null;
            this.viewer = null;
            Point? position = MyraUI.Desktop.TouchPosition;
            if (!position.HasValue || !MyraUI.IsTop(this.root))
            {
                return;
            }
            this.popover?.HideIfOutside(position.Value);
            foreach (ScrollViewer viewer in this.viewers)
            {
                if (MyraWidgets.IsShown(viewer) && viewer.ContainsGlobalPoint(position.Value))
                {
                    this.viewer = viewer;
                    this.lastY = position.Value.Y;
                    break;
                }
            }
        }

        private void Desktop_TouchMoved(object sender, MyraEventArgs args)
        {
            Point? position = MyraUI.Desktop.TouchPosition;
            if (!this.lastY.HasValue || !position.HasValue || this.viewer == null)
            {
                return;
            }
            int delta = position.Value.Y - this.lastY.Value;
            this.lastY = position.Value.Y;
            this.distance += Math.Abs(delta);
            if (this.distance > DragThreshold)
            {
                Point scroll = this.viewer.ScrollPosition;
                scroll.Y = MathHelper.Clamp(scroll.Y - delta, 0, this.viewer.ScrollMaximum.Y);
                this.viewer.ScrollPosition = scroll;
            }
        }

        private void Desktop_TouchUp(object sender, MyraEventArgs args)
        {
            this.lastY = null;
            this.viewer = null;
        }
    }
}

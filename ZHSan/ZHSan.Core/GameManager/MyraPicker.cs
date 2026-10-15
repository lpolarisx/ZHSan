using FontStashSharp.RichText;
using Microsoft.Xna.Framework;
using Myra.Graphics2D;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GameManager
{
    /// <summary>
    /// 选择窗口里的一列
    /// </summary>
    public class MyraPickerColumn<T>
    {
        public readonly string Header;
        public readonly int Width;
        public readonly Func<T, string> GetText;

        public MyraPickerColumn(string header, int width, Func<T, string> getText)
        {
            this.Header = header;
            this.Width = width;
            this.GetText = getText;
        }
    }

    /// <summary>
    /// 通用的选择窗口：多列表格，点一行选中或取消（行首打√、底色变蓝），可单选或多选，底部有清空、确定、取消。
    /// MyraUI同一时间只有一个根控件，所以选择窗口不单独弹出，而是盖在宿主窗口铺满屏幕的那层Panel上；
    /// 宿主在右键时先调用Close关掉选择窗口
    /// </summary>
    public class MyraPicker
    {
        private const int DialogPadding = 28;
        private const int CellSpacing = 8;
        private const int MarkWidth = 20;
        private const int RowHeight = 28;
        private const int RowSpacing = 2;
        private const int MaxListHeight = 320;
        private const int ButtonWidth = 90;
        private const int ButtonSpacing = 12;
        // 行内容左边留的空
        private const int RowPaddingLeft = 8;
        // 竖向滚动条占的宽度
        private const int ScrollBarWidth = 20;

        private readonly Panel host;
        private Widget cover;

        private MyraPicker(Panel host)
        {
            this.host = host;
        }

        /// <summary>
        /// 选择窗口是否还开着
        /// </summary>
        public bool IsOpen => this.cover != null;

        /// <summary>
        /// 在宿主上盖一层选择窗口
        /// </summary>
        /// <param name="host">宿主窗口铺满屏幕的那层Panel，选择窗口加在它上面</param>
        /// <param name="title">窗口标题</param>
        /// <param name="columns">各列的表头、宽度和取文字的方法</param>
        /// <param name="items">候选项，按这个顺序列出</param>
        /// <param name="selected">打开时已选中的项</param>
        /// <param name="onConfirm">点确定、窗口关掉之后调用，参数是选中的项，按候选项的顺序</param>
        /// <param name="note">列表下面的一行说明，不需要就传null</param>
        /// <param name="multiple">true可多选，false点一行就只选这一行</param>
        public static MyraPicker Show<T>(Panel host, string title, IReadOnlyList<MyraPickerColumn<T>> columns, IReadOnlyList<T> items, IEnumerable<T> selected, Action<List<T>> onConfirm, string note = null, bool multiple = true)
        {
            var picker = new MyraPicker(host);
            picker.Build(title, columns, items, selected, onConfirm, note, multiple);
            return picker;
        }

        /// <summary>
        /// 关掉选择窗口，不调用onConfirm
        /// </summary>
        public void Close()
        {
            if (this.cover != null)
            {
                this.host.Widgets.Remove(this.cover);
                this.cover = null;
            }
        }

        private void Build<T>(string title, IReadOnlyList<MyraPickerColumn<T>> columns, IReadOnlyList<T> items, IEnumerable<T> selected, Action<List<T>> onConfirm, string note, bool multiple)
        {
            var chosen = new HashSet<T>(selected);
            if (!multiple && chosen.Count > 1)
            {
                chosen = new HashSet<T>(chosen.Take(1));
            }
            int rowWidth = RowPaddingLeft + MarkWidth + columns.Sum(x => x.Width) + columns.Count * CellSpacing;
            int contentWidth = Math.Max(rowWidth + ScrollBarWidth, 3 * ButtonWidth + 2 * ButtonSpacing);

            var panel = new VerticalStackPanel
            {
                Width = contentWidth + 2 * DialogPadding,
                Spacing = 8,
                // 上边留得少，让标题落在底板那条横线的上面
                Padding = new Thickness(DialogPadding, 7, DialogPadding, DialogPadding),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Background = MyraSkin.PanelBackground
            };
            panel.Widgets.Add(CreateLabel(title, 20, MyraSkin.TextTitle, TextHorizontalAlignment.Center, contentWidth));
            panel.Widgets.Add(CreateRow(new Panel { Width = MarkWidth }, columns.Select(x => CreateLabel(x.Header, 14, MyraSkin.TextSecondary, Align(columns, x), x.Width))));

            Label countLabel = CreateLabel("", 14, MyraSkin.TextSecondary, TextHorizontalAlignment.Left, null);
            var rows = new VerticalStackPanel
            {
                Spacing = RowSpacing
            };
            var rowWidgets = new List<(Button Button, Label Mark, T Item)>();
            void Refresh()
            {
                foreach (var (button, mark, item) in rowWidgets)
                {
                    bool on = chosen.Contains(item);
                    button.Background = new SolidBrush(on ? MyraSkin.ListRowSelectedColor : MyraSkin.ListRowColor);
                    button.FocusedBackground = button.Background;
                    mark.Text = on ? "√" : "";
                }
                countLabel.Text = items.Count == 0 ? "没有可选的项。" : multiple ? "已选 " + chosen.Count + " / " + items.Count : "";
            }

            foreach (T item in items)
            {
                Label mark = CreateLabel("", 16, MyraSkin.TextHighlight, TextHorizontalAlignment.Center, MarkWidth);
                var button = new Button
                {
                    Content = CreateRow(mark, columns.Select(x => CreateLabel(x.GetText(item), 16, MyraSkin.TextPrimary, Align(columns, x), x.Width))),
                    Width = rowWidth,
                    Height = RowHeight,
                    OverBackground = new SolidBrush(MyraSkin.ListRowOverColor),
                    PressedBackground = new SolidBrush(MyraSkin.ListRowOverColor)
                };
                T current = item;
                button.Click += (sender, args) =>
                {
                    if (chosen.Contains(current))
                    {
                        chosen.Remove(current);
                    }
                    else
                    {
                        if (!multiple)
                        {
                            chosen.Clear();
                        }
                        chosen.Add(current);
                    }
                    Refresh();
                };
                rowWidgets.Add((button, mark, item));
                rows.Widgets.Add(button);
            }
            panel.Widgets.Add(new ScrollViewer
            {
                Content = rows,
                ShowHorizontalScrollBar = false,
                Width = rowWidth + ScrollBarWidth,
                Height = Math.Clamp(items.Count * (RowHeight + RowSpacing), RowHeight, MaxListHeight)
            });

            if (!string.IsNullOrEmpty(note))
            {
                Label noteLabel = CreateLabel(note, 14, MyraSkin.TextSecondary, TextHorizontalAlignment.Left, contentWidth);
                noteLabel.Wrap = true;
                panel.Widgets.Add(noteLabel);
            }

            // 底部左边是已选数量，右边是按钮
            var bottom = new Panel();
            countLabel.VerticalAlignment = VerticalAlignment.Center;
            bottom.Widgets.Add(countLabel);
            var buttons = new HorizontalStackPanel
            {
                Spacing = ButtonSpacing,
                HorizontalAlignment = HorizontalAlignment.Right
            };
            Button clear = MyraSkin.CreateButton("清空", ButtonWidth, out _);
            clear.Click += (sender, args) =>
            {
                chosen.Clear();
                Refresh();
            };
            Button ok = MyraSkin.CreateButton("确定", ButtonWidth, out _);
            ok.Click += (sender, args) =>
            {
                var result = items.Where(chosen.Contains).ToList();
                this.Close();
                onConfirm(result);
            };
            Button cancel = MyraSkin.CreateButton("取消", ButtonWidth, out _);
            cancel.Click += (sender, args) => this.Close();
            buttons.Widgets.Add(clear);
            buttons.Widgets.Add(ok);
            buttons.Widgets.Add(cancel);
            bottom.Widgets.Add(buttons);
            panel.Widgets.Add(bottom);

            Refresh();

            // 再压暗一层，挡住宿主窗口的点击
            var cover = new Panel
            {
                Background = new SolidBrush(new Color(0, 0, 0, 120))
            };
            cover.Widgets.Add(panel);
            this.cover = cover;
            this.host.Widgets.Add(cover);
        }

        // 第一列（一般是名字）靠左，其余居中
        private static TextHorizontalAlignment Align<T>(IReadOnlyList<MyraPickerColumn<T>> columns, MyraPickerColumn<T> column)
        {
            return column == columns[0] ? TextHorizontalAlignment.Left : TextHorizontalAlignment.Center;
        }

        private static HorizontalStackPanel CreateRow(Widget first, IEnumerable<Widget> cells)
        {
            var row = new HorizontalStackPanel
            {
                Spacing = CellSpacing,
                Padding = new Thickness(RowPaddingLeft, 0, 0, 0)
            };
            row.Widgets.Add(first);
            foreach (var cell in cells)
            {
                row.Widgets.Add(cell);
            }
            return row;
        }

        private static Label CreateLabel(string text, int fontSize, Color color, TextHorizontalAlignment align, int? width)
        {
            return new Label
            {
                Text = text,
                Font = TextManager.GetFont(fontSize),
                TextColor = color,
                TextAlign = align,
                Width = width,
                VerticalAlignment = VerticalAlignment.Center
            };
        }
    }
}

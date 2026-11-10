using FontStashSharp;
using FontStashSharp.RichText;
using GameGlobal;
using GameManager;
using GameObjects;
using Microsoft.Xna.Framework;
using Myra.Graphics2D;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using static GameManager.MyraWidgets;

namespace WorldOfTheThreeKingdoms.GameScreens
{
    /// <summary>
    /// 信息列表的一列
    /// </summary>
    internal class InfoColumn<T>
    {
        public readonly string Header;

        /// <summary>
        /// 列宽，0表示占满剩下的宽度，只能有一列为0
        /// </summary>
        public readonly int Width;

        public readonly Func<T, string> Text;

        /// <summary>
        /// 自己画的单元格（进度条、效果标签等），参数是这一项和列宽。为null时显示Text
        /// </summary>
        public readonly Func<T, int, Widget> Cell;

        /// <summary>
        /// 排序依据，null表示这一列不能排序
        /// </summary>
        public readonly Func<T, IComparable> SortKey;

        /// <summary>
        /// 文字颜色，null用默认色
        /// </summary>
        public readonly Func<T, Color> Color;

        /// <summary>
        /// 占满剩下宽度的列（Width为0）至少多宽，列多的时候列表横向滚动
        /// </summary>
        public int MinWidth { get; init; } = 80;

        public InfoColumn(string header, int width, Func<T, string> text, Func<T, IComparable> sortKey = null, Func<T, Color> color = null)
        {
            this.Header = header;
            this.Width = width;
            this.Text = text;
            this.SortKey = sortKey;
            this.Color = color;
        }

        public InfoColumn(string header, int width, Func<T, int, Widget> cell, Func<T, IComparable> sortKey = null)
        {
            this.Header = header;
            this.Width = width;
            this.Cell = cell;
            this.SortKey = sortKey;
        }

        public bool CanSort => this.SortKey != null;
    }

    /// <summary>
    /// 信息列表上方的一个筛选页签
    /// </summary>
    internal class InfoFilter<T>
    {
        public readonly string Name;
        public readonly Func<T, bool> Match;

        public InfoFilter(string name, Func<T, bool> match)
        {
            this.Name = name;
            this.Match = match;
        }
    }

    /// <summary>
    /// 信息列表的一个视图页签：一组列。第一个视图是带详情栏的“概览”，后面的视图（例如旧版列表里的基本、能力、内政）不显示详情栏，列表占满宽度
    /// </summary>
    internal class InfoView<T>
    {
        public readonly string Name;
        public readonly IReadOnlyList<InfoColumn<T>> Columns;

        public InfoView(string name, IReadOnlyList<InfoColumn<T>> columns)
        {
            this.Name = name;
            this.Columns = columns;
        }
    }

    /// <summary>
    /// 右键菜单“信息”里的各种一览：左上标题和数量，下面一排圆角筛选页签和可排序、分页的列表。
    /// 点一行在右侧详情栏显示这一项，切换视图页签时详情栏一直在；详情里可以跳到列表里的另一项（例如兵种的升级去向）。
    /// 也可以不要详情栏，点一行直接打开这一项自己的详情窗口（人物、城池）。
    /// 可以再给几个视图页签，切换显示的列；列多时列表横向滚动。没有视图页签也没有详情栏时面板按列宽收窄
    /// </summary>
    internal class InfoListDialog<T> where T : class
    {
        private const int ScreenMargin = 40;
        private const int MinDialogWidth = 960;
        private const int MaxDialogWidth = 1200;
        private const int MinDialogHeight = 560;
        private const int MaxDialogHeight = 720;
        private const int DialogPadding = 28;
        // 上边留得少，让标题落在底板那条横线的上面
        private const int DialogPaddingTop = 3;
        private const int TitleRowHeight = 36;
        private const int SectionSpacing = 14;
        private const int ScrollBarWidth = 16;
        private const int BodySpacing = 20;

        // 右侧详情栏随窗口变宽，最窄时左边列表的列还放得下
        private const int MinDetailWidth = 360;
        private const int MaxDetailWidth = 420;
        private const double DetailWidthRatio = 0.35;
        // 底板四周有花纹，内容要让开
        private const int DetailPadding = 22;
        // 详情内容和滚动条之间再空一点，右对齐的数值才不会贴着滚动条
        private const int DetailScrollGap = 14;

        private const int HeaderHeight = 30;
        private const int HeaderFontSize = 14;
        private const int ListSpacing = 6;
        private const int RowHeight = 44;
        private const int CellSpacing = 12;
        private const int RowPaddingLeft = 18;
        private const int AccentWidth = 3;
        // 能点开的行在右边放一个箭头
        private const int OpenArrowWidth = 24;
        private const int PagerHeight = 30;
        private const int PagerButtonWidth = 90;

        private const int PillHeight = 30;
        private const int PillFontSize = 16;
        // 页签文字两边留的空
        private const int PillPadding = 34;
        private const int PillSpacing = 10;

        private const int ViewTabHeight = 32;
        private const int ViewTabFontSize = 16;
        private const int ViewTabPadding = 24;
        private const int ViewTabLineSpacing = 2;
        private const int ViewTabUnderline = 2;

        /// <summary>
        /// 每页多少行。Myra没有虚拟列表，上千行一次建出来会卡，所以分页
        /// </summary>
        private const int PageSize = 100;

        // 强调色沿用游戏里的金色：选中的页签、行首竖条、排序中的表头
        private static readonly Color Accent = MyraSkin.TextHighlight;
        private static readonly Color PillSelectedText = new Color(24, 30, 34);
        // 游戏按预乘透明度混合，淡色用颜色乘系数得到
        private static readonly Color DividerColor = LineColor * 0.45f;
        private static readonly Color RowHoverColor = HoverColor * 0.55f;
        private static readonly Color RowSelectedColor = new Color(30, 44, 54);

        private readonly string title;
        private readonly List<T> allItems;
        private readonly List<InfoView<T>> views;
        private readonly List<InfoFilter<T>> filters;
        // 点一行做的事，null表示行不能点开
        private readonly Action<T> open;
        // 往右侧详情栏里填内容，null表示没有详情栏
        private readonly Action<VerticalStackPanel, T, int, InfoListDialog<T>> fillDetail;
        private readonly MyraDragScroll dragScroll = new MyraDragScroll();

        private readonly int dialogHeight;
        private readonly int dialogWidth;
        private readonly int detailWidth;
        // 以下随视图变化：列表栏宽、可见的行宽、所有列排开的宽度（比行宽大时横向滚动）、占满剩下宽度那一列的宽
        private int listWidth;
        private int rowWidth;
        private int contentWidth;
        private int fillColumnWidth;

        private readonly List<(Button Button, Label Label, int Index)> filterPills = new List<(Button, Label, int)>();
        private readonly List<(Label Label, int Column)> headerLabels = new List<(Label, int)>();
        // 按对象本身区分各行。GameObject只按ID判断相等，有的数据没设ID（例如设施等级），全都算作相等
        private readonly Dictionary<T, (Button Button, Panel AccentBar)> rowWidgets = new Dictionary<T, (Button, Panel)>(ReferenceEqualityComparer.Instance);

        private Panel root;
        private VerticalStackPanel rows;
        private VerticalStackPanel detail;
        private ScrollViewer detailViewer;
        private Label countLabel;
        private ScrollViewer listViewer;
        // 有翻页栏时列表的高度，只有一页时翻页栏隐藏，列表占满它的位置
        private int listViewerHeight;
        private Widget pager;
        private Label pagerLabel;
        private Label prevLabel;
        private Label nextLabel;

        private int viewIndex;
        private int filterIndex;
        // -1表示按原有顺序
        private int sortColumn = -1;
        private bool descending;
        private int page;
        private List<T> visibleItems = new List<T>();
        private T selected;

        private InfoListDialog(string title, IEnumerable<T> items, IReadOnlyList<InfoColumn<T>> columns, IEnumerable<InfoFilter<T>> filters,
            Action<T> open, Action<VerticalStackPanel, T, int, InfoListDialog<T>> fillDetail, IEnumerable<InfoView<T>> views)
        {
            this.title = title;
            this.allItems = items.ToList();
            this.views = new List<InfoView<T>> { new InfoView<T>("概览", columns) };
            if (views != null)
            {
                this.views.AddRange(views);
            }
            this.filters = filters?.ToList() ?? new List<InfoFilter<T>>();
            if (this.filters.Count > 0)
            {
                this.filters.Insert(0, new InfoFilter<T>("全部", x => true));
            }
            this.open = open;
            this.fillDetail = fillDetail;

            Point size = MyraUI.LayoutSize;
            this.dialogHeight = MathHelper.Clamp(size.Y - 2 * ScreenMargin, MinDialogHeight, MaxDialogHeight);
            if (this.HasDetail)
            {
                // 详情栏占右边一块，各视图的列都排在左边，列多就横向滚动
                this.dialogWidth = MathHelper.Clamp(size.X - 2 * ScreenMargin, MinDialogWidth, MaxDialogWidth);
                this.detailWidth = MathHelper.Clamp((int)(this.dialogWidth * DetailWidthRatio), MinDetailWidth, MaxDetailWidth);
                this.listWidth = this.dialogWidth - 2 * DialogPadding - BodySpacing - this.detailWidth;
            }
            else if (this.HasViews)
            {
                // 各视图的列数不一样，面板按最宽的来，列再多就横向滚动
                this.dialogWidth = MathHelper.Clamp(size.X - 2 * ScreenMargin, MinDialogWidth, MaxDialogWidth);
                this.listWidth = this.dialogWidth - 2 * DialogPadding;
            }
            else
            {
                // 宽度刚好放下各列，筛选页签一行放不下时放宽到一行放得下，但不超出窗口
                InfoColumn<T> fill = columns.FirstOrDefault(x => x.Width == 0);
                int columnsWidth = this.GetFixedWidth() + (fill?.MinWidth ?? 0) + ScrollBarWidth;
                int filtersWidth = this.filters.Count > 0 ? this.GetFiltersWidth() : 0;
                this.listWidth = Math.Min(Math.Max(columnsWidth, filtersWidth), size.X - 2 * ScreenMargin - 2 * DialogPadding);
                this.dialogWidth = this.listWidth + 2 * DialogPadding;
            }
            this.LayoutView();
        }

        /// <summary>
        /// 显示信息列表
        /// </summary>
        /// <param name="title">左上角的标题，后面跟当前筛选出的数量</param>
        /// <param name="columns">“概览”的列</param>
        /// <param name="filters">筛选页签，前面自动加一个“全部”；没有就传null</param>
        /// <param name="open">点一行时做的事，例如打开这一项的详情；行不能点开就传null</param>
        /// <param name="views">“概览”后面的视图页签，没有就不传</param>
        public static void Show(string title, IEnumerable<T> items, IReadOnlyList<InfoColumn<T>> columns, IEnumerable<InfoFilter<T>> filters,
            Action<T> open, IEnumerable<InfoView<T>> views = null)
        {
            Show(new InfoListDialog<T>(title, items, columns, filters, open, null, views));
        }

        /// <summary>
        /// 显示带右侧详情栏的信息列表：点一行在右侧显示详情，打开时先显示第一项
        /// </summary>
        /// <param name="fillDetail">往详情栏里填内容，参数是详情栏、选中的项、文字宽度和列表本身（用来跳到另一项）</param>
        public static void ShowWithDetail(string title, IEnumerable<T> items, IReadOnlyList<InfoColumn<T>> columns, IEnumerable<InfoFilter<T>> filters,
            Action<VerticalStackPanel, T, int, InfoListDialog<T>> fillDetail, IEnumerable<InfoView<T>> views = null)
        {
            Show(new InfoListDialog<T>(title, items, columns, filters, null, fillDetail, views));
        }

        /// <summary>
        /// 显示带右侧详情栏的信息列表，详情里不用跳到别的项
        /// </summary>
        public static void ShowWithDetail(string title, IEnumerable<T> items, IReadOnlyList<InfoColumn<T>> columns, IEnumerable<InfoFilter<T>> filters,
            Action<VerticalStackPanel, T, int> fillDetail, IEnumerable<InfoView<T>> views = null)
        {
            ShowWithDetail(title, items, columns, filters, (panel, item, width, list) => fillDetail(panel, item, width), views);
        }

        /// <summary>
        /// 选中列表里的某一项：不在当前筛选里就切到“全部”，翻到它所在的页，在右边显示详情
        /// </summary>
        public void Select(T item)
        {
            if (item == null || !this.allItems.Any(x => ReferenceEquals(x, item)))
            {
                return;
            }
            if (this.filterIndex != 0 && !this.filters[this.filterIndex].Match(item))
            {
                this.filterIndex = 0;
                this.RefreshFilterPills();
                this.UpdateVisibleItems();
            }
            int index = this.visibleItems.FindIndex(x => ReferenceEquals(x, item));
            if (index >= 0 && index / PageSize != this.page)
            {
                this.page = index / PageSize;
                this.RefreshRows();
            }
            this.SetSelected(item);
            // 滚到选中的那一行。刚换过页时还没重新排版，滚动上限是旧的，不能拿来夹
            if (index >= 0)
            {
                this.listViewer.ScrollPosition = new Point(this.listViewer.ScrollPosition.X, index % PageSize * RowHeight);
            }
        }

        private static void Show(InfoListDialog<T> dialog)
        {
            // 面板显示期间占住一个未完成工作，地图和工具栏就不会响应鼠标键盘
            Session.MainGame.mainGameScreen.PushUndoneWork(new UndoneWorkItem(UndoneWorkKind.SubDialog, DialogKind.InfoList));
            dialog.root = dialog.Build();
            dialog.dragScroll.Attach(dialog.root, null);
            MyraUI.Show(dialog.root, dialog.Close);
        }

        private bool HasViews => this.views.Count > 1;

        private bool HasDetail => this.fillDetail != null;

        private IReadOnlyList<InfoColumn<T>> Columns => this.views[this.viewIndex].Columns;

        private void Close()
        {
            this.dragScroll.Detach();
            MyraUI.Close(() =>
            {
                // 等鼠标键松开之后再把输入还给地图，关闭面板的这次点击才不会落到地图上
                if (Session.MainGame.mainGameScreen.PopUndoneWork().Kind != UndoneWorkKind.SubDialog)
                {
                    throw new Exception("The UndoneWork is not a SubDialog.");
                }
            });
        }

        // 各定宽列加上间距和行首留白，能点开的行再加上右边的箭头
        private int GetFixedWidth()
        {
            IReadOnlyList<InfoColumn<T>> columns = this.Columns;
            return RowPaddingLeft + columns.Sum(x => x.Width) + (columns.Count - 1) * CellSpacing + (this.open != null ? OpenArrowWidth : 0);
        }

        // 按当前视图算列表各处的宽度
        private void LayoutView()
        {
            this.rowWidth = this.listWidth - ScrollBarWidth;
            int fixedWidth = this.GetFixedWidth();
            InfoColumn<T> fill = this.Columns.FirstOrDefault(x => x.Width == 0);
            if (fill == null)
            {
                this.fillColumnWidth = 0;
                this.contentWidth = Math.Max(this.rowWidth, fixedWidth);
            }
            else
            {
                this.fillColumnWidth = Math.Max(fill.MinWidth, this.rowWidth - fixedWidth);
                this.contentWidth = Math.Max(this.rowWidth, fixedWidth + this.fillColumnWidth);
            }
        }

        private Panel Build()
        {
            var mask = new Panel
            {
                // 半透明遮罩铺满窗口，压暗后面的游戏界面
                Background = new SolidBrush(new Color(0, 0, 0, 150))
            };
            mask.Widgets.Add(this.BuildDialog());
            this.root = mask;
            this.UpdateVisibleItems();
            if (this.HasDetail)
            {
                this.SetSelected(this.visibleItems.FirstOrDefault());
            }
            return mask;
        }

        private Widget BuildDialog()
        {
            var dialog = new VerticalStackPanel
            {
                Width = this.dialogWidth,
                Height = this.dialogHeight,
                Spacing = SectionSpacing,
                Padding = new Thickness(DialogPadding, DialogPaddingTop, DialogPadding, DialogPadding),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Background = MyraSkin.PanelBackground
            };
            dialog.Widgets.Add(this.BuildTitle());

            int bodyHeight = this.dialogHeight - (DialogPaddingTop + DialogPadding + TitleRowHeight + SectionSpacing);
            // 左栏从上到下是视图页签、筛选页签和列表；详情栏在右边，从标题下面开始占满整个高度。
            // 筛选页签一行放不下（例如称号的种类很多）时换行
            var left = new VerticalStackPanel
            {
                Width = this.listWidth,
                Height = bodyHeight,
                Spacing = SectionSpacing
            };
            int listHeight = bodyHeight;
            if (this.HasViews)
            {
                left.Widgets.Add(this.BuildViewTabs(this.listWidth, out int tabsHeight));
                listHeight -= tabsHeight + SectionSpacing;
            }
            if (this.filters.Count > 0)
            {
                left.Widgets.Add(this.BuildFilters(this.listWidth, out int filterHeight));
                listHeight -= filterHeight + SectionSpacing;
            }
            left.Widgets.Add(this.BuildList(listHeight));
            if (!this.HasDetail)
            {
                dialog.Widgets.Add(left);
                return dialog;
            }
            var body = new HorizontalStackPanel
            {
                Spacing = BodySpacing,
                Height = bodyHeight
            };
            body.Widgets.Add(left);
            body.Widgets.Add(this.BuildDetail(bodyHeight));
            dialog.Widgets.Add(body);
            return dialog;
        }

        // 左边大字标题，后面小字写当前有几项；右上角关闭按钮
        private Widget BuildTitle()
        {
            var titleBar = new Panel
            {
                Height = TitleRowHeight
            };
            var left = new HorizontalStackPanel
            {
                Spacing = 14,
                VerticalAlignment = VerticalAlignment.Center
            };
            left.Widgets.Add(new Label
            {
                Text = this.title,
                Font = TextManager.GetFont(26),
                TextColor = MyraSkin.TextTitle,
                VerticalAlignment = VerticalAlignment.Center
            });
            this.countLabel = new Label
            {
                Font = TextManager.GetFont(15),
                TextColor = MyraSkin.TextSecondary,
                VerticalAlignment = VerticalAlignment.Center
            };
            left.Widgets.Add(this.countLabel);
            titleBar.Widgets.Add(left);
            Button close = CreateCloseButton(this.Close);
            close.VerticalAlignment = VerticalAlignment.Center;
            titleBar.Widgets.Add(close);
            return titleBar;
        }

        // 视图页签：只有文字，选中的金字并在下面压一条金线，一行放不下就换行
        private Widget BuildViewTabs(int maxWidth, out int height)
        {
            SpriteFontBase font = TextManager.GetFont(ViewTabFontSize);
            var panel = new VerticalStackPanel
            {
                Spacing = ViewTabLineSpacing
            };
            HorizontalStackPanel line = null;
            int lineWidth = 0;
            for (int i = 0; i < this.views.Count; i++)
            {
                int width = (int)Math.Ceiling(font.MeasureString(this.views[i].Name).X) + ViewTabPadding;
                if (line == null || lineWidth + width > maxWidth)
                {
                    line = new HorizontalStackPanel();
                    panel.Widgets.Add(line);
                    lineWidth = 0;
                }
                lineWidth += width;
                bool isSelected = i == this.viewIndex;
                var label = new Label
                {
                    Text = this.views[i].Name,
                    Font = font,
                    TextColor = isSelected ? MyraSkin.TextTitle : MyraSkin.TextSecondary,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };
                var content = new Panel();
                content.Widgets.Add(label);
                content.Widgets.Add(new Panel
                {
                    Height = ViewTabUnderline,
                    Margin = new Thickness(ViewTabPadding / 4, 0),
                    VerticalAlignment = VerticalAlignment.Bottom,
                    Background = new SolidBrush(isSelected ? Accent : Color.Transparent)
                });
                var button = new Button
                {
                    Content = content,
                    Width = width,
                    Height = ViewTabHeight,
                    Background = Transparent,
                    FocusedBackground = Transparent,
                    OverBackground = Transparent,
                    PressedBackground = Transparent
                };
                if (!isSelected)
                {
                    button.MouseEntered += (sender, args) => label.TextColor = MyraSkin.TextPrimary;
                    button.MouseLeft += (sender, args) => label.TextColor = MyraSkin.TextSecondary;
                }
                int index = i;
                button.Click += (sender, args) => this.SelectView(index);
                line.Widgets.Add(button);
            }
            int lines = panel.Widgets.Count;
            height = lines * ViewTabHeight + (lines - 1) * ViewTabLineSpacing;
            panel.Height = height;
            return panel;
        }

        // 换视图：列和宽度都变了，整个面板重建，保留筛选和刚打开过的那一行的高亮
        private void SelectView(int index)
        {
            if (index == this.viewIndex)
            {
                return;
            }
            this.viewIndex = index;
            this.sortColumn = -1;
            this.descending = false;
            this.LayoutView();
            this.root.Widgets.Clear();
            this.filterPills.Clear();
            this.headerLabels.Clear();
            this.rowWidgets.Clear();
            this.root.Widgets.Add(this.BuildDialog());
            this.UpdateVisibleItems();
            if (this.HasDetail)
            {
                // 详情栏是新建的，把选中的那一项重新填上
                this.SetSelected(this.selected ?? this.visibleItems.FirstOrDefault());
            }
        }

        // 筛选页签按字宽定宽，一行放不下就换行
        // 所有筛选页签排成一行要多宽
        private int GetFiltersWidth()
        {
            SpriteFontBase font = TextManager.GetFont(PillFontSize);
            return this.filters.Sum(x => GetPillWidth(font, x.Name)) + (this.filters.Count - 1) * PillSpacing;
        }

        private static int GetPillWidth(SpriteFontBase font, string text)
        {
            return (int)Math.Ceiling(font.MeasureString(text).X) + PillPadding;
        }

        private Widget BuildFilters(int maxWidth, out int height)
        {
            SpriteFontBase font = TextManager.GetFont(PillFontSize);
            var panel = new VerticalStackPanel
            {
                Spacing = PillSpacing
            };
            HorizontalStackPanel line = null;
            int lineWidth = 0;
            for (int i = 0; i < this.filters.Count; i++)
            {
                int width = GetPillWidth(font, this.filters[i].Name);
                if (line == null || lineWidth + PillSpacing + width > maxWidth)
                {
                    line = new HorizontalStackPanel
                    {
                        Spacing = PillSpacing
                    };
                    panel.Widgets.Add(line);
                    lineWidth = width;
                }
                else
                {
                    lineWidth += PillSpacing + width;
                }
                var label = new Label
                {
                    Text = this.filters[i].Name,
                    Font = font,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };
                var button = new Button
                {
                    Content = label,
                    Width = width,
                    Height = PillHeight
                };
                int index = i;
                button.Click += (sender, args) =>
                {
                    if (this.filterIndex == index)
                    {
                        return;
                    }
                    this.filterIndex = index;
                    this.RefreshFilterPills();
                    this.UpdateVisibleItems();
                    // 选中的那一项筛掉了，详情栏换成第一项
                    if (this.HasDetail && !this.visibleItems.Any(x => ReferenceEquals(x, this.selected)))
                    {
                        this.SetSelected(this.visibleItems.FirstOrDefault());
                    }
                };
                line.Widgets.Add(button);
                this.filterPills.Add((button, label, i));
            }
            int lines = panel.Widgets.Count;
            height = lines * PillHeight + (lines - 1) * PillSpacing;
            panel.Height = height;
            this.RefreshFilterPills();
            return panel;
        }

        // 选中的金底深字，其余细边框浅字，鼠标移上去稍微亮一点
        private void RefreshFilterPills()
        {
            foreach (var (button, label, index) in this.filterPills)
            {
                bool isSelected = index == this.filterIndex;
                FrameBrush brush = isSelected
                    ? new FrameBrush(Accent, Accent, false)
                    : new FrameBrush(Color.Transparent, LineColor, false);
                IBrush over = isSelected ? brush : brush.WithFill(RowHoverColor);
                button.Background = brush;
                button.FocusedBackground = brush;
                button.OverBackground = over;
                button.PressedBackground = over;
                label.TextColor = isSelected ? PillSelectedText : MyraSkin.TextPrimary;
            }
        }

        // 列都排开比行宽时，列表横向滚动，表头跟着列表左右移
        private bool ScrollsHorizontally => this.contentWidth > this.rowWidth;

        // 左栏：表头、列表、翻页
        private Widget BuildList(int height)
        {
            var panel = new VerticalStackPanel
            {
                Width = this.listWidth,
                Height = height,
                Spacing = ListSpacing
            };
            this.rows = new VerticalStackPanel();
            this.listViewerHeight = height - HeaderHeight - PagerHeight - 2 * ListSpacing;
            this.listViewer = new ScrollViewer
            {
                Content = this.rows,
                ShowHorizontalScrollBar = this.ScrollsHorizontally,
                Width = this.listWidth,
                Height = this.listViewerHeight
            };
            panel.Widgets.Add(this.BuildHeader());
            this.dragScroll.Add(this.listViewer);
            panel.Widgets.Add(this.listViewer);
            this.pager = this.BuildPager();
            panel.Widgets.Add(this.pager);
            return panel;
        }

        // 表头只有小字，下面一条分隔线。能排序的列点一下按升序，再点反向，排序中的列金字带箭头
        private Widget BuildHeader()
        {
            var header = new Panel
            {
                Width = this.contentWidth,
                Height = HeaderHeight,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            var cells = new HorizontalStackPanel
            {
                Spacing = CellSpacing,
                Padding = new Thickness(RowPaddingLeft, 0, 0, 0)
            };
            SpriteFontBase font = TextManager.GetFont(HeaderFontSize);
            IReadOnlyList<InfoColumn<T>> columns = this.Columns;
            for (int i = 0; i < columns.Count; i++)
            {
                InfoColumn<T> column = columns[i];
                int width = this.GetColumnWidth(column);
                var label = new Label
                {
                    Text = column.Header,
                    Font = font,
                    TextColor = MyraSkin.TextSecondary,
                    TextAlign = this.IsLeftAligned(i) ? TextHorizontalAlignment.Left : TextHorizontalAlignment.Center,
                    Width = width,
                    VerticalAlignment = VerticalAlignment.Center
                };
                if (!column.CanSort)
                {
                    cells.Widgets.Add(label);
                    continue;
                }
                var button = new Button
                {
                    Content = label,
                    Width = width,
                    Height = HeaderHeight,
                    Background = Transparent,
                    FocusedBackground = Transparent,
                    OverBackground = Transparent,
                    PressedBackground = Transparent
                };
                int index = i;
                button.MouseEntered += (sender, args) => label.TextColor = this.sortColumn == index ? Accent : MyraSkin.TextPrimary;
                button.MouseLeft += (sender, args) => label.TextColor = this.sortColumn == index ? Accent : MyraSkin.TextSecondary;
                button.Click += (sender, args) =>
                {
                    this.descending = this.sortColumn == index && !this.descending;
                    this.sortColumn = index;
                    this.RefreshHeader();
                    this.UpdateVisibleItems();
                };
                cells.Widgets.Add(button);
                this.headerLabels.Add((label, i));
            }
            header.Widgets.Add(cells);
            header.Widgets.Add(CreateDivider());
            if (!this.ScrollsHorizontally)
            {
                return header;
            }
            // 表头放进一个不显示滚动条的滚动区，每次画之前对齐列表的横向位置。
            // 内容比列表多留出竖滚动条的宽，两边能滚的距离才一样
            var holder = new Panel
            {
                Width = this.contentWidth + ScrollBarWidth,
                Height = HeaderHeight
            };
            holder.Widgets.Add(header);
            return new FollowingScrollViewer(this.listViewer)
            {
                Content = holder,
                ShowHorizontalScrollBar = false,
                ShowVerticalScrollBar = false,
                Width = this.listWidth,
                Height = HeaderHeight
            };
        }

        private void RefreshHeader()
        {
            foreach (var (label, index) in this.headerLabels)
            {
                bool sorting = index == this.sortColumn;
                label.TextColor = sorting ? Accent : MyraSkin.TextSecondary;
                label.Text = this.Columns[index].Header + (sorting ? (this.descending ? " ▼" : " ▲") : "");
            }
        }

        // 第一列一般是名称，自己画的单元格（标签、进度条）也从左边排；其余居中
        private bool IsLeftAligned(int column)
        {
            return column == 0 || this.Columns[column].Cell != null;
        }

        // 翻页：两边是文字按钮，中间写页数和数量
        private Widget BuildPager()
        {
            var pager = new Panel
            {
                Width = this.rowWidth,
                Height = PagerHeight,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            pager.Widgets.Add(this.CreateTextButton("‹ 上一页", HorizontalAlignment.Left, () => this.TurnPage(-1), out this.prevLabel));
            this.pagerLabel = new Label
            {
                Font = TextManager.GetFont(14),
                TextColor = MyraSkin.TextSecondary,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            pager.Widgets.Add(this.pagerLabel);
            pager.Widgets.Add(this.CreateTextButton("下一页 ›", HorizontalAlignment.Right, () => this.TurnPage(1), out this.nextLabel));
            return pager;
        }

        private Button CreateTextButton(string text, HorizontalAlignment alignment, Action onClick, out Label label)
        {
            Label content = new Label
            {
                Text = text,
                Font = TextManager.GetFont(15),
                TextColor = MyraSkin.TextSecondary,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            var button = new Button
            {
                Content = content,
                Width = PagerButtonWidth,
                Height = PagerHeight,
                HorizontalAlignment = alignment,
                Background = Transparent,
                FocusedBackground = Transparent,
                OverBackground = Transparent,
                PressedBackground = Transparent
            };
            // 变灰的按钮不变亮
            button.MouseEntered += (sender, args) =>
            {
                if (content.TextColor != MyraSkin.TextDisabled)
                {
                    content.TextColor = Accent;
                }
            };
            button.MouseLeft += (sender, args) =>
            {
                if (content.TextColor != MyraSkin.TextDisabled)
                {
                    content.TextColor = MyraSkin.TextSecondary;
                }
            };
            button.Click += (sender, args) => onClick();
            label = content;
            return button;
        }

        private int PageCount => Math.Max(1, (this.visibleItems.Count + PageSize - 1) / PageSize);

        private void TurnPage(int delta)
        {
            int page = MathHelper.Clamp(this.page + delta, 0, this.PageCount - 1);
            if (page == this.page)
            {
                return;
            }
            this.page = page;
            this.RefreshRows();
            this.listViewer.ScrollPosition = new Point(this.listViewer.ScrollPosition.X, 0);
        }

        // 右侧详情栏：与人物、部队详情侧栏一样的不带标题横线的底板，内容多时滚动
        private Widget BuildDetail(int height)
        {
            var panel = new Panel
            {
                Width = this.detailWidth,
                Height = height,
                Padding = new Thickness(DetailPadding),
                Background = MyraSkin.PlainPanelBackground
            };
            this.detail = new VerticalStackPanel { Spacing = 6 };
            this.detailViewer = new ScrollViewer
            {
                Content = this.detail,
                ShowHorizontalScrollBar = false,
                Width = this.detailWidth - 2 * DetailPadding,
                Height = height - 2 * DetailPadding
            };
            this.dragScroll.Add(this.detailViewer);
            panel.Widgets.Add(this.detailViewer);
            return panel;
        }

        private int GetColumnWidth(InfoColumn<T> column)
        {
            return column.Width > 0 ? column.Width : this.fillColumnWidth;
        }

        // 按筛选和排序重新取出要显示的项，回到第一页
        private void UpdateVisibleItems()
        {
            IEnumerable<T> items = this.allItems;
            if (this.filters.Count > 0)
            {
                items = items.Where(this.filters[this.filterIndex].Match);
            }
            if (this.sortColumn >= 0)
            {
                Func<T, IComparable> key = this.Columns[this.sortColumn].SortKey;
                items = this.descending ? items.OrderByDescending(key) : items.OrderBy(key);
            }
            this.visibleItems = items.ToList();
            this.page = 0;
            this.RefreshRows();
            this.listViewer.ScrollPosition = new Point(this.listViewer.ScrollPosition.X, 0);
        }

        private void RefreshRows()
        {
            this.rows.Widgets.Clear();
            this.rowWidgets.Clear();
            foreach (T item in this.visibleItems.Skip(this.page * PageSize).Take(PageSize))
            {
                this.rows.Widgets.Add(this.CreateRow(item));
            }
            if (this.visibleItems.Count == 0)
            {
                Label empty = CreateLabel("没有符合条件的项", 15, MyraSkin.TextSecondary);
                empty.Margin = new Thickness(RowPaddingLeft, 12, 0, 0);
                this.rows.Widgets.Add(empty);
            }
            this.countLabel.Text = this.visibleItems.Count + " 项";
            // 只有一页时不要翻页栏，列表往下占满
            bool paged = this.PageCount > 1;
            this.pager.Visible = paged;
            this.listViewer.Height = paged ? this.listViewerHeight : this.listViewerHeight + PagerHeight + ListSpacing;
            this.pagerLabel.Text = "第 " + (this.page + 1) + " / " + this.PageCount + " 页";
            this.prevLabel.TextColor = this.page > 0 ? MyraSkin.TextSecondary : MyraSkin.TextDisabled;
            this.nextLabel.TextColor = this.page < this.PageCount - 1 ? MyraSkin.TextSecondary : MyraSkin.TextDisabled;
            this.RefreshRowLooks();
        }

        // 一行：左边一条金色竖条（选中时显示），各列内容，底下一条分隔线
        private Button CreateRow(T item)
        {
            var cells = new HorizontalStackPanel
            {
                Spacing = CellSpacing,
                Padding = new Thickness(RowPaddingLeft, 0, 0, 0),
                VerticalAlignment = VerticalAlignment.Center
            };
            IReadOnlyList<InfoColumn<T>> columns = this.Columns;
            for (int i = 0; i < columns.Count; i++)
            {
                InfoColumn<T> column = columns[i];
                int width = this.GetColumnWidth(column);
                if (column.Cell != null)
                {
                    var holder = new Panel
                    {
                        Width = width,
                        ClipToBounds = true,
                        VerticalAlignment = VerticalAlignment.Center
                    };
                    Widget cell = column.Cell(item, width);
                    cell.VerticalAlignment = VerticalAlignment.Center;
                    holder.Widgets.Add(cell);
                    cells.Widgets.Add(holder);
                    continue;
                }
                SpriteFontBase font = TextManager.GetFont(i == 0 ? 17 : 16);
                cells.Widgets.Add(new Label
                {
                    Text = FitText(font, column.Text(item), width),
                    Font = font,
                    TextColor = column.Color?.Invoke(item) ?? MyraSkin.TextPrimary,
                    TextAlign = this.IsLeftAligned(i) ? TextHorizontalAlignment.Left : TextHorizontalAlignment.Center,
                    Width = width,
                    VerticalAlignment = VerticalAlignment.Center
                });
            }
            var accentBar = new Panel
            {
                Width = AccentWidth,
                HorizontalAlignment = HorizontalAlignment.Left,
                Background = new SolidBrush(Accent),
                Visible = false
            };
            var content = new Panel();
            content.Widgets.Add(accentBar);
            content.Widgets.Add(cells);
            if (this.open != null)
            {
                content.Widgets.Add(new Label
                {
                    Text = "›",
                    Font = TextManager.GetFont(20),
                    TextColor = MyraSkin.TextSecondary,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(0, 0, 8, 0)
                });
            }
            content.Widgets.Add(CreateDivider());
            var button = new Button
            {
                Content = content,
                Width = this.contentWidth,
                Height = RowHeight
            };
            button.Click += (sender, args) =>
            {
                this.SetSelected(item);
                this.open?.Invoke(item);
            };
            this.rowWidgets[item] = (button, accentBar);
            return button;
        }

        // 文字比列宽长时截短，以省略号结尾，不压到后面的列上
        private static string FitText(SpriteFontBase font, string text, int width)
        {
            if (string.IsNullOrEmpty(text) || font.MeasureString(text).X <= width)
            {
                return text;
            }
            const string Ellipsis = "…";
            int length = text.Length - 1;
            while (length > 0 && font.MeasureString(text.Substring(0, length) + Ellipsis).X > width)
            {
                length--;
            }
            return text.Substring(0, length) + Ellipsis;
        }

        private static Panel CreateDivider()
        {
            return new Panel
            {
                Height = 1,
                VerticalAlignment = VerticalAlignment.Bottom,
                Background = new SolidBrush(DividerColor)
            };
        }

        // 高亮选中的那一行：有详情栏时在右侧显示它的详情；没有时是刚点开的那一行，关掉详情回到列表时还看得出看到哪了
        private void SetSelected(T item)
        {
            this.selected = item;
            this.RefreshRowLooks();
            if (!this.HasDetail)
            {
                return;
            }
            this.detail.Widgets.Clear();
            if (item == null)
            {
                this.detail.Widgets.Add(CreateLabel("没有可显示的项", 14, MyraSkin.TextSecondary));
            }
            else
            {
                this.fillDetail(this.detail, item, this.detailWidth - 2 * DetailPadding - ScrollBarWidth - DetailScrollGap, this);
            }
            this.detailViewer.ScrollPosition = Point.Zero;
        }

        // 选中的行底色加深、显示竖条；其余透明，鼠标移上去稍亮
        private void RefreshRowLooks()
        {
            foreach (var pair in this.rowWidgets)
            {
                bool isSelected = pair.Key == this.selected;
                Button button = pair.Value.Button;
                IBrush normal = isSelected ? new SolidBrush(RowSelectedColor) : Transparent;
                IBrush over = isSelected ? normal : new SolidBrush(RowHoverColor);
                button.Background = normal;
                button.FocusedBackground = normal;
                button.OverBackground = over;
                button.PressedBackground = over;
                pair.Value.AccentBar.Visible = isSelected;
            }
        }

        /// <summary>
        /// 跟着另一个滚动区左右移动的滚动区，用来放表头。Myra的滚动区没有滚动事件，画之前对齐一次
        /// </summary>
        private class FollowingScrollViewer : ScrollViewer
        {
            private readonly ScrollViewer leader;

            public FollowingScrollViewer(ScrollViewer leader)
            {
                this.leader = leader;
            }

            public override void InternalRender(RenderContext context)
            {
                if (this.ScrollPosition.X != this.leader.ScrollPosition.X)
                {
                    this.ScrollPosition = new Point(this.leader.ScrollPosition.X, 0);
                }
                base.InternalRender(context);
            }
        }
    }
}

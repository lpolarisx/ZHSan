using FontStashSharp.RichText;
using GameGlobal;
using GameManager;
using GameObjects;
using GameObjects.FactionDetail;
using Microsoft.Xna.Framework;
using Myra.Graphics2D;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WorldOfTheThreeKingdoms.GameScreens
{
    /// <summary>
    /// 外交关系列表：列出一个势力与其他势力的关系和剩余停战天数，只用来查看。
    /// 点表头的关系、停战按该列排序，再点一次反向。用Myra绘制，外观与外交面板一致。
    /// </summary>
    internal class DiplomaticRelationListDialog
    {
        private const int DialogPadding = 28;
        private const int FactionColumnWidth = 200;
        private const int ValueColumnWidth = 110;
        private const int CellSpacing = 8;
        // 行内容左边留的空
        private const int RowPaddingLeft = 8;
        private const int RowHeight = 28;
        private const int RowSpacing = 2;
        private const int RowWidth = RowPaddingLeft + FactionColumnWidth + 2 * (CellSpacing + ValueColumnWidth);
        // 竖向滚动条占的宽度
        private const int ScrollBarWidth = 20;
        private const int ContentWidth = RowWidth + ScrollBarWidth;
        private const int MaxListHeight = 390;
        private const int CloseButtonSize = 28;

        private enum SortColumn
        {
            Relation,
            Truce
        }

        private readonly Faction faction;
        private readonly List<(DiplomaticRelationDisplay Display, Button Row)> items = new List<(DiplomaticRelationDisplay, Button)>();
        private readonly VerticalStackPanel rows = new VerticalStackPanel { Spacing = RowSpacing };

        private ScrollViewer scrollViewer;
        private Button relationHeader;
        private Button truceHeader;
        // 默认关系好的排在前面，与外交面板一致
        private SortColumn sortColumn = SortColumn.Relation;
        private bool descending = true;

        private DiplomaticRelationListDialog(Faction faction)
        {
            this.faction = faction;
        }

        /// <summary>
        /// 显示外交关系列表
        /// </summary>
        public static void Show(Faction faction)
        {
            var dialog = new DiplomaticRelationListDialog(faction);
            // 面板显示期间占住一个未完成工作，地图和工具栏就不会响应鼠标键盘
            Session.MainGame.mainGameScreen.PushUndoneWork(new UndoneWorkItem(UndoneWorkKind.SubDialog, DialogKind.Diplomacy));
            MyraUI.Show(dialog.Build(), dialog.Close);
        }

        private void Close()
        {
            MyraUI.Close(() =>
            {
                if (Session.MainGame.mainGameScreen.PopUndoneWork().Kind != UndoneWorkKind.SubDialog)
                {
                    throw new Exception("The UndoneWork is not a SubDialog.");
                }
            });
        }

        private Widget Build()
        {
            var dialog = new VerticalStackPanel
            {
                Width = ContentWidth + 2 * DialogPadding,
                Spacing = 8,
                // 上边留得少，让标题落在底板那条横线的上面
                Padding = new Thickness(DialogPadding, 7, DialogPadding, DialogPadding),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Background = MyraSkin.PanelBackground
            };

            var titleBar = new Panel();
            titleBar.Widgets.Add(new Label
            {
                Text = this.faction.Name + " 外交关系",
                Font = TextManager.GetFont(24),
                TextColor = MyraSkin.TextTitle,
                HorizontalAlignment = HorizontalAlignment.Center
            });
            titleBar.Widgets.Add(this.CreateCloseButton());
            dialog.Widgets.Add(titleBar);
            dialog.Widgets.Add(this.BuildHeader());

            foreach (var display in Session.Current.Scenario.GetDiplomaticRelationDisplays(this.faction.ID))
            {
                this.items.Add((display, CreateRow(display)));
            }
            this.scrollViewer = new ScrollViewer
            {
                Content = this.rows,
                ShowHorizontalScrollBar = false,
                Width = ContentWidth,
                Height = Math.Clamp(this.items.Count * (RowHeight + RowSpacing), RowHeight, MaxListHeight)
            };
            dialog.Widgets.Add(this.scrollViewer);

            this.Refresh();

            // 半透明遮罩铺满窗口，压暗后面的游戏界面
            var mask = new Panel
            {
                Background = new SolidBrush(new Color(0, 0, 0, 150))
            };
            mask.Widgets.Add(dialog);
            return mask;
        }

        // 叉号按钮太小，用不了按钮底图（四角各14像素会挤变形），平时透明，鼠标移上去才显出底色
        private Button CreateCloseButton()
        {
            var background = new SolidBrush(Color.Transparent);
            var over = new SolidBrush(MyraSkin.ListRowOverColor);
            var button = new Button
            {
                Content = new Label
                {
                    Text = "×",
                    Font = TextManager.GetFont(20),
                    TextColor = MyraSkin.TextPrimary,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                },
                Width = CloseButtonSize,
                Height = CloseButtonSize,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center,
                Background = background,
                FocusedBackground = background,
                OverBackground = over,
                PressedBackground = over
            };
            button.Click += (sender, args) => this.Close();
            return button;
        }

        private Widget BuildHeader()
        {
            var header = new HorizontalStackPanel
            {
                Spacing = CellSpacing,
                Padding = new Thickness(RowPaddingLeft, 0, 0, 0)
            };
            header.Widgets.Add(CreateCell("势力", MyraSkin.TextSecondary, TextHorizontalAlignment.Left, FactionColumnWidth));
            this.relationHeader = this.CreateSortButton("关系", SortColumn.Relation);
            header.Widgets.Add(this.relationHeader);
            this.truceHeader = this.CreateSortButton("停战", SortColumn.Truce);
            header.Widgets.Add(this.truceHeader);
            return header;
        }

        private Button CreateSortButton(string text, SortColumn column)
        {
            Button button = MyraSkin.CreateButton(text, ValueColumnWidth, out _);
            button.Click += (sender, args) =>
            {
                // 换一列时先把大的排在前面，点当前列才反向
                this.descending = this.sortColumn != column || !this.descending;
                this.sortColumn = column;
                this.Refresh();
                this.scrollViewer.ScrollPosition = Point.Zero;
            };
            return button;
        }

        private static Button CreateRow(DiplomaticRelationDisplay display)
        {
            var content = new HorizontalStackPanel
            {
                Spacing = CellSpacing,
                Padding = new Thickness(RowPaddingLeft, 0, 0, 0)
            };
            content.Widgets.Add(CreateCell(display.FactionName, MyraSkin.TextPrimary, TextHorizontalAlignment.Left, FactionColumnWidth));
            content.Widgets.Add(CreateCell(display.Relation.ToString(), DiplomacyDialog.GetStatusColor(display.LinkedDiplomaticRelation), TextHorizontalAlignment.Center, ValueColumnWidth));
            content.Widgets.Add(display.Truce > 0
                ? CreateCell(display.Truce + "天", MyraSkin.TextHighlight, TextHorizontalAlignment.Center, ValueColumnWidth)
                : CreateCell("——", MyraSkin.TextDisabled, TextHorizontalAlignment.Center, ValueColumnWidth));

            var background = new SolidBrush(MyraSkin.ListRowColor);
            var over = new SolidBrush(MyraSkin.ListRowOverColor);
            return new Button
            {
                Content = content,
                Width = RowWidth,
                Height = RowHeight,
                Background = background,
                FocusedBackground = background,
                OverBackground = over,
                PressedBackground = over
            };
        }

        private static Label CreateCell(string text, Color color, TextHorizontalAlignment align, int width)
        {
            return new Label
            {
                Text = text,
                Font = TextManager.GetFont(16),
                TextColor = color,
                TextAlign = align,
                Width = width,
                VerticalAlignment = VerticalAlignment.Center
            };
        }

        private void Refresh()
        {
            MyraSkin.SetButtonLook(this.relationHeader, this.sortColumn == SortColumn.Relation ? MyraButtonLook.Selected : MyraButtonLook.Normal);
            MyraSkin.SetButtonLook(this.truceHeader, this.sortColumn == SortColumn.Truce ? MyraButtonLook.Selected : MyraButtonLook.Normal);

            Func<DiplomaticRelationDisplay, int> key = this.sortColumn == SortColumn.Relation ? x => x.Relation : x => x.Truce;
            var sorted = this.descending ? this.items.OrderByDescending(x => key(x.Display)) : this.items.OrderBy(x => key(x.Display));
            // 没有停战的势力很多，停战天数相同时再按关系排
            this.rows.Widgets.Clear();
            foreach (var item in sorted.ThenByDescending(x => x.Display.Relation))
            {
                this.rows.Widgets.Add(item.Row);
            }
        }
    }
}

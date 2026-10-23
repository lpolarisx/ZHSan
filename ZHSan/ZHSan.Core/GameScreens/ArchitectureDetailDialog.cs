using GameEnums;
using GameGlobal;
using GameManager;
using GameObjects;
using GameObjects.ArchitectureDetail;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra.Events;
using Myra.Graphics2D;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.UI;
using Platforms;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WorldOfTheThreeKingdoms.GameScreens
{
    /// <summary>
    /// 建筑详情，只用来查看。顶栏是建筑图和资金、军粮、人口，左列是发展和特色，右侧分设施和概况两页。
    /// 显示的数据与旧的建筑详情插件一致，设施改成带图的卡片，点卡片在右边看说明。
    /// </summary>
    internal class ArchitectureDetailDialog
    {
        // 内容离底板边缘的距离，要避开底板四周的花纹
        private const int DialogPadding = 28;
        private const int ContentWidth = 924;
        private const int DialogWidth = ContentWidth + 2 * DialogPadding;
        private const int SectionSpacing = 10;
        // 分区也用底板图做边框，内容要避开四周的花纹
        private const int PanelPadding = 14;
        // 分区标题占的高度。加上上边距和间距，正好让下面的内容从底板那条横线之下开始
        private const int PanelTitleTop = 12;
        private const int PanelTitleHeight = 28;
        private const int RowHeight = 20;

        // 顶栏：建筑图、资金军粮人口三个核心数、其他标签
        // 建筑类型图原图448x266，按比例缩小
        private const int PictureWidth = 160;
        private const int PictureHeight = PictureWidth * 266 / 448;
        private const int HeaderHeight = PictureHeight;
        // 有几张类型图底部带了十几行白边，画的时候裁掉底部16行，左右各裁一点保持比例
        private static readonly Rectangle KindPictureSource = new Rectangle(13, 0, 422, 250);
        private const int StatCardWidth = 190;
        private const int HeaderInfoWidth = ContentWidth - PictureWidth - 3 * StatCardWidth - 4 * SectionSpacing;

        // 下半部分：左列发展和特色，右侧设施和概况
        private const int BodyHeight = 440;
        private const int LeftColumnWidth = 300;
        private const int DevelopmentHeight = 210;
        private const int CharacteristicHeight = BodyHeight - DevelopmentHeight - SectionSpacing;
        private const int BarHeight = 8;
        private const int BarLabelWidth = 40;
        private const int BarValueWidth = 80;
        // 进度条和数值之间的空隙
        private const int BarValueGap = 12;
        private const int RightPanelWidth = ContentWidth - LeftColumnWidth - SectionSpacing;
        private const int RightInnerWidth = RightPanelWidth - 2 * PanelPadding;
        private const int RightBodyHeight = BodyHeight - PanelTitleTop - PanelTitleHeight - 6 - PanelPadding;
        private const int TabButtonWidth = 80;

        // 概况页：每行2项
        private const int SummaryColumns = 2;
        private const int SummaryLabelWidth = 100;

        // 设施图原图165x91，卡片上按比例缩小
        private const int CardWidth = 100;
        private const int CardHeight = 88;
        private const int CardSpacing = 6;
        private const int CardsPerRow = 3;
        private const int CardPictureWidth = CardWidth - 8;
        private const int CardPictureHeight = CardPictureWidth * 91 / 165;
        private const int ScrollBarWidth = 18;
        private const int FacilityListWidth = CardsPerRow * CardWidth + (CardsPerRow - 1) * CardSpacing + ScrollBarWidth;
        private const int FacilityDetailWidth = RightInnerWidth - FacilityListWidth - SectionSpacing;
        private const int DetailPictureWidth = 165;
        private const int DetailPictureHeight = 91;
        private const int FilterButtonWidth = 44;
        private const int FilterButtonHeight = 22;
        private const int FacilityBodyHeight = RightBodyHeight - FilterButtonHeight - 6;
        // 设施空间格子条最宽占多少
        private const int SpaceBarMaxWidth = 180;
        // 按住拖动超过这个距离才算滑动列表，否则算点击卡片
        private const int DragThreshold = 6;

        private const string KindPicturePath = "Content/Textures/GameComponents/ArchitectureDetail/Data/TheInformationPage/ArchitectureKindPicture/";
        private const string FacilityPicturePath = "Content/Textures/GameComponents/ArchitectureDetail/Data/TheFacilityPage/Facility/";

        private static readonly Color LineColor = new Color(90, 110, 120);
        private static readonly Color BarBackColor = new Color(16, 24, 30);
        private static readonly Color SpaceEmptyColor = new Color(50, 66, 76);
        // 进度条中档的蓝色，与其他文字配色一样降低了饱和度
        private static readonly Color BarBlueColor = new Color(0x5E, 0x9C, 0xC8);
        // 筛选按钮太窄，用不了底图，改用纯色，颜色取自底图的蓝和绿
        private static readonly Color FilterColor = MyraSkin.ListRowColor;
        private static readonly Color FilterSelectedColor = MyraSkin.ListRowSelectedColor;
        private static readonly Color FilterOverColor = new Color(21, 117, 2);

        private readonly Architecture architecture;
        // 正在建造的设施，没有时为null
        private readonly FacilityKindLevel buildingLevel;
        private readonly List<FacilityItem> items = new List<FacilityItem>();
        private readonly List<Button> filterButtons = new List<Button>();
        private readonly List<ScrollViewer> dragViewers = new List<ScrollViewer>();

        // null表示全部
        private FacilityType? filter;
        private FacilityItem selected;
        private VerticalStackPanel cardsPanel;
        private ScrollViewer cardsViewer;
        private VerticalStackPanel detailPanel;
        private ScrollViewer detailViewer;
        private Label facilityTabLabel;
        private Label summaryTabLabel;
        private Panel facilityTabLine;
        private Panel summaryTabLine;
        private Widget facilityPage;
        private Widget summaryPage;

        private ScrollViewer dragViewer;
        private int? dragLastY;
        private int dragDistance;

        private ArchitectureDetailDialog(Architecture architecture)
        {
            this.architecture = architecture;
            var common = Session.Current.Scenario.GameCommonData;
            common.AllFacilityKindLevels.TryGetValue(architecture.BuildingFacility, out this.buildingLevel);
            if (this.buildingLevel != null)
            {
                this.items.Add(new FacilityItem(null, this.buildingLevel, architecture.BuildingDaysLeft));
            }
            foreach (Facility facility in architecture.Facilities)
            {
                if (common.AllFacilityKindLevels.TryGetValue(facility.LevelId, out FacilityKindLevel level))
                {
                    this.items.Add(new FacilityItem(facility, level, 0));
                }
            }
        }

        public static void Show(Architecture architecture)
        {
            if (architecture == null)
            {
                return;
            }
            var dialog = new ArchitectureDetailDialog(architecture);
            // 面板显示期间占住一个未完成工作，地图和工具栏就不会响应鼠标键盘
            Session.MainGame.mainGameScreen.PushUndoneWork(new UndoneWorkItem(UndoneWorkKind.SubDialog, DialogKind.ArchitectureDetail));
            Desktop desktop = MyraUI.Desktop;
            desktop.TouchDown += dialog.Desktop_TouchDown;
            desktop.TouchMoved += dialog.Desktop_TouchMoved;
            desktop.TouchUp += dialog.Desktop_TouchUp;
            MyraUI.Show(dialog.Build(), dialog.Close);
        }

        private void Close()
        {
            Desktop desktop = MyraUI.Desktop;
            desktop.TouchDown -= this.Desktop_TouchDown;
            desktop.TouchMoved -= this.Desktop_TouchMoved;
            desktop.TouchUp -= this.Desktop_TouchUp;
            MyraUI.Close(() =>
            {
                // 等鼠标键松开之后再把输入还给地图，关闭面板的这次点击才不会落到地图上
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
                Width = DialogWidth,
                Spacing = SectionSpacing,
                // 上边留得少，让标题落在底板那条横线的上面
                Padding = new Thickness(DialogPadding, 7, DialogPadding, DialogPadding),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Background = MyraSkin.PanelBackground
            };
            dialog.Widgets.Add(this.BuildTitle());
            dialog.Widgets.Add(this.BuildHeader());

            var body = new HorizontalStackPanel
            {
                Spacing = SectionSpacing
            };
            var left = new VerticalStackPanel
            {
                Width = LeftColumnWidth,
                Spacing = SectionSpacing
            };
            left.Widgets.Add(this.BuildDevelopmentPanel());
            left.Widgets.Add(this.BuildCharacteristicPanel());
            body.Widgets.Add(left);
            body.Widgets.Add(this.BuildRightPanel());
            dialog.Widgets.Add(body);

            this.SelectTab(true);
            this.RefreshCards();

            // 半透明遮罩铺满窗口，压暗后面的游戏界面
            var mask = new Panel
            {
                Background = new SolidBrush(new Color(0, 0, 0, 150))
            };
            mask.Widgets.Add(dialog);
            return mask;
        }

        // 标题：城名和标签居中，右上角一个关闭按钮
        private Widget BuildTitle()
        {
            var row = new Panel
            {
                Height = 34
            };
            var title = new HorizontalStackPanel
            {
                Spacing = 12,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            title.Widgets.Add(CreateLabel(this.architecture.Name, 24, MyraSkin.TextTitle));
            Label sub = CreateLabel(string.Join(" · ", this.architecture.KindString, this.architecture.FactionString, this.architecture.StateString, this.architecture.RegionString), 16, MyraSkin.TextSecondary);
            sub.VerticalAlignment = VerticalAlignment.Bottom;
            title.Widgets.Add(sub);
            row.Widgets.Add(title);

            row.Widgets.Add(this.CreateCloseButton());
            return row;
        }

        // 关闭按钮只画一个“×”，不用按钮底图，鼠标移上去变成金色
        private Button CreateCloseButton()
        {
            var label = new Label
            {
                Text = "×",
                Font = TextManager.GetFont(28),
                TextColor = MyraSkin.TextSecondary,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            IBrush transparent = new SolidBrush(Color.Transparent);
            var close = new Button
            {
                Content = label,
                Width = 32,
                Height = 32,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center,
                Background = transparent,
                FocusedBackground = transparent,
                PressedBackground = transparent,
                OverBackground = transparent
            };
            close.MouseEntered += (sender, args) => label.TextColor = MyraSkin.TextHighlight;
            close.MouseLeft += (sender, args) => label.TextColor = MyraSkin.TextSecondary;
            close.Click += (sender, args) => this.Close();
            return close;
        }

        // 顶栏：建筑图、资金军粮人口（带增量）、规模等标签
        private Widget BuildHeader()
        {
            var header = new HorizontalStackPanel
            {
                Spacing = SectionSpacing,
                Height = HeaderHeight
            };
            string picture = KindPicturePath + this.architecture.Kind.ID + ".jpg";
            if (Platform.Current.FileExists(picture))
            {
                header.Widgets.Add(new Image
                {
                    Renderable = new TextureImage(picture, PictureWidth, PictureHeight, KindPictureSource),
                    Width = PictureWidth,
                    Height = PictureHeight,
                    Border = new SolidBrush(LineColor),
                    BorderThickness = new Thickness(1)
                });
            }
            else
            {
                header.Widgets.Add(CreatePlaceholder(PictureWidth, PictureHeight, this.architecture.KindString, 24));
            }

            Architecture a = this.architecture;
            header.Widgets.Add(CreateStatCard("资金", a.Fund.ToString("#,0"), FormatIncome(a.ExpectedFund) + "/月", a.ExpectedFund));
            header.Widgets.Add(CreateStatCard("军粮", a.Food.ToString("#,0"), FormatIncome(a.ExpectedFood) + "/月", a.ExpectedFood));
            header.Widgets.Add(CreateStatCard("人口", a.Population.ToString("#,0"), "增长 " + a.PopulationDevelopingRateString + "‱", a.PopulationDevelopingRate));

            var info = new VerticalStackPanel
            {
                Width = HeaderInfoWidth,
                Spacing = 4,
                VerticalAlignment = VerticalAlignment.Center
            };
            info.Widgets.Add(CreatePair("规模", a.JianzhuGuimo.ToString()));
            info.Widgets.Add(CreatePair("要冲", a.IsStrategicCenterString));
            info.Widgets.Add(CreatePair("内政", Math.Round(a.CurrentRateOfInternal * 100) + "%"));
            info.Widgets.Add(CreatePair("粮耗", a.FoodReduceDayRateString));
            header.Widgets.Add(info);
            return header;
        }

        // 核心数值卡片：名称、大号数值、增量。增量为正青绿，为负赭红
        private static Widget CreateStatCard(string title, string value, string delta, double deltaValue)
        {
            var card = new VerticalStackPanel
            {
                Width = StatCardWidth,
                Height = HeaderHeight,
                Spacing = 2,
                Padding = new Thickness(12, 8, 12, 8),
                Background = new SolidBrush(MyraSkin.ListRowColor),
                Border = new SolidBrush(LineColor),
                BorderThickness = new Thickness(1)
            };
            card.Widgets.Add(CreateLabel(title, 16, MyraSkin.TextSecondary, HorizontalAlignment.Left));
            card.Widgets.Add(CreateLabel(value, 26, MyraSkin.TextHighlight, HorizontalAlignment.Left));
            Color color = deltaValue > 0 ? MyraSkin.TextFriendly : deltaValue < 0 ? MyraSkin.TextHostile : MyraSkin.TextSecondary;
            card.Widgets.Add(CreateLabel(delta, 15, color, HorizontalAlignment.Left));
            return card;
        }

        private static string FormatIncome(int income)
        {
            return income > 0 ? "+" + income.ToString("#,0") : income.ToString("#,0");
        }

        private Widget BuildDevelopmentPanel()
        {
            VerticalStackPanel panel = CreatePanel(LeftColumnWidth, "发展");
            panel.Height = DevelopmentHeight;
            Architecture a = this.architecture;
            panel.Widgets.Add(CreateBarRow("农业", a.Agriculture, a.AgricultureCeiling, false));
            panel.Widgets.Add(CreateBarRow("商业", a.Commerce, a.CommerceCeiling, false));
            panel.Widgets.Add(CreateBarRow("技术", a.Technology, a.TechnologyCeiling, false));
            panel.Widgets.Add(CreateBarRow("民心", a.Morale, a.MoraleCeiling, false));
            panel.Widgets.Add(CreateBarRow("统治", a.Domination, a.DominationCeiling, true));
            panel.Widgets.Add(CreateBarRow("耐久", a.Endurance, a.EnduranceCeiling, true));
            return panel;
        }

        private Widget BuildCharacteristicPanel()
        {
            VerticalStackPanel panel = CreatePanel(LeftColumnWidth, "特色");
            panel.Height = CharacteristicHeight;
            var list = new VerticalStackPanel
            {
                Spacing = 6
            };
            int textWidth = LeftColumnWidth - 2 * PanelPadding - ScrollBarWidth;
            foreach (var influence in this.architecture.Characteristics.Values)
            {
                list.Widgets.Add(CreateWrappedLabel("• " + influence.Description, 15, MyraSkin.TextPrimary, textWidth));
            }
            if (list.Widgets.Count == 0)
            {
                list.Widgets.Add(CreateLabel("无", 16, MyraSkin.TextSecondary, HorizontalAlignment.Left));
            }
            var viewer = new ScrollViewer
            {
                Content = list,
                ShowHorizontalScrollBar = false,
                Height = CharacteristicHeight - PanelTitleTop - PanelTitleHeight - 6 - PanelPadding
            };
            this.dragViewers.Add(viewer);
            panel.Widgets.Add(viewer);
            return panel;
        }

        // 右侧大块：标题行切换设施和概况两页
        private Widget BuildRightPanel()
        {
            var panel = new VerticalStackPanel
            {
                Width = RightPanelWidth,
                Height = BodyHeight,
                Spacing = 6,
                Padding = new Thickness(PanelPadding, PanelTitleTop, PanelPadding, PanelPadding),
                Background = MyraSkin.PanelBackground
            };

            // 标题行：左边两个页签，右边设施空间和维持费
            var titleRow = new Panel
            {
                Height = PanelTitleHeight
            };
            var tabs = new HorizontalStackPanel
            {
                Spacing = 6,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top
            };
            tabs.Widgets.Add(CreateTab("设施（" + this.architecture.Facilities.Count + "）", TabButtonWidth + 20, () => this.SelectTab(true), out this.facilityTabLabel, out this.facilityTabLine));
            tabs.Widgets.Add(CreateTab("概况", TabButtonWidth, () => this.SelectTab(false), out this.summaryTabLabel, out this.summaryTabLine));
            titleRow.Widgets.Add(tabs);

            var right = new HorizontalStackPanel
            {
                Spacing = 8,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(0, 4, 0, 0)
            };
            right.Widgets.Add(this.CreateSpaceBar());
            right.Widgets.Add(CreatePair("空间", this.architecture.FacilityPositionString));
            right.Widgets.Add(CreatePair("维持", this.architecture.FacilityMaintenanceCostString));
            titleRow.Widgets.Add(right);
            panel.Widgets.Add(titleRow);

            var pages = new Panel
            {
                Height = RightBodyHeight
            };
            this.facilityPage = this.BuildFacilityPage();
            this.summaryPage = this.BuildSummaryPage();
            pages.Widgets.Add(this.facilityPage);
            pages.Widgets.Add(this.summaryPage);
            panel.Widgets.Add(pages);
            return panel;
        }

        // 页签：只有文字，选中的金色并在下面压一条金线，没选中的灰色
        private static Button CreateTab(string text, int width, Action onClick, out Label label, out Panel line)
        {
            label = new Label
            {
                Text = text,
                Font = TextManager.GetFont(18),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            line = new Panel
            {
                Height = 3,
                VerticalAlignment = VerticalAlignment.Bottom
            };
            var content = new Panel();
            content.Widgets.Add(label);
            content.Widgets.Add(line);
            IBrush transparent = new SolidBrush(Color.Transparent);
            var tab = new Button
            {
                Content = content,
                Width = width,
                Height = PanelTitleHeight,
                Background = transparent,
                FocusedBackground = transparent,
                PressedBackground = transparent,
                OverBackground = new SolidBrush(new Color(44, 64, 76, 120))
            };
            tab.Click += (sender, args) => onClick();
            return tab;
        }

        private void SelectTab(bool facility)
        {
            this.facilityTabLabel.TextColor = facility ? MyraSkin.TextTitle : MyraSkin.TextSecondary;
            this.summaryTabLabel.TextColor = facility ? MyraSkin.TextSecondary : MyraSkin.TextTitle;
            this.facilityTabLine.Background = new SolidBrush(facility ? MyraSkin.TextHighlight : Color.Transparent);
            this.summaryTabLine.Background = new SolidBrush(facility ? Color.Transparent : MyraSkin.TextHighlight);
            this.facilityPage.Visible = facility;
            this.summaryPage.Visible = !facility;
        }

        // 设施页：上面类别筛选，下面左边卡片、右边说明
        private Widget BuildFacilityPage()
        {
            var page = new VerticalStackPanel
            {
                Spacing = 6
            };
            var filters = new HorizontalStackPanel
            {
                Spacing = 6,
                Height = FilterButtonHeight
            };
            filters.Widgets.Add(this.CreateFilterButton("全部", null));
            filters.Widgets.Add(this.CreateFilterButton("通用", FacilityType.General));
            filters.Widgets.Add(this.CreateFilterButton("专用", FacilityType.Special));
            filters.Widgets.Add(this.CreateFilterButton("军事", FacilityType.Military));
            page.Widgets.Add(filters);

            var body = new HorizontalStackPanel
            {
                Spacing = SectionSpacing
            };
            this.cardsPanel = new VerticalStackPanel
            {
                Spacing = CardSpacing
            };
            this.cardsViewer = new ScrollViewer
            {
                Content = this.cardsPanel,
                ShowHorizontalScrollBar = false,
                Width = FacilityListWidth,
                Height = FacilityBodyHeight
            };
            this.dragViewers.Add(this.cardsViewer);
            body.Widgets.Add(this.cardsViewer);

            this.detailPanel = new VerticalStackPanel
            {
                Spacing = 4
            };
            this.detailViewer = new ScrollViewer
            {
                Content = this.detailPanel,
                ShowHorizontalScrollBar = false,
                Width = FacilityDetailWidth,
                Height = FacilityBodyHeight
            };
            this.dragViewers.Add(this.detailViewer);
            body.Widgets.Add(this.detailViewer);
            page.Widgets.Add(body);
            return page;
        }

        // 概况页：旧界面里不常看的数据，每行两项
        private Widget BuildSummaryPage()
        {
            var page = new VerticalStackPanel
            {
                Spacing = 8
            };
            Architecture a = this.architecture;
            var pairs = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("人物数量", a.PersonCount.ToString()),
                new KeyValuePair<string, string>("在野数量", a.NoFactionPersonCount.ToString()),
                new KeyValuePair<string, string>("编队数量", a.MilitaryCount.ToString()),
                new KeyValuePair<string, string>("军队数量", a.ArmyQuantity.ToString()),
                new KeyValuePair<string, string>("军队规模", a.ArmyScale.ToString()),
                new KeyValuePair<string, string>("设施有效", a.FacilityEnabledString),
                new KeyValuePair<string, string>("设施维持费", a.FacilityMaintenanceCostString),
                new KeyValuePair<string, string>("粮道维持费", a.RoutewayActiveCostString),
                new KeyValuePair<string, string>("情报维持费", a.InformationCostString),
                new KeyValuePair<string, string>("州效果", a.StateEffectString),
                new KeyValuePair<string, string>("地区效果", a.RegionEffectString),
            };
            int cellWidth = RightInnerWidth / SummaryColumns;
            HorizontalStackPanel row = null;
            foreach (var pair in pairs)
            {
                if (row == null || row.Widgets.Count >= SummaryColumns)
                {
                    row = new HorizontalStackPanel
                    {
                        Height = RowHeight
                    };
                    page.Widgets.Add(row);
                }
                var cell = new HorizontalStackPanel
                {
                    Width = cellWidth
                };
                Label label = CreateLabel(pair.Key, 16, MyraSkin.TextSecondary, HorizontalAlignment.Left);
                label.Width = SummaryLabelWidth;
                cell.Widgets.Add(label);
                cell.Widgets.Add(CreateLabel(pair.Value, 16, MyraSkin.TextPrimary, HorizontalAlignment.Left));
                row.Widgets.Add(cell);
            }
            return page;
        }

        private Button CreateFilterButton(string text, FacilityType? type)
        {
            var button = new Button
            {
                Content = new Label
                {
                    Text = text,
                    Font = TextManager.GetFont(14),
                    TextColor = MyraSkin.TextPrimary,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                },
                Width = FilterButtonWidth,
                Height = FilterButtonHeight,
                VerticalAlignment = VerticalAlignment.Top,
                Tag = type
            };
            button.Click += (sender, args) =>
            {
                this.filter = type;
                this.RefreshCards();
            };
            this.filterButtons.Add(button);
            return button;
        }

        // 设施空间画成一排格子：已占用的亮、空着的暗
        private Widget CreateSpaceBar()
        {
            int total = this.architecture.FacilityPositionCount;
            int used = Math.Min(total, total - this.architecture.FacilityPositionLeft);
            var bar = new HorizontalStackPanel
            {
                Spacing = 1,
                Height = 12,
                VerticalAlignment = VerticalAlignment.Center
            };
            if (total <= 0)
            {
                return bar;
            }
            int cellWidth = MathHelper.Clamp(SpaceBarMaxWidth / total - 1, 2, 10);
            for (int i = 0; i < total; i++)
            {
                bar.Widgets.Add(new Panel
                {
                    Width = cellWidth,
                    Height = 12,
                    Background = new SolidBrush(i < used ? MyraSkin.TextHighlight : SpaceEmptyColor)
                });
            }
            return bar;
        }

        // 按筛选重新排设施卡片。选中的设施被筛掉时改选第一个
        private void RefreshCards()
        {
            foreach (Button button in this.filterButtons)
            {
                bool isSelected = (FacilityType?)button.Tag == this.filter;
                Color color = isSelected ? FilterSelectedColor : FilterColor;
                button.Background = new SolidBrush(color);
                button.FocusedBackground = new SolidBrush(color);
                button.OverBackground = new SolidBrush(FilterOverColor);
                button.PressedBackground = new SolidBrush(FilterOverColor);
            }

            this.cardsPanel.Widgets.Clear();
            List<FacilityItem> shown = this.items.Where(x => this.filter == null || x.Type == this.filter).ToList();
            HorizontalStackPanel row = null;
            foreach (FacilityItem item in shown)
            {
                if (row == null || row.Widgets.Count >= CardsPerRow)
                {
                    row = new HorizontalStackPanel
                    {
                        Spacing = CardSpacing
                    };
                    this.cardsPanel.Widgets.Add(row);
                }
                item.Card = this.CreateCard(item);
                row.Widgets.Add(item.Card);
            }
            if (shown.Count == 0)
            {
                this.cardsPanel.Widgets.Add(CreateLabel(this.items.Count == 0 ? "尚无设施" : "没有此类设施", 16, MyraSkin.TextSecondary, HorizontalAlignment.Left));
            }
            this.cardsViewer.ScrollPosition = Point.Zero;
            this.Select(shown.Contains(this.selected) ? this.selected : shown.FirstOrDefault());
        }

        private Button CreateCard(FacilityItem item)
        {
            var content = new VerticalStackPanel
            {
                Spacing = 3,
                Padding = new Thickness(4)
            };
            content.Widgets.Add(CreateFacilityPicture(item, CardPictureWidth, CardPictureHeight));
            content.Widgets.Add(new Label
            {
                Text = item.IsBuilding ? "建造中" : item.DisplayName,
                Font = TextManager.GetFont(14),
                TextColor = item.IsBuilding ? MyraSkin.TextWarning : MyraSkin.TextPrimary,
                HorizontalAlignment = HorizontalAlignment.Center
            });
            // 底下一条细条：建造中的是工程进度，建好的是耐久
            Color barColor = item.IsBuilding ? MyraSkin.TextWarning : GetBarColor(item.BarValue, item.BarMax);
            content.Widgets.Add(CreateBar(CardPictureWidth, 4, item.BarValue, item.BarMax, barColor));

            var card = new Button
            {
                Content = content,
                Width = CardWidth,
                Height = CardHeight,
                Background = new SolidBrush(MyraSkin.ListRowColor),
                OverBackground = new SolidBrush(MyraSkin.ListRowOverColor),
                PressedBackground = new SolidBrush(MyraSkin.ListRowOverColor),
                FocusedBackground = new SolidBrush(MyraSkin.ListRowColor),
                BorderThickness = new Thickness(2)
            };
            card.Click += (sender, args) =>
            {
                // 拖动列表之后松手不算点击
                if (this.dragDistance <= DragThreshold)
                {
                    this.Select(item);
                }
            };
            return card;
        }

        private void Select(FacilityItem item)
        {
            this.selected = item;
            foreach (FacilityItem other in this.items)
            {
                if (other.Card != null)
                {
                    other.Card.Border = new SolidBrush(other == item ? MyraSkin.TextHighlight : LineColor);
                }
            }

            this.detailPanel.Widgets.Clear();
            this.detailViewer.ScrollPosition = Point.Zero;
            if (item == null)
            {
                return;
            }
            int textWidth = FacilityDetailWidth - ScrollBarWidth;
            Widget picture = CreateFacilityPicture(item, DetailPictureWidth, DetailPictureHeight);
            picture.HorizontalAlignment = HorizontalAlignment.Left;
            this.detailPanel.Widgets.Add(picture);
            this.detailPanel.Widgets.Add(CreateLabel(item.DisplayName, 18, MyraSkin.TextTitle, HorizontalAlignment.Left));

            FacilityKindLevel level = item.Level;
            if (item.IsBuilding)
            {
                int daysLeft = this.architecture.BuildingDaysLeft * Session.Parameters.DayInTurn;
                this.detailPanel.Widgets.Add(CreateWrappedLabel("建造中，剩余" + daysLeft + "天（工期" + level.DaysText + "天）", 15, MyraSkin.TextWarning, textWidth));
            }
            this.detailPanel.Widgets.Add(CreateDetailRow("类别", GetTypeName(item.Type)));
            this.detailPanel.Widgets.Add(CreateDetailRow("占用空间", level.PositionOccupied.ToString()));
            this.detailPanel.Widgets.Add(CreateDetailRow("维持费", level.MaintenanceCost * 30 + "/月"));
            if (!item.IsBuilding)
            {
                this.detailPanel.Widgets.Add(CreateDetailRow("耐久", item.Facility.Endurance + "/" + item.Facility.EnduranceCeiling));
            }
            // IsDemolishable名字起反了，数据里为true表示不可拆除，与Architecture.CanRemoveFacility一致
            this.detailPanel.Widgets.Add(CreateDetailRow("可拆除", StaticMethods.ToMark(!level.IsDemolishable)));

            this.detailPanel.Widgets.Add(CreateLabel("效果", 15, MyraSkin.TextSecondary, HorizontalAlignment.Left));
            if (level.ConcubineCapacity > 0)
            {
                this.detailPanel.Widgets.Add(CreateWrappedLabel("• 可以容纳" + level.ConcubineCapacity + "名美女", 15, MyraSkin.TextPrimary, textWidth));
            }
            foreach (var influence in level.Influences)
            {
                this.detailPanel.Widgets.Add(CreateWrappedLabel("• " + influence.Description, 15, MyraSkin.TextPrimary, textWidth));
            }
        }

        private static string GetTypeName(FacilityType type)
        {
            switch (type)
            {
                case FacilityType.Special:
                    return "专用";
                case FacilityType.Military:
                    return "军事";
                default:
                    return "通用";
            }
        }

        // 设施图按种类ID取，没有图时用一块写着名字的底色占位
        private static Widget CreateFacilityPicture(FacilityItem item, int width, int height)
        {
            string picture = FacilityPicturePath + item.Level.KindId + ".png";
            if (Platform.Current.FileExists(picture))
            {
                return new Image
                {
                    Renderable = new TextureImage(picture, width, height),
                    Width = width,
                    Height = height,
                    HorizontalAlignment = HorizontalAlignment.Center
                };
            }
            return CreatePlaceholder(width, height, item.Level.Name, 14);
        }

        private static Widget CreatePlaceholder(int width, int height, string text, int fontSize)
        {
            var placeholder = new Panel
            {
                Width = width,
                Height = height,
                HorizontalAlignment = HorizontalAlignment.Center,
                Background = new SolidBrush(new Color(50, 66, 76))
            };
            Label label = CreateLabel(text, fontSize, MyraSkin.TextSecondary);
            label.VerticalAlignment = VerticalAlignment.Center;
            placeholder.Widgets.Add(label);
            return placeholder;
        }

        private static VerticalStackPanel CreatePanel(int width, string title)
        {
            var panel = new VerticalStackPanel
            {
                Width = width,
                Spacing = 6,
                Padding = new Thickness(PanelPadding, PanelTitleTop, PanelPadding, PanelPadding),
                Background = MyraSkin.PanelBackground
            };
            panel.Widgets.Add(new Panel
            {
                Height = PanelTitleHeight,
                Widgets =
                {
                    CreateLabel(title, 16, MyraSkin.TextTitle, HorizontalAlignment.Left)
                }
            });
            return panel;
        }

        private static Widget CreateValueRow(string title, string value, int labelWidth)
        {
            var row = new HorizontalStackPanel
            {
                Height = RowHeight
            };
            Label label = CreateLabel(title, 16, MyraSkin.TextSecondary, HorizontalAlignment.Left);
            label.Width = labelWidth;
            row.Widgets.Add(label);
            row.Widgets.Add(CreateLabel(value, 16, MyraSkin.TextHighlight, HorizontalAlignment.Left));
            return row;
        }

        private static Widget CreateDetailRow(string title, string value)
        {
            var row = new HorizontalStackPanel
            {
                Spacing = 8
            };
            row.Widgets.Add(CreateLabel(title, 15, MyraSkin.TextSecondary, HorizontalAlignment.Left));
            row.Widgets.Add(CreateLabel(value, 15, MyraSkin.TextPrimary, HorizontalAlignment.Left));
            return row;
        }

        // 一对“名称 数值”挤在一起，用在空间紧的地方
        private static Widget CreatePair(string title, string value)
        {
            var pair = new HorizontalStackPanel
            {
                Spacing = 4
            };
            pair.Widgets.Add(CreateLabel(title, 16, MyraSkin.TextSecondary, HorizontalAlignment.Left));
            pair.Widgets.Add(CreateLabel(value, 16, MyraSkin.TextPrimary, HorizontalAlignment.Left));
            return pair;
        }

        private static Widget CreateBarRow(string title, int value, int max, bool warnLow)
        {
            var row = new HorizontalStackPanel
            {
                Height = RowHeight
            };
            Label label = CreateLabel(title, 16, MyraSkin.TextSecondary, HorizontalAlignment.Left);
            label.Width = BarLabelWidth;
            row.Widgets.Add(label);
            int barWidth = LeftColumnWidth - 2 * PanelPadding - BarLabelWidth - BarValueWidth - BarValueGap;
            Widget bar = CreateBar(barWidth, BarHeight, value, max, GetBarColor(value, max, warnLow));
            bar.VerticalAlignment = VerticalAlignment.Center;
            row.Widgets.Add(bar);
            Label text = CreateLabel(value + "/" + max, 15, MyraSkin.TextPrimary, HorizontalAlignment.Left);
            text.Width = BarValueWidth;
            text.Margin = new Thickness(BarValueGap, 0, 0, 0);
            row.Widgets.Add(text);
            return row;
        }

        private static Widget CreateBar(int width, int height, int value, int max, Color color)
        {
            var bar = new Panel
            {
                Width = width,
                Height = height,
                HorizontalAlignment = HorizontalAlignment.Center,
                Background = new SolidBrush(BarBackColor)
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

        // 进度条颜色：七成以上青绿，不足七成蓝色。
        // warnLow为true时不足三成用赭红提醒，用在统治、耐久这类低了有危险的数值；
        // 农业、商业、技术、民心低只是还没开发，不用红色，与旧界面的分组一致
        private static Color GetBarColor(int value, int max, bool warnLow = true)
        {
            double rate = max <= 0 ? 0 : (double)value / max;
            if (warnLow && rate < 0.3)
            {
                return MyraSkin.TextHostile;
            }
            if (rate < 0.7)
            {
                return BarBlueColor;
            }
            return MyraSkin.TextFriendly;
        }

        private static Label CreateLabel(string text, int fontSize, Color color, HorizontalAlignment alignment = HorizontalAlignment.Center)
        {
            return new Label
            {
                Text = text,
                Font = TextManager.GetFont(fontSize),
                TextColor = color,
                HorizontalAlignment = alignment
            };
        }

        private static Label CreateWrappedLabel(string text, int fontSize, Color color, int width)
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

        // 按住列表上下拖动来滑动，触屏上没有滚轮，主要靠这个
        private void Desktop_TouchDown(object sender, MyraEventArgs args)
        {
            this.dragDistance = 0;
            this.dragLastY = null;
            this.dragViewer = null;
            Point? position = MyraUI.Desktop.TouchPosition;
            if (!position.HasValue)
            {
                return;
            }
            foreach (ScrollViewer viewer in this.dragViewers)
            {
                if (viewer.ContainsGlobalPoint(position.Value))
                {
                    this.dragViewer = viewer;
                    this.dragLastY = position.Value.Y;
                    break;
                }
            }
        }

        private void Desktop_TouchMoved(object sender, MyraEventArgs args)
        {
            Point? position = MyraUI.Desktop.TouchPosition;
            if (!this.dragLastY.HasValue || !position.HasValue || this.dragViewer == null)
            {
                return;
            }
            int delta = position.Value.Y - this.dragLastY.Value;
            this.dragLastY = position.Value.Y;
            this.dragDistance += Math.Abs(delta);
            if (this.dragDistance > DragThreshold)
            {
                Point scroll = this.dragViewer.ScrollPosition;
                scroll.Y = MathHelper.Clamp(scroll.Y - delta, 0, this.dragViewer.ScrollMaximum.Y);
                this.dragViewer.ScrollPosition = scroll;
            }
        }

        private void Desktop_TouchUp(object sender, MyraEventArgs args)
        {
            this.dragLastY = null;
            this.dragViewer = null;
        }

        // 一张设施卡片对应的数据：已建好的设施，或者正在建造的设施（Facility为null）
        private class FacilityItem
        {
            public readonly Facility Facility;
            public readonly FacilityKindLevel Level;
            public readonly FacilityType Type;
            public readonly string DisplayName;
            public Button Card;

            private readonly int buildingDaysLeft;

            public FacilityItem(Facility facility, FacilityKindLevel level, int buildingDaysLeft)
            {
                this.Facility = facility;
                this.Level = level;
                this.buildingDaysLeft = buildingDaysLeft;
                var kinds = Session.Current.Scenario.GameCommonData.AllFacilityKinds;
                this.Type = kinds.TryGetValue(level.KindId, out FacilityKind kind) ? kind.Type : FacilityType.General;
                // 只有分等级的设施才标出等级
                bool hasLevels = Session.Current.Scenario.GameCommonData.AllFacilityKindLevels.Values.Count(x => x.KindId == level.KindId) > 1;
                this.DisplayName = hasLevels ? level.Name + " Lv" + level.Level : level.Name;
            }

            public bool IsBuilding => this.Facility == null;

            // 建造中的是已过的工期，建好的是耐久
            public int BarValue => this.IsBuilding ? Math.Max(0, this.Level.Days - this.buildingDaysLeft) : this.Facility.Endurance;

            public int BarMax => this.IsBuilding ? this.Level.Days : this.Facility.EnduranceCeiling;
        }

        // 按需加载的图片。材质可能被游戏的缓存清理掉，所以每次画之前检查一下，失效了就重新取
        private class TextureImage : IImage
        {
            private readonly string name;
            // 只画图片的这一块，null时画整张
            private readonly Rectangle? source;
            private Texture2D texture;

            public TextureImage(string name, int width, int height, Rectangle? source = null)
            {
                this.name = name;
                this.source = source;
                this.Size = new Point(width, height);
            }

            public Point Size { get; }

            public void Draw(RenderContext context, Rectangle dest, Color color)
            {
                if (this.texture == null || this.texture.IsDisposed)
                {
                    this.texture = CacheManager.LoadTexture(this.name);
                }
                if (this.texture != null)
                {
                    context.Draw(this.texture, dest, this.source, color, 0f, 0f, null);
                }
            }
        }
    }
}

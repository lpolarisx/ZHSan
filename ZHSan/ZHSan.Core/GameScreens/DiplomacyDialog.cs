using GameGlobal;
using GameManager;
using GameObjects;
using GameObjects.FactionDetail;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra.Events;
using Myra.Graphics2D;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WorldOfTheThreeKingdoms.GameScreens
{
    /// <summary>
    /// 外交面板：左边列出可以打交道的势力（用君主头像表示），右边与选中的势力谈判——
    /// 选使者、拟定我方给出和索要的条款、提议后对方接受、还价或拒绝；下面还有解除同盟、声讨、劝降几个单方面行动。
    /// 也用来对AI的来使还价，这时只能和来使的势力谈，谈成当场交割。
    /// 用Myra绘制，底板和按钮取自MyraSkin的基础图，势力卡片还是纯色。
    /// </summary>
    internal class DiplomacyDialog
    {
        private const int RelationPanelWidth = 540;
        // 分区也用底板图做边框，内容要避开四周的花纹
        private const int PanelPadding = 14;
        // 分区标题占的高度。加上上边距和间距，正好让下面的内容从底板那条横线之下开始
        private const int PanelTitleTop = 12;
        private const int PanelTitleHeight = 28;
        private const int PanelSpacing = 12;
        private const int CardWidth = 96;
        private const int CardHeight = 136;
        private const int CardSpacing = 6;
        private const int CardsPerRow = 3;
        // 小头像按原大小64x80显示，与关系栏的君主头像一样大
        private const int CardPortraitWidth = HeaderPortraitWidth;
        private const int CardPortraitHeight = HeaderPortraitHeight;
        // 势力列表与右边的谈判区一样高，放不下就滚动
        private const int ListWidth = CardsPerRow * CardWidth + (CardsPerRow - 1) * CardSpacing + 20;
        private const int ListHeight = 356;
        private const int ColumnSpacing = 12;
        private const int ColumnWidth = (RelationPanelWidth - 2 * PanelPadding - ColumnSpacing) / 2;
        private const int RowHeight = MyraSkin.ButtonHeight;
        private const int RowLabelWidth = 36;
        private const int StepButtonWidth = 28;
        private const int PickButtonWidth = 60;
        private const int ActionButtonWidth = 90;
        private const int ContentWidth = ListWidth + 2 * PanelPadding + PanelSpacing + RelationPanelWidth;
        // 内容离底板边缘的距离，要避开底板四周的花纹
        private const int DialogPadding = 28;
        private const int DialogWidth = ContentWidth + 2 * DialogPadding;
        private const int BottomButtonWidth = 90;
        // 底部说明区：说明文字最多占的行数，和下面一行不满足条件的原因
        private const int HintLines = 4;
        private const int ReasonRowHeight = 22;
        // 按住拖动超过这个距离才算滑动列表，否则算点击卡片
        private const int DragThreshold = 6;

        // 关系栏两边的名旗和君主头像
        private const string NameBannerTexture = "Content/Textures/GameComponents/Diplomacy/NameBanner.png";
        private const int BannerWidth = 30;
        private const int BannerHeight = 80;
        private const int HeaderPortraitWidth = 64;
        private const int HeaderPortraitHeight = 80;

        // 资金和粮草滑块的步长：拖动时取整到它的倍数，滚轮一格走一步。粮草的量级大约是资金的一百倍
        private const int FundStep = 5000;
        private const int FoodStep = 100000;
        // 数量滑块右边显示数值的宽度，放得下“20,000,000”
        private const int AmountValueWidth = 90;
        private const int SliderKnobWidth = 12;
        private const int SliderKnobHeight = 20;

        private static readonly Color CardColor = MyraSkin.ListRowColor;
        private static readonly Color CardOverColor = MyraSkin.ListRowOverColor;
        private static readonly Color LineColor = new Color(90, 110, 120);
        // 增减、切换用的小按钮太窄，用不了底图（四角各14像素会挤变形），改用纯色，颜色取自底图的蓝和绿
        private static readonly Color StepButtonColor = MyraSkin.ListRowSelectedColor;
        private static readonly Color StepButtonOverColor = new Color(21, 117, 2);

        private readonly Architecture architecture;
        private readonly Faction faction;
        // 对来使还价时不为null：谈成当场交割，不派使者，也不能做单方面行动
        private readonly DiplomacyDeal proposal;
        private readonly Action<DiplomacyKind, DiplomaticRelation, DiplomacyDeal> onConfirm;
        private readonly Action<DiplomacyDeal> onConclude;
        private readonly Action onCancel;
        private readonly List<DiplomaticRelation> relations;
        private readonly List<Person> envoys;
        private readonly List<Button> cards = new List<Button>();
        private readonly List<ActionOption> options = new List<ActionOption>();
        // 每次界面刷新时把条款的值写回各个控件
        private readonly List<Action> refreshers = new List<Action>();

        private FactionListViewer scrollViewer;
        private Widget cardsPanel;
        private Widget targetSide;
        private Image targetBannerImage;
        private Label targetBannerLabel;
        private Image targetPortraitImage;
        private Label relationValueLabel;
        private Label statusLabel;
        private Label truceLabel;
        private Label envoyLabel;
        // 外交面板的根控件，选择窗口盖在它上面
        private Panel mask;
        // 选使者、选城池的窗口，没打开过为null
        private MyraPicker picker;
        private Label sourceStockLabel;
        private Label targetStockLabel;
        private Label attitudeLabel;
        private Widget negotiationPanel;
        private Label hintLabel;
        private Label reasonLabel;
        private Image warningIcon;
        private Button okButton;
        private Label okLabel;
        private Button agreeButton;
        private DiplomaticRelation selectedRelation;
        private ActionOption selectedOption;
        // 选中的使者，换势力时保留
        private List<Person> selectedEnvoys = new List<Person>();
        private DiplomacyDeal deal;
        private DiplomacyResponse response;
        // 对方已经答应了现在的条款，确认按钮变成缔约
        private bool accepted;

        private int? dragLastY;
        private int dragDistance;

        private DiplomacyDialog(Architecture architecture, DiplomaticRelation selected, Action<DiplomacyKind, DiplomaticRelation, DiplomacyDeal> onConfirm)
        {
            this.architecture = architecture;
            this.faction = architecture.BelongedFaction;
            this.onConfirm = onConfirm;
            // 关系好的排在前面
            this.relations = architecture.GetEnhanceDiplomaticRelationList().OrderByDescending(x => x.Relation).ToList();
            // 使者从城中可以派遣的武将里选，政治高的在前
            var diplomaticPersons = architecture.GetDiplomaticPersons();
            this.envoys = architecture.GetPersonsExcludeNvGuan().Where(x => diplomaticPersons.Contains(x)).OrderByDescending(x => x.Politics).ToList();
            this.selectedEnvoys = this.envoys.Take(1).ToList();
            // 对方势力可能已经灭亡，不在列表里了
            if (selected != null && this.relations.Contains(selected))
            {
                this.SelectRelation(selected);
            }

            this.options.Add(new ActionOption(DiplomacyKind.Reset, "解除同盟", "立即生效", "背弃与对方的盟约：对方关系降为 " + Highlight(DiplomacyEvaluator.BetrayalRelation) + "，君主声誉受损，其他势力对本势力的关系下降 " + Highlight(DiplomacyEvaluator.BetrayalPenalty) + "，" + DiplomacyEvaluator.AllianceCooldownMonths + "个月内不能再结盟。停战期间仍不可开战。本势力有部队在外时不可解除。"));
            this.options.Add(new ActionOption(DiplomacyKind.Denounce, "声讨", "立即生效", "花费 " + Highlight(120000) + " 资金声讨目标势力，双方关系大跌，其他势力与其关系也会下降。"));
            this.options.Add(new ActionOption(DiplomacyKind.QuanXiang, "劝降", "派遣武将", "派一名武将劝说接壤的非同盟势力归降，花费 " + Highlight(50000) + " 资金。"));
        }

        private DiplomacyDialog(DiplomacyDeal proposal, Action<DiplomacyDeal> onConclude, Action onCancel)
        {
            this.proposal = proposal;
            this.faction = proposal.Target;
            this.architecture = proposal.Target.Capital;
            this.onConclude = onConclude;
            this.onCancel = onCancel;
            this.envoys = new List<Person>();
            Session.Current.Scenario.TryGetDiplomaticRelation(proposal.Source.ID, proposal.Target.ID, out var relation);
            this.relations = new List<DiplomaticRelation> { relation };
            this.selectedRelation = relation;
            // 从来使的条款开始改：对方给的变成我方索要的
            this.deal = proposal.Reverse(this.architecture);
        }

        /// <summary>
        /// 显示外交面板
        /// </summary>
        /// <param name="architecture">发起外交的城池，必须有所属势力</param>
        /// <param name="selected">打开时预先选中的势力，没有就传null</param>
        /// <param name="onConfirm">点确认、面板关闭之后调用，参数为外交方式、与目标势力的关系和谈成的条款（谈判以外为null）</param>
        public static void Show(Architecture architecture, DiplomaticRelation selected, Action<DiplomacyKind, DiplomaticRelation, DiplomacyDeal> onConfirm)
        {
            new DiplomacyDialog(architecture, selected, onConfirm).Open();
        }

        /// <summary>
        /// 对AI的来使还价
        /// </summary>
        /// <param name="proposal">来使提出的条款，提议方是AI</param>
        /// <param name="onConclude">谈成、面板关闭之后调用，参数是谈成的条款，提议方是玩家</param>
        /// <param name="onCancel">没谈成就关闭面板时调用</param>
        public static void ShowCounter(DiplomacyDeal proposal, Action<DiplomacyDeal> onConclude, Action onCancel)
        {
            new DiplomacyDialog(proposal, onConclude, onCancel).Open();
        }

        private bool IsCounter => this.proposal != null;

        private void Open()
        {
            // 面板显示期间占住一个未完成工作，地图和工具栏就不会响应鼠标键盘
            Session.MainGame.mainGameScreen.PushUndoneWork(new UndoneWorkItem(UndoneWorkKind.SubDialog, DialogKind.Diplomacy));
            Desktop desktop = MyraUI.Desktop;
            desktop.TouchDown += this.Desktop_TouchDown;
            desktop.TouchMoved += this.Desktop_TouchMoved;
            desktop.TouchUp += this.Desktop_TouchUp;
            // 右键先关选择窗口，没有选择窗口才关外交面板
            MyraUI.Show(this.Build(), () =>
            {
                if (this.picker != null && this.picker.IsOpen)
                {
                    this.picker.Close();
                }
                else
                {
                    this.Close(this.onCancel);
                }
            });
        }

        private void Close(Action afterClose)
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
                afterClose?.Invoke();
            });
        }

        private Widget Build()
        {
            var dialog = new VerticalStackPanel
            {
                Width = DialogWidth,
                Spacing = 12,
                // 上边留得少，让标题落在背景图那条横线的上面
                Padding = new Thickness(DialogPadding, 7, DialogPadding, DialogPadding),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Background = MyraSkin.PanelBackground
            };

            dialog.Widgets.Add(new Label
            {
                Text = this.IsCounter ? "回复来使" : "外交",
                Font = TextManager.GetFont(24),
                TextColor = MyraSkin.TextTitle,
                HorizontalAlignment = HorizontalAlignment.Center
            });

            var body = new HorizontalStackPanel
            {
                Spacing = PanelSpacing
            };
            body.Widgets.Add(this.BuildFactionList());
            body.Widgets.Add(this.BuildRelationPanel());
            dialog.Widgets.Add(body);
            dialog.Widgets.Add(this.BuildBottomRow());

            this.Refresh();

            // 半透明遮罩铺满窗口，压暗后面的游戏界面
            this.mask = new Panel
            {
                Background = new SolidBrush(new Color(0, 0, 0, 150))
            };
            this.mask.Widgets.Add(dialog);
            return this.mask;
        }

        private Widget BuildFactionList()
        {
            var rows = new VerticalStackPanel
            {
                Spacing = CardSpacing
            };
            HorizontalStackPanel row = null;
            foreach (DiplomaticRelation relation in this.relations)
            {
                if (row == null || row.Widgets.Count >= CardsPerRow)
                {
                    row = new HorizontalStackPanel
                    {
                        Spacing = CardSpacing
                    };
                    rows.Widgets.Add(row);
                }
                Button card = this.CreateCard(relation);
                this.cards.Add(card);
                row.Widgets.Add(card);
            }
            this.cardsPanel = rows;

            this.scrollViewer = new FactionListViewer
            {
                Content = rows,
                ShowHorizontalScrollBar = false,
                Width = ListWidth,
                Height = ListHeight
            };
            // 打开时已经选中了势力（办完一项外交回到面板），列表滚到它那一行，让它显示在可见区域的下一行
            int selectedIndex = this.relations.IndexOf(this.selectedRelation);
            if (selectedIndex >= 0)
            {
                int selectedRow = selectedIndex / CardsPerRow;
                this.scrollViewer.PendingScrollY = Math.Max(0, selectedRow - 1) * (CardHeight + CardSpacing);
            }

            string title = this.IsCounter ? "来使势力" : "选择外交势力（" + this.relations.Count + "）";
            VerticalStackPanel panel = CreatePanel(ListWidth + 2 * PanelPadding, title, out _, out _);
            panel.Widgets.Add(this.scrollViewer);
            return panel;
        }

        private Widget BuildRelationPanel()
        {
            VerticalStackPanel panel = CreatePanel(RelationPanelWidth, "外交关系", out _, out _);
            panel.Spacing = 8;

            // 发起方在左：名旗、君主头像；选中的势力在右：君主头像、名旗。中间是关系状态、关系值和停战
            Panel header = CreateRow(BannerHeight);
            var ourSide = new HorizontalStackPanel
            {
                Spacing = 6,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            ourSide.Widgets.Add(CreateBanner(this.faction, out _, out _));
            ourSide.Widgets.Add(CreateHeaderPortrait(this.faction.Leader));
            header.Widgets.Add(ourSide);

            var targetSide = new HorizontalStackPanel
            {
                Spacing = 6,
                HorizontalAlignment = HorizontalAlignment.Right
            };
            this.targetPortraitImage = CreateHeaderPortrait(null);
            targetSide.Widgets.Add(this.targetPortraitImage);
            targetSide.Widgets.Add(CreateBanner(null, out this.targetBannerImage, out this.targetBannerLabel));
            this.targetSide = targetSide;
            header.Widgets.Add(targetSide);

            var center = new VerticalStackPanel
            {
                Spacing = 2,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            this.statusLabel = CreateLabel("", 24, MyraSkin.TextPrimary);
            this.relationValueLabel = CreateLabel("", 16, MyraSkin.TextPrimary);
            this.truceLabel = CreateLabel("", 14, MyraSkin.TextSecondary);
            center.Widgets.Add(this.statusLabel);
            center.Widgets.Add(CreateLabel("←——→", 16, MyraSkin.TextHighlight));
            center.Widgets.Add(this.relationValueLabel);
            center.Widgets.Add(this.truceLabel);
            header.Widgets.Add(center);
            panel.Widgets.Add(header);

            panel.Widgets.Add(CreateLine());

            var negotiation = new VerticalStackPanel
            {
                Spacing = 8
            };
            negotiation.Widgets.Add(this.BuildEnvoyRow());
            negotiation.Widgets.Add(this.BuildTermColumns());
            negotiation.Widgets.Add(this.BuildMutualRow());

            var attitude = new HorizontalStackPanel
            {
                Spacing = 8,
                Height = 22
            };
            attitude.Widgets.Add(CreateLabel("对方态度", 16, MyraSkin.TextSecondary));
            this.attitudeLabel = CreateLabel("", 16, MyraSkin.TextPrimary);
            attitude.Widgets.Add(this.attitudeLabel);
            negotiation.Widgets.Add(attitude);
            this.negotiationPanel = negotiation;
            panel.Widgets.Add(negotiation);

            if (!this.IsCounter)
            {
                panel.Widgets.Add(CreateLine());
                var actions = new HorizontalStackPanel
                {
                    Spacing = 8
                };
                Label actionTitle = CreateLabel("其他", 16, MyraSkin.TextTitle);
                actionTitle.Width = RowLabelWidth;
                actionTitle.VerticalAlignment = VerticalAlignment.Center;
                actions.Widgets.Add(actionTitle);
                foreach (ActionOption option in this.options)
                {
                    ActionOption current = option;
                    current.Button = MyraSkin.CreateButton(current.Name, ActionButtonWidth, out current.Label);
                    current.Button.Click += (sender, args) => this.SelectOption(current);
                    // 鼠标移上去就在说明栏预览这种行动，移开后恢复
                    current.Button.MouseEntered += (sender, args) => this.ShowOptionHint(current);
                    current.Button.MouseLeft += (sender, args) => this.ShowDefaultHint();
                    actions.Widgets.Add(current.Button);
                }
                panel.Widgets.Add(actions);
            }
            return panel;
        }

        // 使者一行：来回切换城中可派的武将。还价时对方使者就在这里，不用派人
        private Widget BuildEnvoyRow()
        {
            var row = new HorizontalStackPanel
            {
                Spacing = 4,
                Height = RowHeight
            };
            row.Widgets.Add(CreateRowLabel("使者"));
            if (this.IsCounter)
            {
                Label text = CreateLabel("对方使者在此，谈成即刻交割", 16, MyraSkin.TextSecondary);
                text.VerticalAlignment = VerticalAlignment.Center;
                row.Widgets.Add(text);
                return row;
            }
            this.envoyLabel = CreateLabel("", 16, MyraSkin.TextPrimary, HorizontalAlignment.Left);
            this.envoyLabel.Width = RelationPanelWidth - 2 * PanelPadding - RowLabelWidth - PickButtonWidth - 2 * 4;
            this.envoyLabel.VerticalAlignment = VerticalAlignment.Center;
            row.Widgets.Add(this.envoyLabel);
            row.Widgets.Add(this.CreatePickButton(this.PickEnvoys));
            return row;
        }

        // 左栏我方给出，右栏我方索要，各有资金、粮草、城池三行
        private Widget BuildTermColumns()
        {
            var columns = new HorizontalStackPanel
            {
                Spacing = ColumnSpacing
            };

            var give = CreateColumn("我方给出", out this.sourceStockLabel);
            give.Widgets.Add(this.CreateAmountRow("资金", () => this.deal.GiveFund, x => this.deal.GiveFund = x, () => this.deal.SourceArchitecture.Fund, FundStep));
            give.Widgets.Add(this.CreateAmountRow("粮草", () => this.deal.GiveFood, x => this.deal.GiveFood = x, () => this.deal.SourceArchitecture.Food, FoodStep));
            give.Widgets.Add(this.CreateCityRow(() => this.deal.GiveArchitectures, () => this.PickArchitectures(true)));
            columns.Widgets.Add(give);

            var request = CreateColumn("我方索要", out this.targetStockLabel);
            request.Widgets.Add(this.CreateAmountRow("资金", () => this.deal.RequestFund, x => this.deal.RequestFund = x, () => this.deal.Target.Capital?.Fund ?? 0, FundStep));
            request.Widgets.Add(this.CreateAmountRow("粮草", () => this.deal.RequestFood, x => this.deal.RequestFood = x, () => this.deal.Target.Capital?.Food ?? 0, FoodStep));
            request.Widgets.Add(this.CreateCityRow(() => this.deal.RequestArchitectures, () => this.PickArchitectures(false)));
            columns.Widgets.Add(request);
            return columns;
        }

        private static VerticalStackPanel CreateColumn(string title, out Label stockLabel)
        {
            var column = new VerticalStackPanel
            {
                Width = ColumnWidth,
                Spacing = 4
            };
            column.Widgets.Add(CreateLabel(title, 16, MyraSkin.TextTitle, HorizontalAlignment.Left));
            // 标题下面一行小字显示这一方现有的钱粮，增减时心里有数
            stockLabel = CreateLabel("", 14, MyraSkin.TextSecondary, HorizontalAlignment.Left);
            column.Widgets.Add(stockLabel);
            return column;
        }

        // 双方共同的条款：停战时长、结盟
        private Widget BuildMutualRow()
        {
            var row = new HorizontalStackPanel
            {
                Spacing = 4,
                Height = RowHeight
            };
            row.Widgets.Add(CreateRowLabel("停战"));
            Label truce = CreateValueLabel(80);
            row.Widgets.Add(this.CreateStepButton("<", () => this.ChangeTruce(-1)));
            row.Widgets.Add(truce);
            row.Widgets.Add(this.CreateStepButton(">", () => this.ChangeTruce(1)));
            this.refreshers.Add(() => truce.Text = this.deal == null ? "" : DiplomacyDeal.TruceText(this.deal.TruceDays));

            row.Widgets.Add(new Panel { Width = 40 });
            // 结盟是一个开关：按下就把结盟写进条款。已经是同盟时没什么可谈的，显示已结盟，解除同盟在下面的单方面行动里
            Button alliance = MyraSkin.CreateButton("", 80, out Label allianceLabel);
            alliance.Click += (sender, args) =>
            {
                if (this.selectedRelation != null && !this.selectedRelation.IsAlliance)
                {
                    this.ChangeDeal(() => this.deal.Alliance = !this.deal.Alliance);
                }
            };
            row.Widgets.Add(alliance);
            this.refreshers.Add(() =>
            {
                bool allied = this.selectedRelation != null && this.selectedRelation.IsAlliance;
                bool on = this.deal != null && this.deal.Alliance;
                allianceLabel.Text = allied ? "已结盟" : "结盟";
                MyraSkin.SetButtonLook(alliance, allied ? MyraButtonLook.Disabled : on ? MyraButtonLook.Selected : MyraButtonLook.Normal);
            });
            return row;
        }

        // 一行数量：滑块拖到想要的数，右边显示数值。范围是这一方现有的数量，取整到步长；滚轮一格走一步，方便微调
        private Widget CreateAmountRow(string title, Func<int> get, Action<int> set, Func<int> max, int step)
        {
            var row = new HorizontalStackPanel
            {
                Spacing = 4,
                Height = RowHeight
            };
            row.Widgets.Add(CreateRowLabel(title));
            var slider = new HorizontalSlider
            {
                Width = ColumnWidth - RowLabelWidth - AmountValueWidth - 2 * 4,
                Height = SliderKnobHeight,
                VerticalAlignment = VerticalAlignment.Center,
                Minimum = 0,
                WheelStep = step,
                Background = new TrackBrush(LineColor, MyraSkin.TextHighlight, () => this.deal == null ? 0f : (float)get() / Math.Max(1, max()))
            };
            // 手柄不用Myra默认皮肤的图，换成和小按钮一样的纯色块
            slider.ImageButton.Content = null;
            slider.ImageButton.Width = SliderKnobWidth;
            slider.ImageButton.Height = SliderKnobHeight;
            slider.ImageButton.Background = new SolidBrush(StepButtonColor);
            slider.ImageButton.OverBackground = new SolidBrush(StepButtonOverColor);
            slider.ImageButton.PressedBackground = new SolidBrush(StepButtonOverColor);
            slider.ImageButton.Border = new SolidBrush(MyraSkin.TextPrimary);
            slider.ImageButton.BorderThickness = new Thickness(1);
            slider.ValueChangedByUser += (sender, args) =>
            {
                // 还没选势力时没有条款。滑块虽然设成了不可用，Myra仍会把点击交给它
                if (this.deal == null)
                {
                    return;
                }
                // 取整到步长；拖到最右边时取满，不留零头
                int limit = Math.Max(0, max());
                int amount = slider.Value >= slider.Maximum ? limit : (int)Math.Round(slider.Value / step) * step;
                amount = Math.Clamp(amount, 0, limit);
                if (amount != get())
                {
                    this.ChangeDeal(() => set(amount));
                }
            };
            row.Widgets.Add(slider);

            Label value = CreateValueLabel(AmountValueWidth);
            row.Widgets.Add(value);
            this.refreshers.Add(() =>
            {
                int amount = this.deal == null ? 0 : get();
                int limit = this.deal == null ? 0 : Math.Max(0, max());
                // 一方什么都没有时滑块不能拖，范围至少留1免得最大最小相等
                slider.Maximum = Math.Max(1, limit);
                slider.Value = Math.Min(amount, slider.Maximum);
                slider.Enabled = limit > 0;
                value.Text = amount.ToString("N0");
                value.TextColor = amount > 0 ? MyraSkin.TextHighlight : MyraSkin.TextSecondary;
            });
            return row;
        }

        /// <summary>
        /// 名旗底图按势力颜色染色。图片缺失时画一块势力颜色的纯色
        /// </summary>
        private class BannerImage : IImage
        {
            private readonly Color color;
            private Texture2D texture;

            public BannerImage(Color color)
            {
                this.color = color;
            }

            public Point Size => new Point(BannerWidth, BannerHeight);

            public void Draw(RenderContext context, Rectangle dest, Color color)
            {
                if ((this.texture == null || this.texture.IsDisposed) && Platforms.Platform.Current.FileExists(NameBannerTexture))
                {
                    this.texture = CacheManager.LoadTexture(NameBannerTexture);
                }
                if (this.texture == null || this.texture.IsDisposed)
                {
                    context.FillRectangle(dest, this.color);
                }
                else
                {
                    context.Draw(this.texture, dest, null, this.color, 0f, 0f, null);
                }
            }
        }

        /// <summary>
        /// 滑块的轨道：竖直居中的一条细线，左边已选的部分用重点色
        /// </summary>
        private class TrackBrush : IBrush
        {
            private const int TrackHeight = 4;
            private readonly Color color;
            private readonly Color filledColor;
            private readonly Func<float> getRatio;

            public TrackBrush(Color color, Color filledColor, Func<float> getRatio)
            {
                this.color = color;
                this.filledColor = filledColor;
                this.getRatio = getRatio;
            }

            public void Draw(RenderContext context, Rectangle dest, Color color)
            {
                var track = new Rectangle(dest.X, dest.Y + (dest.Height - TrackHeight) / 2, dest.Width, TrackHeight);
                context.FillRectangle(track, this.color);
                int filled = (int)(track.Width * MathHelper.Clamp(this.getRatio(), 0f, 1f));
                if (filled > 0)
                {
                    context.FillRectangle(new Rectangle(track.X, track.Y, filled, track.Height), this.filledColor);
                }
            }
        }

        // 一行城池：显示已选的城池，点「选择」打开城池列表，可以选多座
        private Widget CreateCityRow(Func<List<Architecture>> get, Action pick)
        {
            var row = new HorizontalStackPanel
            {
                Spacing = 2,
                Height = RowHeight
            };
            row.Widgets.Add(CreateRowLabel("城池"));
            Label value = CreateValueLabel(ColumnWidth - RowLabelWidth - PickButtonWidth - 2 * 2);
            row.Widgets.Add(value);
            row.Widgets.Add(this.CreatePickButton(pick));
            this.refreshers.Add(() =>
            {
                List<Architecture> list = this.deal == null ? new List<Architecture>() : get();
                value.Text = list.Count == 0 ? "无" : list.Count <= 2 ? string.Join("、", list.Select(x => x.Name)) : list[0].Name + "等" + list.Count + "城";
                value.TextColor = list.Count == 0 ? MyraSkin.TextSecondary : MyraSkin.TextHighlight;
            });
            return row;
        }

        private Button CreatePickButton(Action pick)
        {
            Button button = MyraSkin.CreateButton("选择", PickButtonWidth, out _);
            button.Click += (sender, args) =>
            {
                if (this.selectedRelation != null)
                {
                    pick();
                }
            };
            return button;
        }

        private void PickEnvoys()
        {
            Person leader = this.deal.Target.Leader;
            var columns = new[]
            {
                new MyraPickerColumn<Person>("姓名", 110, p => p.Name),
                new MyraPickerColumn<Person>("政治", 60, p => p.Politics.ToString()),
                new MyraPickerColumn<Person>("相性差", 70, p => Person.GetIdealOffset(p, leader).ToString()),
                new MyraPickerColumn<Person>("与对方君主", 100, p => leader.Hates(p) ? "厌恶" : leader.IsCloseTo(p) ? "亲近" : "—")
            };
            this.picker = MyraPicker.Show(this.mask, "选择使者", columns, this.envoys, this.deal.Envoys,
                chosen =>
                {
                    this.selectedEnvoys = chosen.OrderByDescending(x => x.Politics).ToList();
                    this.ChangeDeal(() => this.deal.Envoys = new List<Person>(this.selectedEnvoys));
                },
                "最得力者为主使，随行的按各自能力添一半情面（最多算两人）；对方君主厌恶的人同去反而坏事。");
        }

        // 我方给出的城看与对方是否相连（对方得了好不好守），索要的城看与我方是否相连
        private void PickArchitectures(bool giving)
        {
            List<Architecture> candidates = giving ? this.deal.GetGivableArchitectures() : this.deal.GetRequestableArchitectures();
            List<Architecture> current = giving ? this.deal.GiveArchitectures : this.deal.RequestArchitectures;
            Faction linkedTo = giving ? this.deal.Target : this.deal.Source;
            string title = giving ? "选择割让的城池" : "选择索要的城池";
            string linkHeader = giving ? "连对方" : "连我方";
            var columns = new[]
            {
                new MyraPickerColumn<Architecture>("城池", 100, a => a.Name),
                new MyraPickerColumn<Architecture>("规模", 50, a => a.JianzhuGuimo.ToString()),
                new MyraPickerColumn<Architecture>("人口", 90, a => a.Population.ToString("N0")),
                new MyraPickerColumn<Architecture>(linkHeader, 60, a => DiplomacyEvaluator.IsLinkedTo(a, linkedTo) ? "是" : "否"),
                new MyraPickerColumn<Architecture>("估值", 100, a => DiplomacyEvaluator.GetArchitectureValue(a).ToString("N0"))
            };
            this.picker = MyraPicker.Show(this.mask, title, columns, candidates, current,
                chosen => this.ChangeDeal(() =>
                {
                    if (giving)
                    {
                        this.deal.GiveArchitectures = chosen;
                    }
                    else
                    {
                        this.deal.RequestArchitectures = chosen;
                    }
                }),
                giving ? "都城和发起外交的本城不能割让。估值是城池本身的价值，不相连的城对方只算一半。" : "对方都城不能索要。对方割城时按估值的一倍半算作损失。");
        }

        // 箭头用ASCII的<和>：游戏字体只有GBK字符，‹›«»这类符号显示不出来
        private Button CreateStepButton(string text, Action action)
        {
            var button = new Button
            {
                Content = new Label
                {
                    Text = text,
                    Font = TextManager.GetFont(16),
                    TextColor = MyraSkin.TextPrimary,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                },
                Width = StepButtonWidth,
                Height = RowHeight - 4,
                VerticalAlignment = VerticalAlignment.Center,
                Background = new SolidBrush(StepButtonColor),
                FocusedBackground = new SolidBrush(StepButtonColor),
                OverBackground = new SolidBrush(StepButtonOverColor),
                PressedBackground = new SolidBrush(StepButtonOverColor),
                Border = new SolidBrush(LineColor),
                BorderThickness = new Thickness(1)
            };
            button.Click += (sender, args) =>
            {
                if (this.selectedRelation != null)
                {
                    action();
                }
            };
            return button;
        }

        private static Label CreateRowLabel(string text)
        {
            Label label = CreateLabel(text, 16, MyraSkin.TextSecondary, HorizontalAlignment.Left);
            label.Width = RowLabelWidth;
            label.VerticalAlignment = VerticalAlignment.Center;
            return label;
        }

        private static Label CreateValueLabel(int width)
        {
            Label label = CreateLabel("", 16, MyraSkin.TextPrimary);
            label.Width = width;
            label.TextAlign = FontStashSharp.RichText.TextHorizontalAlignment.Center;
            label.VerticalAlignment = VerticalAlignment.Center;
            return label;
        }

        // 底部一行：左边是说明（行动说明、对方的回应、不能执行的原因），右边是按钮
        private Widget BuildBottomRow()
        {
            var row = new HorizontalStackPanel
            {
                Spacing = 12,
                Width = ContentWidth
            };

            int hintWidth = ContentWidth - 3 * BottomButtonWidth - 3 * 12;
            // 说明区固定高度，留够最长的说明（解除同盟）换行后的行数加一行原因。
            // 弹窗是居中的，高度随文字变化的话，鼠标在按钮间移动时整个弹窗会上下跳
            var hint = new VerticalStackPanel
            {
                Width = hintWidth,
                Height = HintLines * TextManager.GetFont(16).LineHeight + 2 + ReasonRowHeight,
                Spacing = 2
            };
            this.hintLabel = new Label
            {
                Font = TextManager.GetFont(16),
                // 说明里的数值靠富文本命令变色
                SupportsCommands = true,
                Wrap = true,
                Width = hintWidth
            };
            this.reasonLabel = new Label
            {
                Font = TextManager.GetFont(16),
                TextColor = MyraSkin.TextWarning,
                VerticalAlignment = VerticalAlignment.Center
            };
            var reasonRow = new HorizontalStackPanel
            {
                Spacing = 4,
                Height = ReasonRowHeight
            };
            // 有警告图标就放在原因前面，没有的话原因文字前加“※”
            this.warningIcon = MyraSkin.CreateWarningIcon();
            if (this.warningIcon != null)
            {
                this.warningIcon.VerticalAlignment = VerticalAlignment.Center;
                reasonRow.Widgets.Add(this.warningIcon);
            }
            reasonRow.Widgets.Add(this.reasonLabel);
            hint.Widgets.Add(this.hintLabel);
            hint.Widgets.Add(reasonRow);
            row.Widgets.Add(hint);

            // 对方还价时出现，点了就按对方的要求加上钱粮
            this.agreeButton = MyraSkin.CreateButton("依其所请", BottomButtonWidth, out _);
            this.agreeButton.VerticalAlignment = VerticalAlignment.Center;
            this.agreeButton.Click += this.AgreeButton_Click;
            row.Widgets.Add(this.agreeButton);

            this.okButton = MyraSkin.CreateButton("提议", BottomButtonWidth, out this.okLabel);
            this.okButton.VerticalAlignment = VerticalAlignment.Center;
            this.okButton.Click += this.OkButton_Click;
            row.Widgets.Add(this.okButton);

            Button cancelButton = MyraSkin.CreateButton(this.IsCounter ? "回绝" : "关闭", BottomButtonWidth, out _);
            cancelButton.VerticalAlignment = VerticalAlignment.Center;
            cancelButton.Click += (sender, args) => this.Close(this.onCancel);
            row.Widgets.Add(cancelButton);
            return row;
        }

        // 说明文字里的数值用重点金色标出来，靠Label的富文本命令实现
        private static string Highlight(int value)
        {
            Color color = MyraSkin.TextHighlight;
            return "/c[#" + color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2") + "]" + value.ToString("N0") + "/cd";
        }

        private Button CreateCard(DiplomaticRelation relation)
        {
            Faction target = relation.GetDiplomaticFaction(this.faction.ID);

            var content = new VerticalStackPanel
            {
                Spacing = 3,
                Padding = new Thickness(6)
            };
            content.Widgets.Add(new Image
            {
                Renderable = new PortraitImage(target.Leader, PortraitSize.Small, CardPortraitWidth, CardPortraitHeight),
                Width = CardPortraitWidth,
                Height = CardPortraitHeight,
                HorizontalAlignment = HorizontalAlignment.Center
            });
            content.Widgets.Add(CreateFactionColorBar(target, CardPortraitWidth));
            content.Widgets.Add(CreateLabel(TextManager.TruncateText(target.Name, CardWidth - 12, 16f / 30f), 16, MyraSkin.TextPrimary));
            content.Widgets.Add(CreateLabel(relation.Relation.ToString(), 14, GetStatusColor(relation)));

            var card = new Button
            {
                Content = content,
                Width = CardWidth,
                Height = CardHeight,
                Background = new SolidBrush(CardColor),
                OverBackground = new SolidBrush(CardOverColor),
                PressedBackground = new SolidBrush(CardOverColor),
                FocusedBackground = new SolidBrush(CardColor),
                BorderThickness = new Thickness(2)
            };
            card.Click += (sender, args) =>
            {
                // 拖动列表之后松手不算点击；还价时只能和来使的势力谈
                if (this.dragDistance <= DragThreshold && !this.IsCounter)
                {
                    this.SelectRelation(relation);
                    this.Refresh();
                }
            };
            return card;
        }

        // 竖挂的名旗：底图按势力颜色染色，势力名竖排写在上面。势力为null时先留空，选中势力后再填
        private static Panel CreateBanner(Faction faction, out Image image, out Label label)
        {
            var panel = new Panel
            {
                Width = BannerWidth,
                Height = BannerHeight
            };
            image = new Image
            {
                Width = BannerWidth,
                Height = BannerHeight
            };
            label = new Label
            {
                Font = TextManager.GetFont(16),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Top,
                TextAlign = FontStashSharp.RichText.TextHorizontalAlignment.Center,
                Top = 6
            };
            panel.Widgets.Add(image);
            panel.Widgets.Add(label);
            SetBanner(image, label, faction);
            return panel;
        }

        private static void SetBanner(Image image, Label label, Faction faction)
        {
            image.Renderable = faction == null ? null : new BannerImage(faction.FactionColor);
            if (faction == null)
            {
                label.Text = "";
                return;
            }
            // 旗面上半截写字，名字长的换小字，最多写四个字
            string name = faction.Name.Length > 4 ? faction.Name.Substring(0, 4) : faction.Name;
            label.Font = TextManager.GetFont(name.Length > 3 ? 13 : 16);
            label.Text = string.Join("\n", name.ToCharArray());
            label.TextColor = GetBannerTextColor(faction.FactionColor);
        }

        // 浅色的旗写深字，深色的旗写浅字
        private static Color GetBannerTextColor(Color color)
        {
            float luminance = 0.299f * color.R + 0.587f * color.G + 0.114f * color.B;
            return luminance > 150 ? new Color(30, 30, 30) : MyraSkin.TextPrimary;
        }

        private static Image CreateHeaderPortrait(Person leader)
        {
            return new Image
            {
                Width = HeaderPortraitWidth,
                Height = HeaderPortraitHeight,
                Renderable = leader == null ? null : new PortraitImage(leader, PortraitSize.Small, HeaderPortraitWidth, HeaderPortraitHeight)
            };
        }

        // 分区和整个面板用同一张底板：标题行在底板那条横线的上面，内容从横线下面开始
        private static VerticalStackPanel CreatePanel(int width, string title, out Panel titleRow, out Label titleLabel)
        {
            var panel = new VerticalStackPanel
            {
                Width = width,
                Spacing = 6,
                Padding = new Thickness(PanelPadding, PanelTitleTop, PanelPadding, PanelPadding),
                Background = MyraSkin.PanelBackground
            };
            titleRow = CreateRow(PanelTitleHeight);
            titleLabel = CreateLabel(title, 16, MyraSkin.TextTitle, HorizontalAlignment.Left);
            titleRow.Widgets.Add(titleLabel);
            panel.Widgets.Add(titleRow);
            return panel;
        }

        // 一行里放几个各自靠左、居中、靠右的控件
        private static Panel CreateRow(int height)
        {
            return new Panel
            {
                Height = height
            };
        }

        private static Panel CreateLine()
        {
            return new Panel
            {
                Height = 1,
                Background = new SolidBrush(LineColor)
            };
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

        // 头像下面压一条势力颜色，和地图上的势力颜色对应
        private static Widget CreateFactionColorBar(Faction faction, int width)
        {
            return new Panel
            {
                Width = width,
                Height = 4,
                HorizontalAlignment = HorizontalAlignment.Center,
                Background = new SolidBrush(faction.FactionColor)
            };
        }

        // 单方面行动对选中的势力能不能用，条件与原来右键菜单里各项的显示条件、可选势力列表一致
        private bool IsAvailable(DiplomacyKind kind, DiplomaticRelation relation)
        {
            switch (kind)
            {
                case DiplomacyKind.Reset:
                    return this.architecture.ResetDiplomaticRelationAvail() && this.architecture.GetResetDiplomaticRelationList().Contains(relation);
                case DiplomacyKind.Denounce:
                    return this.architecture.DenounceDiplomaticRelationAvail() && this.architecture.GetDenounceDiplomaticRelationList().Contains(relation);
                case DiplomacyKind.QuanXiang:
                    return this.architecture.QuanXiangDiplomaticRelationAvail() && this.architecture.GetQuanXiangDiplomaticRelationList().Contains(relation);
                default:
                    return false;
            }
        }

        // 单方面行动不能执行的具体原因，只用来给玩家看。能不能执行以IsAvailable为准，这里按同样的条件逐条找出没满足的那一条
        private string GetUnavailableReason(DiplomacyKind kind, DiplomaticRelation relation)
        {
            int threshold = Session.GlobalVariables.FriendlyDiplomacyThreshold;
            int movablePersons = this.architecture.GetPersonsExcludeNvGuan().Count;
            switch (kind)
            {
                case DiplomacyKind.Reset:
                    if (relation.Relation < threshold)
                    {
                        return "双方并非同盟，无需解除。";
                    }
                    return this.faction.TroopCountExcludeTransport > 0 ? "本势力有部队在外，不能解除同盟。" : null;
                case DiplomacyKind.Denounce:
                    return relation.Relation >= threshold ? "不能声讨同盟势力。" : this.GetCostReason(120000);
                case DiplomacyKind.QuanXiang:
                    if (!Session.GlobalVariables.PermitQuanXiang)
                    {
                        return "当前游戏设置不允许劝降。";
                    }
                    if (relation.Relation >= threshold)
                    {
                        return "不能劝降同盟势力。";
                    }
                    if (!this.faction.GetAdjecentFactions().Contains(relation.GetDiplomaticFaction(this.faction.ID)))
                    {
                        return "只能劝降与本势力接壤的势力。";
                    }
                    return this.GetCostReason(50000) ?? GetPersonReason(movablePersons);
                default:
                    return null;
            }
        }

        // 原来的条件是资金大于这个数，不含等于
        private string GetCostReason(int cost)
        {
            return this.architecture.Fund > cost ? null : "资金不足，需要超过 " + cost.ToString("N0") + "，无法执行该行动。";
        }

        private static string GetPersonReason(int movablePersons)
        {
            return movablePersons > 0 ? null : "城中没有可以派遣的武将。";
        }

        private static string GetStatusText(DiplomaticRelation relation)
        {
            int threshold = Session.GlobalVariables.FriendlyDiplomacyThreshold;
            if (relation.Relation >= threshold)
            {
                return "同盟";
            }
            if (relation.Relation > 0)
            {
                return "友好";
            }
            if (relation.Relation == 0)
            {
                return "中立";
            }
            return relation.Relation > -threshold ? "不睦" : "敌对";
        }

        // 关系为正用青绿，为负用赭红，为零用次要文字色。同盟与友好、不睦与敌对靠状态文字区分
        private static Color GetStatusColor(DiplomaticRelation relation)
        {
            if (relation.Relation > 0)
            {
                return MyraSkin.TextFriendly;
            }
            return relation.Relation < 0 ? MyraSkin.TextHostile : MyraSkin.TextSecondary;
        }

        private static Color GetAttitudeColor(int level)
        {
            switch (level)
            {
                case 3:
                    return MyraSkin.TextFriendly;
                case 2:
                    return MyraSkin.TextHighlight;
                case 1:
                    return MyraSkin.TextWarning;
                default:
                    return MyraSkin.TextHostile;
            }
        }

        // 换了势力就重新拟条款
        private void SelectRelation(DiplomaticRelation relation)
        {
            this.selectedRelation = relation;
            Faction target = relation.GetDiplomaticFaction(this.faction.ID);
            this.deal = new DiplomacyDeal(this.faction, target, this.architecture)
            {
                Envoys = new List<Person>(this.selectedEnvoys)
            };
            this.response = null;
            this.accepted = false;
            // 换了势力之后，原先选的行动不一定还能用
            if (this.selectedOption != null && !this.IsAvailable(this.selectedOption.Kind, relation))
            {
                this.selectedOption = null;
            }
        }

        // 条款改了，对方先前的答复就不算数了；动了条款也就不再是要做单方面行动
        private void ChangeDeal(Action change)
        {
            if (this.selectedRelation == null)
            {
                return;
            }
            change();
            this.response = null;
            this.accepted = false;
            this.selectedOption = null;
            this.Refresh();
        }

        private void ChangeTruce(int direction)
        {
            int[] truceOptions = DiplomacyDeal.TruceOptions;
            int index = Array.IndexOf(truceOptions, this.deal.TruceDays);
            index = Math.Clamp((index < 0 ? 0 : index) + direction, 0, truceOptions.Length - 1);
            this.ChangeDeal(() => this.deal.TruceDays = truceOptions[index]);
        }

        // 单方面行动只有能执行的点了才会选中，不能执行的靠鼠标悬浮查看原因
        private void SelectOption(ActionOption option)
        {
            if (!option.Available)
            {
                return;
            }
            this.selectedOption = option;
            this.response = null;
            this.accepted = false;
            this.Refresh();
        }

        // 预览一种单方面行动的说明，不能执行时一并显示原因
        private void ShowOptionHint(ActionOption option)
        {
            string reason = null;
            if (this.selectedRelation == null)
            {
                reason = "请先在左侧选择一个势力。";
            }
            else if (!option.Available)
            {
                reason = this.GetUnavailableReason(option.Kind, this.selectedRelation) ?? "目前不满足条件，无法对" + this.GetTargetName() + option.Name + "。";
            }
            this.ShowHint(option.Name + " · " + option.Method + "：" + option.Description, MyraSkin.TextPrimary, reason);
        }

        private string GetTargetName()
        {
            return this.selectedRelation.GetDiplomaticFaction(this.faction.ID).Name;
        }

        // 谈判现在为什么不能提议，可以提议时返回null。条款还是空的不算原因，只是确认按钮不可用
        private string GetNegotiationBlocker()
        {
            if (!this.IsCounter && this.envoys.Count == 0)
            {
                return "城中没有可以派遣的武将，无法遣使谈判。";
            }
            if (!this.IsCounter && this.deal.Envoys.Count == 0)
            {
                return "请选择使者。";
            }
            if (this.deal.IsEmpty)
            {
                return null;
            }
            return this.deal.GetInvalidReason(false);
        }

        private void Refresh()
        {
            for (int i = 0; i < this.cards.Count; i++)
            {
                bool isSelected = this.relations[i] == this.selectedRelation;
                this.cards[i].Border = new SolidBrush(isSelected ? MyraSkin.TextHighlight : LineColor);
            }

            bool hasTarget = this.selectedRelation != null;
            this.negotiationPanel.Enabled = hasTarget;
            if (!hasTarget)
            {
                this.targetSide.Visible = false;
                this.statusLabel.Text = "未选择";
                this.statusLabel.TextColor = MyraSkin.TextSecondary;
                this.relationValueLabel.Text = "";
                this.truceLabel.Text = "";
                this.sourceStockLabel.Text = "";
                this.targetStockLabel.Text = "";
                this.attitudeLabel.Text = "";
            }
            else
            {
                Faction target = this.selectedRelation.GetDiplomaticFaction(this.faction.ID);
                this.targetSide.Visible = true;
                SetBanner(this.targetBannerImage, this.targetBannerLabel, target);
                this.targetPortraitImage.Renderable = new PortraitImage(target.Leader, PortraitSize.Small, HeaderPortraitWidth, HeaderPortraitHeight);
                this.statusLabel.Text = GetStatusText(this.selectedRelation);
                this.statusLabel.TextColor = GetStatusColor(this.selectedRelation);
                this.relationValueLabel.Text = this.selectedRelation.Relation.ToString();
                this.relationValueLabel.TextColor = GetStatusColor(this.selectedRelation);
                this.truceLabel.Text = this.selectedRelation.Truce > 0 ? "停战中，剩余" + this.selectedRelation.Truce + "天" : "";
                this.sourceStockLabel.Text = FormatStock(this.deal.SourceArchitecture);
                this.targetStockLabel.Text = FormatStock(this.deal.Target.Capital);
                if (this.deal.IsEmpty)
                {
                    this.attitudeLabel.Text = "——";
                    this.attitudeLabel.TextColor = MyraSkin.TextSecondary;
                }
                else
                {
                    int level = DiplomacyEvaluator.GetAttitudeLevel(this.deal);
                    this.attitudeLabel.Text = DiplomacyEvaluator.GetAttitudeText(level);
                    this.attitudeLabel.TextColor = GetAttitudeColor(level);
                }
            }

            if (this.envoyLabel != null)
            {
                Person lead = this.deal?.LeadEnvoy;
                int count = this.deal?.Envoys.Count ?? 0;
                this.envoyLabel.Text = this.envoys.Count == 0 ? "无可派之人" : lead == null ? "未选" : lead.Name + "（政治 " + lead.Politics + "）" + (count > 1 ? " 等 " + count + " 人" : "");
                this.envoyLabel.TextColor = lead == null ? MyraSkin.TextSecondary : MyraSkin.TextPrimary;
            }
            foreach (Action refresh in this.refreshers)
            {
                refresh();
            }

            foreach (ActionOption option in this.options)
            {
                option.Available = hasTarget && this.IsAvailable(option.Kind, this.selectedRelation);
                // 不能用的行动显示成灰色，但仍然可以点，用来查看它的条件
                MyraSkin.SetButtonLook(option.Button, option == this.selectedOption ? MyraButtonLook.Selected : option.Available ? MyraButtonLook.Normal : MyraButtonLook.Disabled);
                option.Label.TextColor = option.Available ? MyraSkin.TextPrimary : MyraSkin.TextSecondary;
            }

            bool canConfirm;
            if (this.selectedOption != null)
            {
                this.okLabel.Text = "执行";
                canConfirm = true;
            }
            else
            {
                this.okLabel.Text = this.accepted ? "缔约" : "提议";
                canConfirm = hasTarget && !this.deal.IsEmpty && this.GetNegotiationBlocker() == null;
            }
            this.okButton.Enabled = canConfirm;
            this.okLabel.TextColor = canConfirm ? MyraSkin.TextPrimary : MyraSkin.TextSecondary;
            this.agreeButton.Visible = this.response != null && this.response.Kind == DiplomacyResponseKind.Counter;

            this.ShowDefaultHint();
        }

        // 城名和现有的钱粮
        private static string FormatStock(Architecture a)
        {
            return a == null ? "" : a.Name + "  资金 " + a.Fund.ToString("N0") + "  粮 " + a.Food.ToString("N0");
        }

        private void ShowDefaultHint()
        {
            if (this.relations.Count == 0)
            {
                this.ShowHint("目前没有可以打交道的势力。", MyraSkin.TextSecondary);
            }
            else if (this.selectedRelation == null)
            {
                this.ShowHint("请先在左侧选择一个势力。", MyraSkin.TextSecondary);
            }
            else if (this.selectedOption != null)
            {
                this.ShowHint(this.selectedOption.Name + " · " + this.selectedOption.Method + "：" + this.selectedOption.Description, MyraSkin.TextPrimary);
            }
            else if (this.response != null)
            {
                Color color = this.response.Kind == DiplomacyResponseKind.Accept ? MyraSkin.TextFriendly : this.response.Kind == DiplomacyResponseKind.Counter ? MyraSkin.TextHighlight : MyraSkin.TextHostile;
                this.ShowHint(this.deal.Target.Leader.Name + "：" + this.response.Text, color);
            }
            else
            {
                string text = this.IsCounter
                    ? "改动条款后点「提议」，看对方是否接受；点「回绝」则拒绝来使。"
                    : "拟定双方条款后点「提议」，对方会接受、还价或拒绝。谈成后使者出发，到达对方都城时交割。";
                this.ShowHint(text, MyraSkin.TextSecondary, this.GetNegotiationBlocker());
            }
        }

        private void ShowHint(string text, Color color, string reason = null)
        {
            this.hintLabel.Text = text;
            this.hintLabel.TextColor = color;
            if (this.warningIcon != null)
            {
                this.warningIcon.Visible = reason != null;
                this.reasonLabel.Text = reason ?? "";
            }
            else
            {
                // 游戏字体里没有⚠，没有图标时用※代替
                this.reasonLabel.Text = reason == null ? "" : "※ " + reason;
            }
        }

        private void OkButton_Click(object sender, MyraEventArgs args)
        {
            if (this.selectedRelation == null)
            {
                return;
            }
            DiplomaticRelation relation = this.selectedRelation;
            if (this.selectedOption != null)
            {
                DiplomacyKind kind = this.selectedOption.Kind;
                this.Close(() => this.onConfirm(kind, relation, null));
                return;
            }

            if (this.accepted)
            {
                // 缔约前再查一次条款，提议之后城里的钱粮可能有变
                if (this.GetNegotiationBlocker() != null)
                {
                    this.accepted = false;
                    this.response = null;
                    this.Refresh();
                    return;
                }
                DiplomacyDeal concluded = this.deal;
                if (this.IsCounter)
                {
                    this.Close(() => this.onConclude(concluded));
                }
                else
                {
                    this.Close(() => this.onConfirm(DiplomacyKind.Negotiate, relation, concluded));
                }
                return;
            }

            this.response = DiplomacyEvaluator.Respond(this.deal);
            if (this.response.Kind == DiplomacyResponseKind.Accept)
            {
                this.accepted = true;
            }
            else if (this.response.Kind != DiplomacyResponseKind.Refuse)
            {
                DiplomacyEvaluator.RecordFailure(this.deal);
            }
            this.Refresh();
        }

        // 依对方的还价加上钱粮，对方答应过的，直接可以缔约
        private void AgreeButton_Click(object sender, MyraEventArgs args)
        {
            if (this.response == null || this.response.Kind != DiplomacyResponseKind.Counter)
            {
                return;
            }
            this.deal.GiveFund += this.response.ExtraFund;
            this.deal.GiveFood += this.response.ExtraFood;
            this.response = new DiplomacyResponse { Kind = DiplomacyResponseKind.Accept, Text = "既如此，便依此缔约。" };
            this.accepted = true;
            this.Refresh();
        }

        // 按住势力列表上下拖动来滑动，触屏上没有滚轮，主要靠这个
        private void Desktop_TouchDown(object sender, MyraEventArgs args)
        {
            this.dragDistance = 0;
            this.dragLastY = null;
            if (this.picker != null && this.picker.IsOpen)
            {
                return;
            }
            Point? position = MyraUI.Desktop.TouchPosition;
            if (position.HasValue && this.scrollViewer.ContainsGlobalPoint(position.Value) && this.cardsPanel.ContainsGlobalPoint(position.Value))
            {
                this.dragLastY = position.Value.Y;
            }
        }

        private void Desktop_TouchMoved(object sender, MyraEventArgs args)
        {
            Point? position = MyraUI.Desktop.TouchPosition;
            if (!this.dragLastY.HasValue || !position.HasValue)
            {
                return;
            }
            int delta = position.Value.Y - this.dragLastY.Value;
            this.dragLastY = position.Value.Y;
            this.dragDistance += Math.Abs(delta);
            if (this.dragDistance > DragThreshold)
            {
                Point scroll = this.scrollViewer.ScrollPosition;
                scroll.Y = MathHelper.Clamp(scroll.Y - delta, 0, this.scrollViewer.ScrollMaximum.Y);
                this.scrollViewer.ScrollPosition = scroll;
            }
        }

        private void Desktop_TouchUp(object sender, MyraEventArgs args)
        {
            this.dragLastY = null;
        }

        // 刚建好的列表还没排版，不知道能滚多远，所以先记下要滚到的位置，等排版之后再滚过去
        private class FactionListViewer : ScrollViewer
        {
            public int? PendingScrollY { get; set; }

            protected override void InternalArrange()
            {
                base.InternalArrange();
                if (this.PendingScrollY.HasValue && this.ScrollMaximum.Y > 0)
                {
                    int y = MathHelper.Clamp(this.PendingScrollY.Value, 0, this.ScrollMaximum.Y);
                    this.PendingScrollY = null;
                    this.ScrollPosition = new Point(this.ScrollPosition.X, y);
                }
            }
        }

        private class ActionOption
        {
            public readonly DiplomacyKind Kind;
            public readonly string Name;
            // 这种行动怎么执行：派人去，还是立即生效
            public readonly string Method;
            public readonly string Description;
            public Button Button;
            public Label Label;
            public bool Available;

            public ActionOption(DiplomacyKind kind, string name, string method, string description)
            {
                this.Kind = kind;
                this.Name = name;
                this.Method = method;
                this.Description = description;
            }
        }

        /// <summary>
        /// 头像材质放在游戏的临时缓存里，面板开着的时候可能被清掉，所以每次画之前检查一下，失效了就重新取
        /// </summary>
        internal class PortraitImage : IImage
        {
            private readonly Person person;
            private readonly PortraitSize portraitSize;
            private Texture2D texture;

            public PortraitImage(Person person, PortraitSize portraitSize, int width, int height)
            {
                this.person = person;
                this.portraitSize = portraitSize;
                this.Size = new Point(width, height);
            }

            public Point Size { get; }

            public void Draw(RenderContext context, Rectangle dest, Color color)
            {
                if (this.texture == null || this.texture.IsDisposed)
                {
                    this.texture = CacheManager.LoadZhsanAvatar(this.person, this.portraitSize);
                }
                if (this.texture != null)
                {
                    context.Draw(this.texture, dest, null, color, 0f, 0f, null);
                }
            }
        }
    }
}

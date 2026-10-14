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
    /// 外交面板：左边列出可以打交道的势力（用君主头像表示），右边显示与选中势力的关系并选择外交行动，
    /// 下面是选中行动的说明。用Myra绘制，底板和按钮取自MyraSkin的基础图，势力卡片还是纯色。
    /// </summary>
    internal class DiplomacyDialog
    {
        private const int RelationPanelWidth = 400;
        // 分区也用底板图做边框，内容要避开四周的花纹
        private const int PanelPadding = 14;
        // 分区标题占的高度。加上上边距和间距，正好让下面的内容从底板那条横线之下开始
        private const int PanelTitleTop = 12;
        private const int PanelTitleHeight = 28;
        private const int PanelSpacing = 12;
        private const int CardWidth = 96;
        private const int CardHeight = 156;
        private const int CardSpacing = 6;
        private const int CardsPerRow = 3;
        // 小头像文件是64x80，放大四分之一显示
        private const int CardPortraitWidth = 80;
        private const int CardPortraitHeight = 100;
        // 势力列表一次显示两行，再多就滚动
        private const int ListWidth = CardsPerRow * CardWidth + (CardsPerRow - 1) * CardSpacing + 20;
        private const int ListHeight = 2 * CardHeight + CardSpacing;
        private const int ActionsPerRow = 3;
        private const int ActionSpacing = 8;
        private const int ActionButtonWidth = (RelationPanelWidth - 2 * PanelPadding - (ActionsPerRow - 1) * ActionSpacing) / ActionsPerRow;
        private const int ContentWidth = ListWidth + 2 * PanelPadding + PanelSpacing + RelationPanelWidth;
        // 内容离底板边缘的距离，要避开底板四周的花纹
        private const int DialogPadding = 28;
        private const int DialogWidth = ContentWidth + 2 * DialogPadding;
        // 按住拖动超过这个距离才算滑动列表，否则算点击卡片
        private const int DragThreshold = 6;

        private static readonly Color CardColor = new Color(26, 38, 46);
        private static readonly Color CardOverColor = new Color(44, 64, 76);
        private static readonly Color LineColor = new Color(90, 110, 120);

        private readonly Architecture architecture;
        private readonly Faction faction;
        private readonly Action<DiplomacyKind, DiplomaticRelation> onConfirm;
        private readonly List<DiplomaticRelation> relations;
        private readonly List<Button> cards = new List<Button>();
        private readonly List<ActionOption> options = new List<ActionOption>();

        private FactionListViewer scrollViewer;
        private Widget cardsPanel;
        private Label targetNameLabel;
        private Label relationValueLabel;
        private Label statusLabel;
        private Label truceLabel;
        private Label hintTitleLabel;
        private Label hintLabel;
        private Label reasonLabel;
        private Image warningIcon;
        private Button okButton;
        private Label okLabel;
        private DiplomaticRelation selectedRelation;
        private ActionOption selectedOption;

        private int? dragLastY;
        private int dragDistance;

        private DiplomacyDialog(Architecture architecture, DiplomaticRelation selected, Action<DiplomacyKind, DiplomaticRelation> onConfirm)
        {
            this.architecture = architecture;
            this.faction = architecture.BelongedFaction;
            this.onConfirm = onConfirm;
            // 关系好的排在前面
            this.relations = architecture.GetEnhanceDiplomaticRelationList().OrderByDescending(x => x.Relation).ToList();
            // 对方势力可能已经灭亡，不在列表里了
            if (selected != null && this.relations.Contains(selected))
            {
                this.selectedRelation = selected;
            }

            this.options.Add(new ActionOption(DiplomacyKind.Enhance, "亲善", "派遣武将", "派武将前往对方都城，提升双方关系。每名武将花费 " + Highlight(10000) + " 资金。"));
            this.options.Add(new ActionOption(DiplomacyKind.Ally, "结盟", "派遣武将", "关系达到 " + Highlight(AllyRelation) + " 以上时，可派武将前往缔结同盟。每名武将花费 " + Highlight(20000) + " 资金。"));
            this.options.Add(new ActionOption(DiplomacyKind.Reset, "断交", "立即生效", "解除与对方的同盟，关系归零并有损君主声誉。本势力有部队在外时不可断交。"));
            this.options.Add(new ActionOption(DiplomacyKind.Denounce, "声讨", "立即生效", "花费 " + Highlight(120000) + " 资金声讨目标势力，双方关系大跌，其他势力与其关系也会下降。"));
            this.options.Add(new ActionOption(DiplomacyKind.Truce, "停战", "派遣武将", "派武将前往对方都城商议停战。每名武将花费 " + Highlight(50000) + " 资金，停战期内不可重复。"));
            this.options.Add(new ActionOption(DiplomacyKind.QuanXiang, "劝降", "派遣武将", "派一名武将劝说接壤的非同盟势力归降，花费 " + Highlight(50000) + " 资金。"));
        }

        /// <summary>
        /// 显示面板
        /// </summary>
        /// <param name="architecture">发起外交的城池，必须有所属势力</param>
        /// <param name="selected">打开时预先选中的势力，没有就传null</param>
        /// <param name="onConfirm">点确认、面板关闭之后调用，参数为选中的外交方式和与目标势力的关系</param>
        public static void Show(Architecture architecture, DiplomaticRelation selected, Action<DiplomacyKind, DiplomaticRelation> onConfirm)
        {
            var dialog = new DiplomacyDialog(architecture, selected, onConfirm);
            // 面板显示期间占住一个未完成工作，地图和工具栏就不会响应鼠标键盘
            Session.MainGame.mainGameScreen.PushUndoneWork(new UndoneWorkItem(UndoneWorkKind.SubDialog, DialogKind.Diplomacy));
            Desktop desktop = MyraUI.Desktop;
            desktop.TouchDown += dialog.Desktop_TouchDown;
            desktop.TouchMoved += dialog.Desktop_TouchMoved;
            desktop.TouchUp += dialog.Desktop_TouchUp;
            MyraUI.Show(dialog.Build(), () => dialog.Close(null));
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
                Text = "外交",
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
            dialog.Widgets.Add(this.BuildHintPanel());

            var buttons = new HorizontalStackPanel
            {
                Spacing = 12,
                HorizontalAlignment = HorizontalAlignment.Right
            };
            this.okButton = MyraSkin.CreateButton("确认", 90, out this.okLabel);
            this.okButton.Click += this.OkButton_Click;
            Button cancelButton = MyraSkin.CreateButton("取消", 90, out _);
            cancelButton.Click += (sender, args) => this.Close(null);
            buttons.Widgets.Add(this.okButton);
            buttons.Widgets.Add(cancelButton);
            dialog.Widgets.Add(buttons);

            this.Refresh();
            this.ShowDefaultHint();

            // 半透明遮罩铺满窗口，压暗后面的游戏界面
            var mask = new Panel
            {
                Background = new SolidBrush(new Color(0, 0, 0, 150))
            };
            mask.Widgets.Add(dialog);
            return mask;
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

            VerticalStackPanel panel = CreatePanel(ListWidth + 2 * PanelPadding, "选择外交势力（" + this.relations.Count + "）", out _, out _);
            panel.Widgets.Add(this.scrollViewer);
            return panel;
        }

        private Widget BuildRelationPanel()
        {
            VerticalStackPanel panel = CreatePanel(RelationPanelWidth, "外交关系", out _, out _);
            panel.Spacing = 10;

            // 发起方在左，选中的势力在右
            Panel names = CreateRow(28);
            this.targetNameLabel = CreateLabel("", 20, MyraSkin.TextPrimary, HorizontalAlignment.Right);
            names.Widgets.Add(CreateLabel(this.faction.Name, 20, MyraSkin.TextPrimary, HorizontalAlignment.Left));
            names.Widgets.Add(CreateLabel("→", 20, MyraSkin.TextSecondary));
            names.Widgets.Add(this.targetNameLabel);
            panel.Widgets.Add(names);

            Panel status = CreateRow(28);
            this.statusLabel = CreateLabel("", 20, MyraSkin.TextPrimary, HorizontalAlignment.Left);
            this.relationValueLabel = CreateLabel("", 20, MyraSkin.TextPrimary, HorizontalAlignment.Right);
            status.Widgets.Add(this.statusLabel);
            status.Widgets.Add(this.relationValueLabel);
            panel.Widgets.Add(status);

            this.truceLabel = CreateLabel("", 14, MyraSkin.TextSecondary, HorizontalAlignment.Left);
            // 固定高度，有没有停战协定下面的按钮位置都不变
            this.truceLabel.Height = 18;
            panel.Widgets.Add(this.truceLabel);

            panel.Widgets.Add(new Panel
            {
                Height = 1,
                Background = new SolidBrush(LineColor)
            });

            panel.Widgets.Add(CreateLabel("外交行动", 16, MyraSkin.TextTitle, HorizontalAlignment.Left));
            HorizontalStackPanel row = null;
            foreach (ActionOption option in this.options)
            {
                if (row == null || row.Widgets.Count >= ActionsPerRow)
                {
                    row = new HorizontalStackPanel
                    {
                        Spacing = ActionSpacing
                    };
                    panel.Widgets.Add(row);
                }
                ActionOption current = option;
                current.Button = MyraSkin.CreateButton(current.Name, ActionButtonWidth, out current.Label);
                current.Button.Click += (sender, args) => this.SelectOption(current);
                // 鼠标移上去就在说明栏预览这种行动，移开后恢复成已选行动的说明
                current.Button.MouseEntered += (sender, args) => this.ShowOptionHint(current);
                current.Button.MouseLeft += (sender, args) => this.ShowDefaultHint();
                row.Widgets.Add(current.Button);
            }
            return panel;
        }

        // 说明栏：标题行左边是选中的外交行动，右边是本城资金；下面一行是行动内容，再下面一行是不能执行的原因。
        // 三样东西颜色各不相同，玩家一眼能分清做什么、花多少、为什么不行
        private Widget BuildHintPanel()
        {
            VerticalStackPanel panel = CreatePanel(ContentWidth, "", out Panel titleRow, out this.hintTitleLabel);
            var fund = new HorizontalStackPanel
            {
                Spacing = 6,
                HorizontalAlignment = HorizontalAlignment.Right
            };
            fund.Widgets.Add(CreateLabel("资金", 16, MyraSkin.TextSecondary));
            fund.Widgets.Add(CreateLabel(this.architecture.Fund.ToString("N0"), 16, MyraSkin.TextHighlight));
            titleRow.Widgets.Add(fund);

            // 两行都固定高度，文字变化时面板大小不跳动
            this.hintLabel = new Label
            {
                Font = TextManager.GetFont(16),
                // 说明里的数值靠富文本命令变色
                SupportsCommands = true,
                Width = ContentWidth - 2 * PanelPadding,
                Height = 22
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
                Height = 22
            };
            // 有警告图标就放在原因前面，没有的话原因文字前加“※”
            this.warningIcon = MyraSkin.CreateWarningIcon();
            if (this.warningIcon != null)
            {
                this.warningIcon.VerticalAlignment = VerticalAlignment.Center;
                reasonRow.Widgets.Add(this.warningIcon);
            }
            reasonRow.Widgets.Add(this.reasonLabel);
            panel.Widgets.Add(this.hintLabel);
            panel.Widgets.Add(reasonRow);
            return panel;
        }

        // 说明文字里的数值用重点金色标出来，靠Label的富文本命令实现
        private static string Highlight(int value)
        {
            Color color = MyraSkin.TextHighlight;
            return "/c[#" + color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2") + "]" + value.ToString("N0") + "/cd";
        }

        // 结盟要求的最低关系，与Architecture.GetAllyDiplomaticRelationList一致
        private static int AllyRelation
        {
            get
            {
                return (int)Math.Ceiling(Session.GlobalVariables.FriendlyDiplomacyThreshold * 0.9);
            }
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
            content.Widgets.Add(CreateLabel(relation.Relation.ToString(), 14, this.GetStatusColor(relation)));

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
                // 拖动列表之后松手不算点击
                if (this.dragDistance <= DragThreshold)
                {
                    this.SelectRelation(relation);
                }
            };
            return card;
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

        // 各种外交方式对选中的势力能不能用，条件与原来右键菜单里各项的显示条件、可选势力列表一致
        private bool IsAvailable(DiplomacyKind kind, DiplomaticRelation relation)
        {
            switch (kind)
            {
                case DiplomacyKind.Enhance:
                    return this.architecture.EnhanceDiplomaticRelationAvail() && this.architecture.GetEnhanceDiplomaticRelationList().Contains(relation);
                case DiplomacyKind.Ally:
                    return this.architecture.AllyDiplomaticRelationAvail() && this.architecture.GetAllyDiplomaticRelationList().Contains(relation);
                case DiplomacyKind.Reset:
                    return this.architecture.ResetDiplomaticRelationAvail() && this.architecture.GetResetDiplomaticRelationList().Contains(relation);
                case DiplomacyKind.Denounce:
                    return this.architecture.DenounceDiplomaticRelationAvail() && this.architecture.GetDenounceDiplomaticRelationList().Contains(relation);
                case DiplomacyKind.Truce:
                    return this.architecture.TruceDiplomaticRelationAvail() && this.architecture.GetTruceDiplomaticRelationList().Contains(relation);
                case DiplomacyKind.QuanXiang:
                    return this.architecture.QuanXiangDiplomaticRelationAvail() && this.architecture.GetQuanXiangDiplomaticRelationList().Contains(relation);
                default:
                    return false;
            }
        }

        // 外交行动不能执行的具体原因，只用来给玩家看。能不能执行以IsAvailable为准，这里按同样的条件逐条找出没满足的那一条
        private string GetUnavailableReason(DiplomacyKind kind, DiplomaticRelation relation)
        {
            int threshold = Session.GlobalVariables.FriendlyDiplomacyThreshold;
            int movablePersons = this.architecture.GetPersonsExcludeNvGuan().Count;
            switch (kind)
            {
                case DiplomacyKind.Enhance:
                    return this.GetCostReason(10000) ?? GetPersonReason(movablePersons);
                case DiplomacyKind.Ally:
                    if (relation.Relation >= threshold)
                    {
                        return "双方已经结盟。";
                    }
                    if (relation.Relation < AllyRelation)
                    {
                        return "双方关系需达到 " + AllyRelation + " 才能结盟。";
                    }
                    return this.GetCostReason(20000) ?? GetPersonReason(movablePersons);
                case DiplomacyKind.Reset:
                    if (relation.Relation < threshold)
                    {
                        return "双方并非同盟，无需断交。";
                    }
                    return this.faction.TroopCountExcludeTransport > 0 ? "本势力有部队在外，不能断交。" : null;
                case DiplomacyKind.Denounce:
                    return relation.Relation >= threshold ? "不能声讨同盟势力。" : this.GetCostReason(120000);
                case DiplomacyKind.Truce:
                    if (relation.Truce > 0)
                    {
                        return "双方已有停战协定。";
                    }
                    return this.GetCostReason(50000) ?? GetPersonReason(movablePersons);
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

        private string GetStatusText(DiplomaticRelation relation)
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
        private Color GetStatusColor(DiplomaticRelation relation)
        {
            if (relation.Relation > 0)
            {
                return MyraSkin.TextFriendly;
            }
            return relation.Relation < 0 ? MyraSkin.TextHostile : MyraSkin.TextSecondary;
        }

        private void SelectRelation(DiplomaticRelation relation)
        {
            this.selectedRelation = relation;
            // 换了势力之后，原先选的外交方式不一定还能用
            if (this.selectedOption != null && !this.IsAvailable(this.selectedOption.Kind, relation))
            {
                this.selectedOption = null;
            }
            this.Refresh();
            this.ShowDefaultHint();
        }

        // 只有能执行的行动点了才会选中，选中后说明栏常驻显示它；不能执行的行动靠鼠标悬浮查看原因
        private void SelectOption(ActionOption option)
        {
            if (!option.Available)
            {
                return;
            }
            this.selectedOption = option;
            this.Refresh();
            this.ShowDefaultHint();
        }

        // 预览一种行动的说明，不能执行时一并显示原因
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
            this.ShowHint(option.Name + " · " + option.Method, option.Description, MyraSkin.TextPrimary, reason);
        }

        private string GetTargetName()
        {
            return this.selectedRelation.GetDiplomaticFaction(this.faction.ID).Name;
        }

        private void Refresh()
        {
            for (int i = 0; i < this.cards.Count; i++)
            {
                bool isSelected = this.relations[i] == this.selectedRelation;
                this.cards[i].Border = new SolidBrush(isSelected ? MyraSkin.TextHighlight : LineColor);
            }

            if (this.selectedRelation == null)
            {
                this.targetNameLabel.Text = "未选择";
                this.targetNameLabel.TextColor = MyraSkin.TextSecondary;
                this.statusLabel.Text = "";
                this.relationValueLabel.Text = "";
                this.truceLabel.Text = "";
            }
            else
            {
                this.targetNameLabel.Text = this.GetTargetName();
                this.targetNameLabel.TextColor = MyraSkin.TextPrimary;
                this.statusLabel.Text = this.GetStatusText(this.selectedRelation);
                this.statusLabel.TextColor = this.GetStatusColor(this.selectedRelation);
                this.relationValueLabel.Text = this.selectedRelation.Relation.ToString();
                this.relationValueLabel.TextColor = this.GetStatusColor(this.selectedRelation);
                this.truceLabel.Text = this.selectedRelation.Truce > 0 ? "停战中，剩余" + this.selectedRelation.Truce + "天" : "";
            }

            foreach (ActionOption option in this.options)
            {
                option.Available = this.selectedRelation != null && this.IsAvailable(option.Kind, this.selectedRelation);
                // 不能用的外交行动显示成灰色，但仍然可以点，用来查看它的条件
                MyraSkin.SetButtonLook(option.Button, option == this.selectedOption ? MyraButtonLook.Selected : option.Available ? MyraButtonLook.Normal : MyraButtonLook.Disabled);
                option.Label.TextColor = option.Available ? MyraSkin.TextPrimary : MyraSkin.TextSecondary;
            }

            bool canConfirm = this.selectedRelation != null && this.selectedOption != null;
            this.okButton.Enabled = canConfirm;
            this.okLabel.TextColor = canConfirm ? MyraSkin.TextPrimary : MyraSkin.TextSecondary;
        }

        private void ShowDefaultHint()
        {
            if (this.relations.Count == 0)
            {
                this.ShowHint("外交", "目前没有可以打交道的势力。", MyraSkin.TextSecondary);
            }
            else if (this.selectedRelation == null)
            {
                this.ShowHint("选择外交势力", "请先在左侧选择一个势力。", MyraSkin.TextSecondary);
            }
            else if (this.selectedOption == null)
            {
                this.ShowHint("选择外交行动", "请选择要对" + this.GetTargetName() + "开展的外交行动。", MyraSkin.TextSecondary);
            }
            else
            {
                this.ShowHint(this.selectedOption.Name + " · " + this.selectedOption.Method, this.selectedOption.Description, MyraSkin.TextPrimary);
            }
        }

        private void ShowHint(string title, string text, Color color, string reason = null)
        {
            this.hintTitleLabel.Text = title;
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
            if (this.selectedRelation == null || this.selectedOption == null)
            {
                return;
            }
            DiplomacyKind kind = this.selectedOption.Kind;
            DiplomaticRelation relation = this.selectedRelation;
            this.Close(() => this.onConfirm(kind, relation));
        }

        // 按住势力列表上下拖动来滑动，触屏上没有滚轮，主要靠这个
        private void Desktop_TouchDown(object sender, MyraEventArgs args)
        {
            this.dragDistance = 0;
            this.dragLastY = null;
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

        // 头像材质放在游戏的临时缓存里，面板开着的时候可能被清掉，所以每次画之前检查一下，失效了就重新取
        private class PortraitImage : IImage
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

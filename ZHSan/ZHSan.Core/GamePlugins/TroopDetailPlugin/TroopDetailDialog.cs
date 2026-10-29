using GameGlobal;
using GameManager;
using GameObjects;
using GameObjects.Influences;
using GameObjects.PersonDetail;
using GameObjects.TroopDetail;
using Microsoft.Xna.Framework;
using Myra.Graphics2D;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.UI;
using PersonDetailPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using WorldOfTheThreeKingdoms.GameScreens;
using static GameManager.MyraWidgets;

namespace TroopDetailPlugin
{
    /// <summary>
    /// 部队详情，单页两栏。左栏是部队的身份和状态：主将头像、称号势力兵种、攻击防御、兵力士气战意疲劳、移动补给等两列信息、战斗几率；
    /// 右栏分两列，左列战斗能力、战法、特技，最下面随行人物；右列兵种特性。
    /// 战法、特技悬停在下方浮出说明，头像点击看人物详情。底板、配色与人物详情一致
    /// </summary>
    internal class TroopDetailDialog
    {
        // 弹窗离窗口边缘至少留这么多
        private const int ScreenMarginX = 40;
        private const int ScreenMarginY = 30;
        private const int MinDialogWidth = 860;
        private const int MaxDialogWidth = 920;
        // 左栏内容是固定的，再矮就放不下战斗几率表
        private const int MinDialogHeight = 560;
        private const int MaxDialogHeight = 580;

        // 底板，与人物详情一致
        private const int DialogPadding = 28;
        private const int DialogPaddingTop = 7;
        private const int TitleRowHeight = 34;
        private const int BodySpacing = 10;
        // 分区底板右边的花纹比别的边宽，右边多留一些，右对齐的数值才不会压到花纹上
        private const int ColumnPadding = 16;
        private const int ColumnPaddingRight = 22;
        private const int SectionGap = 10;
        // 分区标题：大字标题，后面跟一个小字的说明
        private const int TitleHeight = 26;
        private const int ScrollBarWidth = 18;

        // 左栏
        private const int LeftWidth = 270;
        private const int LeftInnerWidth = LeftWidth - ColumnPadding - ColumnPaddingRight;
        // 头像随弹窗高度变大，剩下的空留在左栏最下面
        private const int MinPortraitSize = 72;
        private const int MaxPortraitSize = 104;
        private const int PortraitGap = 12;
        private const int NameFontSize = 24;
        private const int LongNameFontSize = 20;
        // 攻击、防御两张卡片并排，顶上一条各自颜色的线
        private const int CoreHeight = 52;
        private const int CoreSpacing = 10;
        private const int CoreFontSize = 26;
        // 兵力、士气等一行：名称、进度条、数值
        private const int StatusRowHeight = 20;
        private const int StatusRowSpacing = 5;
        private const int StatusLabelWidth = 34;
        private const int StatusValueWidth = 100;
        private const int StatusBarHeight = 6;
        // 疲劳下面的信息一行两项，名称靠左、数值靠右，每项下面一条分隔线
        private const int InfoRowHeight = 24;
        private const int InfoPairGap = 14;
        // 战斗几率表：第一列是类别，后面三列发动、抵抗、致乱
        private const int ChanceRowHeight = 22;
        private const int ChanceNameWidth = 44;
        private const int ChanceRowCount = 4;
        // 进度条和下面两列信息之间、两列信息和战斗几率之间的空隙
        private const int StatusInfoGap = 8;
        private const int InfoChanceGap = 12;
        // 左栏除头像和行动那一行以外占的高度
        private const int LeftFixedHeight = SectionGap + CoreHeight + SectionGap + 4 * StatusRowHeight + 3 * StatusRowSpacing
            + StatusInfoGap + 3 * InfoRowHeight + InfoChanceGap + TitleHeight + ChanceRowCount * ChanceRowHeight;

        // 右栏分两列，左列占这么多
        private const float CombatColumnRatio = 0.6f;
        private const int ColumnGap = 20;
        private const int AbilityCellHeight = 34;
        private const int AbilityLabelFontSize = 12;
        private const int AbilityValueFontSize = 22;
        // 行高里基线以下的部分大约占这么多，用来把不同字号的字对到同一条基线上
        private const float DescentRatio = 0.22f;
        private const int AbilityCellSpacing = 8;
        // 战法、特技与战斗能力一样一行三个
        private const int SlotsPerRow = 3;
        private const int SlotPaddingLeft = 8;
        private const int SlotPaddingRight = 6;
        private const int SlotHeight = 32;
        private const int SlotSpacing = 6;
        // 随行人物：头像下面写姓名，最多显示4个，多的收在“查看全部”里
        private const int PersonsVisible = 4;
        private const int PersonTileSize = 56;
        private const int PersonNameGap = 4;
        private const int PersonNameHeight = 16;
        private const int PersonTileHeight = PersonTileSize + PersonNameGap + PersonNameHeight;
        private const int PersonSpacing = 10;
        // 展开全部时最多显示两行，再多就上下滚动
        private const int MaxPersonRows = 2;

        // 兵种特性每条上下留的空
        private const int KindRowPadding = 6;

        // 疲劳超过这个数，部队的几率开始下降；到这个上限时降到最低，与Troop.TirednessFactor一致
        private const int TirednessFree = 30;
        private const int TirednessMax = 174;

        private static readonly Color TextPrimary = MyraSkin.TextPrimary;
        private static readonly Color TextSecondary = MyraSkin.TextSecondary;
        private static readonly Color TextMuted = MyraSkin.TextDisabled;
        private static readonly Color Gold = MyraSkin.TextHighlight;
        private static readonly Color Green = MyraSkin.TextFriendly;
        private static readonly Color Warning = MyraSkin.TextWarning;
        private static readonly Color Hostile = MyraSkin.TextHostile;
        // 攻击、防御是部队最核心的两个数，各用一种颜色突出：攻击朱橙，防御青蓝
        private static readonly Color AttackColor = new Color(0xE0, 0x86, 0x50);
        private static readonly Color DefenceColor = new Color(0x6A, 0xA6, 0xD6);
        // 卡片比分区底色更深一些
        private static readonly Color DeepCardColor = new Color(16, 28, 38);
        private static readonly Color DividerColor = new Color(58, 76, 86);
        // 正在使用的战法、特技：蓝底蓝边
        private static readonly Color CurrentColor = new Color(0x5E, 0x9C, 0xC8);

        private readonly Troop troop;
        private readonly List<Person> persons;

        // 随窗口大小算出来的尺寸
        private readonly int dialogWidth;
        private readonly int dialogHeight;
        private readonly int bodyHeight;
        private readonly int innerHeight;
        private readonly int rightWidth;
        private readonly int combatWidth;
        private readonly int kindWidth;
        private readonly int portraitSize;

        private Panel mask;
        private MyraPopover popover;
        private readonly MyraDragScroll dragScroll = new MyraDragScroll();
        // 传给MyraUI的根控件，用来判断本窗口是否在最上层
        private Widget root;

        // 随行人物展开、收起时，人物区和上面战法列表的高度跟着变
        private ScrollViewer listViewer;
        private ScrollViewer personsViewer;

        private TroopDetailDialog(Troop troop)
        {
            this.troop = troop;
            // 随行人物不含主将，主将点左栏头像查看
            this.persons = troop.Persons.Cast<Person>().Where(x => x != troop.Leader).ToList();
            Point size = MyraUI.LayoutSize;
            this.dialogWidth = MathHelper.Clamp(size.X - 2 * ScreenMarginX, MinDialogWidth, MaxDialogWidth);
            this.dialogHeight = MathHelper.Clamp(size.Y - 2 * ScreenMarginY, MinDialogHeight, MaxDialogHeight);
            this.bodyHeight = this.dialogHeight - DialogPaddingTop - TitleRowHeight - BodySpacing - DialogPadding;
            this.innerHeight = this.bodyHeight - 2 * ColumnPadding;
            this.rightWidth = this.dialogWidth - 2 * DialogPadding - LeftWidth - BodySpacing;
            int rightInnerWidth = this.rightWidth - ColumnPadding - ColumnPaddingRight;
            this.combatWidth = (int)((rightInnerWidth - ColumnGap) * CombatColumnRatio);
            this.kindWidth = rightInnerWidth - ColumnGap - this.combatWidth;
            int spare = this.innerHeight - LeftFixedHeight - (this.HasCurrentAction ? InfoRowHeight : 0);
            this.portraitSize = MathHelper.Clamp(spare, MinPortraitSize, MaxPortraitSize);
        }

        /// <summary>
        /// 显示部队详情
        /// </summary>
        public static void Show(Troop troop)
        {
            if (troop == null || troop.Army == null)
            {
                return;
            }
            var dialog = new TroopDetailDialog(troop);
            // 面板显示期间占住一个未完成工作，地图和工具栏就不会响应鼠标键盘
            Session.MainGame.mainGameScreen.PushUndoneWork(new UndoneWorkItem(UndoneWorkKind.SubDialog, DialogKind.TroopDetail));
            dialog.root = dialog.Build();
            dialog.dragScroll.Attach(dialog.root, dialog.popover);
            MyraUI.Show(dialog.root, dialog.Close);
        }

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

        // 有正在进行的战斗行动或特技时，左栏多显示一行
        private bool HasCurrentAction => this.troop.CurrentCombatAction != "----" || this.troop.CurrentStunt != null;

        private Widget Build()
        {
            this.mask = new Panel
            {
                // 半透明遮罩铺满窗口，压暗后面的游戏界面
                Background = new SolidBrush(new Color(0, 0, 0, 150))
            };
            var dialog = new VerticalStackPanel
            {
                Width = this.dialogWidth,
                Height = this.dialogHeight,
                Spacing = BodySpacing,
                Padding = new Thickness(DialogPadding, DialogPaddingTop, DialogPadding, DialogPadding),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Background = MyraSkin.PanelBackground
            };
            // 说明浮层按弹窗本体定位，盖在内容上面，所以最后加到遮罩里。浮层放在格子下面，不挡住同一行的其他格子
            this.popover = new MyraPopover(this.mask, dialog)
            {
                PlaceBelow = true
            };
            dialog.Widgets.Add(this.BuildTitle());
            var body = new HorizontalStackPanel
            {
                Spacing = BodySpacing,
                Height = this.bodyHeight
            };
            body.Widgets.Add(this.BuildLeft());
            body.Widgets.Add(this.BuildRight());
            dialog.Widgets.Add(body);
            this.mask.Widgets.Add(dialog);
            this.mask.Widgets.Add(this.popover.Widget);
            return this.mask;
        }

        // 标题：居中写“部队详情”，右上角关闭按钮。部队名放在左栏
        private Widget BuildTitle()
        {
            var row = new Panel
            {
                Height = TitleRowHeight
            };
            Label title = CreateLabel("部队详情", 22, MyraSkin.TextTitle);
            title.HorizontalAlignment = HorizontalAlignment.Center;
            title.VerticalAlignment = VerticalAlignment.Center;
            row.Widgets.Add(title);
            Button close = CreateCloseButton(this.Close);
            close.VerticalAlignment = VerticalAlignment.Center;
            row.Widgets.Add(close);
            return row;
        }

        private VerticalStackPanel CreateColumn(int width)
        {
            return new VerticalStackPanel
            {
                Width = width,
                Height = this.bodyHeight,
                Padding = new Thickness(ColumnPadding, ColumnPadding, ColumnPaddingRight, ColumnPadding),
                Background = MyraSkin.PlainPanelBackground
            };
        }

        // 分区标题，右边可以再放一个靠右的小字
        private static Widget CreateTitle(string title, string note, int width, string rightNote = null)
        {
            var row = new Panel
            {
                Width = width,
                Height = TitleHeight
            };
            var left = new HorizontalStackPanel
            {
                Spacing = 8,
                VerticalAlignment = VerticalAlignment.Center
            };
            left.Widgets.Add(CreateLabel(title, 16, TextPrimary));
            if (note != null)
            {
                Label noteLabel = CreateLabel(note, 12, TextSecondary);
                noteLabel.VerticalAlignment = VerticalAlignment.Bottom;
                left.Widgets.Add(noteLabel);
            }
            row.Widgets.Add(left);
            // 放不下时不显示靠右的小字，免得和标题叠在一起
            if (rightNote != null && MeasureWidth(title, 16) + MeasureWidth(note ?? "", 12) + MeasureWidth(rightNote, 12) + 24 <= width)
            {
                Label right = CreateLabel(rightNote, 12, TextMuted);
                right.HorizontalAlignment = HorizontalAlignment.Right;
                right.VerticalAlignment = VerticalAlignment.Center;
                row.Widgets.Add(right);
            }
            return row;
        }

        private static int MeasureWidth(string text, int fontSize)
        {
            return (int)Math.Ceiling(TextManager.GetFont(fontSize).MeasureString(text).X);
        }

        private static Widget CreateDivider(Color color)
        {
            return new Panel
            {
                Height = 1,
                VerticalAlignment = VerticalAlignment.Bottom,
                Background = new SolidBrush(color)
            };
        }

        #region 左栏

        private Widget BuildLeft()
        {
            Troop t = this.troop;
            VerticalStackPanel column = this.CreateColumn(LeftWidth);
            column.Widgets.Add(this.BuildHeader());
            column.Widgets.Add(CreateSpacer(SectionGap));

            int coreWidth = (LeftInnerWidth - CoreSpacing) / 2;
            var core = new HorizontalStackPanel
            {
                Spacing = CoreSpacing
            };
            core.Widgets.Add(CreateCoreCell("攻击力", t.Offence, AttackColor, coreWidth));
            core.Widgets.Add(CreateCoreCell("防御力", t.Defence, DefenceColor, LeftInnerWidth - CoreSpacing - coreWidth));
            column.Widgets.Add(core);
            column.Widgets.Add(CreateSpacer(SectionGap));

            var status = new VerticalStackPanel
            {
                Spacing = StatusRowSpacing
            };
            int maxScale = t.Army.Kind.MaxScale;
            status.Widgets.Add(CreateStatusRow("兵力", t.Quantity.ToString("#,0") + " / " + maxScale.ToString("#,0"), t.Quantity, maxScale, Gold));
            status.Widgets.Add(CreateStatusRow("士气", t.Morale + " / " + t.Army.MoraleCeiling, t.Morale, t.Army.MoraleCeiling, Gold));
            status.Widgets.Add(CreateStatusRow("战意", t.Combativity + " / " + t.Army.CombativityCeiling, t.Combativity, t.Army.CombativityCeiling, Gold));
            // 疲劳低的时候不影响作战，条用暗色；超过就用警示色
            Color tiredColor = t.Tiredness > TirednessFree ? Warning : LineColor;
            status.Widgets.Add(CreateStatusRow("疲劳", t.Tiredness.ToString(), t.Tiredness, TirednessMax, tiredColor));
            column.Widgets.Add(status);
            column.Widgets.Add(CreateSpacer(StatusInfoGap));

            // 口粮充足绿色，不到一半橙黄，只剩一天以内红色，粮草跟着一起变色
            bool lowFood = t.RationDaysLeft * 2 < t.RationDays;
            Color rationColor = t.RationDaysLeft <= 1 ? Hostile : lowFood ? Warning : Green;
            Color foodColor = t.RationDaysLeft <= 1 ? Hostile : lowFood ? Warning : TextPrimary;
            var info = new VerticalStackPanel();
            info.Widgets.Add(CreateInfoPair("移动", t.Speed.ToString(), TextPrimary, "地形乘数", t.CurrentRate.ToString("0.##"), TextPrimary));
            info.Widgets.Add(CreateInfoPair("伤兵", t.InjuryQuantity.ToString("#,0"), t.InjuryQuantity > 0 ? Warning : TextPrimary,
                "伤兵几率", t.InjuryChance.ToString(), TextPrimary));
            info.Widgets.Add(CreateInfoPair("粮草", t.Food.ToString("#,0"), foodColor,
                "口粮", t.RationDaysLeft + " / " + t.RationDays + " 天", rationColor));
            // 行动、特技只在有值时显示这一行
            if (this.HasCurrentAction)
            {
                string action = t.CurrentCombatAction;
                string stunt = t.CurrentStunt == null ? "----" : t.CurrentStunt.Name + "(" + t.StuntDayLeft + "天)";
                info.Widgets.Add(CreateInfoPair("行动", action, action == "----" ? TextMuted : TextPrimary,
                    "特技", stunt, t.CurrentStunt == null ? TextMuted : Gold));
            }
            column.Widgets.Add(info);
            column.Widgets.Add(CreateSpacer(InfoChanceGap));

            column.Widgets.Add(this.BuildChances(LeftInnerWidth));
            return column;
        }

        // 头部：头像在左，右边竖排部队名、称号、势力、兵种标签。部队名顶着头像上沿，标签贴着头像下沿，中间两行均匀分布
        private Widget BuildHeader()
        {
            Troop t = this.troop;
            int size = this.portraitSize;
            var header = new HorizontalStackPanel
            {
                Spacing = PortraitGap,
                Height = size
            };
            header.Widgets.Add(this.BuildPortrait(size));

            // 文字这一列给定宽度，宽度不定时Myra会把超宽的字居中摆放，左边被裁掉
            int textWidth = LeftInnerWidth - PortraitGap - size;
            var text = new Panel
            {
                Width = textWidth,
                Height = size
            };
            string name = t.DisplayName ?? "";
            int nameSize = name.Length >= 5 ? LongNameFontSize : NameFontSize;
            Label nameLabel = CreateLabel(name, nameSize, TextPrimary);
            nameLabel.VerticalAlignment = VerticalAlignment.Top;
            text.Widgets.Add(nameLabel);

            // 标签高度是13号字的行高加上下各1像素的内边距
            int nameHeight = TextManager.GetFont(nameSize).LineHeight;
            int lineHeight = TextManager.GetFont(13).LineHeight;
            int tagHeight = lineHeight + 2;
            int gap = Math.Max(0, (size - nameHeight - 2 * lineHeight - tagHeight) / 3);
            string title = t.CombatTitleString;
            Label titleLabel = CreateLabel("称号 " + title, 13, title == "----" ? TextSecondary : Gold);
            titleLabel.VerticalAlignment = VerticalAlignment.Top;
            titleLabel.Top = nameHeight + gap;
            text.Widgets.Add(titleLabel);
            Label factionLabel = CreateLabel("势力 " + t.FactionString, 13, TextSecondary);
            factionLabel.VerticalAlignment = VerticalAlignment.Top;
            factionLabel.Top = nameHeight + 2 * gap + lineHeight;
            text.Widgets.Add(factionLabel);
            // 兵种和兵种类型两个标签
            var tags = new HorizontalStackPanel
            {
                Spacing = 6,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Bottom
            };
            tags.Widgets.Add(CreateTag(t.Army.Kind.Name, Gold));
            tags.Widgets.Add(CreateTag(t.Army.Kind.TypeName, TextSecondary));
            text.Widgets.Add(tags);
            header.Widgets.Add(text);
            return header;
        }

        // 主将头像，点击打开主将的人物详情；鼠标移上去边框变金色
        private Widget BuildPortrait(int size)
        {
            Person leader = this.troop.Leader;
            var frame = new Panel
            {
                Width = size,
                Height = size,
                Background = new SolidBrush(DeepCardColor),
                Border = new SolidBrush(LineColor),
                BorderThickness = new Thickness(1)
            };
            if (leader == null)
            {
                return frame;
            }
            frame.Widgets.Add(new Image
            {
                Renderable = new DiplomacyDialog.PortraitImage(leader, PortraitSize.Medium, size - 2, size - 2)
            });
            Button button = CreateBareButton(frame, size, size);
            button.MouseEntered += (sender, args) => frame.Border = new SolidBrush(Gold);
            button.MouseLeft += (sender, args) => frame.Border = new SolidBrush(LineColor);
            button.Click += (sender, args) => PersonDetailDialog.Show(leader, false);
            return button;
        }

        // 攻击、防御：深色底、细边框，顶上一条各自颜色的线；左上小字名称，下面大字数值
        private static Widget CreateCoreCell(string title, int value, Color color, int width)
        {
            var cell = new Panel
            {
                Width = width,
                Height = CoreHeight,
                Padding = new Thickness(12, 7, 10, 4),
                Background = new AccentBrush(DeepCardColor, DividerColor, color)
            };
            Label label = CreateLabel(title, 12, TextSecondary);
            label.VerticalAlignment = VerticalAlignment.Top;
            cell.Widgets.Add(label);
            Label number = CreateLabel(value.ToString("#,0"), CoreFontSize, color);
            number.VerticalAlignment = VerticalAlignment.Bottom;
            cell.Widgets.Add(number);
            return cell;
        }

        private static Widget CreateStatusRow(string title, string value, int current, int max, Color barColor)
        {
            var row = new HorizontalStackPanel
            {
                Height = StatusRowHeight,
                Spacing = 8
            };
            Label label = CreateLabel(title, 13, TextSecondary);
            label.Width = StatusLabelWidth;
            label.VerticalAlignment = VerticalAlignment.Center;
            row.Widgets.Add(label);
            Widget bar = CreateBar(LeftInnerWidth - StatusLabelWidth - StatusValueWidth - 16, StatusBarHeight, current, max, barColor);
            bar.VerticalAlignment = VerticalAlignment.Center;
            row.Widgets.Add(bar);
            var valueHolder = new Panel
            {
                Width = StatusValueWidth
            };
            Label number = CreateLabel(value, 14, TextPrimary);
            number.HorizontalAlignment = HorizontalAlignment.Right;
            number.VerticalAlignment = VerticalAlignment.Center;
            valueHolder.Widgets.Add(number);
            row.Widgets.Add(valueHolder);
            return row;
        }

        // 一行两项，中间隔开一点
        private static Widget CreateInfoPair(string title1, string value1, Color color1, string title2, string value2, Color color2)
        {
            int width = (LeftInnerWidth - InfoPairGap) / 2;
            var row = new HorizontalStackPanel
            {
                Spacing = InfoPairGap
            };
            row.Widgets.Add(CreateInfoCell(title1, value1, color1, width));
            row.Widgets.Add(CreateInfoCell(title2, value2, color2, LeftInnerWidth - InfoPairGap - width));
            return row;
        }

        // 名称靠左、数值靠右，下面一条分隔线
        private static Widget CreateInfoCell(string title, string value, Color valueColor, int width)
        {
            var cell = new Panel
            {
                Width = width,
                Height = InfoRowHeight
            };
            Label label = CreateLabel(title, 13, TextSecondary);
            label.VerticalAlignment = VerticalAlignment.Center;
            cell.Widgets.Add(label);
            Label text = CreateLabel(value, 14, valueColor);
            text.HorizontalAlignment = HorizontalAlignment.Right;
            text.VerticalAlignment = VerticalAlignment.Center;
            cell.Widgets.Add(text);
            cell.Widgets.Add(CreateDivider(DividerColor));
            return cell;
        }

        // 战斗几率表：行是暴击、包围、计略，列是发动、抵抗、致乱
        private Widget BuildChances(int width)
        {
            Troop t = this.troop;
            var table = new VerticalStackPanel();
            table.Widgets.Add(CreateTitle("战斗几率", null, width));
            table.Widgets.Add(CreateChanceRow(width, "", new[] { "发动", "抵抗", "致乱" }, null));
            table.Widgets.Add(CreateChanceRow(width, "暴击", null, new int?[] { t.CriticalStrikeChance, t.AntiCriticalStrikeChance, t.ChaosAfterCriticalStrikeChance }));
            // 包围没有“发动”几率，抵抗一格是避免被围攻
            table.Widgets.Add(CreateChanceRow(width, "包围", null, new int?[] { null, t.AvoidSurroundedChance, t.ChaosAfterSurroundAttackChance }));
            table.Widgets.Add(CreateChanceRow(width, "计略", null, new int?[] { t.StratagemChanceIncrement, t.AntiStratagemChanceIncrement, t.ChaosAfterStratagemSuccessChance }));
            return table;
        }

        // 表格的一行：表头给headers，数据行给values。数值0用暗色，负数用红色；没有的格子写横线
        private static Widget CreateChanceRow(int width, string name, string[] headers, int?[] values)
        {
            int cellWidth = (width - ChanceNameWidth) / 3;
            var row = new Panel
            {
                Height = ChanceRowHeight
            };
            var cells = new HorizontalStackPanel();
            Label nameLabel = CreateLabel(name, 13, TextSecondary);
            nameLabel.Width = ChanceNameWidth;
            nameLabel.VerticalAlignment = VerticalAlignment.Center;
            cells.Widgets.Add(nameLabel);
            for (int i = 0; i < 3; i++)
            {
                var cell = new Panel
                {
                    Width = cellWidth
                };
                Label label;
                if (headers != null)
                {
                    label = CreateLabel(headers[i], 12, TextSecondary);
                }
                else if (values[i] == null)
                {
                    // 隶变字库里没有长横线，用普通减号
                    label = CreateLabel("-", 15, TextMuted);
                }
                else
                {
                    int value = values[i].Value;
                    label = CreateLabel(value.ToString(), 15, value < 0 ? Hostile : value == 0 ? TextMuted : TextPrimary);
                }
                label.HorizontalAlignment = HorizontalAlignment.Center;
                label.VerticalAlignment = VerticalAlignment.Center;
                cell.Widgets.Add(label);
                cells.Widgets.Add(cell);
            }
            row.Widgets.Add(cells);
            row.Widgets.Add(CreateDivider(headers != null ? LineColor : DividerColor));
            return row;
        }

        #endregion

        #region 右栏

        // 两列：左列战斗能力、战法、特技、随行人物，右列兵种特性
        private Widget BuildRight()
        {
            VerticalStackPanel right = this.CreateColumn(this.rightWidth);
            var columns = new HorizontalStackPanel
            {
                Spacing = ColumnGap,
                Height = this.innerHeight
            };
            columns.Widgets.Add(this.BuildCombat());
            columns.Widgets.Add(this.BuildKind());
            right.Widgets.Add(columns);
            return right;
        }

        // 上面战斗能力，中间战法、特技，太多时滚动；最下面随行人物，没有就不显示
        private Widget BuildCombat()
        {
            int width = this.combatWidth;
            var column = new VerticalStackPanel
            {
                Width = width,
                Height = this.innerHeight
            };
            column.Widgets.Add(CreateTitle("战斗能力", null, width));
            column.Widgets.Add(this.BuildAbilities(width));
            column.Widgets.Add(CreateSpacer(SectionGap));

            this.listViewer = this.CreateViewer(this.BuildLists(width - ScrollBarWidth), width, SlotHeight);
            column.Widgets.Add(this.listViewer);
            if (this.persons.Count > 0)
            {
                column.Widgets.Add(CreateSpacer(SectionGap));
                column.Widgets.Add(CreateTitle("随行人物", this.persons.Count + "人", width));
                this.personsViewer = this.CreateViewer(null, width, PersonTileHeight);
                column.Widgets.Add(this.personsViewer);
            }
            this.ShowPersons(false);
            return column;
        }

        // 三张卡片：左下小字名称，右边大字数值
        private Widget BuildAbilities(int width)
        {
            Troop t = this.troop;
            var cells = new[]
            {
                new KeyValuePair<string, int>("武学", t.TroopStrength),
                new KeyValuePair<string, int>("将略", t.TroopCommand),
                new KeyValuePair<string, int>("谋略", t.TroopIntelligence),
            };
            int cellWidth = (width - (cells.Length - 1) * AbilityCellSpacing) / cells.Length;
            int valueLineHeight = TextManager.GetFont(AbilityValueFontSize).LineHeight;
            int labelLineHeight = TextManager.GetFont(AbilityLabelFontSize).LineHeight;
            int baselineLift = (int)Math.Round((valueLineHeight - labelLineHeight) * DescentRatio);
            var row = new HorizontalStackPanel
            {
                Spacing = AbilityCellSpacing
            };
            foreach (var cell in cells)
            {
                var panel = new Panel
                {
                    Width = cellWidth,
                    Height = AbilityCellHeight,
                    Padding = new Thickness(10, 0, 10, 0),
                    Background = new FrameBrush(DeepCardColor, DividerColor, false)
                };
                // 名称和数值排在同一条基线上，这一行在卡片里上下居中。
                // 两行字都贴着这一行的底边，大字基线以下留的空比小字多，小字往上抬这个差值
                var line = new Panel
                {
                    Height = valueLineHeight,
                    VerticalAlignment = VerticalAlignment.Center
                };
                Label label = CreateLabel(cell.Key, AbilityLabelFontSize, TextSecondary);
                label.VerticalAlignment = VerticalAlignment.Bottom;
                label.Margin = new Thickness(0, 0, 0, baselineLift);
                line.Widgets.Add(label);
                Label number = CreateLabel(cell.Value.ToString(), AbilityValueFontSize, TextPrimary);
                number.HorizontalAlignment = HorizontalAlignment.Right;
                number.VerticalAlignment = VerticalAlignment.Bottom;
                line.Widgets.Add(number);
                panel.Widgets.Add(line);
                row.Widgets.Add(panel);
            }
            return row;
        }

        // 战法、特技都按战意消耗从低到高排
        private Widget BuildLists(int width)
        {
            Troop t = this.troop;
            var column = new VerticalStackPanel();
            column.Widgets.Add(CreateTitle("战法", t.CombatMethods.Count + "种", width, "悬浮查看效果 · 按战意消耗从低到高"));
            List<CombatMethod> methods = t.CombatMethods.Values.OrderBy(this.GetCost).ToList();
            column.Widgets.Add(this.BuildSlots(methods, width, method =>
            {
                int cost = this.GetCost(method);
                return new Slot(method.Name, cost, this.CanCast(cost, method.IsCastable(t)), t.CurrentCombatMethod == method,
                    (panel, textWidth) => this.ShowCombatMethod(panel, method, textWidth));
            }));
            column.Widgets.Add(CreateSpacer(SectionGap));
            column.Widgets.Add(CreateTitle("特技", t.Stunts.Count + "种", width));
            List<Stunt> stunts = t.Stunts.Values.OrderBy(x => x.Combativity).ToList();
            column.Widgets.Add(this.BuildSlots(stunts, width, stunt =>
                new Slot(stunt.Name, stunt.Combativity, this.CanCast(stunt.Combativity, stunt.IsCastable(t)), t.CurrentStunt == stunt,
                    (panel, textWidth) => this.ShowStunt(panel, stunt, textWidth))));
            return column;
        }

        // 随行人物：收起时最多显示4个，多的用“+N 查看全部”代替；展开时全部排开，最多两行，再多上下滚动。
        // 人物区变高变矮时，上面战法列表的高度跟着变
        private void ShowPersons(bool expanded)
        {
            int width = this.combatWidth;
            int fixedHeight = TitleHeight + AbilityCellHeight + SectionGap;
            int personsHeight = 0;
            if (this.personsViewer != null)
            {
                int rowHeight;
                if (expanded)
                {
                    int perRow = Math.Max(1, (width - ScrollBarWidth + PersonSpacing) / (PersonTileSize + PersonSpacing));
                    List<Widget> tiles = this.persons.Select(this.CreatePersonTile).ToList();
                    tiles.Add(CreateMoreTile("收起", null, () => this.ShowPersons(false)));
                    int rows = (tiles.Count + perRow - 1) / perRow;
                    this.personsViewer.Content = CreateTileGrid(tiles, perRow);
                    rowHeight = Math.Min(rows, MaxPersonRows) * (PersonTileHeight + PersonSpacing) - PersonSpacing;
                }
                else
                {
                    int perRow = Math.Max(1, (width + PersonSpacing) / (PersonTileSize + PersonSpacing));
                    int maxShown = Math.Min(PersonsVisible, perRow);
                    var tiles = new List<Widget>();
                    if (this.persons.Count <= maxShown)
                    {
                        tiles.AddRange(this.persons.Select(this.CreatePersonTile));
                    }
                    else
                    {
                        // 留一格给“查看全部”
                        int shown = Math.Max(0, Math.Min(PersonsVisible, perRow - 1));
                        tiles.AddRange(this.persons.Take(shown).Select(this.CreatePersonTile));
                        tiles.Add(CreateMoreTile("+" + (this.persons.Count - shown), "查看全部", () => this.ShowPersons(true)));
                    }
                    this.personsViewer.Content = CreateTileGrid(tiles, tiles.Count);
                    rowHeight = PersonTileHeight;
                }
                this.personsViewer.Height = rowHeight;
                this.personsViewer.ScrollPosition = Point.Zero;
                personsHeight = SectionGap + TitleHeight + rowHeight;
            }
            this.listViewer.Height = Math.Max(SlotHeight, this.innerHeight - fixedHeight - personsHeight);
        }

        private static Widget CreateTileGrid(List<Widget> tiles, int perRow)
        {
            var rows = new VerticalStackPanel
            {
                Spacing = PersonSpacing
            };
            HorizontalStackPanel row = null;
            foreach (Widget tile in tiles)
            {
                if (row == null || row.Widgets.Count >= perRow)
                {
                    row = new HorizontalStackPanel
                    {
                        Spacing = PersonSpacing
                    };
                    rows.Widgets.Add(row);
                }
                row.Widgets.Add(tile);
            }
            return rows;
        }

        // 一个人物：头像下面写姓名，点击打开人物详情，鼠标移上去头像边框和姓名变金色
        private Widget CreatePersonTile(Person person)
        {
            var frame = new Panel
            {
                Width = PersonTileSize,
                Height = PersonTileSize,
                Background = new SolidBrush(DeepCardColor),
                Border = new SolidBrush(DividerColor),
                BorderThickness = new Thickness(1)
            };
            frame.Widgets.Add(new Image
            {
                Renderable = new DiplomacyDialog.PortraitImage(person, PortraitSize.Medium, PersonTileSize - 2, PersonTileSize - 2)
            });
            Label name = CreateLabel(person.Name, 12, TextPrimary);
            Button button = CreateTileButton(frame, name);
            button.MouseEntered += (sender, args) =>
            {
                frame.Border = new SolidBrush(Gold);
                name.TextColor = Gold;
            };
            button.MouseLeft += (sender, args) =>
            {
                frame.Border = new SolidBrush(DividerColor);
                name.TextColor = TextPrimary;
            };
            button.Click += (sender, args) => PersonDetailDialog.Show(person, false);
            return button;
        }

        // “+N 查看全部”和“收起”：金色虚线框，框里写字
        private static Widget CreateMoreTile(string text, string caption, Action onClick)
        {
            var frame = new Panel
            {
                Width = PersonTileSize,
                Height = PersonTileSize,
                Background = new FrameBrush(Color.Transparent, GoldDim, true)
            };
            Label label = CreateLabel(text, text.StartsWith("+") ? 20 : 14, Gold);
            label.HorizontalAlignment = HorizontalAlignment.Center;
            label.VerticalAlignment = VerticalAlignment.Center;
            frame.Widgets.Add(label);
            Label name = CreateLabel(caption ?? "", 12, TextSecondary);
            Button button = CreateTileButton(frame, name);
            button.MouseEntered += (sender, args) => frame.Background = new FrameBrush(Gold * 0.12f, Gold, true);
            button.MouseLeft += (sender, args) => frame.Background = new FrameBrush(Color.Transparent, GoldDim, true);
            button.Click += (sender, args) => onClick();
            return button;
        }

        private static Button CreateTileButton(Widget frame, Label name)
        {
            var tile = new VerticalStackPanel
            {
                Width = PersonTileSize,
                Spacing = PersonNameGap
            };
            tile.Widgets.Add(frame);
            name.HorizontalAlignment = HorizontalAlignment.Center;
            tile.Widgets.Add(name);
            return CreateBareButton(tile, PersonTileSize, PersonTileHeight);
        }

        // 不画底的按钮，外观由里面的内容自己变
        private static Button CreateBareButton(Widget content, int width, int height)
        {
            return new Button
            {
                Content = content,
                Width = width,
                Height = height,
                Padding = new Thickness(0),
                Background = Transparent,
                FocusedBackground = Transparent,
                PressedBackground = Transparent,
                OverBackground = Transparent
            };
        }

        // 战法的战意消耗要减去部队的减免，与旧界面一致
        private int GetCost(CombatMethod method)
        {
            return method.Combativity - this.troop.DecrementOfCombatMethodCombativityConsuming;
        }

        private bool CanCast(int cost, bool castable)
        {
            return castable && this.troop.Combativity >= cost;
        }

        // 一个战法或特技格子要显示的东西
        private class Slot
        {
            public readonly string Name;
            public readonly int Cost;
            public readonly bool Castable;
            public readonly bool Current;
            public readonly Action<VerticalStackPanel, int> Fill;

            public Slot(string name, int cost, bool castable, bool current, Action<VerticalStackPanel, int> fill)
            {
                this.Name = name;
                this.Cost = cost;
                this.Castable = castable;
                this.Current = current;
                this.Fill = fill;
            }
        }

        // 名称靠左、战意靠右。能发动的深色底，不能发动的暗色虚线框，正在用的蓝底蓝边；鼠标移上去边框变金色
        private Widget BuildSlots<T>(List<T> items, int width, Func<T, Slot> toSlot)
        {
            var rows = new VerticalStackPanel
            {
                Spacing = SlotSpacing
            };
            if (items.Count == 0)
            {
                rows.Widgets.Add(CreateLabel("无", 13, TextSecondary));
                return rows;
            }
            int perRow = SlotsPerRow;
            int slotWidth = (width - (perRow - 1) * SlotSpacing) / perRow;
            HorizontalStackPanel row = null;
            foreach (T item in items)
            {
                if (row == null || row.Widgets.Count >= perRow)
                {
                    row = new HorizontalStackPanel
                    {
                        Spacing = SlotSpacing
                    };
                    rows.Widgets.Add(row);
                }
                Slot slot = toSlot(item);
                bool bright = slot.Castable || slot.Current;
                var content = new Panel();
                Label name = CreateLabel(slot.Name, 13, bright ? TextPrimary : TextMuted);
                name.VerticalAlignment = VerticalAlignment.Center;
                content.Widgets.Add(name);
                // 名称长、格子放不下时只写战意的数
                string costText = "战意 " + slot.Cost;
                if (MeasureWidth(slot.Name, 13) + MeasureWidth(costText, 11) + 6 > slotWidth - SlotPaddingLeft - SlotPaddingRight)
                {
                    costText = slot.Cost.ToString();
                }
                Label cost = CreateLabel(costText, 11, bright ? MyraSkin.TextTitle : TextMuted);
                cost.HorizontalAlignment = HorizontalAlignment.Right;
                cost.VerticalAlignment = VerticalAlignment.Center;
                content.Widgets.Add(cost);

                Color fill = slot.Current ? CurrentColor * 0.22f : slot.Castable ? DeepCardColor : Color.Transparent;
                IBrush normal = slot.Current ? new FrameBrush(fill, CurrentColor, false)
                    : slot.Castable ? new FrameBrush(fill, DividerColor, false)
                    : new FrameBrush(fill, LineColor, true);
                IBrush over = new FrameBrush(fill, Gold, !bright);
                var button = new Button
                {
                    Content = content,
                    Width = slotWidth,
                    Height = SlotHeight,
                    Padding = new Thickness(SlotPaddingLeft, 0, SlotPaddingRight, 0),
                    Background = normal,
                    FocusedBackground = normal,
                    OverBackground = over,
                    PressedBackground = over
                };
                this.popover.Attach(button, slot.Fill);
                row.Widgets.Add(button);
            }
            return rows;
        }

        private void ShowCombatMethod(VerticalStackPanel panel, CombatMethod method, int width)
        {
            int cost = this.GetCost(method);
            AddPopoverHeader(panel, method.Name, "战法 · 战意 " + cost, width);
            this.AddCastState(panel, cost, method.IsCastable(this.troop), this.troop.CurrentCombatMethod == method);
            if (!string.IsNullOrWhiteSpace(method.Description))
            {
                panel.Widgets.Add(CreateWrappedLabel(method.Description, 13, TextSecondary, width));
            }
            AddInfluences(panel, method.Influences, width);
            AddConditions(panel, "发动条件", method.CastConditions.Select(x => Check(x, x.CheckCondition(this.troop))), width);
        }

        private void ShowStunt(VerticalStackPanel panel, Stunt stunt, int width)
        {
            AddPopoverHeader(panel, stunt.Name, "特技 · 战意 " + stunt.Combativity, width);
            this.AddCastState(panel, stunt.Combativity, stunt.IsCastable(this.troop), this.troop.CurrentStunt == stunt);
            panel.Widgets.Add(CreateLabel("持续 " + stunt.Period * Session.Parameters.DayInTurn + " 天", 13, TextSecondary));
            AddInfluences(panel, stunt.Influences.Values, width);
            AddConditions(panel, "使用条件", stunt.CastConditions.Select(x => Check(x, x.CheckCondition(this.troop))), width);
        }

        // 说明第一行：左边名称，右边类别和战意
        private static void AddPopoverHeader(VerticalStackPanel panel, string name, string note, int width)
        {
            var row = new Panel
            {
                Width = width
            };
            Label nameLabel = CreateLabel(name, 18, TextPrimary);
            nameLabel.VerticalAlignment = VerticalAlignment.Center;
            row.Widgets.Add(nameLabel);
            Label noteLabel = CreateLabel(note, 12, Gold);
            noteLabel.HorizontalAlignment = HorizontalAlignment.Right;
            noteLabel.VerticalAlignment = VerticalAlignment.Center;
            row.Widgets.Add(noteLabel);
            panel.Widgets.Add(row);
        }

        // 能否发动
        private void AddCastState(VerticalStackPanel panel, int cost, bool castable, bool current)
        {
            string state;
            Color color;
            if (current)
            {
                state = "正在使用";
                color = CurrentColor;
            }
            else if (!castable)
            {
                state = "条件不足";
                color = Hostile;
            }
            else if (this.troop.Combativity < cost)
            {
                state = "战意不足";
                color = Warning;
            }
            else
            {
                state = "可发动";
                color = Green;
            }
            panel.Widgets.Add(CreateLabel(state, 12, color));
        }

        // 右列：兵种特性，标题后面写兵种和兵种类型，下面一条一条的特性，内容多时滚动
        private Widget BuildKind()
        {
            MilitaryKind kind = this.troop.Army.Kind;
            int width = this.kindWidth;
            var column = new VerticalStackPanel
            {
                Width = width,
                Height = this.innerHeight
            };
            column.Widgets.Add(CreateTitle("兵种特性", kind.Name + " · " + kind.TypeName, width));
            column.Widgets.Add(CreateSpacer(4));
            int height = Math.Max(0, this.innerHeight - TitleHeight - 4);
            column.Widgets.Add(this.CreateViewer(BuildKindRows(kind, width - ScrollBarWidth), width, height));
            return column;
        }

        // 每条特性名称一行、说明一行，下面一条分隔线
        private static Widget BuildKindRows(MilitaryKind kind, int width)
        {
            var panel = new VerticalStackPanel();
            if (kind.Influences.Count == 0)
            {
                panel.Widgets.Add(CreateLabel("无", 13, TextSecondary));
                return panel;
            }
            foreach (Influence influence in kind.Influences)
            {
                var item = new VerticalStackPanel
                {
                    Width = width,
                    Spacing = 2,
                    Padding = new Thickness(0, KindRowPadding, 0, KindRowPadding)
                };
                item.Widgets.Add(CreateWrappedLabel(influence.Name, 14, TextPrimary, width));
                if (!string.IsNullOrWhiteSpace(influence.Description))
                {
                    item.Widgets.Add(CreateWrappedLabel(influence.Description, 12, TextSecondary, width));
                }
                panel.Widgets.Add(item);
                panel.Widgets.Add(new Panel
                {
                    Width = width,
                    Height = 1,
                    Background = new SolidBrush(DividerColor)
                });
            }
            return panel;
        }

        #endregion

        private ScrollViewer CreateViewer(Widget content, int width, int height)
        {
            var viewer = new ScrollViewer
            {
                Content = content,
                ShowHorizontalScrollBar = false,
                Width = width,
                Height = height
            };
            this.dragScroll.Add(viewer);
            return viewer;
        }

        // 深色底、1像素边框，顶上一条2像素的彩色线
        private class AccentBrush : IBrush
        {
            private const int AccentHeight = 2;
            private readonly Color fill;
            private readonly Color line;
            private readonly Color accent;

            public AccentBrush(Color fill, Color line, Color accent)
            {
                this.fill = fill;
                this.line = line;
                this.accent = accent;
            }

            public void Draw(RenderContext context, Rectangle dest, Color color)
            {
                context.FillRectangle(dest, this.fill);
                context.FillRectangle(new Rectangle(dest.X, dest.Bottom - 1, dest.Width, 1), this.line);
                context.FillRectangle(new Rectangle(dest.X, dest.Y, 1, dest.Height), this.line);
                context.FillRectangle(new Rectangle(dest.Right - 1, dest.Y, 1, dest.Height), this.line);
                context.FillRectangle(new Rectangle(dest.X, dest.Y, dest.Width, AccentHeight), this.accent);
            }
        }
    }
}

using FontStashSharp;
using GameEnums;
using GameGlobal;
using GameManager;
using GameObjects;
using GameObjects.Conditions;
using GameObjects.Influences;
using GameObjects.PersonDetail;
using Microsoft.Xna.Framework;
using Myra.Events;
using Myra.Graphics2D;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.UI;
using Platforms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using WorldOfTheThreeKingdoms.GameScreens;

namespace PersonDetailPlugin
{
    /// <summary>
    /// 人物详情。左栏固定显示头像、姓名、所在和战力，右侧页签切换总览、称号与技能、生平、档案四页。
    /// 底板和分区都用MyraSkin的底板图，与城池详情一致。左栏宽度固定，右侧随窗口大小和界面缩放变宽变窄。
    /// 称号、技能、特技、宝物的说明和旧界面一样，鼠标悬停时浮出；触屏上点一下也能看。宝物栏点击赏赐。
    /// </summary>
    internal class PersonDetailDialog
    {
        // 弹窗离窗口边缘至少留这么多
        private const int ScreenMarginX = 40;
        private const int ScreenMarginY = 30;
        private const int MinDialogWidth = 940;
        private const int MaxDialogWidth = 1060;
        private const int MinDialogHeight = 540;
        private const int MaxDialogHeight = 620;

        // 底板：内容要避开四周的花纹，上边留得少，让标题落在底板那条横线的上面。与城池详情一致
        private const int DialogPadding = 28;
        private const int DialogPaddingTop = 7;
        private const int TitleRowHeight = 34;
        private const int BodySpacing = 10;
        // 分区也用底板图，标题写在分区那条横线上面
        private const int PanelPadding = 14;
        private const int PanelTitleTop = 12;
        private const int PanelTitleHeight = 28;
        private const int PanelTitleGap = 6;

        // 左栏
        private const int LeftWidth = 300;
        private const int LeftInnerWidth = LeftWidth - 2 * PanelPadding;
        // 头部：头像在左，右边竖排姓名、字、性别生卒、性格
        private const int MaxPortraitSize = 150;
        private const int MinPortraitSize = 110;
        private const int PortraitGap = 12;
        private const int NameFontSize = 30;
        // 四个字以上的姓名用小一号的字，头像右边放得下
        private const int LongNameFontSize = 24;
        private const int HeaderTextSpacing = 10;
        private const int SealWidth = 22;
        // 战力：一张扁卡片，两个数并排
        private const int ForceHeight = 48;
        private const int ForceFontSize = 18;
        // 君主、所在等排成两列
        private const int InfoRowHeight = 22;
        private const int InfoRowSpacing = 8;
        private const int InfoLabelWidth = 40;
        private const int LeftGap = 10;
        // 性情：六个小格子，下面几行“名称 数值”
        private const int TemperTitleHeight = 26;
        private const int TemperCellHeight = 40;
        private const int TemperCellSpacing = 6;
        private const int TemperRowCount = 4;
        private const int TemperRowHeight = 22;
        private const int MinTemperRowHeight = 20;
        private const int MaxTemperRowHeight = 28;
        // 左栏除头像和性情那几行以外占的高度
        private const int LeftFixedHeight = LeftGap + ForceHeight + LeftGap + 3 * InfoRowHeight + 2 * InfoRowSpacing + LeftGap
            + TemperTitleHeight + 2 * TemperCellHeight + TemperCellSpacing + 8;

        // 右侧
        private const int ColumnGap = 24;
        // 生平页内容太长时用的小页签
        private const int SubTabHeight = 34;
        private const int SubTabGap = 16;
        private const int SectionTitleHeight = 30;
        private const int SectionGap = 22;
        private const int ScrollBarWidth = 18;

        // 总览页
        // 能力占整个宽度：左边雷达图，右边五行，每行名称、数值、Exp和经验条排在一条线上
        private const int MaxRadarSize = 190;
        private const int AbilityGap = 24;
        private const int AbilityNameWidth = 44;
        private const int AbilityValueWidth = 50;
        // 经验条和能力数值隔开，免得看成是数值的进度条
        private const int AbilityExperienceGap = 24;
        private const int AbilityExpLabelSpacing = 8;
        private const int AbilityBarHeight = 6;
        // 宝物一行7个，两行放得下12个固定栏位和多出来的一两件
        private const int TreasureSlotsPerRow = 7;
        private const int TreasureSlotSpacing = 8;
        private const int MaxTreasureSlotSize = 80;
        private const int TreasureHintGap = 10;
        private const int TreasureHintHeight = 16;
        private const int TreasureDetailPictureSize = 52;
        private const int TemperCardSpacing = 10;

        // 称号与技能页：格子大小按数据里的种类数、行列数算，这里是上限
        private const int SlotSpacing = 8;
        private const int TitleSlotsPerRow = 6;
        private const int TitleSlotHeight = 48;
        private const int MaxSkillCellWidth = 64;
        private const int MaxSkillCellHeight = 34;
        private const int StuntSlotsPerRow = 2;
        private const int MaxStuntSlotHeight = 46;

        // 说明浮层
        private const int PopoverWidth = 300;
        private const int PopoverPadding = 18;
        private const int PopoverGap = 8;

        // 档案页
        private const int ExperienceCellHeight = 60;
        private const int ExperienceColumns = 3;
        // 与经验图标原图36×36一致，不缩放时一个像素对一个像素
        private const int ExperienceIconSize = 36;
        private const int CloseTileWidth = 64;
        private const int CloseTileSpacing = 10;
        // 经验图标，文件名是经验的英文名，开了界面缩放时有@2x图就用@2x图
        private const string ExperienceIconPath = "Content/Textures/GameComponents/PersonDetail/Data/Experience/";
        // 没有专门的经验图标时，兵种经验借用部队标题栏的兵种图标，文件名是兵种类型编号：0步兵、1弓弩、2骑兵、3水军、4器械
        private const string TroopKindIconPath = "Content/Textures/GameComponents/TroopTitle/Data/TroopKind/";
        // 经验分级的门槛，取自数据里修习称号、技能的经验条件：兵种经验条件从1000到30000，其他经验从1000到10000
        private static readonly int[] TroopExperienceGrades = { 1000, 4000, 10000, 20000 };
        private static readonly int[] OtherExperienceGrades = { 1000, 3000, 5000, 8000 };
        private static readonly string[] GradeNames = { "D", "C", "B", "A", "S" };

        // 按住拖动超过这个距离才算滑动列表，否则算点击
        private const int DragThreshold = 6;

        private const string DefaultTreasurePicture = "Content/Textures/Resources/Treasure/9999.png";

        // 人物面板上显示的宝物栏位，与旧界面的顺序一致。不在这些种类里的宝物排在后面
        private static readonly int[] TreasureGroups = { 4000, 4100, 3214, 1100, 4700, 3200, 1200, 1300, 1700, 300, 2900, 2001 };
        // 这几种影响本身不加数值，而是限定后面的效果何时生效：仅君主、仅部队主将、满足条件、限定兵种类型、限定兵种。
        // 说明里把它们写在“效果”标题后面
        private static readonly int[] RestrictionInfluenceKinds = { 280, 281, 285, 290, 300 };
        private static readonly Regex InGameLine = new Regex(@"^(\d+年\d+月(\d+日)?)[：:，,]\s*(.*)$");

        // 配色沿用MyraSkin，与城池详情一致；性格印章用朱红点缀
        private static readonly Color CardColor = MyraSkin.ListRowColor;
        private static readonly Color CardSelectedColor = MyraSkin.ListRowSelectedColor;
        private static readonly Color HoverColor = MyraSkin.ListRowOverColor;
        private static readonly Color LineColor = new Color(90, 110, 120);
        private static readonly Color DividerColor = new Color(58, 76, 86);
        private static readonly Color BarTrackColor = new Color(16, 24, 30);
        private static readonly Color TextPrimary = MyraSkin.TextPrimary;
        private static readonly Color TextSecondary = MyraSkin.TextSecondary;
        private static readonly Color TextMuted = MyraSkin.TextDisabled;
        private static readonly Color Gold = MyraSkin.TextHighlight;
        private static readonly Color GoldLight = MyraSkin.TextHighlight;
        private static readonly Color GoldDim = new Color(0x8A, 0x74, 0x48);
        private static readonly Color Red = new Color(184, 67, 47);
        private static readonly Color Green = MyraSkin.TextFriendly;
        private static readonly Color Warning = MyraSkin.TextWarning;
        private static readonly Color Hostile = MyraSkin.TextHostile;
        // 称号等级分档的颜色，从低到高：普通、绿、蓝、紫、橙
        private static readonly Color TierGreen = new Color(0x8F, 0xBF, 0x6A);
        private static readonly Color TierBlue = new Color(0x5E, 0x9C, 0xC8);
        private static readonly Color TierViolet = new Color(0xB4, 0x8C, 0xD2);
        private static readonly Color TierOrange = new Color(0xE0, 0x9A, 0x50);
        // 经验分级的颜色，从D到S：浅灰、草绿、青蓝、雅紫、金橙
        private static readonly Color[] GradeColors =
        {
            new Color(0x9B, 0x9D, 0x97),
            new Color(0x6A, 0x9A, 0x62),
            new Color(0x62, 0x99, 0xB8),
            new Color(0xA4, 0x7A, 0xBB),
            new Color(0xD9, 0xA0, 0x40)
        };
        private static readonly Color PopoverColor = new Color(30, 44, 54);
        private static readonly IBrush Transparent = new SolidBrush(Color.Transparent);

        private readonly Person person;
        private readonly bool playVoice;
        private readonly List<Skill> skills;
        private readonly List<ScrollViewer> dragViewers = new List<ScrollViewer>();
        private readonly List<Tab> tabs = new List<Tab>();

        // 随窗口大小算出来的尺寸
        private readonly int dialogWidth;
        private readonly int dialogHeight;
        private readonly int bodyHeight;
        private readonly int rightWidth;
        private readonly int rightInnerWidth;
        private readonly int contentHeight;
        private readonly int portraitSize;
        private readonly int temperRowHeight;

        private VerticalStackPanel treasureSlotsPanel;
        private RadarChart radarChart;
        private Label treasureCountLabel;

        // 最外层的遮罩和弹窗本体，说明浮层按它们定位
        private Panel mask;
        private Widget dialog;
        private Panel popover;
        private VerticalStackPanel popoverContent;
        private Widget popoverAnchor;

        // 传给MyraUI的根控件，用来判断本窗口是否在最上层
        private Widget root;
        private ScrollViewer dragViewer;
        private int? dragLastY;
        private int dragDistance;

        private PersonDetailDialog(Person person, bool playVoice)
        {
            this.person = person;
            this.playVoice = playVoice;
            this.skills = Session.Current.Scenario.GameCommonData.AllSkills.Values.ToList();

            Point size = MyraUI.LayoutSize;
            this.dialogWidth = MathHelper.Clamp(size.X - 2 * ScreenMarginX, MinDialogWidth, MaxDialogWidth);
            this.dialogHeight = MathHelper.Clamp(size.Y - 2 * ScreenMarginY, MinDialogHeight, MaxDialogHeight);
            this.bodyHeight = this.dialogHeight - DialogPaddingTop - TitleRowHeight - BodySpacing - DialogPadding;
            this.rightWidth = this.dialogWidth - 2 * DialogPadding - LeftWidth - BodySpacing;
            this.rightInnerWidth = this.rightWidth - 2 * PanelPadding;
            this.contentHeight = this.bodyHeight - PanelTitleTop - PanelTitleHeight - PanelTitleGap - PanelPadding;
            // 左栏先保证性情每行有默认高度，剩下的给头像；窗口矮时头像先缩，再压性情的行高
            int leftHeight = this.bodyHeight - 2 * PanelPadding - LeftFixedHeight;
            this.portraitSize = MathHelper.Clamp(leftHeight - TemperRowCount * TemperRowHeight, MinPortraitSize, MaxPortraitSize);
            this.temperRowHeight = MathHelper.Clamp((leftHeight - this.portraitSize) / TemperRowCount, MinTemperRowHeight, MaxTemperRowHeight);
        }

        /// <summary>
        /// 显示人物详情
        /// </summary>
        /// <param name="person">要查看的人物</param>
        /// <param name="playVoice">打开时是否播放人物语音，取自旧界面配置里的PersonSound开关</param>
        public static void Show(Person person, bool playVoice)
        {
            if (person == null)
            {
                return;
            }
            var dialog = new PersonDetailDialog(person, playVoice);
            // 面板显示期间占住一个未完成工作，地图和工具栏就不会响应鼠标键盘
            Session.MainGame.mainGameScreen.PushUndoneWork(new UndoneWorkItem(UndoneWorkKind.SubDialog, DialogKind.PersonDetail));
            Desktop desktop = MyraUI.Desktop;
            desktop.TouchDown += dialog.Desktop_TouchDown;
            desktop.TouchMoved += dialog.Desktop_TouchMoved;
            desktop.TouchUp += dialog.Desktop_TouchUp;
            dialog.root = dialog.Build();
            MyraUI.Show(dialog.root, dialog.Close);
            dialog.PlayVoice();
        }

        private void Close()
        {
            Desktop desktop = MyraUI.Desktop;
            desktop.TouchDown -= this.Desktop_TouchDown;
            desktop.TouchMoved -= this.Desktop_TouchMoved;
            desktop.TouchUp -= this.Desktop_TouchUp;
            this.radarChart?.Dispose();
            MyraUI.Close(() =>
            {
                // 等鼠标键松开之后再把输入还给地图，关闭面板的这次点击才不会落到地图上
                if (Session.MainGame.mainGameScreen.PopUndoneWork().Kind != UndoneWorkKind.SubDialog)
                {
                    throw new Exception("The UndoneWork is not a SubDialog.");
                }
            });
        }

        // 与旧界面一致：大地图菜单展开时不播放
        private void PlayVoice()
        {
            if (!this.playVoice || Session.LargeContextMenu)
            {
                return;
            }
            string sound = Platform.Current.GetPersonVioce(this.person, "");
            if (sound != null && sound.EndsWith(".wav"))
            {
                Session.MainGame.mainGameScreen.PlayNormalSound(sound);
            }
        }

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
            this.dialog = dialog;
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
            this.mask.Widgets.Add(this.BuildPopover());
            this.SelectTab(this.tabs[0]);
            return this.mask;
        }

        #region 左栏

        // 标题：居中写“人物详情”，右上角关闭按钮。姓名放在左栏
        private Widget BuildTitle()
        {
            var row = new Panel
            {
                Height = TitleRowHeight
            };
            Label title = CreateLabel("人物详情", 22, MyraSkin.TextTitle);
            title.HorizontalAlignment = HorizontalAlignment.Center;
            title.VerticalAlignment = VerticalAlignment.Center;
            row.Widgets.Add(title);
            Button close = CreateCloseButton(this.Close);
            close.VerticalAlignment = VerticalAlignment.Center;
            row.Widgets.Add(close);
            return row;
        }

        // 左栏：头部是头像和姓名简介，下面战力、君主所在等两列信息，最下面性情。
        // 不要分区标题，底板用不带标题横线的
        private Widget BuildLeft()
        {
            Person p = this.person;
            var column = new VerticalStackPanel
            {
                Width = LeftWidth,
                Padding = new Thickness(PanelPadding),
                Background = MyraSkin.PlainPanelBackground
            };
            column.Widgets.Add(this.BuildHeader());
            column.Widgets.Add(CreateSpacer(LeftGap));
            column.Widgets.Add(this.BuildForces());
            column.Widgets.Add(CreateSpacer(LeftGap));

            int cellWidth = LeftInnerWidth / 2;
            Person leader = p.BelongedFaction?.Leader;
            var info = new VerticalStackPanel
            {
                Spacing = InfoRowSpacing
            };
            var row = new HorizontalStackPanel();
            row.Widgets.Add(CreateInfoRow("君主", leader == null ? "无" : leader.Name, leader == null ? TextSecondary : TextPrimary, cellWidth));
            row.Widgets.Add(CreateInfoRow("所在", p.Location, TextPrimary, cellWidth));
            info.Widgets.Add(row);
            row = new HorizontalStackPanel();
            row.Widgets.Add(CreateInfoRow("工作", p.WorkKindString, TextPrimary, cellWidth));
            row.Widgets.Add(CreateInfoRow("忠诚", p.BelongedFaction == null ? "----" : p.Loyalty.ToString(), TextPrimary, cellWidth));
            info.Widgets.Add(row);
            info.Widgets.Add(this.CreateStatusRow());
            column.Widgets.Add(info);
            column.Widgets.Add(CreateSpacer(LeftGap));
            column.Widgets.Add(this.BuildTemper());
            return column;
        }

        // 头部：头像在左，右边竖排姓名、字、性别生卒、性格，整体和头像竖直居中
        private Widget BuildHeader()
        {
            Person p = this.person;
            var header = new HorizontalStackPanel
            {
                Spacing = PortraitGap,
                Height = this.portraitSize
            };
            header.Widgets.Add(this.BuildPortrait());
            // 文字这一列给定宽度，宽度不定时Myra会把超宽的字居中摆放，左边被裁掉
            int textWidth = LeftInnerWidth - PortraitGap - this.portraitSize;
            var text = new VerticalStackPanel
            {
                Width = textWidth,
                Spacing = HeaderTextSpacing,
                VerticalAlignment = VerticalAlignment.Center
            };
            int nameSize = (p.Name ?? "").Length >= 4 ? LongNameFontSize : NameFontSize;
            text.Widgets.Add(CreateWrappedLabel(p.Name, nameSize, TextPrimary, textWidth));
            if (!string.IsNullOrEmpty(p.CalledName))
            {
                text.Widgets.Add(CreateLabel("字 " + p.CalledName, 15, MyraSkin.TextTitle));
            }
            // 隶变字库里没有“–”“—”这类长横线，会画成空白，用普通减号
            text.Widgets.Add(CreateLabel(p.SexString + " · " + p.YearBorn + "-" + p.YearDead, 13, TextSecondary));
            text.Widgets.Add(CreateLabel("性格 " + p.CharacterString + (p.Alive ? "" : " · 已故"), 13, TextSecondary));
            header.Widgets.Add(text);
            return header;
        }

        // 战力：一张扁卡片，中间一条竖线，左右各一个数
        private Widget BuildForces()
        {
            var card = new HorizontalStackPanel
            {
                Height = ForceHeight,
                Background = new FrameBrush(CardColor, LineColor, false)
            };
            int half = (LeftInnerWidth - 1) / 2;
            card.Widgets.Add(CreateForceCell("主将战力", this.person.FightingForce, Gold, half));
            card.Widgets.Add(new Panel
            {
                Width = 1,
                Margin = new Thickness(0, 8, 0, 8),
                Background = new SolidBrush(LineColor)
            });
            card.Widgets.Add(CreateForceCell("副将战力", this.person.SubFightingForce, TextPrimary, half));
            return card;
        }

        private static Widget CreateForceCell(string title, int value, Color valueColor, int width)
        {
            var cell = new Panel
            {
                Width = width,
                Padding = new Thickness(4, 0, 4, 0)
            };
            cell.Widgets.Add(CreateCellContent(title, value.ToString("#,0"), ForceFontSize, valueColor));
            return cell;
        }

        // 性情：标题、六个小格子，下面几行虚线分隔的其他数据
        private Widget BuildTemper()
        {
            Person p = this.person;
            var column = new VerticalStackPanel();
            var title = new Panel
            {
                Height = TemperTitleHeight
            };
            Label label = CreateLabel("性情", 16, MyraSkin.TextTitle);
            label.VerticalAlignment = VerticalAlignment.Center;
            title.Widgets.Add(label);
            column.Widgets.Add(title);

            int cellWidth = (LeftInnerWidth - 2 * TemperCellSpacing) / 3;
            var cells = new[]
            {
                new KeyValuePair<string, int>("勇猛", p.Braveness),
                new KeyValuePair<string, int>("冷静", p.Calmness),
                new KeyValuePair<string, int>("义理", p.PersonalLoyalty),
                new KeyValuePair<string, int>("野心", p.Ambition),
                new KeyValuePair<string, int>("相性", p.Ideal),
                new KeyValuePair<string, int>("名声", p.Reputation),
            };
            var grid = new VerticalStackPanel
            {
                Spacing = TemperCellSpacing
            };
            for (int i = 0; i < cells.Length; i += 3)
            {
                var row = new HorizontalStackPanel
                {
                    Spacing = TemperCellSpacing
                };
                for (int j = i; j < i + 3 && j < cells.Length; j++)
                {
                    row.Widgets.Add(CreateTemperCell(cells[j].Key, cells[j].Value.ToString("#,0"), cellWidth));
                }
                grid.Widgets.Add(row);
            }
            column.Widgets.Add(grid);
            column.Widgets.Add(CreateSpacer(8));

            IdealTendencyKind tendency = p.IdealTendency;
            column.Widgets.Add(this.CreateTemperRow("出仕相性考虑", tendency == null ? "----" : tendency.Name, tendency == null ? null : " · " + tendency.Offset));
            column.Widgets.Add(this.CreateTemperRow("是否会独立", p.LeaderPossibility ? "是" : "否", null));
            column.Widgets.Add(this.CreateTemperRow("功绩", p.OfficerMerit.ToString("#,0"), null));
            column.Widgets.Add(this.CreateTemperRow("善名 / 疲劳", p.Karma + " / " + p.Tiredness, null));
            return column;
        }

        // 格子里的小字名称在上、数值在下，两行都居中，整体在格子中间
        private static Widget CreateCellContent(string title, string value, int valueFontSize, Color valueColor)
        {
            var content = new VerticalStackPanel
            {
                Spacing = 2,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            Label label = CreateLabel(title, 12, TextSecondary);
            label.HorizontalAlignment = HorizontalAlignment.Center;
            content.Widgets.Add(label);
            Label number = CreateLabel(value, valueFontSize, valueColor);
            number.HorizontalAlignment = HorizontalAlignment.Center;
            content.Widgets.Add(number);
            return content;
        }

        private static Widget CreateTemperCell(string title, string value, int width)
        {
            var cell = new Panel
            {
                Width = width,
                Height = TemperCellHeight,
                Padding = new Thickness(4, 0, 4, 0),
                Background = new FrameBrush(CardColor, LineColor, false)
            };
            cell.Widgets.Add(CreateCellContent(title, value, 16, TextPrimary));
            return cell;
        }

        private Widget CreateTemperRow(string title, string value, string note)
        {
            var row = new Panel
            {
                Height = this.temperRowHeight
            };
            Label label = CreateLabel(title, 13, TextSecondary);
            label.VerticalAlignment = VerticalAlignment.Center;
            row.Widgets.Add(label);
            var values = new HorizontalStackPanel
            {
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center
            };
            values.Widgets.Add(CreateLabel(value, 13, TextPrimary));
            if (note != null)
            {
                values.Widgets.Add(CreateLabel(note, 13, TextSecondary));
            }
            row.Widgets.Add(values);
            row.Widgets.Add(new Panel
            {
                Height = 1,
                VerticalAlignment = VerticalAlignment.Bottom,
                Background = new DashLineBrush(LineColor)
            });
            return row;
        }

        private Widget BuildPortrait()
        {
            var frame = new Panel
            {
                Width = this.portraitSize,
                Height = this.portraitSize,
                HorizontalAlignment = HorizontalAlignment.Left,
                Background = new SolidBrush(CardColor),
                Border = new SolidBrush(LineColor),
                BorderThickness = new Thickness(1)
            };
            frame.Widgets.Add(new Image
            {
                Renderable = new DiplomacyDialog.PortraitImage(this.person, PortraitSize.Medium, this.portraitSize - 2, this.portraitSize - 2)
            });

            // 性格印章：右上角一个朱红小角标，字竖排
            string character = this.person.CharacterString ?? "";
            if (character.Length > 0)
            {
                var seal = new Panel
                {
                    Width = SealWidth,
                    Height = 8 + 15 * character.Length,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Top,
                    Background = new SolidBrush(Red)
                };
                Label text = CreateLabel(string.Join("\n", character.ToCharArray()), 13, TextPrimary);
                text.HorizontalAlignment = HorizontalAlignment.Center;
                text.VerticalAlignment = VerticalAlignment.Center;
                text.TextAlign = FontStashSharp.RichText.TextHorizontalAlignment.Center;
                seal.Widgets.Add(text);
                var sealHolder = new Panel
                {
                    Padding = new Thickness(0, 6, 6, 0)
                };
                sealHolder.Widgets.Add(seal);
                frame.Widgets.Add(sealHolder);
            }
            return frame;
        }

        private static Widget CreateInfoRow(string title, string value, Color valueColor, int width)
        {
            var row = new HorizontalStackPanel
            {
                Width = width,
                Height = InfoRowHeight
            };
            Label label = CreateLabel(title, 14, TextSecondary);
            label.Width = InfoLabelWidth;
            label.VerticalAlignment = VerticalAlignment.Center;
            row.Widgets.Add(label);
            Label text = CreateLabel(value, 14, valueColor);
            text.VerticalAlignment = VerticalAlignment.Center;
            row.Widgets.Add(text);
            return row;
        }

        // 状态：伤病做成带底色的标签，健康绿、轻伤黄、更重的红，临时状态用红色标签跟在后面。
        // 标签左边与上面几行的数值对齐
        private Widget CreateStatusRow()
        {
            string injury = this.person.InjuryString;
            Color color = injury == "健康" ? Green : injury == "轻伤" ? Warning : Hostile;
            var row = new HorizontalStackPanel
            {
                Height = InfoRowHeight
            };
            Label label = CreateLabel("状态", 14, TextSecondary);
            label.Width = InfoLabelWidth;
            label.VerticalAlignment = VerticalAlignment.Center;
            row.Widgets.Add(label);
            var tags = new HorizontalStackPanel
            {
                Spacing = 6,
                VerticalAlignment = VerticalAlignment.Center
            };
            tags.Widgets.Add(CreateTag(injury, color));
            foreach (string effect in this.person.GetStatusEffects())
            {
                tags.Widgets.Add(CreateTag(effect, Hostile));
            }
            row.Widgets.Add(tags);
            return row;
        }

        // 小标签：同色细边框，底色是同色的淡色。游戏按预乘透明度混合，所以淡色用颜色乘系数得到
        private static Widget CreateTag(string text, Color color)
        {
            var tag = new Panel
            {
                Padding = new Thickness(8, 1, 8, 1),
                Background = new FrameBrush(color * 0.16f, color, false)
            };
            tag.Widgets.Add(CreateLabel(text, 13, color));
            return tag;
        }

        #endregion

        #region 右侧和页签

        private Widget BuildRight()
        {
            var right = new VerticalStackPanel
            {
                Width = this.rightWidth,
                Padding = new Thickness(PanelPadding, PanelTitleTop, PanelPadding, PanelPadding),
                Background = MyraSkin.PanelBackground
            };
            this.tabs.Add(new Tab("总览", this.BuildOverviewPage()));
            this.tabs.Add(new Tab("称号与技能", this.BuildAbilityPage()));
            this.tabs.Add(new Tab("生平", this.BuildLifePage()));
            this.tabs.Add(new Tab("档案", this.BuildArchivePage()));
            right.Widgets.Add(this.BuildTabBar());
            right.Widgets.Add(CreateSpacer(PanelTitleGap));

            var pages = new Panel
            {
                Height = this.contentHeight
            };
            foreach (Tab tab in this.tabs)
            {
                pages.Widgets.Add(tab.Page);
            }
            right.Widgets.Add(pages);
            return right;
        }

        // 页签写在右侧分区的标题位置，落在底板横线上面，与城池详情的页签一样
        private Widget BuildTabBar()
        {
            var bar = new Panel
            {
                Height = PanelTitleHeight
            };
            var row = new HorizontalStackPanel
            {
                Spacing = 4,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            foreach (Tab tab in this.tabs)
            {
                row.Widgets.Add(CreateTabButton(tab, 17, PanelTitleHeight, () => this.SelectTab(tab)));
            }
            bar.Widgets.Add(row);
            return bar;
        }

        // 页签按钮：顶部的大页签和生平页里的小页签共用
        private static Button CreateTabButton(Tab tab, int fontSize, int height, Action onClick)
        {
            SpriteFontBase font = TextManager.GetFont(fontSize);
            int width = (int)Math.Ceiling(font.MeasureString(tab.Title).X) + 28;
            tab.Label = CreateLabel(tab.Title, fontSize, TextSecondary);
            tab.Label.HorizontalAlignment = HorizontalAlignment.Center;
            tab.Label.VerticalAlignment = VerticalAlignment.Center;
            tab.Line = new Panel
            {
                Height = 2,
                Margin = new Thickness(6, 0, 6, 0),
                VerticalAlignment = VerticalAlignment.Bottom
            };
            var content = new Panel();
            content.Widgets.Add(tab.Label);
            content.Widgets.Add(tab.Line);
            tab.Button = new Button
            {
                Content = content,
                Width = width,
                Height = height,
                Background = Transparent,
                FocusedBackground = Transparent,
                PressedBackground = Transparent,
                OverBackground = Transparent
            };
            tab.Button.MouseEntered += (sender, args) =>
            {
                if (!tab.Page.Visible)
                {
                    tab.Label.TextColor = TextPrimary;
                }
            };
            tab.Button.MouseLeft += (sender, args) =>
            {
                if (!tab.Page.Visible)
                {
                    tab.Label.TextColor = TextSecondary;
                }
            };
            tab.Button.Click += (sender, args) => onClick();
            return tab.Button;
        }

        private void SelectTab(Tab selected)
        {
            this.HidePopover();
            ShowTab(this.tabs, selected);
        }

        // 选中的金字加金线，只显示它对应的页
        private static void ShowTab(List<Tab> tabs, Tab selected)
        {
            foreach (Tab tab in tabs)
            {
                bool isSelected = tab == selected;
                tab.Label.TextColor = isSelected ? MyraSkin.TextTitle : TextSecondary;
                tab.Line.Background = new SolidBrush(isSelected ? Gold : Color.Transparent);
                tab.Page.Visible = isSelected;
            }
        }

        // 关闭按钮只画一个“×”，鼠标移上去变成金色
        private static Button CreateCloseButton(Action onClick)
        {
            var label = new Label
            {
                Text = "×",
                Font = TextManager.GetFont(28),
                TextColor = TextSecondary,
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
            close.MouseEntered += (sender, args) => label.TextColor = Gold;
            close.MouseLeft += (sender, args) => label.TextColor = TextSecondary;
            close.Click += (sender, args) => onClick();
            return close;
        }

        // 生平和档案两列等宽
        private int HalfColumnWidth => (this.rightInnerWidth - ColumnGap) / 2;

        #endregion

        #region 总览页

        // 总览：上面能力，下面宝物，都占整个宽度。宝物悬停看说明，点击赏赐
        private Widget BuildOverviewPage()
        {
            int width = this.rightInnerWidth;
            var page = new VerticalStackPanel
            {
                Width = width,
                Height = this.contentHeight
            };

            // 宝物格子先按宽度算，行数多时再按高度缩小，保证不用滚动
            int slotCount = this.GetTreasureSlots().Count;
            int slotRows = Math.Max(2, (slotCount + TreasureSlotsPerRow - 1) / TreasureSlotsPerRow);
            int slotSize = Math.Min(MaxTreasureSlotSize, (width - (TreasureSlotsPerRow - 1) * TreasureSlotSpacing) / TreasureSlotsPerRow);
            int treasureFixed = SectionGap + SectionTitleHeight + TreasureHintGap + TreasureHintHeight;
            int abilityHeight = this.contentHeight - SectionTitleHeight - treasureFixed - (slotRows * slotSize + (slotRows - 1) * TreasureSlotSpacing);
            const int minAbilityHeight = 5 * 34;
            if (abilityHeight < minAbilityHeight)
            {
                abilityHeight = minAbilityHeight;
                int slotsHeight = this.contentHeight - SectionTitleHeight - treasureFixed - abilityHeight;
                slotSize = Math.Max(40, (slotsHeight - (slotRows - 1) * TreasureSlotSpacing) / slotRows);
            }

            page.Widgets.Add(CreateSectionTitle("能力", null, out _));
            page.Widgets.Add(this.BuildAbilities(width, abilityHeight));
            page.Widgets.Add(CreateSpacer(SectionGap));
            page.Widgets.Add(CreateSectionTitle("宝物", "", out this.treasureCountLabel));
            this.treasureSlotsPanel = new VerticalStackPanel
            {
                Spacing = TreasureSlotSpacing
            };
            page.Widgets.Add(this.treasureSlotsPanel);
            this.RefreshTreasures(slotSize);
            page.Widgets.Add(CreateSpacer(TreasureHintGap));
            Label hint = CreateLabel(this.CanAwardTreasure ? "悬停查看效果 · 点击栏位赏赐宝物" : "悬停查看效果", 12, TextMuted);
            hint.Height = TreasureHintHeight;
            page.Widgets.Add(hint);
            return page;
        }

        // 能力：左边五维雷达图，右边每项一行
        private Widget BuildAbilities(int width, int height)
        {
            Person p = this.person;
            int radarSize = Math.Min(MaxRadarSize, height);
            var row = new HorizontalStackPanel
            {
                Spacing = AbilityGap,
                Height = height
            };
            this.radarChart = new RadarChart(
                new[] { "武勇", "统率", "智谋", "政治", "魅力" },
                new[] { p.Strength, p.Command, p.Intelligence, p.Politics, p.Glamour })
            {
                Width = radarSize,
                Height = radarSize,
                VerticalAlignment = VerticalAlignment.Center
            };
            row.Widgets.Add(this.radarChart);
            int rowWidth = width - radarSize - AbilityGap;
            int rowHeight = height / 5;
            var rows = new VerticalStackPanel
            {
                VerticalAlignment = VerticalAlignment.Center
            };
            rows.Widgets.Add(CreateAbilityRow("武勇", p.Strength, p.StrengthExperience, p.StrengthLevelUpNeedExp, rowWidth, rowHeight));
            rows.Widgets.Add(CreateAbilityRow("统率", p.Command, p.CommandExperience, p.CommandLevelUpNeedExp, rowWidth, rowHeight));
            rows.Widgets.Add(CreateAbilityRow("智谋", p.Intelligence, p.IntelligenceExperience, p.IntelligenceLevelUpNeedExp, rowWidth, rowHeight));
            rows.Widgets.Add(CreateAbilityRow("政治", p.Politics, p.PoliticsExperience, p.PoliticsLevelUpNeedExp, rowWidth, rowHeight));
            rows.Widgets.Add(CreateAbilityRow("魅力", p.Glamour, p.GlamourExperience, p.GlamourLevelUpNeedExp, rowWidth, rowHeight));
            row.Widgets.Add(rows);
            return row;
        }

        // 一行：名称、数值，隔开一段后是“Exp”和升级经验条，全部在这一行里竖直居中，排在同一条水平线上。
        // 升级所需经验是升到下一级要累计到的经验，进度条按当前经验占它的比例画，
        // 颜色按比例每20%一档，与经验分级的D到S同色。差多少升级鼠标悬停时显示
        private Widget CreateAbilityRow(string title, int value, int experience, int needExperience, int width, int height)
        {
            var row = new HorizontalStackPanel
            {
                Width = width,
                Height = height
            };
            Label label = CreateLabel(title, 15, TextSecondary);
            label.Width = AbilityNameWidth;
            label.VerticalAlignment = VerticalAlignment.Center;
            row.Widgets.Add(label);
            Label number = CreateLabel(value.ToString(), 20, TextPrimary);
            number.Width = AbilityValueWidth;
            number.VerticalAlignment = VerticalAlignment.Center;
            row.Widgets.Add(number);
            // 带透明底色才接得住鼠标，悬停范围是整行高度，不只是细细的进度条
            var experienceArea = new HorizontalStackPanel
            {
                Height = height,
                Spacing = AbilityExpLabelSpacing,
                Margin = new Thickness(AbilityExperienceGap, 0, 0, 0),
                Background = Transparent
            };
            Label tag = CreateLabel("Exp", 12, TextMuted);
            tag.VerticalAlignment = VerticalAlignment.Center;
            experienceArea.Widgets.Add(tag);
            int barWidth = Math.Max(60, width - AbilityNameWidth - AbilityValueWidth - AbilityExperienceGap - AbilityExpLabelSpacing - (int)Math.Ceiling(tag.Font.MeasureString("Exp").X));
            float ratio = needExperience <= 0 ? 0f : MathHelper.Clamp((float)experience / needExperience, 0f, 1f);
            // 20%以内是D，40%以内是C，依此类推
            int grade = MathHelper.Clamp((int)Math.Ceiling(ratio * GradeNames.Length) - 1, 0, GradeNames.Length - 1);
            Widget bar = CreateBar(barWidth, AbilityBarHeight, experience, needExperience, GetGradeColor(grade));
            bar.VerticalAlignment = VerticalAlignment.Center;
            experienceArea.Widgets.Add(bar);
            this.AttachPopover(experienceArea, (panel, popoverWidth) => ShowAbilityExperience(panel, title, experience, needExperience, GetGradeColor(grade)));
            row.Widgets.Add(experienceArea);
            return row;
        }

        // 悬停说明：当前经验和升级所需，还差多少升级
        private static void ShowAbilityExperience(VerticalStackPanel panel, string title, int experience, int needExperience, Color color)
        {
            panel.Widgets.Add(CreateLabel(title + "经验", 12, TextSecondary));
            var numbers = new HorizontalStackPanel
            {
                Spacing = 6
            };
            numbers.Widgets.Add(CreateLabel(experience.ToString("#,0"), 22, color));
            Label need = CreateLabel("/ " + needExperience.ToString("#,0"), 16, TextSecondary);
            need.VerticalAlignment = VerticalAlignment.Bottom;
            numbers.Widgets.Add(need);
            panel.Widgets.Add(numbers);
            panel.Widgets.Add(CreateLabel("距升级还差 " + Math.Max(0, needExperience - experience).ToString("#,0"), 13, Gold));
        }

        // 12个栏位全部列出，没有宝物的留空；栏位以外的宝物排在后面
        private List<KeyValuePair<int, Treasure>> GetTreasureSlots()
        {
            List<Treasure> treasures = this.person.AllEffectiveTreasures;
            var slots = new List<KeyValuePair<int, Treasure>>();
            foreach (int group in TreasureGroups)
            {
                slots.Add(new KeyValuePair<int, Treasure>(group, treasures.FirstOrDefault(x => x.TreasureGroup == group)));
            }
            foreach (Treasure treasure in treasures.Where(x => !TreasureGroups.Contains(x.TreasureGroup)))
            {
                slots.Add(new KeyValuePair<int, Treasure>(treasure.TreasureGroup, treasure));
            }
            return slots;
        }

        // 重新排宝物栏位，赏赐之后也调用这里
        private void RefreshTreasures(int slotSize)
        {
            this.treasureSlotsPanel.Widgets.Clear();
            List<KeyValuePair<int, Treasure>> slots = this.GetTreasureSlots();
            this.treasureCountLabel.Text = slots.Count(x => x.Value != null) + " / " + slots.Count;

            HorizontalStackPanel row = null;
            foreach (var slot in slots)
            {
                if (row == null || row.Widgets.Count >= TreasureSlotsPerRow)
                {
                    row = new HorizontalStackPanel
                    {
                        Spacing = TreasureSlotSpacing
                    };
                    this.treasureSlotsPanel.Widgets.Add(row);
                }
                row.Widgets.Add(this.CreateTreasureSlot(slot.Key, slot.Value, slotSize));
            }
        }

        // 有宝物的金色细框放图片，空位虚线框。悬停显示说明，可以赏赐时点击打开赏赐窗口
        private Button CreateTreasureSlot(int group, Treasure treasure, int size)
        {
            Widget content;
            if (treasure != null)
            {
                int pictureSize = size - 10;
                content = new Image
                {
                    Renderable = new ArchitectureDetailDialog.TextureImage(GetTreasurePicture(treasure), pictureSize, pictureSize),
                    Width = pictureSize,
                    Height = pictureSize,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };
            }
            else
            {
                Label empty = CreateLabel("空", 12, TextMuted);
                empty.HorizontalAlignment = HorizontalAlignment.Center;
                empty.VerticalAlignment = VerticalAlignment.Center;
                content = empty;
            }
            IBrush normal = treasure != null ? new FrameBrush(CardColor, GoldDim, false) : new FrameBrush(Color.Transparent, LineColor, true);
            Button button = CreateFlatButton(content, size, size, normal);
            this.AttachPopover(button, (panel, textWidth) => this.ShowTreasure(panel, group, textWidth));
            if (this.CanAwardTreasure)
            {
                button.Click += (sender, args) =>
                {
                    this.HidePopover();
                    this.AwardTreasure(group, size);
                };
            }
            return button;
        }

        private static string GetTreasurePicture(Treasure treasure)
        {
            string name = treasure.Picture?.Name;
            return !string.IsNullOrEmpty(name) && Platform.Current.FileExists(name) ? name : DefaultTreasurePicture;
        }

        private void ShowTreasure(VerticalStackPanel panel, int group, int width)
        {
            Treasure treasure = this.person.AllEffectiveTreasures.FirstOrDefault(x => x.TreasureGroup == group);
            if (treasure == null)
            {
                panel.Widgets.Add(CreateLabel("此栏尚无宝物", 14, TextSecondary));
            }
            else
            {
                var header = new HorizontalStackPanel
                {
                    Spacing = 12
                };
                header.Widgets.Add(new Image
                {
                    Renderable = new ArchitectureDetailDialog.TextureImage(GetTreasurePicture(treasure), TreasureDetailPictureSize, TreasureDetailPictureSize),
                    Width = TreasureDetailPictureSize,
                    Height = TreasureDetailPictureSize,
                    Border = new SolidBrush(GoldDim),
                    BorderThickness = new Thickness(1)
                });
                var names = new VerticalStackPanel
                {
                    Spacing = 6,
                    VerticalAlignment = VerticalAlignment.Center
                };
                names.Widgets.Add(CreateWrappedLabel(treasure.Name, 18, TextPrimary, width - TreasureDetailPictureSize - 12));
                names.Widgets.Add(CreateLabel("价值 " + treasure.Worth, 13, Gold));
                header.Widgets.Add(names);
                panel.Widgets.Add(header);
                if (!string.IsNullOrEmpty(treasure.Description))
                {
                    panel.Widgets.Add(CreateWrappedLabel(treasure.Description, 13, TextSecondary, width));
                }
                AddInfluences(panel, treasure.Influences.Values, width);
            }
            if (this.CanAwardTreasure)
            {
                panel.Widgets.Add(CreateDetailHeader("点击栏位赏赐宝物", width));
            }
        }

        // 与城池菜单里的“授予宝物”一致：只能赏赐给玩家势力中身在己方城池的非君主武将
        private bool CanAwardTreasure
        {
            get
            {
                Faction faction = this.person.BelongedFaction;
                return faction != null && faction.Leader != null && Session.Current.Scenario.IsCurrentPlayer(faction)
                    && faction.GetPersonsInArchitectureExceptLeader().Contains(this.person);
            }
        }

        private void AwardTreasure(int group, int slotSize)
        {
            Person p = this.person;
            AwardTreasureDialog.Show(p, group, treasure =>
            {
                // 只赏赐不收回：武将原有的同类宝物仍归他，按价值高低决定哪件生效
                p.BelongedFaction.Leader.LoseTreasure(treasure);
                p.AwardedTreasure(treasure);
                this.RefreshTreasures(slotSize);
            });
        }

        #endregion

        #region 称号与技能页

        // 称号与技能：称号、技能、特技都有上限，全部格位摆出来，有的亮、没有的暗，版面不随人物变化，不用滚动。
        // 上面一排称号，下面左边技能格子、右边特技。鼠标停在某一项上时，旁边浮出它的说明
        private Widget BuildAbilityPage()
        {
            var page = new VerticalStackPanel
            {
                Height = this.contentHeight
            };
            CommonData common = Session.Current.Scenario.GameCommonData;

            // 称号：只列出已有的，等级高的在前，同级按种类排。一行放得下时只占一行，省下的高度给技能和特技
            List<Title> titles = this.person.Titles.Where(x => x != null).OrderByDescending(x => x.Level).ThenBy(x => x.Kind?.ID ?? 0).ToList();
            page.Widgets.Add(CreateSectionTitle("称号", titles.Count.ToString(), out _));
            int titleRows = Math.Max(1, (titles.Count + TitleSlotsPerRow - 1) / TitleSlotsPerRow);
            int titleWidth = (this.rightInnerWidth - (TitleSlotsPerRow - 1) * SlotSpacing) / TitleSlotsPerRow;
            page.Widgets.Add(this.BuildTitleSlots(titles, titleWidth));

            // 下面剩下的高度给技能格子和特技
            int lowerHeight = this.contentHeight - SectionTitleHeight - (titleRows * TitleSlotHeight + (titleRows - 1) * SlotSpacing) - SectionGap - SectionTitleHeight;
            page.Widgets.Add(CreateSpacer(SectionGap));

            int maxRow = this.skills.Count == 0 ? 0 : this.skills.Max(x => x.DisplayRow);
            int maxCol = this.skills.Count == 0 ? 0 : this.skills.Max(x => x.DisplayCol);
            int skillCellWidth = Math.Min(MaxSkillCellWidth, (this.rightInnerWidth / 2) / (maxCol + 1));
            int skillCellHeight = Math.Min(MaxSkillCellHeight, lowerHeight / (maxRow + 1));
            int skillsWidth = skillCellWidth * (maxCol + 1);
            int stuntsWidth = this.rightInnerWidth - ColumnGap - skillsWidth;

            var lower = new HorizontalStackPanel
            {
                Spacing = ColumnGap
            };
            var skillColumn = new VerticalStackPanel
            {
                Width = skillsWidth
            };
            int owned = this.skills.Count(x => this.person.HasSkill(x.ID));
            var skillTitle = new HorizontalStackPanel
            {
                Spacing = 16
            };
            skillTitle.Widgets.Add(CreateSectionTitle("技能", owned + " / " + this.skills.Count, out _));
            skillTitle.Widgets.Add(CreateLegend("已掌握", Gold));
            skillTitle.Widgets.Add(CreateLegend("可修习", Green));
            skillColumn.Widgets.Add(skillTitle);
            skillColumn.Widgets.Add(this.BuildSkillGrid(skillCellWidth, skillCellHeight, maxRow, maxCol));
            lower.Widgets.Add(skillColumn);

            // 特技按消耗的战意从小到大排
            List<Stunt> stunts = common.AllStunts.Values.OrderBy(x => x.Combativity).ThenBy(x => x.ID).ToList();
            var stuntColumn = new VerticalStackPanel
            {
                Width = stuntsWidth
            };
            stuntColumn.Widgets.Add(CreateSectionTitle("特技", this.person.Stunts.Count + " / " + stunts.Count, out _));
            stuntColumn.Widgets.Add(this.BuildStuntSlots(stunts, stuntsWidth, lowerHeight));
            lower.Widgets.Add(stuntColumn);
            page.Widgets.Add(lower);
            return page;
        }

        // 称号格位：写“种类 · 等级”和称号名。一个都没有时留一行高度写“暂无称号”
        private Widget BuildTitleSlots(List<Title> titles, int width)
        {
            var rows = new VerticalStackPanel
            {
                Spacing = SlotSpacing
            };
            if (titles.Count == 0)
            {
                var empty = new Panel
                {
                    Height = TitleSlotHeight
                };
                Label label = CreateLabel("暂无称号", 14, TextSecondary);
                label.VerticalAlignment = VerticalAlignment.Center;
                empty.Widgets.Add(label);
                rows.Widgets.Add(empty);
                return rows;
            }
            HorizontalStackPanel row = null;
            foreach (Title title in titles)
            {
                if (row == null || row.Widgets.Count >= TitleSlotsPerRow)
                {
                    row = new HorizontalStackPanel
                    {
                        Spacing = SlotSpacing
                    };
                    rows.Widgets.Add(row);
                }
                var content = new VerticalStackPanel
                {
                    Spacing = 4,
                    VerticalAlignment = VerticalAlignment.Center
                };
                content.Widgets.Add(CreateLabel(title.KindName + " · " + title.Level + "级", 12, TextSecondary));
                content.Widgets.Add(CreateLabel(title.Name, 15, GetTitleColor(title)));
                Button slot = CreateFlatButton(content, width, TitleSlotHeight, new FrameBrush(CardColor, GoldDim, false));
                slot.Padding = new Thickness(12, 0, 8, 0);
                this.AttachPopover(slot, (panel, textWidth) => this.ShowTitle(panel, title, textWidth));
                row.Widgets.Add(slot);
            }
            return rows;
        }

        // 称号等级的分档：4级以下普通，4起绿，7起蓝，10起紫，13起橙，与旧界面一致
        private static Color GetTitleColor(Title title)
        {
            if (title.Level >= 13)
            {
                return TierOrange;
            }
            if (title.Level >= 10)
            {
                return TierViolet;
            }
            if (title.Level >= 7)
            {
                return TierBlue;
            }
            if (title.Level >= 4)
            {
                return TierGreen;
            }
            return TextPrimary;
        }

        // 技能格子按数据里的行列排，已掌握的金色，可修习的绿色，其他灰色
        private Widget BuildSkillGrid(int cellWidth, int cellHeight, int maxRow, int maxCol)
        {
            var grid = new Panel
            {
                Width = (maxCol + 1) * cellWidth,
                Height = (maxRow + 1) * cellHeight
            };
            int fontSize = cellHeight >= 22 && cellWidth >= 44 ? 14 : 12;
            foreach (Skill skill in this.skills)
            {
                bool has = this.person.HasSkill(skill.ID);
                Color color = has ? GoldLight : skill.CanLearn(this.person) ? Green : TextMuted;
                Label label = CreateLabel(skill.Name, fontSize, color);
                label.HorizontalAlignment = HorizontalAlignment.Center;
                label.VerticalAlignment = VerticalAlignment.Center;
                IBrush normal = has ? new FrameBrush(CardColor, GoldDim, false) : new FrameBrush(Color.Transparent, DividerColor, false);
                // 格子之间留2像素缝
                Button cell = CreateFlatButton(label, cellWidth - 2, cellHeight - 2, normal);
                cell.HorizontalAlignment = HorizontalAlignment.Left;
                cell.VerticalAlignment = VerticalAlignment.Top;
                cell.Left = skill.DisplayCol * cellWidth;
                cell.Top = skill.DisplayRow * cellHeight;
                this.AttachPopover(cell, (panel, textWidth) => this.ShowSkill(panel, skill, textWidth));
                grid.Widgets.Add(cell);
            }
            return grid;
        }

        // 特技全部列出排成两列，已学的亮，未学的虚线框，未学的也能看修习条件
        private Widget BuildStuntSlots(List<Stunt> stunts, int width, int height)
        {
            int rowCount = Math.Max(1, (stunts.Count + StuntSlotsPerRow - 1) / StuntSlotsPerRow);
            int slotHeight = Math.Min(MaxStuntSlotHeight, (height - (rowCount - 1) * SlotSpacing) / rowCount);
            int slotWidth = (width - (StuntSlotsPerRow - 1) * SlotSpacing) / StuntSlotsPerRow;
            var rows = new VerticalStackPanel
            {
                Spacing = SlotSpacing
            };
            HorizontalStackPanel row = null;
            foreach (Stunt stunt in stunts)
            {
                if (row == null || row.Widgets.Count >= StuntSlotsPerRow)
                {
                    row = new HorizontalStackPanel
                    {
                        Spacing = SlotSpacing
                    };
                    rows.Widgets.Add(row);
                }
                bool has = this.person.Stunts.ContainsKey(stunt.ID);
                var content = new HorizontalStackPanel
                {
                    Spacing = 8,
                    VerticalAlignment = VerticalAlignment.Center
                };
                content.Widgets.Add(CreateLabel(stunt.Name, 14, has ? TextPrimary : TextMuted));
                content.Widgets.Add(CreateLabel("战意 " + stunt.Combativity, 12, has ? TextSecondary : TextMuted));
                IBrush normal = has ? new FrameBrush(CardColor, GoldDim, false) : new FrameBrush(Color.Transparent, LineColor, true);
                Button slot = CreateFlatButton(content, slotWidth, slotHeight, normal);
                slot.Padding = new Thickness(12, 0, 8, 0);
                this.AttachPopover(slot, (panel, textWidth) => this.ShowStunt(panel, stunt, textWidth));
                row.Widgets.Add(slot);
            }
            return rows;
        }

        private void ShowTitle(VerticalStackPanel panel, Title title, int width)
        {
            panel.Widgets.Add(CreateLabel(title.Level + "级" + title.KindName, 12, TextSecondary));
            panel.Widgets.Add(CreateWrappedLabel(title.Name, 22, GetTitleColor(title), width));
            if (title.FundForHolder > 0)
            {
                panel.Widgets.Add(CreateLabel("俸金 " + title.FundForHolder + " / 月", 13, Gold));
            }
            AddInfluences(panel, title.Influences.Values, width);
            var conditions = new List<KeyValuePair<string, bool?>>();
            conditions.AddRange(title.Conditions.Select(x => Check(x, x.CheckCondition(this.person))));
            Architecture architecture = this.person.LocationArchitecture;
            conditions.AddRange(title.ArchitectureConditions.Select(x => Check(x, architecture != null && x.CheckCondition(architecture))));
            Faction faction = this.person.BelongedFaction;
            conditions.AddRange(title.FactionConditions.Select(x => Check(x, faction != null && x.CheckCondition(faction))));
            AddConditions(panel, "获得条件", conditions, width);
        }

        private void ShowStunt(VerticalStackPanel panel, Stunt stunt, int width)
        {
            bool has = this.person.Stunts.ContainsKey(stunt.ID);
            panel.Widgets.Add(CreateLabel(has ? "战斗特技 · 已掌握" : "战斗特技 · 未掌握", 12, has ? Gold : TextSecondary));
            panel.Widgets.Add(CreateWrappedLabel(stunt.Name, 22, TextPrimary, width));
            panel.Widgets.Add(CreateLabel("战意消耗 " + stunt.Combativity + " · 持续 " + stunt.Period * Session.Parameters.DayInTurn + " 天", 13, Gold));
            AddInfluences(panel, stunt.Influences.Values, width);
            // 只有本人正带着部队时才能判断使用条件是否满足，否则只列出条件
            Troop troop = this.person.LocationTroop;
            bool canCheck = troop != null && troop.Leader == this.person;
            AddConditions(panel, "使用条件", stunt.CastConditions.Select(x => canCheck ? Check(x, x.CheckCondition(troop)) : new KeyValuePair<string, bool?>(x.Name, null)), width);
            AddConditions(panel, "修习条件", stunt.LearnConditions.Select(x => Check(x, x.CheckCondition(this.person))), width);
        }

        private void ShowSkill(VerticalStackPanel panel, Skill skill, int width)
        {
            string state;
            Color color;
            if (this.person.HasSkill(skill.ID))
            {
                state = "已掌握";
                color = Gold;
            }
            else if (skill.CanLearn(this.person))
            {
                state = "可修习";
                color = Green;
            }
            else
            {
                state = "未掌握";
                color = TextSecondary;
            }
            panel.Widgets.Add(CreateLabel("技能 · " + state, 12, color));
            panel.Widgets.Add(CreateWrappedLabel(skill.Name, 22, TextPrimary, width));
            AddInfluences(panel, skill.Influences.Values, width);
            AddConditions(panel, "修习条件", skill.Conditions.Select(x => Check(x, x.CheckCondition(this.person))), width);
        }

        private static KeyValuePair<string, bool?> Check(Condition condition, bool satisfied)
        {
            return new KeyValuePair<string, bool?>(condition.Name, satisfied);
        }

        // 限定生效时机的影响写在“效果”标题后面，其余的逐条列出
        private static void AddInfluences(VerticalStackPanel panel, IEnumerable<Influence> influences, int width)
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
                panel.Widgets.Add(CreateWrappedLabel("· " + influence.Description, 14, TextPrimary, width));
            }
        }

        // 条件前面一个菱形，满足的绿色，不满足的红色，无法判断的灰色
        private static void AddConditions(VerticalStackPanel panel, string title, IEnumerable<KeyValuePair<string, bool?>> conditions, int width)
        {
            List<KeyValuePair<string, bool?>> list = conditions.ToList();
            panel.Widgets.Add(CreateDetailHeader(title, width));
            if (list.Count == 0)
            {
                panel.Widgets.Add(CreateLabel("无", 13, TextSecondary));
                return;
            }
            foreach (var condition in list)
            {
                Color color = condition.Value == null ? TextMuted : condition.Value.Value ? Green : Hostile;
                var row = new HorizontalStackPanel
                {
                    Spacing = 6
                };
                row.Widgets.Add(CreateLabel("◇", 13, color));
                row.Widgets.Add(CreateWrappedLabel(condition.Key, 13, condition.Value == false ? TextSecondary : TextPrimary, width - 20));
                panel.Widgets.Add(row);
            }
        }

        // 小标题和上一段之间的空隙放在外层容器的内边距里，固定了宽度的文字自己不带外边距
        private static Widget CreateDetailHeader(string title, int width)
        {
            var holder = new Panel
            {
                Padding = new Thickness(0, 10, 0, 0)
            };
            holder.Widgets.Add(CreateWrappedLabel(title, 12, TextSecondary, width));
            return holder;
        }

        private static Widget CreateLegend(string text, Color color)
        {
            var legend = new HorizontalStackPanel
            {
                Spacing = 6,
                Height = SectionTitleHeight
            };
            legend.Widgets.Add(new Panel
            {
                Width = 8,
                Height = 8,
                VerticalAlignment = VerticalAlignment.Center,
                Background = new SolidBrush(color)
            });
            Label label = CreateLabel(text, 13, TextSecondary);
            label.VerticalAlignment = VerticalAlignment.Center;
            legend.Widgets.Add(label);
            return legend;
        }

        #endregion

        #region 说明浮层

        // 鼠标移到某一项上时显示说明，移开就收起。触屏没有悬停，点一下也能显示，点别处收起
        private void AttachPopover(Widget anchor, Action<VerticalStackPanel, int> fill)
        {
            anchor.MouseEntered += (sender, args) => this.ShowPopover(anchor, fill);
            anchor.MouseLeft += (sender, args) => this.HidePopover(anchor);
            if (anchor is Button button)
            {
                button.Click += (sender, args) => this.ShowPopover(anchor, fill);
            }
        }

        private void ShowPopover(Widget anchor, Action<VerticalStackPanel, int> fill)
        {
            if (this.popoverAnchor == anchor && this.popover.Visible)
            {
                return;
            }
            this.popoverAnchor = anchor;
            this.popoverContent.Widgets.Clear();
            fill(this.popoverContent, PopoverWidth - 2 * PopoverPadding);

            // 放在这一项的右边，右边放不下就放左边；上下不超出弹窗
            Point size = this.popover.Measure(new Point(PopoverWidth, int.MaxValue));
            Point anchorTopLeft = this.mask.ToLocal(anchor.ToGlobal(Point.Zero));
            Point dialogTopLeft = this.mask.ToLocal(this.dialog.ToGlobal(Point.Zero));
            var dialogBounds = new Rectangle(dialogTopLeft.X, dialogTopLeft.Y, this.dialogWidth, this.dialogHeight);
            int x = anchorTopLeft.X + anchor.Bounds.Width + PopoverGap;
            if (x + size.X > dialogBounds.Right - PopoverGap)
            {
                x = anchorTopLeft.X - PopoverGap - size.X;
            }
            x = Math.Max(dialogBounds.X + PopoverGap, x);
            int y = MathHelper.Clamp(anchorTopLeft.Y, dialogBounds.Y + PopoverGap, Math.Max(dialogBounds.Y + PopoverGap, dialogBounds.Bottom - PopoverGap - size.Y));
            this.popover.Left = x;
            this.popover.Top = y;
            this.popover.Visible = true;
        }

        private void HidePopover(Widget anchor = null)
        {
            if (anchor == null || anchor == this.popoverAnchor)
            {
                this.popover.Visible = false;
                this.popoverAnchor = null;
            }
        }

        // 浮层放在最外层，盖在弹窗内容上面；位置用Left、Top从左上角算
        private Widget BuildPopover()
        {
            this.popoverContent = new VerticalStackPanel
            {
                Spacing = 6
            };
            this.popover = new Panel
            {
                Width = PopoverWidth,
                Padding = new Thickness(PopoverPadding),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Background = new FrameBrush(PopoverColor, GoldDim, false),
                Visible = false
            };
            this.popover.Widgets.Add(this.popoverContent);
            return this.popover;
        }

        #endregion

        #region 生平页

        // 生平：列传（史实，与本局无关）和剧本里的经历。两列都放得下就左右并排；
        // 有一列要滚动时改成两个小页签切换，各占整个宽度，内容显示得多，也不会出现两条滚动条
        private Widget BuildLifePage()
        {
            int columnWidth = this.HalfColumnWidth;
            int columnTextWidth = columnWidth - ScrollBarWidth;
            Widget biography = this.BuildBiography(columnTextWidth, true);
            Widget scenario = this.BuildScenario(columnTextWidth, true);
            if (FitsHeight(biography, columnWidth, this.contentHeight) && FitsHeight(scenario, columnWidth, this.contentHeight))
            {
                var columns = new HorizontalStackPanel
                {
                    Spacing = ColumnGap,
                    Height = this.contentHeight
                };
                columns.Widgets.Add(this.CreateColumnViewer(biography, columnWidth, this.contentHeight));
                columns.Widgets.Add(this.CreateColumnViewer(scenario, columnWidth, this.contentHeight));
                return columns;
            }

            int textWidth = this.rightInnerWidth - ScrollBarWidth;
            int pageHeight = this.contentHeight - SubTabHeight - SubTabGap;
            var subTabs = new List<Tab>
            {
                new Tab("列传", this.CreateColumnViewer(this.BuildBiography(textWidth, false), this.rightInnerWidth, pageHeight)),
                new Tab("剧本", this.CreateColumnViewer(this.BuildScenario(textWidth, false), this.rightInnerWidth, pageHeight))
            };
            var page = new VerticalStackPanel
            {
                Height = this.contentHeight
            };
            var bar = new Panel
            {
                Height = SubTabHeight
            };
            bar.Widgets.Add(new Panel
            {
                Height = 1,
                VerticalAlignment = VerticalAlignment.Bottom,
                Background = new SolidBrush(DividerColor)
            });
            var row = new HorizontalStackPanel
            {
                Spacing = 4
            };
            foreach (Tab tab in subTabs)
            {
                row.Widgets.Add(CreateTabButton(tab, 14, SubTabHeight, () => ShowTab(subTabs, tab)));
            }
            bar.Widgets.Add(row);
            page.Widgets.Add(bar);
            page.Widgets.Add(CreateSpacer(SubTabGap));
            var pages = new Panel
            {
                Height = pageHeight
            };
            foreach (Tab tab in subTabs)
            {
                pages.Widgets.Add(tab.Page);
            }
            page.Widgets.Add(pages);
            ShowTab(subTabs, subTabs[0]);
            return page;
        }

        // 按给定宽度排版，看内容高度是否放得下
        private static bool FitsHeight(Widget content, int width, int height)
        {
            return content.Measure(new Point(width, int.MaxValue)).Y <= height;
        }

        // 列传：简介、演义、历史分段，最后注明是史实
        private Widget BuildBiography(int textWidth, bool withTitle)
        {
            var biography = new VerticalStackPanel
            {
                Spacing = 12
            };
            if (withTitle)
            {
                biography.Widgets.Add(CreateSectionTitle("列传", null, out _));
            }
            Biography data = this.person.PersonBiography;
            bool hasText = false;
            if (data != null)
            {
                hasText |= AddParagraphs(biography, null, data.Brief, textWidth);
                hasText |= AddParagraphs(biography, "演义", data.Romance, textWidth);
                hasText |= AddParagraphs(biography, "历史", data.History, textWidth);
            }
            if (!hasText)
            {
                biography.Widgets.Add(CreateLabel("暂无列传", 14, TextSecondary));
            }
            else
            {
                Label note = CreateLabel("史实记载 · 与本局进程无关", 12, TextMuted);
                note.Margin = new Thickness(0, 4, 0, 0);
                biography.Widgets.Add(note);
            }
            return biography;
        }

        // 剧本：本局里的经历，排成时间线
        private Widget BuildScenario(int textWidth, bool withTitle)
        {
            var scenario = new VerticalStackPanel();
            if (withTitle)
            {
                scenario.Widgets.Add(CreateSectionTitle("剧本", null, out _));
            }
            scenario.Widgets.Add(this.BuildTimeline(textWidth));
            return scenario;
        }

        private ScrollViewer CreateColumnViewer(Widget content, int width, int height)
        {
            var viewer = new ScrollViewer
            {
                Content = content,
                ShowHorizontalScrollBar = false,
                Width = width,
                Height = height
            };
            this.dragViewers.Add(viewer);
            return viewer;
        }

        // 一段文字按换行拆成几段，返回是否有内容
        private static bool AddParagraphs(VerticalStackPanel panel, string title, string text, int width)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return false;
            }
            if (title != null)
            {
                Label header = CreateLabel(title, 14, Gold);
                header.Margin = new Thickness(0, 6, 0, 0);
                panel.Widgets.Add(header);
            }
            foreach (string line in text.Split('\n'))
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    panel.Widgets.Add(CreateWrappedLabel(line.Trim(), 15, TextPrimary, width));
                }
            }
            return true;
        }

        // 剧本里的经历每行一条，格式是“190年4月：内容”，新的写在前面。这里倒过来按时间先后排
        private Widget BuildTimeline(int width)
        {
            var entries = new List<KeyValuePair<string, string>>();
            string inGame = this.person.PersonBiography?.InGame;
            if (!string.IsNullOrEmpty(inGame))
            {
                foreach (string raw in inGame.Split('\n'))
                {
                    string line = raw.Trim();
                    if (line.Length == 0)
                    {
                        continue;
                    }
                    Match match = InGameLine.Match(line);
                    entries.Add(match.Success
                        ? new KeyValuePair<string, string>(match.Groups[1].Value, match.Groups[3].Value)
                        : new KeyValuePair<string, string>("", line));
                }
                entries.Reverse();
            }
            if (entries.Count == 0)
            {
                return CreateLabel("尚无经历", 14, TextSecondary);
            }

            // 左边一条竖线，每条经历前一个圆点
            var list = new VerticalStackPanel
            {
                Spacing = 14,
                Padding = new Thickness(0, 4, 0, 4),
                Background = new TimelineBrush(DividerColor, 3)
            };
            // 日期栏按最长的日期定宽，和内容只隔一点
            const int dotWidth = 7;
            const int dotGap = 10;
            SpriteFontBase dateFont = TextManager.GetFont(13);
            int dateWidth = (int)Math.Ceiling(entries.Max(x => dateFont.MeasureString(x.Key).X)) + 8;
            foreach (var entry in entries)
            {
                var row = new HorizontalStackPanel();
                var dot = new Panel
                {
                    Padding = new Thickness(0, 5, dotGap, 0),
                    VerticalAlignment = VerticalAlignment.Top
                };
                dot.Widgets.Add(new Panel
                {
                    Width = dotWidth,
                    Height = dotWidth,
                    Background = new SolidBrush(GoldDim)
                });
                row.Widgets.Add(dot);
                Label date = CreateLabel(entry.Key, 13, TextSecondary);
                date.Width = dateWidth;
                row.Widgets.Add(date);
                row.Widgets.Add(CreateWrappedLabel(entry.Value, 14, TextPrimary, width - dotWidth - dotGap - dateWidth));
                list.Widgets.Add(row);
            }
            return list;
        }

        #endregion

        #region 档案页

        // 档案：左边兵种和其他经验的分级，右边家族和亲近的人，都带头像
        private Widget BuildArchivePage()
        {
            Person p = this.person;
            int columnWidth = this.HalfColumnWidth;
            var page = new HorizontalStackPanel
            {
                Spacing = ColumnGap,
                Height = this.contentHeight
            };

            var left = new VerticalStackPanel();
            int textWidth = columnWidth - ScrollBarWidth;
            left.Widgets.Add(CreateSectionTitle("兵种经验", null, out _));
            left.Widgets.Add(this.CreateExperienceGrid(new[]
            {
                new Experience("步兵", "Bubing", (int)MilitaryType.Infantry, p.BubingExperience),
                new Experience("弓弩", "Nubing", (int)MilitaryType.Crossbow, p.NubingExperience),
                new Experience("骑兵", "Qibing", (int)MilitaryType.Cavalry, p.QibingExperience),
                new Experience("水军", "Shuijun", (int)MilitaryType.Navy, p.ShuijunExperience),
                new Experience("器械", "Qixie", (int)MilitaryType.SiegeEquipment, p.QixieExperience),
            }, TroopExperienceGrades, textWidth));
            left.Widgets.Add(CreateSpacer(SectionGap));
            left.Widgets.Add(CreateSectionTitle("其他经验", null, out _));
            left.Widgets.Add(this.CreateExperienceGrid(new[]
            {
                new Experience("内政", "Internal", -1, p.InternalExperience),
                new Experience("策略", "Tactics", -1, p.TacticsExperience),
                new Experience("计略", "Stratagem", -1, p.StratagemExperience),
            }, OtherExperienceGrades, textWidth));
            left.Widgets.Add(CreateSpacer(12));
            // 这些经验只会累积，没有等级，游戏里用作修习称号、技能的条件
            left.Widgets.Add(CreateWrappedLabel("经验只会累积，分级按修习条件里常见的门槛划分；", 12, TextMuted, textWidth));
            left.Widgets.Add(CreateSpacer(4));
            left.Widgets.Add(CreateWrappedLabel("修习称号和技能时，常以某项经验达到多少作为条件。", 12, TextMuted, textWidth));
            page.Widgets.Add(this.CreateColumnViewer(left, columnWidth, this.contentHeight));

            // 家族和亲近用同一种头像小块：关系写在头像上面，姓名写在下面
            var right = new VerticalStackPanel();
            right.Widgets.Add(CreateSectionTitle("家族", null, out _));
            var family = new HorizontalStackPanel
            {
                Spacing = CloseTileSpacing
            };
            family.Widgets.Add(CreatePersonTile(p.Father, "父"));
            family.Widgets.Add(CreatePersonTile(p.Mother, "母"));
            family.Widgets.Add(CreatePersonTile(p.Spouse, "配偶"));
            right.Widgets.Add(family);
            right.Widgets.Add(CreateSpacer(SectionGap));
            List<Person> closePersons = p.GetClosePersons().Where(x => x != null).ToList();
            right.Widgets.Add(CreateSectionTitle("亲近", closePersons.Count == 0 ? null : closePersons.Count.ToString(), out _));
            right.Widgets.Add(closePersons.Count == 0
                ? (Widget)CreateLabel("尚无亲近之人", 14, TextSecondary)
                : CreateCloseTiles(closePersons, textWidth));
            page.Widgets.Add(this.CreateColumnViewer(right, columnWidth, this.contentHeight));
            return page;
        }

        // 一项经验：显示名、图标文件名、数值
        private class Experience
        {
            public readonly string Name;
            public readonly string Icon;
            // 对应的兵种类型编号，不是兵种经验时为-1
            public readonly int TroopKind;
            public readonly int Value;

            public Experience(string name, string icon, int troopKind, int value)
            {
                this.Name = name;
                this.Icon = icon;
                this.TroopKind = troopKind;
                this.Value = value;
            }

            // 优先用经验目录里的图标，没有时兵种经验用部队的兵种图标，都没有返回null
            public string IconFile
            {
                get
                {
                    string icon = ExperienceIconPath + this.Icon + ".png";
                    if (Platform.Current.FileExists(icon))
                    {
                        return icon;
                    }
                    icon = TroopKindIconPath + this.TroopKind + ".png";
                    return this.TroopKind >= 0 && Platform.Current.FileExists(icon) ? icon : null;
                }
            }
        }

        // 达到第几个门槛就是第几级，0是D，4是S
        private static int GetGrade(int value, int[] thresholds)
        {
            int grade = 0;
            while (grade < thresholds.Length && value >= thresholds[grade])
            {
                grade++;
            }
            return grade;
        }

        private static Color GetGradeColor(int grade)
        {
            return GradeColors[grade];
        }

        // 经验排成每行3个的格子：左边图标，右边名称和大字的分级。具体数值鼠标悬停时显示
        private Widget CreateExperienceGrid(Experience[] values, int[] thresholds, int width)
        {
            int cellWidth = (width - (ExperienceColumns - 1) * TemperCellSpacing) / ExperienceColumns;
            var grid = new VerticalStackPanel
            {
                Spacing = TemperCellSpacing
            };
            HorizontalStackPanel row = null;
            foreach (Experience value in values)
            {
                if (row == null || row.Widgets.Count >= ExperienceColumns)
                {
                    row = new HorizontalStackPanel
                    {
                        Spacing = TemperCellSpacing
                    };
                    grid.Widgets.Add(row);
                }
                int grade = GetGrade(value.Value, thresholds);
                var content = new HorizontalStackPanel
                {
                    Spacing = 10,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };
                string icon = value.IconFile;
                if (icon != null)
                {
                    content.Widgets.Add(new Image
                    {
                        Renderable = new ArchitectureDetailDialog.TextureImage(icon, ExperienceIconSize, ExperienceIconSize, null, TextureFiltering.Linear),
                        Width = ExperienceIconSize,
                        Height = ExperienceIconSize,
                        VerticalAlignment = VerticalAlignment.Center
                    });
                }
                var text = new VerticalStackPanel
                {
                    Spacing = 0,
                    VerticalAlignment = VerticalAlignment.Center
                };
                text.Widgets.Add(CreateLabel(value.Name, 12, TextSecondary));
                text.Widgets.Add(CreateLabel(GradeNames[grade], 24, GetGradeColor(grade)));
                content.Widgets.Add(text);
                Button cell = CreateFlatButton(content, cellWidth, ExperienceCellHeight, new FrameBrush(CardColor, LineColor, false));
                this.AttachPopover(cell, (panel, popoverWidth) => ShowExperience(panel, value, thresholds, grade));
                row.Widgets.Add(cell);
            }
            return grid;
        }

        // 悬停说明：数值、离下一级还差多少、各级门槛
        private static void ShowExperience(VerticalStackPanel panel, Experience value, int[] thresholds, int grade)
        {
            panel.Widgets.Add(CreateLabel(value.Name + "经验", 12, TextSecondary));
            var title = new HorizontalStackPanel
            {
                Spacing = 10
            };
            title.Widgets.Add(CreateLabel(GradeNames[grade], 22, GetGradeColor(grade)));
            Label number = CreateLabel(value.Value.ToString("#,0"), 22, TextPrimary);
            title.Widgets.Add(number);
            panel.Widgets.Add(title);
            panel.Widgets.Add(CreateLabel(grade >= thresholds.Length
                ? "已达最高级"
                : "距 " + GradeNames[grade + 1] + " 级还差 " + (thresholds[grade] - value.Value).ToString("#,0"), 13, Gold));
            var levels = new List<string>();
            for (int i = 0; i < thresholds.Length; i++)
            {
                levels.Add(GradeNames[i + 1] + " " + thresholds[i].ToString("#,0"));
            }
            panel.Widgets.Add(CreateDetailHeader("分级门槛", PopoverWidth - 2 * PopoverPadding));
            panel.Widgets.Add(CreateLabel(string.Join(" · ", levels), 13, TextPrimary));
        }

        // 人物头像，细框、不加底色。没有这个人或还没登场时只画一个空的虚线框；已登场的点击打开该人物的详情，叠在本窗口上面
        private Widget CreateRelativePortrait(Person relative, int size)
        {
            bool shown = relative != null && relative.Available;
            var frame = new Panel
            {
                Width = size,
                Height = size,
                HorizontalAlignment = HorizontalAlignment.Center,
                Padding = new Thickness(1),
                Background = new FrameBrush(Color.Transparent, LineColor, !shown)
            };
            if (shown)
            {
                frame.Widgets.Add(new Image
                {
                    Renderable = new DiplomacyDialog.PortraitImage(relative, PortraitSize.Medium, size - 2, size - 2)
                });
                Button button = CreateFlatButton(frame, size, size, new SolidBrush(Color.Transparent));
                button.HorizontalAlignment = HorizontalAlignment.Center;
                button.Click += (sender, args) =>
                {
                    this.popover.Hide();
                    Show(relative, this.playVoice);
                };
                return button;
            }
            return frame;
        }

        // 头像小块：可选的关系写在头像上面，头像下面写姓名；没有这个人写“无”，数据里有但还没登场写“未登场”
        private Widget CreatePersonTile(Person person, string relation)
        {
            bool shown = person != null && person.Available;
            var tile = new VerticalStackPanel
            {
                Width = CloseTileWidth,
                Spacing = 4
            };
            if (relation != null)
            {
                Label label = CreateLabel(relation, 12, TextSecondary);
                label.HorizontalAlignment = HorizontalAlignment.Center;
                tile.Widgets.Add(label);
            }
            tile.Widgets.Add(CreateRelativePortrait(person, CloseTileWidth));
            Label name = CreateLabel(shown ? person.Name : person == null ? "无" : "未登场", 13, shown ? TextPrimary : TextMuted);
            name.HorizontalAlignment = HorizontalAlignment.Center;
            tile.Widgets.Add(name);
            return tile;
        }

        // 亲近的人：头像下面写姓名，排满一行换行
        private Widget CreateCloseTiles(List<Person> persons, int width)
        {
            int perRow = Math.Max(1, (width + CloseTileSpacing) / (CloseTileWidth + CloseTileSpacing));
            var grid = new VerticalStackPanel
            {
                Spacing = CloseTileSpacing
            };
            HorizontalStackPanel row = null;
            foreach (Person closePerson in persons)
            {
                if (row == null || row.Widgets.Count >= perRow)
                {
                    row = new HorizontalStackPanel
                    {
                        Spacing = CloseTileSpacing
                    };
                    grid.Widgets.Add(row);
                }
                row.Widgets.Add(CreatePersonTile(closePerson, null));
            }
            return grid;
        }

        #endregion

        #region 通用控件

        // 竖向间隔。Myra里固定了高度的控件，外边距算在高度里面，所以分区之间的空隙单独用一个空控件
        private static Widget CreateSpacer(int height)
        {
            return new Panel
            {
                Height = height
            };
        }

        // 分区标题：大字标题，后面可以跟一个小字的数量
        private static Widget CreateSectionTitle(string title, string count, out Label countLabel)
        {
            var row = new HorizontalStackPanel
            {
                Spacing = 8,
                Height = SectionTitleHeight
            };
            Label label = CreateLabel(title, 18, TextPrimary);
            label.VerticalAlignment = VerticalAlignment.Top;
            row.Widgets.Add(label);
            countLabel = null;
            if (count != null)
            {
                countLabel = CreateLabel(count, 13, TextSecondary);
                countLabel.VerticalAlignment = VerticalAlignment.Top;
                countLabel.Margin = new Thickness(0, 5, 0, 0);
                row.Widgets.Add(countLabel);
            }
            return row;
        }

        // 扁平按钮：平时用给定的底，鼠标移上去整块稍微变亮
        private static Button CreateFlatButton(Widget content, int width, int height, IBrush normal)
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

        private static void SetFlatBackground(Button button, IBrush brush)
        {
            button.Background = brush;
            button.FocusedBackground = brush;
            IBrush over = brush is FrameBrush frame ? frame.WithFill(HoverColor) : new SolidBrush(HoverColor);
            button.OverBackground = over;
            button.PressedBackground = over;
        }

        private static Widget CreateBar(int width, int height, int value, int max, Color color)
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

        private static Label CreateLabel(string text, int fontSize, Color color)
        {
            return new Label
            {
                Text = text,
                Font = TextManager.GetFont(fontSize),
                TextColor = color,
                HorizontalAlignment = HorizontalAlignment.Left
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

        #endregion

        #region 拖动滑动

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
            if (!MyraUI.IsTop(this.root))
            {
                return;
            }
            // 触屏上点到说明浮层所属的那一项以外的地方，收起浮层
            if (this.popoverAnchor != null && !this.popoverAnchor.ContainsGlobalPoint(position.Value))
            {
                this.HidePopover();
            }
            foreach (ScrollViewer viewer in this.dragViewers)
            {
                // 隐藏的页里的列表也在原位置，要跳过
                if (IsShown(viewer) && viewer.ContainsGlobalPoint(position.Value))
                {
                    this.dragViewer = viewer;
                    this.dragLastY = position.Value.Y;
                    break;
                }
            }
        }

        private static bool IsShown(Widget widget)
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

        #endregion

        // 一个页签：顶部的按钮和对应的页
        private class Tab
        {
            public readonly string Title;
            public readonly Widget Page;
            public Button Button;
            public Label Label;
            public Panel Line;

            public Tab(string title, Widget page)
            {
                this.Title = title;
                this.Page = page;
            }
        }

        // 带1像素边框的底，边框可以是虚线
        private class FrameBrush : IBrush
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
        private class DashLineBrush : IBrush
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

        // 时间线左边的竖线，画在圆点中间
        private class TimelineBrush : IBrush
        {
            private readonly Color line;
            private readonly int x;

            public TimelineBrush(Color line, int x)
            {
                this.line = line;
                this.x = x;
            }

            public void Draw(RenderContext context, Rectangle dest, Color color)
            {
                context.FillRectangle(new Rectangle(dest.X + this.x, dest.Y, 1, dest.Height), this.line);
            }
        }

        // 五维雷达图：外圈和半径一半处各一个五边形做刻度，数值连成半透明的金色多边形，满值按100算。
        // Myra画斜线没有抗锯齿，所以图形先按屏幕像素大小生成一张带抗锯齿的图片再画上去，文字仍然直接画
        private class RadarChart : Widget
        {
            private const int LabelSpace = 22;
            private const float FullValue = 100f;
            private const float GridWidth = 1f;
            private const float OutlineWidth = 1.5f;
            private static readonly Color GridColor = new Color(70, 90, 100);
            // 游戏按预乘透明度混合，半透明色用颜色乘系数得到
            private static readonly Color FillColor = MyraSkin.TextHighlight * 0.25f;

            private readonly string[] labels;
            private readonly int[] values;
            private Microsoft.Xna.Framework.Graphics.Texture2D texture;
            private Point textureSize;

            public RadarChart(string[] labels, int[] values)
            {
                this.labels = labels;
                this.values = values;
            }

            public void Dispose()
            {
                this.texture?.Dispose();
                this.texture = null;
            }

            private Vector2 Direction(int i)
            {
                // 第一项在正上方，顺时针排
                double angle = -Math.PI / 2 + 2 * Math.PI * i / this.values.Length;
                return new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
            }

            public override void InternalRender(RenderContext context)
            {
                Rectangle bounds = this.ActualBounds;
                float radius = Math.Min(bounds.Width, bounds.Height) / 2f - LabelSpace;
                if (radius <= 0)
                {
                    return;
                }
                Vector2 scale = MyraUI.Scale;
                var size = new Point((int)Math.Ceiling(bounds.Width * scale.X), (int)Math.Ceiling(bounds.Height * scale.Y));
                if (this.texture == null || this.texture.IsDisposed || this.textureSize != size)
                {
                    this.texture?.Dispose();
                    this.texture = this.CreateTexture(size, radius * Math.Min(scale.X, scale.Y), Math.Min(scale.X, scale.Y));
                    this.textureSize = size;
                }
                context.Draw(this.texture, bounds, null, Color.White, 0f, 0f, TextureFiltering.Linear);

                var center = new Vector2(bounds.X + bounds.Width / 2f, bounds.Y + bounds.Height / 2f);
                SpriteFontBase font = TextManager.GetFont(12);
                for (int i = 0; i < this.values.Length; i++)
                {
                    Vector2 labelSize = font.MeasureString(this.labels[i]);
                    Vector2 position = center + this.Direction(i) * (radius + LabelSpace / 2f + 4) - labelSize / 2;
                    context.DrawString(font, this.labels[i], position, TextSecondary, 0f);
                }
            }

            // 逐个像素算覆盖了多少：线按到线段的距离、填充按到边的距离，在边缘半个像素内渐变，就没有锯齿。
            // 颜色都是预乘透明度的，从下往上一层层叠
            private Microsoft.Xna.Framework.Graphics.Texture2D CreateTexture(Point size, float radius, float pixelScale)
            {
                int count = this.values.Length;
                var center = new Vector2(size.X / 2f, size.Y / 2f);
                var outer = new Vector2[count];
                var inner = new Vector2[count];
                var points = new Vector2[count];
                for (int i = 0; i < count; i++)
                {
                    Vector2 direction = this.Direction(i);
                    outer[i] = center + direction * radius;
                    inner[i] = center + direction * radius / 2;
                    points[i] = center + direction * radius * MathHelper.Clamp(this.values[i] / FullValue, 0f, 1f);
                }
                float gridHalf = GridWidth * pixelScale / 2;
                float outlineHalf = OutlineWidth * pixelScale / 2;
                Vector4 grid = GridColor.ToVector4();
                Vector4 fill = FillColor.ToVector4();
                Vector4 outline = Gold.ToVector4();

                var data = new Color[size.X * size.Y];
                for (int y = 0; y < size.Y; y++)
                {
                    for (int x = 0; x < size.X; x++)
                    {
                        var pixel = new Vector2(x + 0.5f, y + 0.5f);
                        Vector4 result = Vector4.Zero;
                        result = Blend(result, grid, LineCoverage(pixel, outer, gridHalf));
                        result = Blend(result, grid, LineCoverage(pixel, inner, gridHalf));
                        result = Blend(result, fill, FillCoverage(pixel, points));
                        result = Blend(result, outline, LineCoverage(pixel, points, outlineHalf));
                        data[y * size.X + x] = new Color(result.X, result.Y, result.Z, result.W);
                    }
                }
                var texture = new Microsoft.Xna.Framework.Graphics.Texture2D(Platform.GraphicsDevice, size.X, size.Y);
                texture.SetData(data);
                return texture;
            }

            private static Vector4 Blend(Vector4 below, Vector4 color, float coverage)
            {
                if (coverage <= 0)
                {
                    return below;
                }
                Vector4 top = color * coverage;
                return top + below * (1 - top.W);
            }

            private static float LineCoverage(Vector2 pixel, Vector2[] polygon, float halfWidth)
            {
                float distance = DistanceToEdges(pixel, polygon);
                return MathHelper.Clamp(halfWidth + 0.5f - distance, 0f, 1f);
            }

            private static float FillCoverage(Vector2 pixel, Vector2[] polygon)
            {
                float distance = DistanceToEdges(pixel, polygon);
                return MathHelper.Clamp(IsInside(pixel, polygon) ? 0.5f + distance : 0.5f - distance, 0f, 1f);
            }

            private static float DistanceToEdges(Vector2 pixel, Vector2[] polygon)
            {
                float best = float.MaxValue;
                for (int i = 0; i < polygon.Length; i++)
                {
                    Vector2 a = polygon[i];
                    Vector2 b = polygon[(i + 1) % polygon.Length];
                    Vector2 ab = b - a;
                    float lengthSquared = ab.LengthSquared();
                    float t = lengthSquared == 0 ? 0 : MathHelper.Clamp(Vector2.Dot(pixel - a, ab) / lengthSquared, 0f, 1f);
                    best = Math.Min(best, Vector2.Distance(pixel, a + ab * t));
                }
                return best;
            }

            private static bool IsInside(Vector2 pixel, Vector2[] polygon)
            {
                bool inside = false;
                for (int i = 0, j = polygon.Length - 1; i < polygon.Length; j = i++)
                {
                    Vector2 a = polygon[i];
                    Vector2 b = polygon[j];
                    if ((a.Y > pixel.Y) != (b.Y > pixel.Y) && pixel.X < (b.X - a.X) * (pixel.Y - a.Y) / (b.Y - a.Y) + a.X)
                    {
                        inside = !inside;
                    }
                }
                return inside;
            }
        }
    }
}

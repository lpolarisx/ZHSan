using FontStashSharp;
using GameManager;
using GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra.Events;
using Myra.Graphics2D;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;
using Platforms;
using System;
using System.Collections.Generic;

namespace PersonDetailPlugin
{
    /// <summary>
    /// 赏赐宝物弹窗：横向列出君主手里某一种类的宝物，选中后点确认赏赐给武将。
    /// 用Myra绘制。还没有美术素材，底板、边框、按钮都先用纯色代替。
    /// </summary>
    internal class AwardTreasureDialog
    {
        private const int DialogWidth = 760;
        private const int CardWidth = 150;
        private const int CardHeight = 230;
        private const int PictureSize = 96;
        // 按住拖动超过这个距离才算滑动列表，否则算点击卡片
        private const int DragThreshold = 6;
        private const string DefaultPicture = "Content/Textures/Resources/Treasure/9999.png";

        private static readonly Color PanelColor = new Color(38, 54, 64);
        private static readonly Color CardColor = new Color(26, 38, 46);
        private static readonly Color CardOverColor = new Color(44, 64, 76);
        private static readonly Color LineColor = new Color(90, 110, 120);
        private static readonly Color GoldColor = new Color(214, 182, 96);

        private readonly Person person;
        private readonly Person leader;
        private readonly Action<Treasure> onConfirm;
        private readonly List<Button> cards = new List<Button>();
        private readonly List<Treasure> treasures;

        private HorizontalScrollViewer scrollViewer;
        private Widget cardsPanel;
        private Label selectedLabel;
        private Button okButton;
        private Label okLabel;
        private Treasure selected;

        private int? dragLastX;
        private int dragDistance;

        private AwardTreasureDialog(Person person, int treasureGroup, Action<Treasure> onConfirm)
        {
            this.person = person;
            this.leader = person.BelongedFaction.Leader;
            this.onConfirm = onConfirm;
            this.treasures = this.leader.TreasureListforGroup(treasureGroup);
        }

        /// <summary>
        /// 显示弹窗
        /// </summary>
        /// <param name="person">接受赏赐的武将，必须有所属势力和君主</param>
        /// <param name="treasureGroup">宝物种类</param>
        /// <param name="onConfirm">点确认后调用，参数为选中的宝物</param>
        public static void Show(Person person, int treasureGroup, Action<Treasure> onConfirm)
        {
            var dialog = new AwardTreasureDialog(person, treasureGroup, onConfirm);
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
            MyraUI.Close();
        }

        private Widget Build()
        {
            var dialog = new VerticalStackPanel
            {
                Width = DialogWidth,
                Spacing = 12,
                Padding = new Thickness(20, 16),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Background = new SolidBrush(PanelColor),
                Border = new SolidBrush(GoldColor),
                BorderThickness = new Thickness(2)
            };

            dialog.Widgets.Add(new Label
            {
                Text = "赏赐宝物",
                Font = TextManager.GetFont(28),
                TextColor = GoldColor,
                HorizontalAlignment = HorizontalAlignment.Center
            });
            dialog.Widgets.Add(new Label
            {
                Text = this.leader.Name + " 赏赐给 " + this.person.Name,
                Font = TextManager.GetFont(18),
                TextColor = Color.WhiteSmoke,
                HorizontalAlignment = HorizontalAlignment.Center
            });

            if (this.treasures.Count > 0)
            {
                dialog.Widgets.Add(this.BuildTreasureList());
            }
            else
            {
                dialog.Widgets.Add(new Label
                {
                    Text = "君主没有可赏赐的同类宝物",
                    Font = TextManager.GetFont(20),
                    TextColor = Color.Gray,
                    Height = CardHeight,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                });
            }

            this.selectedLabel = new Label
            {
                Font = TextManager.GetFont(18),
                TextColor = Color.WhiteSmoke,
                Wrap = true,
                // 固定两行的高度，选中不同宝物时弹窗大小不跳动
                Height = 52
            };
            dialog.Widgets.Add(this.selectedLabel);

            var buttons = new HorizontalStackPanel
            {
                Spacing = 40,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            this.okButton = CreateButton("确认", out this.okLabel);
            this.okButton.Click += this.OkButton_Click;
            Button cancelButton = CreateButton("取消", out _);
            cancelButton.Click += (sender, args) => this.Close();
            buttons.Widgets.Add(this.okButton);
            buttons.Widgets.Add(cancelButton);
            dialog.Widgets.Add(buttons);

            this.Select(null);

            // 半透明遮罩铺满窗口，压暗后面的游戏界面
            var mask = new Panel
            {
                Background = new SolidBrush(new Color(0, 0, 0, 150))
            };
            mask.Widgets.Add(dialog);
            return mask;
        }

        private Widget BuildTreasureList()
        {
            var panel = new HorizontalStackPanel
            {
                Spacing = 10
            };
            foreach (Treasure treasure in this.treasures)
            {
                Button card = this.CreateCard(treasure);
                this.cards.Add(card);
                panel.Widgets.Add(card);
            }
            this.cardsPanel = panel;

            this.scrollViewer = new HorizontalScrollViewer
            {
                Content = panel,
                ShowVerticalScrollBar = false,
                Width = DialogWidth - 44,
                // 多留出横向滚动条的高度
                Height = CardHeight + 20
            };
            return this.scrollViewer;
        }

        private Button CreateCard(Treasure treasure)
        {
            var content = new VerticalStackPanel
            {
                Spacing = 4,
                Padding = new Thickness(8)
            };

            Texture2D picture = LoadPicture(treasure);
            if (picture != null)
            {
                content.Widgets.Add(new Image
                {
                    Renderable = new TextureRegion(picture),
                    Width = PictureSize,
                    Height = PictureSize,
                    HorizontalAlignment = HorizontalAlignment.Center
                });
            }
            else
            {
                // 没有图片时留一块空白占位
                content.Widgets.Add(new Panel
                {
                    Width = PictureSize,
                    Height = PictureSize,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Background = new SolidBrush(LineColor)
                });
            }

            content.Widgets.Add(new Label
            {
                Text = treasure.Name,
                Font = TextManager.GetFont(18),
                TextColor = Color.Khaki,
                HorizontalAlignment = HorizontalAlignment.Center
            });
            content.Widgets.Add(new Label
            {
                Text = "价值 " + treasure.Worth,
                Font = TextManager.GetFont(15),
                TextColor = Color.Orange,
                HorizontalAlignment = HorizontalAlignment.Center
            });
            content.Widgets.Add(new Label
            {
                Text = string.Join("\n", GetInfluenceDescriptions(treasure)),
                Font = TextManager.GetFont(15),
                TextColor = Color.SkyBlue,
                Wrap = true,
                Width = CardWidth - 16,
                HorizontalAlignment = HorizontalAlignment.Center
            });
            if (!this.WillTakeEffect(treasure))
            {
                content.Widgets.Add(new Label
                {
                    Text = "（不生效）",
                    Font = TextManager.GetFont(15),
                    TextColor = Color.Gray,
                    HorizontalAlignment = HorizontalAlignment.Center
                });
            }

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
                    this.Select(treasure);
                }
            };
            return card;
        }

        private static Button CreateButton(string text, out Label label)
        {
            label = new Label
            {
                Text = text,
                Font = TextManager.GetFont(20),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            return new Button
            {
                Content = label,
                Width = 130,
                Height = 42,
                Background = new SolidBrush(CardColor),
                OverBackground = new SolidBrush(CardOverColor),
                PressedBackground = new SolidBrush(LineColor),
                FocusedBackground = new SolidBrush(CardColor),
                DisabledBackground = new SolidBrush(CardColor),
                Border = new SolidBrush(GoldColor),
                DisabledBorder = new SolidBrush(LineColor),
                BorderThickness = new Thickness(1)
            };
        }

        private static Texture2D LoadPicture(Treasure treasure)
        {
            try
            {
                string name = treasure.Picture.Name.ToString();
                if (!Platform.Current.FileExists(name))
                {
                    name = DefaultPicture;
                }
                return CacheManager.LoadTexture(name);
            }
            catch
            {
                return null;
            }
        }

        private static IEnumerable<string> GetInfluenceDescriptions(Treasure treasure)
        {
            foreach (var influence in treasure.Influences.Values)
            {
                yield return influence.Description;
            }
        }

        // 同类宝物只有一件生效，规则与Person.ApplyTreasure一致：价值高的生效，价值相同则ID小的生效
        private bool WillTakeEffect(Treasure treasure)
        {
            if (!this.person.effectiveTreasures.TryGetValue(treasure.TreasureGroup, out Treasure old))
            {
                return true;
            }
            return treasure.Worth > old.Worth || (treasure.Worth == old.Worth && treasure.ID < old.ID);
        }

        private void Select(Treasure treasure)
        {
            this.selected = treasure;
            for (int i = 0; i < this.cards.Count; i++)
            {
                bool isSelected = this.treasures[i] == treasure;
                this.cards[i].Border = new SolidBrush(isSelected ? GoldColor : LineColor);
            }

            this.okButton.Enabled = treasure != null;
            this.okLabel.TextColor = treasure != null ? Color.WhiteSmoke : Color.Gray;

            if (treasure == null)
            {
                this.selectedLabel.Text = this.treasures.Count > 0 ? "请选择要赏赐的宝物" : "";
                this.selectedLabel.TextColor = Color.Gray;
            }
            else if (this.WillTakeEffect(treasure))
            {
                this.selectedLabel.Text = "已选：" + treasure.Name + "　" + treasure.InfluenceString;
                this.selectedLabel.TextColor = Color.WhiteSmoke;
            }
            else
            {
                this.selectedLabel.Text = "已选：" + treasure.Name + "\n" + this.person.Name + "已有价值更高的同类宝物，赏赐后不会生效";
                this.selectedLabel.TextColor = Color.Orange;
            }
        }

        private void OkButton_Click(object sender, MyraEventArgs args)
        {
            if (this.selected == null)
            {
                return;
            }
            Treasure treasure = this.selected;
            this.Close();
            this.onConfirm(treasure);
        }

        // 按住卡片区域左右拖动来滑动列表，触屏上没有滚轮，主要靠这个
        private void Desktop_TouchDown(object sender, MyraEventArgs args)
        {
            this.dragDistance = 0;
            this.dragLastX = null;
            Point? position = MyraUI.Desktop.TouchPosition;
            if (position.HasValue && this.cardsPanel != null && this.scrollViewer.ContainsGlobalPoint(position.Value) && this.cardsPanel.ContainsGlobalPoint(position.Value))
            {
                this.dragLastX = position.Value.X;
            }
        }

        private void Desktop_TouchMoved(object sender, MyraEventArgs args)
        {
            Point? position = MyraUI.Desktop.TouchPosition;
            if (!this.dragLastX.HasValue || !position.HasValue)
            {
                return;
            }
            int delta = position.Value.X - this.dragLastX.Value;
            this.dragLastX = position.Value.X;
            this.dragDistance += Math.Abs(delta);
            if (this.dragDistance > DragThreshold)
            {
                this.scrollViewer.ScrollBy(-delta);
            }
        }

        private void Desktop_TouchUp(object sender, MyraEventArgs args)
        {
            this.dragLastX = null;
        }

        // Myra自带的滚动容器只让滚轮控制纵向滚动，这里改成滚轮横向滚动
        private class HorizontalScrollViewer : ScrollViewer
        {
            private const int WheelStep = 80;

            protected override bool AcceptsMouseWheel => true;

            public override void OnMouseWheel(float delta)
            {
                if (delta != 0)
                {
                    this.ScrollBy(delta < 0 ? WheelStep : -WheelStep);
                }
            }

            public void ScrollBy(int offset)
            {
                Point position = this.ScrollPosition;
                position.X = MathHelper.Clamp(position.X + offset, 0, this.ScrollMaximum.X);
                this.ScrollPosition = position;
            }
        }
    }
}

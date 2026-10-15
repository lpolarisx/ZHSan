using GameGlobal;
using GameManager;
using GameObjects;
using GameObjects.FactionDetail;
using Microsoft.Xna.Framework;
using Myra.Graphics2D;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.UI;
using System;

namespace WorldOfTheThreeKingdoms.GameScreens
{
    /// <summary>
    /// AI遣使来访：列出对方的条款，玩家选择接受、还价或回绝。还价打开外交面板，从来使的条款改起
    /// </summary>
    internal class DiplomacyProposalDialog
    {
        private const int DialogWidth = 520;
        private const int DialogPadding = 28;
        private const int PortraitWidth = 96;
        private const int PortraitHeight = 120;
        private const int ButtonWidth = 90;
        private const int TextWidth = DialogWidth - 2 * DialogPadding - PortraitWidth - 16;

        private readonly DiplomacyDeal proposal;

        private DiplomacyProposalDialog(DiplomacyDeal proposal)
        {
            this.proposal = proposal;
        }

        /// <summary>
        /// 显示来使的提议。提议已经无法履行（例如对方的钱粮已经花掉）时不显示，返回false
        /// </summary>
        public static bool Show(DiplomacyDeal proposal)
        {
            if (proposal.Source.Leader == null || proposal.Source.Capital == null || proposal.Target.Capital == null)
            {
                return false;
            }
            var dialog = new DiplomacyProposalDialog(proposal);
            Session.MainGame.mainGameScreen.PushUndoneWork(new UndoneWorkItem(UndoneWorkKind.SubDialog, DialogKind.Diplomacy));
            MyraUI.Show(dialog.Build(), () => dialog.Close(dialog.Reject));
            return true;
        }

        private void Close(Action afterClose)
        {
            MyraUI.Close(() =>
            {
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
                Padding = new Thickness(DialogPadding, 7, DialogPadding, DialogPadding),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Background = MyraSkin.PanelBackground
            };
            dialog.Widgets.Add(new Label
            {
                Text = "来使",
                Font = TextManager.GetFont(24),
                TextColor = MyraSkin.TextTitle,
                HorizontalAlignment = HorizontalAlignment.Center
            });

            var body = new HorizontalStackPanel
            {
                Spacing = 16
            };
            body.Widgets.Add(new Image
            {
                Renderable = new DiplomacyDialog.PortraitImage(this.proposal.Source.Leader, PortraitSize.Small, PortraitWidth, PortraitHeight),
                Width = PortraitWidth,
                Height = PortraitHeight,
                VerticalAlignment = VerticalAlignment.Top
            });

            var text = new VerticalStackPanel
            {
                Width = TextWidth,
                Spacing = 6
            };
            // 只送不取的是援助：交好的势力见我方缺钱缺粮，送来接济
            bool gift = DiplomacyEvaluator.IsPureGift(this.proposal);
            text.Widgets.Add(CreateText(this.proposal.Source.Name + (gift ? "听闻我方钱粮不济，遣使送来接济：" : "遣使来访，提出以下条款："), MyraSkin.TextPrimary));
            // 来使的条款从玩家这边看：对方给予的是提议方给出的，对方索要的是提议方索要的
            text.Widgets.Add(CreateTermRow("对方给予", this.proposal.DescribeSide(true)));
            text.Widgets.Add(CreateTermRow("对方索要", this.proposal.DescribeSide(false)));
            text.Widgets.Add(CreateTermRow("双方约定", this.proposal.DescribeMutual()));
            string reason = this.proposal.GetInvalidReason(false);
            if (reason != null)
            {
                text.Widgets.Add(CreateText("※ " + reason, MyraSkin.TextWarning));
            }
            body.Widgets.Add(text);
            dialog.Widgets.Add(body);

            var buttons = new HorizontalStackPanel
            {
                Spacing = 12,
                HorizontalAlignment = HorizontalAlignment.Right
            };
            Button accept = MyraSkin.CreateButton("接受", ButtonWidth, out Label acceptLabel);
            accept.Enabled = reason == null;
            acceptLabel.TextColor = reason == null ? MyraSkin.TextPrimary : MyraSkin.TextSecondary;
            accept.Click += (sender, args) => this.Close(this.Accept);
            Button counter = MyraSkin.CreateButton("还价", ButtonWidth, out _);
            counter.Click += (sender, args) => this.Close(this.Counter);
            Button reject = MyraSkin.CreateButton("回绝", ButtonWidth, out _);
            reject.Click += (sender, args) => this.Close(this.Reject);
            buttons.Widgets.Add(accept);
            // 白送的没什么可还价的
            if (!gift)
            {
                buttons.Widgets.Add(counter);
            }
            buttons.Widgets.Add(reject);
            dialog.Widgets.Add(buttons);

            var mask = new Panel
            {
                Background = new SolidBrush(new Color(0, 0, 0, 150))
            };
            mask.Widgets.Add(dialog);
            return mask;
        }

        private static Label CreateText(string text, Color color)
        {
            return new Label
            {
                Text = text,
                Font = TextManager.GetFont(16),
                TextColor = color,
                Wrap = true,
                Width = TextWidth
            };
        }

        private static Widget CreateTermRow(string title, string terms)
        {
            var row = new HorizontalStackPanel
            {
                Spacing = 8
            };
            row.Widgets.Add(new Label
            {
                Text = title,
                Font = TextManager.GetFont(16),
                TextColor = MyraSkin.TextSecondary,
                Width = 72
            });
            row.Widgets.Add(new Label
            {
                Text = terms,
                Font = TextManager.GetFont(16),
                TextColor = terms == "无" ? MyraSkin.TextSecondary : MyraSkin.TextHighlight,
                Wrap = true,
                Width = TextWidth - 80
            });
            return row;
        }

        private void Accept()
        {
            // 弹窗期间钱粮可能有变，交割前再查一次
            if (this.proposal.GetInvalidReason(false) != null)
            {
                return;
            }
            ShowConcluded(this.proposal.Source, this.proposal.Target, this.proposal.Execute(false));
        }

        private void Counter()
        {
            DiplomacyDialog.ShowCounter(this.proposal, deal =>
            {
                // 对方使者就在都城，谈成当场交割
                ShowConcluded(this.proposal.Source, this.proposal.Target, deal.Execute(false));
            }, this.Reject);
        }

        // 回绝伤关系，回绝对方的索要伤得更重
        private void Reject()
        {
            var relation = this.proposal.GetRelation();
            if (relation == null)
            {
                return;
            }
            bool demand = this.proposal.RequestFund > 0 || this.proposal.RequestFood > 0 || this.proposal.RequestArchitectures.Count > 0;
            relation.Worsen(demand ? 20 : 5);
        }

        private static void ShowConcluded(Faction source, Faction target, string result)
        {
            Session.MainGame.mainGameScreen.xianshishijiantupian(source.Leader, result, "NegotiationConcluded", "AllyDiplomaticRelation.jpg", "TruceDiplomaticRelation", target.Name, true);
        }
    }
}

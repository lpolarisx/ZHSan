using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using GameFreeText;
using GameGlobal;
using GameObjects;
using GameObjects.FactionDetail;
using GameObjects.PersonDetail;
using GameObjects.SectionDetail;
using GameObjects.TroopDetail;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PluginInterface;
using WorldOfTheThreeKingdoms.GameLogic;
using WorldOfTheThreeKingdoms.GameScreens;
using WorldOfTheThreeKingdoms.GameScreens.ScreenLayers;
using WorldOfTheThreeKingdoms.Resources;
using System.Diagnostics;
using GameManager;

//using GameObjects.PersonDetail.PersonMessages;

namespace WorldOfTheThreeKingdoms.GameScreens
{
    partial class MainGameScreen : Screen
    {

        private void HandleKey(GameTime gameTime)
        {
            if (this.currentKey != Keys.None)
            {
                //if (!base.KeyState.IsKeyUp(this.currentKey))
                if (!InputManager.KeyBoardState.IsKeyUp(this.currentKey))
                {
                    return;
                }
                this.currentKey = Keys.None;
            }
            if ((Session.Current.Scenario.CurrentPlayer != null) && Session.Current.Scenario.CurrentPlayer.Controlling)
            {
                if (InputManager.KeyBoardState.IsKeyDown(Keys.D1))
                {
                    this.currentKey = Keys.D1;
                    if (this.editMode)
                    {
                        this.ditukuaidezhi = 1;
                    }
                    else
                    {
                        this.DateGo(1);
                    }
                }
                else if (InputManager.KeyBoardState.IsKeyDown(Keys.D2))
                {
                    this.currentKey = Keys.D2;
                    if (this.editMode)
                    {
                        this.ditukuaidezhi = 2;
                    }
                    else
                    {
                        this.DateGo(2);
                    }
                }
                else if (InputManager.KeyBoardState.IsKeyDown(Keys.D3))
                {
                    this.currentKey = Keys.D3;
                    if (this.editMode)
                    {
                        this.ditukuaidezhi = 3;
                    }
                    else
                    {
                        this.DateGo(3);
                    }
                }
                else if (InputManager.KeyBoardState.IsKeyDown(Keys.D4))
                {
                    this.currentKey = Keys.D4;
                    if (this.editMode)
                    {
                        this.ditukuaidezhi = 4;
                    }
                    else
                    {
                        this.DateGo(4);
                    }
                }
                else if (InputManager.KeyBoardState.IsKeyDown(Keys.D5))
                {
                    this.currentKey = Keys.D5;
                    if (this.editMode)
                    {
                        this.ditukuaidezhi = 5;
                    }
                    else
                    {
                        this.DateGo(5);
                    }
                }
                else if (InputManager.KeyBoardState.IsKeyDown(Keys.D6))
                {
                    this.currentKey = Keys.D6;
                    if (this.editMode)
                    {
                        this.ditukuaidezhi = 6;
                    }
                    else
                    {
                        this.DateGo(6);
                    }
                }
                else if (InputManager.KeyBoardState.IsKeyDown(Keys.D7))
                {
                    this.currentKey = Keys.D7;
                    if (this.editMode)
                    {
                        this.ditukuaidezhi = 7;
                    }
                    else
                    {
                        this.DateGo(7);
                    }
                }
                else if (InputManager.KeyBoardState.IsKeyDown(Keys.D8))
                {
                    this.currentKey = Keys.D8;
                    if (this.editMode)
                    {
                        this.ditukuaidezhi = 8;
                    }
                    else
                    {
                        this.DateGo(8);
                    }
                }
                else if (InputManager.KeyBoardState.IsKeyDown(Keys.D9))
                {
                    this.currentKey = Keys.D9;
                    if (this.editMode)
                    {
                        this.ditukuaidezhi = 9;
                    }
                    else
                    {
                        this.DateGo(9);
                    }
                }
                else if (InputManager.KeyBoardState.IsKeyDown(Keys.D0))
                {
                    this.currentKey = Keys.D0;
                    if (this.editMode)
                    {
                        this.ditukuaidezhi = 10;
                    }
                    else
                    {
                        this.DateGo(10);
                    }
                }
                else if (InputManager.KeyBoardState.IsKeyDown(Keys.F1))
                {
                    this.currentKey = Keys.F1;
                    if (!this.editMode)
                    {
                        this.DateGo(30);
                    }
                }
                else if (InputManager.KeyBoardState.IsKeyDown(Keys.F2))
                {
                    this.currentKey = Keys.F2;
                    if (!this.editMode)
                    {
                        this.DateGo(60);
                    }
                }
                else if (InputManager.KeyBoardState.IsKeyDown(Keys.F3))
                {
                    this.currentKey = Keys.F3;
                    if (!this.editMode)
                    {
                        this.DateGo(90);
                    }
                }
                else if (InputManager.KeyBoardState.IsKeyDown(Keys.F5))
                {
                    this.currentKey = Keys.F5;
                    if (!this.editMode)
                    {
                        this.DateGo(-999);
                    }
                }
                else if (InputManager.KeyBoardState.IsKeyDown(Keys.Tab))
                {
                    this.currentKey = Keys.Tab;
                    if (!this.editMode)
                    {
                        bool shift = InputManager.KeyBoardState.IsKeyDown(Keys.LeftShift) || InputManager.KeyBoardState.IsKeyDown(Keys.RightShift);
                        this.JumpToAdjacentUnoperatedTroop(!shift, true);
                    }
                }
                else if (InputManager.KeyBoardState.IsKeyDown(Keys.Q))
                {
                    this.currentKey = Keys.Q;
                    Session.GlobalVariables.ShowGrid = !Session.GlobalVariables.ShowGrid;
                }
                else if (InputManager.KeyBoardState.IsKeyDown(Keys.LeftAlt) && InputManager.KeyBoardState.IsKeyDown(Keys.Z))
                {
                    Session.GlobalVariables.EnableCheat = true;
                }
                else if (InputManager.KeyBoardState.IsKeyDown(Keys.LeftAlt) && InputManager.KeyBoardState.IsKeyDown(Keys.C) && Session.GlobalVariables.EnableCheat)
                {
                    this.currentKey = Keys.C;
                    if (!this.editMode)
                    {
                        ChangeFaction();
                    }
                }
                else if (InputManager.KeyBoardState.IsKeyDown(Keys.LeftAlt) && InputManager.KeyBoardState.IsKeyDown(Keys.E) && Session.GlobalVariables.EnableCheat)
                {
                    this.currentKey = Keys.E;
                    this.editMode = true;
                    this.mainMapLayer.xianshidituxiaokuai = true;
                    this.Plugins.youcelanPlugin.IsShowing = false;
                    this.mapEdited = true;
                }
                else if (InputManager.KeyBoardState.IsKeyDown(Keys.LeftAlt) && InputManager.KeyBoardState.IsKeyDown(Keys.F) && Session.GlobalVariables.EnableCheat)
                {
                    this.currentKey = Keys.F;//q已经被网格占用，改为f关闭
                    this.editMode = false;
                    this.mainMapLayer.xianshidituxiaokuai = false;
                    this.Plugins.youcelanPlugin.IsShowing = true;
                    this.mapEdited = true;
                }
                else if (InputManager.KeyBoardState.IsKeyDown(Keys.T))
                {
                    this.currentKey = Keys.T;
                    if (this.editMode)
                    {
                        this.mainMapLayer.xianshidituxiaokuai = !this.mainMapLayer.xianshidituxiaokuai;
                    }
                }
                else if (InputManager.KeyBoardState.IsKeyDown(Keys.L))
                {
                    this.currentKey = Keys.L;
                    this.ShowArchitectureConnectedLine = !this.ShowArchitectureConnectedLine;
                }
                else if (InputManager.KeyBoardState.IsKeyDown(Keys.B))
                {
                    this.currentKey = Keys.B;
                    this.ShowFactionBorder = !this.ShowFactionBorder;
                }
            }
            if (InputManager.KeyBoardState.IsKeyDown(Keys.Space))
            {

                this.currentKey = Keys.Space;
                if (!this.editMode)
                {
                    this.Plugins.DateRunnerPlugin.Run();
                }
            }
            else if (InputManager.KeyBoardState.IsKeyDown(Keys.W))
            {
                this.currentKey = Keys.W;
            }
            else if (InputManager.KeyBoardState.IsKeyDown(Keys.A))
            {
                this.currentKey = Keys.A;
            }
            else if (InputManager.KeyBoardState.IsKeyDown(Keys.S))
            {
                this.currentKey = Keys.S;
            }
            else if (InputManager.KeyBoardState.IsKeyDown(Keys.D))
            {
                this.currentKey = Keys.D;
            }
            else if (InputManager.KeyBoardState.IsKeyDown(Keys.OemPlus) || InputManager.KeyBoardState.IsKeyDown(Keys.Add))
            {
                this.currentKey = Keys.OemPlus;
            }
            else if (InputManager.KeyBoardState.IsKeyDown(Keys.OemMinus) || InputManager.KeyBoardState.IsKeyDown(Keys.Subtract))
            {
                this.currentKey = Keys.OemMinus;
            } 
            //if (InputManager.KeyBoardState.IsKeyDown(Keys.Z) && InputManager.KeyBoardState.IsKeyDown(Keys.LeftControl))
            //{

            //    this.currentKey = Keys.Z;
            //    this.Player.settings.volume += 10;
            //}
            //if (InputManager.KeyBoardState.IsKeyDown(Keys.X) && InputManager.KeyBoardState.IsKeyDown(Keys.LeftControl))
            //{

            //    this.currentKey = Keys.X;
            //    this.Player.settings.volume -= 10;
            //}
        }

        /// <summary>
        /// 按势力部队顺序跳到下一支（或上一支）未行动的部队，到头后从另一端继续
        /// </summary>
        /// <param name="openMenu">跳过去后顺便打开该部队的菜单（Tab快捷键用）</param>
        public void JumpToAdjacentUnoperatedTroop(bool next, bool openMenu = false)
        {
            Faction faction = Session.Current.Scenario.CurrentPlayer;
            if (faction == null || faction.Troops.Count == 0)
            {
                return;
            }
            int count = faction.Troops.Count;
            // troopSequence记的是上次跳到的部队在Troops中的位置，-1表示本回合还没跳过
            int start = faction.troopSequence;
            if (start < 0 || start >= count)
            {
                start = next ? -1 : count;
            }
            for (int i = 1; i <= count; i++)
            {
                int index = ((start + (next ? i : -i)) % count + count) % count;
                Troop troop = faction.Troops[index] as Troop;
                if (troop != null && !troop.Operated)
                {
                    // 只高亮当前这一支，之前跳过的部队取消高亮
                    foreach (Troop t in faction.Troops)
                    {
                        t.DrawSelected = false;
                    }
                    faction.troopSequence = index;
                    troop.DrawSelected = true;
                    this.JumpTo(troop.Position);
                    if (openMenu)
                    {
                        this.OpenTroopMenu(troop);
                    }
                    return;
                }
            }
        }

        private void OpenTroopMenu(Troop troop)
        {
            if (this.Plugins.ContextMenuPlugin.IsShowing || !Session.Current.Scenario.IsPlayerControlling())
            {
                return;
            }
            this.CurrentTroop = troop;
            Rectangle destination = this.GetDestination(troop.Position);
            this.Plugins.ContextMenuPlugin.IsShowing = true;
            this.Plugins.ContextMenuPlugin.SetCurrentGameObject(troop);
            this.Plugins.ContextMenuPlugin.SetMenuKindByName("TroopLeftClick");
            this.Plugins.ContextMenuPlugin.Prepare(destination.Right, destination.Top, base.viewportSize);
            this.bianduiLiebiaoBiaoji = "TroopLeftClick";
        }

        /// <summary>
        /// 部队菜单打开时的快捷键：Tab/Shift+Tab换到下一支/上一支部队，1~9用第N个战法，R用该部队上次的战法
        /// </summary>
        private void HandleContextMenuKey()
        {
            if (this.currentKey != Keys.None)
            {
                if (!InputManager.KeyBoardState.IsKeyUp(this.currentKey))
                {
                    return;
                }
                this.currentKey = Keys.None;
            }
            if (this.bianduiLiebiaoBiaoji != "TroopLeftClick" || this.CurrentTroop == null)
            {
                return;
            }
            KeyboardState keyboard = InputManager.KeyBoardState;
            if (keyboard.IsKeyDown(Keys.Tab))
            {
                this.currentKey = Keys.Tab;
                bool shift = keyboard.IsKeyDown(Keys.LeftShift) || keyboard.IsKeyDown(Keys.RightShift);
                this.Plugins.ContextMenuPlugin.Close(ContextMenuResult.None, 0);
                this.JumpToAdjacentUnoperatedTroop(!shift, true);
                return;
            }
            if (keyboard.IsKeyDown(Keys.R))
            {
                this.currentKey = Keys.R;
                if (this.CurrentTroop.LastCombatMethodID == -1)
                {
                    this.ShowTroopHint(this.CurrentTroop, "部队还没有用过战法");
                }
                else
                {
                    this.UseCombatMethodByKey(this.CurrentTroop.LastCombatMethodID);
                }
                return;
            }
            for (int i = 1; i <= 9; i++)
            {
                Keys digit = Keys.D0 + i;
                Keys numPad = Keys.NumPad0 + i;
                if (keyboard.IsKeyDown(digit) || keyboard.IsKeyDown(numPad))
                {
                    this.currentKey = keyboard.IsKeyDown(digit) ? digit : numPad;
                    List<CombatMethod> combatMethods = this.CurrentTroop.GetOrderedCombatMethods();
                    if (i > combatMethods.Count)
                    {
                        this.ShowTroopHint(this.CurrentTroop, "部队没有第" + i + "个战法");
                    }
                    else
                    {
                        this.UseCombatMethodByKey(combatMethods[i - 1].ID);
                    }
                    return;
                }
            }
        }

        /// <summary>
        /// 能用就关闭菜单，与点击菜单里的战法效果相同；不能用就提示原因，菜单保持打开
        /// </summary>
        private void UseCombatMethodByKey(int id)
        {
            string failReason = this.CurrentTroop.GetCombatMethodFailReason(id);
            if (failReason != null)
            {
                this.ShowTroopHint(this.CurrentTroop, failReason);
                return;
            }
            this.Plugins.ContextMenuPlugin.Close(ContextMenuResult.TroopCombatMethod, id);
        }

        private void ShowTroopHint(Troop troop, string text)
        {
            this.Plugins.PersonBubblePlugin.AddPersonText(troop.Leader, troop.Position, text);
        }


    }
}

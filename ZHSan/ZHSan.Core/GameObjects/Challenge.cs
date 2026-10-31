using GameGlobal;
using GameObjects.Animations;
using GameObjects.Influences;
using GameObjects.MapDetail;
using GameObjects.PersonDetail;
using GameObjects.TroopDetail;
using GameObjects.FactionDetail;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Diagnostics;
using GameManager;
using WorldOfTheThreeKingdoms.GameScreens.ScreenLayers;
using System.Linq;

namespace GameObjects
{
    /// <summary>
    /// 单挑里的一段1对1交手。Source为发起方（左），Destination为应战方（右），Result同HandleChallengeResult的结果码
    /// </summary>
    public class ChallengeBout
    {
        public Person Source;
        public Person Destination;
        public int Result;
        public bool Rescued;  //败走的一方是被己方援将救回的
    }

	public class Challenge
	{
        private bool ChallengeOftenShow = false;  //暴击必然触发单挑，调试单挑程序用，默认为false


        public void ChallgenEvent(Troop sourceTroop, Troop troop, TroopDamage damage)
        {
            //攻击被格挡时HandleTroopDamage不处理单挑结果，不发起单挑
            if (damage.AntiAttack || damage.AntiArrowAttack) return;

            //一次攻击命中多支部队也只单挑一场；演示中的单挑未结束时，再开一场会把它的结果覆盖掉
            if (sourceTroop.ChallengedInThisAttack || Session.MainGame.mainGameScreen.dantiaoLayer != null) return;

            if ((!sourceTroop.IsFriendly(troop.BelongedFaction) && !sourceTroop.AirOffence) && (this.ChallengeOftenShow || GameObject.GetChance(20)))
            {
                Person maxStrengthPerson = sourceTroop.Persons.MaxBy(x => x.Strength);
                Person destination = troop.Persons.MaxBy(x => x.Strength);
                if (((maxStrengthPerson != null) && (destination != null)) && (this.ChallengeOftenShow || (GameObject.Random(GameObject.Square(destination.Calmness)) < GameObject.Random(0x19))))
                {
                    if (maxStrengthPerson.IsCivil() || destination.IsCivil())  //文官不单挑
                    {
                        return;
                    }
                    int chance = Person.ChanlengeWinningChance(maxStrengthPerson, destination);
                    if (this.ChallengeOftenShow || (maxStrengthPerson.Character.ChallengeChance + chance) >= 60)
                    {
                        var sourceReserves = PickReserves(maxStrengthPerson, sourceTroop.Persons);
                        var destinationReserves = PickReserves(destination, troop.Persons);
                        sourceTroop.ChallengedInThisAttack = true;
                        this.challengeHappen(damage, maxStrengthPerson, destination, chance, sourceReserves, destinationReserves);
                    }
                }
            }
        }

        /// <summary>
        /// 候补武将，最多两名：除主将外武力最高的一名，再加一名与主将关系最亲的（没有亲近的就取武力次高的）。文官不上阵
        /// </summary>
        public static List<Person> PickReserves(Person main, IEnumerable<Person> pool)
        {
            var candidates = pool.Where(p => p != null && p != main && !p.IsCivil()).ToList();
            var reserves = new List<Person>();

            var strongest = candidates.OrderByDescending(p => p.Strength).FirstOrDefault();
            if (strongest == null) return reserves;
            reserves.Add(strongest);

            var rest = candidates.Where(p => p != strongest).ToList();
            var closest = rest.Where(p => DantiaoRules.Bond(p, main) > 0)
                .OrderByDescending(p => DantiaoRules.Bond(p, main)).ThenByDescending(p => p.Strength).FirstOrDefault()
                ?? rest.OrderByDescending(p => p.Strength).FirstOrDefault();
            if (closest != null) reserves.Add(closest);

            return reserves;
        }

        static readonly Random rng = new Random();

        /// <summary>
        /// 不演示时的结算：一段段按胜率判胜负，败将可能被候补救走，援将武力够的接着打
        /// </summary>
        static void SimulateBouts(TroopDamage damage, Person source, Person destination, List<Person> sourceReserves, List<Person> destinationReserves)
        {
            var reserves = new[] { new List<Person>(sourceReserves), new List<Person>(destinationReserves) };
            var fighters = new[] { source, destination };
            var fought = new[] { 0, 0 };  //连战的段数，打得越久越吃亏
            damage.ChallengeBouts.Clear();

            while (true)
            {
                var chance = Person.ChanlengeWinningChance(fighters[0], fighters[1]) + (fought[1] - fought[0]) * 10;
                var loserSide = GameObject.GetChance(chance) ? 1 : 0;
                var winner = fighters[1 - loserSide];
                var loser = fighters[loserSide];
                var bout = new ChallengeBout() { Source = fighters[0], Destination = fighters[1] };
                damage.ChallengeBouts.Add(bout);

                //只是被打下马，不像演示里那样性命攸关，来救的少些
                var enemyForce = winner.ChallengeStrength - fought[1 - loserSide] * 5;
                var helper = DantiaoRules.PickRescuer(reserves[loserSide], loser, enemyForce, 0.5f, rng, out _);
                if (helper == null)
                {
                    bout.Result = loserSide == 0 ? 2 : 1;
                    return;
                }

                reserves[loserSide].Remove(helper);
                bout.Result = loserSide == 0 ? 5 : 6;
                bout.Rescued = true;
                if (!DantiaoRules.PrefersRelay(helper, enemyForce)) return;

                fighters[loserSide] = helper;
                fought[loserSide] = 0;
                fought[1 - loserSide]++;
            }
        }

        private void challengeHappen(TroopDamage damage, Person maxStrengthPerson, Person destination, int chance, List<Person> sourceReserves, List<Person> destinationReserves)
        {
            int flag = 0;
            //damage.ChallengeHappened = true;  //单挑发生
            if ((Setting.Current.GlobalVariables.ShowChallengeAnimation) &&   //Session.GlobalVariables.ShowChallengeAnimation
                (Session.Current.Scenario.IsPlayer(maxStrengthPerson.BelongedFaction) || Session.Current.Scenario.IsPlayer(destination.BelongedFaction) || (Session.GlobalVariables.SkyEye && !Session.GlobalVariables.SkyEyeSimpleNotification) || this.ChallengeOftenShow))  //单挑双方有玩家的武将才演示
            {

                try
                {
                    Session.MainGame.mainGameScreen.EnableUpdate = false;

                    challengeShow(damage, maxStrengthPerson, destination, sourceReserves, destinationReserves);

                    //int returnValue;
                    //returnValue = this.challengeShow(maxStrengthPerson, destination);
                    //returnValue = 10;

                    //if (returnValue >= -4 && returnValue <= 10 && returnValue != 0)
                    //{
                    //    flag = returnValue;
                    //}
                    //else   //返回值出错时避免跳出
                    //{
                    //    flag = (GameObject.Chance(chance) ? 1 : 2);
                    //}

                }
                catch
                {
                    SimulateBouts(damage, maxStrengthPerson, destination, sourceReserves, destinationReserves);
                    damage.ChallengeHappened = true;  //单挑发生
                }
            }
            else
            {
                SimulateBouts(damage, maxStrengthPerson, destination, sourceReserves, destinationReserves);
                damage.ChallengeHappened = true;  //单挑发生
            }

            //flag = -4;
            if (damage.ChallengeHappened)
            {
                var last = damage.ChallengeBouts.LastOrDefault();
                damage.ChallengeResult = last?.Result ?? flag;
                damage.ChallengeSourcePerson = last?.Source ?? maxStrengthPerson;
                damage.ChallengeDestinationPerson = last?.Destination ?? destination;
            }

        }

        private string challengePersonString(Person person)
        {
            //武将ID,姓,名,字,性别(0,女、1,男),头像编号,身份(0 盗贼，1 武将，2君主)
            //生命,体力,力量,敏捷,
            //武艺,统御,智谋,政治,魅力,
            //相性,勇猛,冷静,义理,野心,名声,
            //坐骑(1、没有马；300、赤兔马；301、的卢；302、绝影；303、爪黄飞电；304、大宛马),
            //忠诚度,当前所属势力声望,
            //当前效力君主的魅力(没有君主传-1),当前效力君主的相性(没有君主传-1)


            string para;
            para = person.ID.ToString() + "," + person.SurName + "," + person.GivenName + "," + (person.CalledName == "" ? "无" : person.CalledName) + "," + (person.Sex ? "0" : "1") + "," + person.PictureIndex.ToString() + ","+person.Identity().ToString()+",";
            para += person.ChallengeStrength.ToString() + "," + person.ChallengeStrength.ToString() + "," + person.ChallengeStrength.ToString() + "," + person.ChallengeStrength.ToString() + ",";
            para += person.ChallengeStrength.ToString() + "," + person.Command.ToString() + "," + person.Intelligence.ToString() + "," + person.Politics.ToString() + "," + person.Glamour.ToString() + ",";
            para += person.Ideal.ToString() + "," + person.Braveness.ToString() + "," + person.Calmness.ToString() + "," + person.PersonalLoyalty.ToString() + "," + person.Ambition.ToString() + "," + person.Reputation.ToString() + ",";
            ///////////////////////////////判断有没有宝物马
            if (person.HasHorse() == -1)  //游戏中用-1表示没有马，而单挑程序中用1表示没有马
            {
                para += "1" + ",";
            }
            else
            {
                para += person.HasHorse().ToString() + ",";
            }
            /////////////////////////////////
            para += person.Loyalty.ToString() + "," + (person.BelongedFaction == null ? 0 : person.BelongedFaction.Reputation).ToString()+",";
            para += (person.BelongedFaction == null ? -1 : person.BelongedFaction.Leader.Glamour).ToString() + "," + (person.BelongedFaction == null ? -1 : person.BelongedFaction.Leader.Ideal).ToString();
            para += "\r\n";
            return para;
        }

        private void challengeShow(TroopDamage damage, Person maxStrengthPerson, Person destination, List<Person> sourceReserves, List<Person> destinationReserves)
        {
            damage.ChallengeStarted = true;

            Session.MainGame.mainGameScreen.cloudLayer.Reverse = true;

            Session.MainGame.mainGameScreen.cloudLayer.Start();

            Session.MainGame.mainGameScreen.dantiaoLayer = new DantiaoLayer(maxStrengthPerson, destination, sourceReserves, destinationReserves);

            Session.MainGame.mainGameScreen.dantiaoLayer.damage = damage;

            //throw new NotImplementedException("跨平台單挑程序尚未實現！");

            /////////////////////////////////////////////////调用单挑程序
            //string fileName = @"Dantiao\start.exe";

            //string para = "";
            //para += this.challengePersonString(maxStrengthPerson);
            //para += this.challengePersonString(destination);

            //Process myProcess = new Process();
            //ProcessStartInfo myProcessStartInfo = new ProcessStartInfo(fileName, para);
            //myProcess.StartInfo = myProcessStartInfo;
            //myProcess.Start();
            //while (!myProcess.HasExited)
            //{

            //    myProcess.WaitForExit();

            //}

            //return myProcess.ExitCode;
        }

        /// <summary>
        /// 几段交手各自结算后，士气、战意的总变化不超过这个数
        /// </summary>
        const int MaxBoutsChange = 40;

        /// <summary>
        /// 按交手记录逐段结算，没有记录时按damage上的那一对结算
        /// </summary>
        public void HandleChallengeResults(TroopDamage damage)
        {
            var bouts = damage.ChallengeBouts.Count > 0 ? damage.ChallengeBouts.ToList()
                : new List<ChallengeBout>() { new ChallengeBout() { Source = damage.ChallengeSourcePerson, Destination = damage.ChallengeDestinationPerson, Result = damage.ChallengeResult } };

            var sourceMorale = damage.SourceMoraleChange;
            var destinationMorale = damage.DestinationMoraleChange;
            var sourceCombativity = damage.SourceCombativityChange;
            var destinationCombativity = damage.DestinationCombativityChange;

            foreach (var bout in bouts)
            {
                HandleChallengeResult(damage, bout.Result, damage.SourceTroop, bout.Source, damage.DestinationTroop, bout.Destination, bout.Rescued);
            }

            if (bouts.Count > 1)
            {
                damage.SourceMoraleChange = sourceMorale + Math.Clamp(damage.SourceMoraleChange - sourceMorale, -MaxBoutsChange, MaxBoutsChange);
                damage.DestinationMoraleChange = destinationMorale + Math.Clamp(damage.DestinationMoraleChange - destinationMorale, -MaxBoutsChange, MaxBoutsChange);
                damage.SourceCombativityChange = sourceCombativity + Math.Clamp(damage.SourceCombativityChange - sourceCombativity, -MaxBoutsChange, MaxBoutsChange);
                damage.DestinationCombativityChange = destinationCombativity + Math.Clamp(damage.DestinationCombativityChange - destinationCombativity, -MaxBoutsChange, MaxBoutsChange);
            }
        }

        /// <param name="rescued">败走的一方是被己方援将救回的，士气损失减半</param>
        public void HandleChallengeResult(TroopDamage damage, int result, Troop sourceTroop, Person sourcePerson, Troop destinationTroop, Person destinationPerson, bool rescued = false)
        {
            Session.MainGame.mainGameScreen.TroopPersonChallenge(result, sourceTroop, sourcePerson, destinationTroop, destinationPerson);

            switch (result)
            {
                case 1: //P1武将胜利
                    Session.Current.Scenario.YearTable.addChallengeEntry(Session.Current.Scenario.Date, sourcePerson, destinationPerson, "被打下馬");
                    damage.SourceMoraleChange += 20;
                    damage.DestinationMoraleChange -= 20;
                    damage.SourceCombativityChange += 20;
                    damage.DestinationCombativityChange -= 20;  //第2只军队战意下降
                    break;
                case 2: //2：P2武将胜利
                    Session.Current.Scenario.YearTable.addChallengeEntry(Session.Current.Scenario.Date, destinationPerson, sourcePerson, "被打下馬");
                    damage.SourceMoraleChange -= 20;
                    damage.DestinationMoraleChange += 20;
                    damage.SourceCombativityChange -= 20;
                    damage.DestinationCombativityChange += 20;
                    break;
                case 3: //3：P1武将被杀
                    Session.Current.Scenario.YearTable.addChallengeEntry(Session.Current.Scenario.Date, destinationPerson, sourcePerson, "被擊殺");
                    this.challengePersonDie(sourcePerson,sourceTroop, destinationPerson);
                    damage.SourceMoraleChange -= 30;
                    damage.DestinationMoraleChange += 30;
                    damage.SourceCombativityChange -= 30;
                    damage.DestinationCombativityChange += 30;
                    break;
                case 4: //4：P2武将被杀
                    Session.Current.Scenario.YearTable.addChallengeEntry(Session.Current.Scenario.Date, sourcePerson, destinationPerson, "被擊殺");
                    this.challengePersonDie(destinationPerson, destinationTroop, sourcePerson);
                    damage.SourceMoraleChange += 30;
                    damage.DestinationMoraleChange -= 30;
                    damage.SourceCombativityChange += 30;
                    damage.DestinationCombativityChange -= 30;
                    break;
                case 5: //5：P1武将逃跑
                    Session.Current.Scenario.YearTable.addChallengeEntry(Session.Current.Scenario.Date, destinationPerson, sourcePerson, rescued ? "被救回" : "逃跑");
                    damage.SourceMoraleChange -= rescued ? 10 : 20;
                    damage.DestinationMoraleChange += rescued ? 10 : 20;
                    break;
                case 6: //6：P2武将逃跑
                    Session.Current.Scenario.YearTable.addChallengeEntry(Session.Current.Scenario.Date, sourcePerson, destinationPerson, rescued ? "被救回" : "逃跑");
                    damage.SourceMoraleChange += rescued ? 10 : 20;
                    damage.DestinationMoraleChange -= rescued ? 10 : 20;
                    break;
                case 7: //7、P1武将被俘虏
                    Session.Current.Scenario.YearTable.addChallengeEntry(Session.Current.Scenario.Date, destinationPerson, sourcePerson, "被俘虜");
                    destinationTroop.CatchCaptiveFromTroop(sourcePerson);
                    sourceTroop.RefreshAfterLosePerson();
                    damage.SourceMoraleChange -= 20;
                    damage.DestinationMoraleChange += 20;
                    damage.SourceCombativityChange -= 20;
                    damage.DestinationCombativityChange += 20;
                    break;
                case 8: //8、P2武将被俘虏
                    Session.Current.Scenario.YearTable.addChallengeEntry(Session.Current.Scenario.Date, sourcePerson, destinationPerson, "被俘虜");
                    sourceTroop.CatchCaptiveFromTroop(destinationPerson);
                    destinationTroop.RefreshAfterLosePerson();
                    damage.SourceMoraleChange += 20;
                    damage.DestinationMoraleChange -= 20;
                    damage.SourceCombativityChange += 20;
                    damage.DestinationCombativityChange -= 20; 
                    break;
                case 9: //9、P1武将被说服
                    if (sourcePerson.IsFactionLeader) break;
                    Session.Current.Scenario.YearTable.addChallengeEntry(Session.Current.Scenario.Date, destinationPerson, sourcePerson, "被说服");
                    destinationPerson.ConvincePersonSuccess(sourcePerson);
                    damage.SourceCombativityChange -= 30;
                    damage.DestinationCombativityChange += 30;
                    break;
                case 10: //10、P2武将被说服
                    if (destinationPerson.IsFactionLeader) break;
                    Session.Current.Scenario.YearTable.addChallengeEntry(Session.Current.Scenario.Date, sourcePerson, destinationPerson, "被说服");
                    sourcePerson.ConvincePersonSuccess(destinationPerson);
                    damage.SourceCombativityChange += 30;
                    damage.DestinationCombativityChange -= 30; 
                    break;
                case -1: //-1：平局
                    Session.Current.Scenario.YearTable.addChallengeDrawEntry(Session.Current.Scenario.Date, sourcePerson, destinationPerson);
                    break;
                case -2: //-2：平局：P1武将被杀
                    Session.Current.Scenario.YearTable.addChallengeDrawKilledEntry(Session.Current.Scenario.Date, destinationPerson, sourcePerson);
                    this.challengePersonDie(sourcePerson, sourceTroop, destinationPerson);
                    break;
                case -3: //-3：平局：P2武将被杀
                    Session.Current.Scenario.YearTable.addChallengeDrawKilledEntry(Session.Current.Scenario.Date, sourcePerson, destinationPerson);
                    this.challengePersonDie(destinationPerson, destinationTroop, sourcePerson);
                    break;
                case -4: //-4：平局：双方武将被杀
                    Session.Current.Scenario.YearTable.addChallengeDrawBothKilledEntry(Session.Current.Scenario.Date, sourcePerson, destinationPerson);
                    this.challengePersonDie(sourcePerson, sourceTroop, destinationPerson);
                    this.challengePersonDie(destinationPerson, destinationTroop, sourcePerson);
                    break;
            }


        }



        private void challengePersonDie(Person challengePerson,Troop troop, Person killer)
        {
            if (Session.GlobalVariables.PersonDieInChallenge)
            {
                if (troop == troop.StartingArchitecture.RobberTroop)
                {
                    troop.Persons.Remove(troop.Leader);
                    troop.Leader.LocationTroop = null;
                }
                else
                {
                    challengePerson.KilledInBattle(troop, killer);
                }
            }
        }


    }
}

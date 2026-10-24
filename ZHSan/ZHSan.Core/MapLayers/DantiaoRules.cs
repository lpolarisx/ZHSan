using System;
using System.Collections.Generic;
using GameManager;
using GameObjects;

namespace WorldOfTheThreeKingdoms.GameScreens.ScreenLayers
{
    public enum StrikeOutcome
    {
        Dodge,
        Parry,
        Hit
    }

    public enum PursuitOutcome
    {
        Kill,
        Capture,
        Wound,
        Escape
    }

    public enum ShotOutcome
    {
        Kill,
        Fall,
        Graze,
        Miss
    }

    public class StrikeResult
    {
        public StrikeOutcome Outcome;
        public float Damage;
        public bool Critical;
        public float AttackerStaminaLoss;
        public float DefenderStaminaLoss;
    }

    /// <summary>
    /// 单挑数值判定。武力平均分配给力量、敏捷、武艺；当前体力越低，发挥越差
    /// </summary>
    public static class DantiaoRules
    {
        public const int MaxRounds = 50;

        static float Clamp(float v, float min, float max) => Math.Max(min, Math.Min(max, v));

        static float Range(Random rng, float min, float max) => min + (float)rng.NextDouble() * (max - min);

        /// <summary>
        /// 当前发挥出的武力：体力、生命越低发挥越差，死战时反而更猛
        /// </summary>
        public static float Effective(DantiaoGeneral g)
        {
            return g.Force * (0.8f + 0.2f * Math.Max(0f, g.Stamina) / 100f) * (0.85f + 0.15f * Math.Max(0f, g.Life) / 100f)
                * (g.Desperate ? 1.12f : 1f);
        }

        /// <summary>
        /// 武力差对各项判定的影响系数。调小可让弱者也有胜算：差8点时弱者约两成半胜率
        /// </summary>
        public const float GapFactor = 0.3f;

        /// <summary>
        /// surprise：突袭加成（拖刀计、回马枪），提高命中与伤害
        /// </summary>
        public static StrikeResult Strike(DantiaoGeneral attacker, DantiaoGeneral defender, bool chop, bool charge, Random rng, float surprise = 0f)
        {
            var ea = Effective(attacker);
            var ed = Effective(defender);
            var gap = ea - ed;
            var diff = gap * GapFactor;

            //被激怒：出手更狠，但破绽也更多
            var rageHit = (attacker.Rage > 0 ? 0.04f : 0f) + (defender.Rage > 0 ? 0.04f : 0f);
            var rageDodge = defender.Rage > 0 ? 0.06f : 0f;

            var pDodge = Clamp(0.20f - diff * 0.006f + (defender.HorseRate - 1f) * 0.1f - rageDodge - surprise * 0.5f, 0.03f, 0.40f);
            //死战的一方只顾进攻，也更容易中招
            var pHit = Clamp(0.09f + diff * 0.004f + (1f - defender.Stamina / 100f) * 0.10f + rageHit + surprise + (defender.Desperate ? 0.04f : 0f), 0.03f, 0.65f);

            var result = new StrikeResult();
            var roll = (float)rng.NextDouble();

            if (roll < pHit)
            {
                result.Outcome = StrikeOutcome.Hit;
                result.Damage = (4f + attacker.Force * 0.14f) * Range(rng, 0.75f, 1.25f) * (chop ? 1.1f : 1f) * (charge ? 1.15f : 1f)
                    * (attacker.Rage > 0 ? 1.15f : 1f) * (1f + surprise)
                    * (0.75f + 0.25f * Math.Max(0f, attacker.Life) / 100f) * (attacker.Desperate ? 1.2f : 1f);
                if (rng.NextDouble() < 0.08f + Math.Max(0f, diff) * 0.003f)
                {
                    result.Critical = true;
                    result.Damage *= 1.6f;
                }
                result.AttackerStaminaLoss = 1f;
                result.DefenderStaminaLoss = 2f;
            }
            else if (roll < pHit + pDodge)
            {
                result.Outcome = StrikeOutcome.Dodge;
                result.AttackerStaminaLoss = 1.5f;
                result.DefenderStaminaLoss = 1f;
            }
            else
            {
                result.Outcome = StrikeOutcome.Parry;
                //兵器相交比拼力气，吃亏的一方消耗更多体力
                var mid = (ea + ed) / 2;
                var pa = (mid + diff / 2) * Range(rng, 0.75f, 1.25f);
                var pd = (mid - diff / 2) * Range(rng, 0.75f, 1.25f);
                var loss = 3f + Math.Abs(pa - pd) * 0.05f + Range(rng, 0f, 1.5f);
                var gain = 1.5f + Range(rng, 0f, 0.5f);
                if (pd >= pa)
                {
                    result.AttackerStaminaLoss = loss;
                    result.DefenderStaminaLoss = gain;
                }
                else
                {
                    result.DefenderStaminaLoss = loss;
                    result.AttackerStaminaLoss = gain;
                }
            }

            return result;
        }

        /// <summary>
        /// 生命归零时是否被斩杀
        /// </summary>
        public static bool IsKilled(float lastDamage, bool critical, Random rng)
        {
            if (!PersonCanDie) return false;

            var chance = Clamp(0.15f + lastDamage / 100f + (critical ? 0.15f : 0f), 0.15f, 0.7f);
            return rng.NextDouble() < chance;
        }

        public static bool PersonCanDie
        {
            get
            {
                try
                {
                    return Session.GlobalVariables.PersonDieInChallenge;
                }
                catch
                {
                    return true;
                }
            }
        }

        /// <summary>
        /// 伤重或力竭的程度：0为无碍，1为撑不住了
        /// </summary>
        public static float Danger(DantiaoGeneral g)
        {
            return Math.Max(Clamp((55f - g.Life) / 45f, 0f, 1f), Clamp((40f - g.Stamina) / 35f, 0f, 1f));
        }

        /// <summary>
        /// 逃跑几率：越伤越累越想走；勇猛高的撑得久，冷静的懂得进退，义理高的不肯轻易弃战
        /// </summary>
        public static float FleeChance(DantiaoGeneral g, DantiaoGeneral opponent)
        {
            if (g.Desperate) return 0f;
            var danger = Danger(g);
            if (danger <= 0f) return 0f;

            var p = g.Person;
            var chance = 0.1f + danger * 0.5f + (opponent.Force - g.Force) * 0.006f;
            //对方伤得更重时不急着走
            chance += (opponent.Life - g.Life) / 100f * 0.2f;
            chance *= Math.Max(0.3f, 1.3f - p.Braveness * 0.08f);
            chance *= 1f + p.Calmness * 0.03f;
            chance *= 1f - p.PersonalLoyalty * 0.08f;
            return Clamp(chance, 0f, 0.8f);
        }

        /// <summary>
        /// 伤重时奋起死战的几率：勇猛高、冷静低的才会
        /// </summary>
        public static float DesperateChance(DantiaoGeneral g)
        {
            if (g.Desperate || Danger(g) < 0.4f) return 0f;
            var p = g.Person;
            return Clamp((p.Braveness - 5) * 0.07f + (5 - p.Calmness) * 0.04f, 0f, 0.45f);
        }

        /// <summary>
        /// 对方逃跑时追击的几率：自己体力比对方多得越多越想追，勇猛的爱追，冷静的怕有诈
        /// </summary>
        public static float PursueChance(DantiaoGeneral winner, DantiaoGeneral fleer)
        {
            if (!winner.Mounted || winner.Stamina < 15f) return 0f;
            var p = winner.Person;
            var chance = 0.3f + (winner.Stamina - fleer.Stamina) / 100f * 0.8f + (winner.Life - 50f) / 100f * 0.3f
                + (p.Braveness - 5) * 0.05f - (p.Calmness - 5) * 0.02f;
            return Clamp(chance, 0.05f, 0.9f);
        }

        /// <summary>
        /// 放冷箭的几率：善射的才会，冷静的更愿意放箭而不是去追
        /// </summary>
        public static float ShootChance(DantiaoGeneral w)
        {
            if (!w.IsArcher || !w.Mounted) return 0f;
            return Clamp(0.5f + (w.Person.Calmness - 5) * 0.04f, 0.3f, 0.75f);
        }

        /// <summary>
        /// 冷箭的结果：射杀、射落马下、带箭逃走、射空。武力高的射得准
        /// </summary>
        public static ShotOutcome RollShot(DantiaoGeneral w, DantiaoGeneral f, Random rng)
        {
            var skill = (w.Force - 70) * 0.004f;
            var kill = PersonCanDie ? Math.Max(0f, 0.06f + (100f - f.Life) / 100f * 0.12f + skill * 0.5f) : 0f;
            var fall = Math.Max(0.1f, 0.3f + skill);
            var graze = 0.3f;
            var miss = Math.Max(0.1f, 0.3f - skill);

            var roll = (float)rng.NextDouble() * (kill + fall + graze + miss);
            if ((roll -= kill) < 0f) return ShotOutcome.Kill;
            if ((roll -= fall) < 0f) return ShotOutcome.Fall;
            if ((roll -= graze) < 0f) return ShotOutcome.Graze;
            return ShotOutcome.Miss;
        }

        /// <summary>
        /// 诈败的几率：武力、智力都高的才会用，带点伤时更像真败
        /// </summary>
        public static float FeintChance(DantiaoGeneral a)
        {
            var p = a.Person;
            if (p.Intelligence < 55 || a.Force < 70 || a.Stamina <= 30f) return 0f;
            var chance = 0.04f + (p.Intelligence - 55) * 0.002f + (a.Force - 70) * 0.002f;
            if (a.Life > 40f && a.Life < 85f) chance += 0.03f;
            return chance;
        }

        /// <summary>
        /// 识破诈败的几率：智力高、冷静的不上当，勇猛性急的追得紧
        /// </summary>
        public static float SeeThroughChance(DantiaoGeneral feigner, DantiaoGeneral pursuer)
        {
            var p = pursuer.Person;
            return Clamp((p.Intelligence - feigner.Person.Intelligence + 20) * 0.01f + (p.Calmness - 5) * 0.04f - (p.Braveness - 5) * 0.03f, 0.05f, 0.8f);
        }

        /// <summary>
        /// 诈败回身一击得手后，直接把追兵斩于马下的几率
        /// </summary>
        public static float FeintKillChance(DantiaoGeneral feigner, DantiaoGeneral pursuer)
        {
            if (!PersonCanDie) return 0f;
            return Clamp(0.25f + (feigner.Force - pursuer.Force) * 0.015f + (pursuer.Person.Braveness - 5) * 0.03f, 0.05f, 0.6f);
        }

        /// <summary>
        /// 追上的几率：看马快慢和体力
        /// </summary>
        public static float CatchChance(DantiaoGeneral pursuer, DantiaoGeneral fleer)
        {
            return Clamp(0.4f + (pursuer.HorseRate - fleer.HorseRate) * 1.5f + (pursuer.Stamina - fleer.Stamina) / 100f * 0.5f, 0.1f, 0.85f);
        }

        /// <summary>
        /// 追上以后的结果：斩杀、生擒、打落马下，或被逃将回身逼退而逃脱
        /// </summary>
        public static PursuitOutcome RollPursuit(DantiaoGeneral pursuer, DantiaoGeneral fleer, Random rng)
        {
            var gap = (pursuer.Force - fleer.Force) * 0.01f;
            var kill = PersonCanDie ? Math.Max(0.05f, 0.15f + (100f - fleer.Life) / 100f * 0.35f + gap) : 0f;
            var capture = CaptureChance(pursuer, fleer);
            var wound = 0.45f;
            var escape = Math.Max(0.05f, 0.15f - gap + (fleer.Person.Intelligence - 60) * 0.002f);

            var roll = (float)rng.NextDouble() * (kill + capture + wound + escape);
            if ((roll -= kill) < 0f) return PursuitOutcome.Kill;
            if ((roll -= capture) < 0f) return PursuitOutcome.Capture;
            if ((roll -= wound) < 0f) return PursuitOutcome.Wound;
            return PursuitOutcome.Escape;
        }

        /// <summary>
        /// 生擒成功率
        /// </summary>
        public static float CaptureChance(DantiaoGeneral a, DantiaoGeneral d)
        {
            return Clamp(0.2f + (a.Force - d.Force) * 0.012f + (a.Stamina - d.Stamina) / 100f * 0.4f, 0.05f, 0.6f);
        }

        /// <summary>
        /// 能否发起说服：双方须分属不同势力，被说服者不能是君主或盗贼
        /// </summary>
        public static bool CanPersuade(DantiaoGeneral persuader, DantiaoGeneral target)
        {
            var p = persuader.Person;
            var t = target.Person;
            return p.Glamour >= 60
                && p.Identity() != 0 && t.Identity() == 1
                && p.BelongedFaction != null && t.BelongedFaction != null
                && p.BelongedFaction != t.BelongedFaction
                && p.BelongedFaction.Leader != null && t.BelongedFaction.Leader != null
                && t.BelongedFaction.Leader.ID != t.ID;
        }

        static int IdealDistance(int a, int b)
        {
            var d = Math.Abs(a - b);
            return Math.Min(d, 150 - d);
        }

        /// <summary>
        /// 说服成功率：与魅力、忠诚、义理、相性、势力声望、当前体力有关
        /// </summary>
        public static float PersuadeChance(DantiaoGeneral persuader, DantiaoGeneral target)
        {
            var p = persuader.Person;
            var t = target.Person;

            var chance = 0.05f;
            chance += (p.Glamour - 70) * 0.006f;
            chance += (90 - t.Loyalty) * 0.006f;
            chance -= t.PersonalLoyalty * 0.06f;
            chance += (20 - IdealDistance(t.Ideal, p.BelongedFaction.Leader.Ideal)) * 0.006f;
            chance += (IdealDistance(t.Ideal, t.BelongedFaction.Leader.Ideal) - 20) * 0.004f;
            chance += p.BelongedFaction.Reputation > t.BelongedFaction.Reputation ? 0.05f : -0.03f;
            chance += (persuader.Stamina - target.Stamina) / 100f * 0.25f;
            chance += (p.Intelligence - 70) * 0.002f;
            return Clamp(chance, 0f, 0.6f);
        }

        #region 台词与评书

        //文本都在 Content\Data\Dantiao\DantiaoText.json 里，这里只按情况挑条目、填名字

        static string T(Random rng, string key, params object[] vars) => DantiaoText.Pick(rng, key, vars);

        /// <summary>
        /// 自称，如“赵子龙”
        /// </summary>
        public static string SelfName(Person p)
        {
            return String.IsNullOrEmpty(p.CalledName) || String.IsNullOrEmpty(p.SurName) ? p.Name : p.SurName + p.CalledName;
        }

        public static string OpeningLine(Person p, Random rng) => T(rng, "开场台词", "自称", SelfName(p));

        public static string ReplyLine(Person p, Random rng) => T(rng, "应战台词", "自称", SelfName(p));

        /// <summary>
        /// 一招的评书。firstInRound为false时是同一回合里的第二招，不再写回合数；withAct为false时出招已另写，只写结果
        /// </summary>
        public static string StrikeText(int round, DantiaoGeneral a, DantiaoGeneral d, bool chop, bool charge, StrikeResult r, Random rng, bool firstInRound = true, bool withAct = true)
        {
            var vars = new object[] { "攻", a.Name, "守", d.Name, "伤", chop ? "砍" : "刺" };

            string result;
            switch (r.Outcome)
            {
                case StrikeOutcome.Dodge:
                    result = DantiaoText.PickAny(rng, new[] { "闪避", d.Mounted ? "闪避.骑马" : null }, vars);
                    break;
                case StrikeOutcome.Parry:
                    result = r.DefenderStaminaLoss >= 4.5f
                        ? DantiaoText.PickAny(rng, new[] { "招架.吃力", d.Mounted ? "招架.吃力.骑马" : "招架.吃力.步战" }, vars)
                        : T(rng, "招架", vars);
                    break;
                default:
                    result = T(rng, r.Critical ? "中招.重击" : "中招", vars);
                    break;
            }

            if (!withAct)
            {
                return result;
            }
            var head = firstInRound ? T(rng, "回合", "回合", round) + (charge ? T(rng, "两马相交") : "") : "";
            var act = DantiaoText.PickFor(rng, chop ? "出招.砍" : "出招.刺", a, vars);
            return head + act + "，" + result;
        }

        /// <summary>
        /// 每满十合的战况，n为整十的回合数
        /// </summary>
        public static string MilestoneText(int n, Random rng) => T(rng, "战况", "数", TensNumeral(n));

        public static string RestText(Random rng) => T(rng, "歇马");

        public static string RestLine(Random rng) => T(rng, "歇马台词");

        // 整十的中文数字：十、二十……一百
        static string TensNumeral(int n)
        {
            const string digits = "一二三四五六七八九";
            var tens = n / 10;
            if (tens <= 1) return "十";
            if (tens < 10) return digits[tens - 1] + "十";
            return n == 100 ? "一百" : n.ToString();
        }

        public static string FleeText(DantiaoGeneral g, Random rng)
        {
            var key = g.Stamina <= 0f ? "逃跑.力竭" : g.Life < 35f ? "逃跑.重伤" : "逃跑";
            return T(rng, key, "逃", g.Name);
        }

        public static string FleeLine(Random rng) => T(rng, "逃跑台词");

        public static string PursueText(DantiaoGeneral w, Random rng) => T(rng, "追击", "追", w.Name);

        public static string PursueLine(Random rng) => T(rng, "追击台词");

        public static string LetGoText(DantiaoGeneral w, Random rng) => T(rng, "不追", "追", w.Name);

        /// <summary>
        /// 不追时的话：沉稳的不屑去追，性急的出言嘲讽
        /// </summary>
        public static string LetGoLine(DantiaoGeneral w, Random rng) => T(rng, w.Person.Calmness >= 6 ? "不追台词.沉稳" : "不追台词.性急");

        public static string LostText(DantiaoGeneral w, DantiaoGeneral f, Random rng)
        {
            return T(rng, f.HorseRate > w.HorseRate ? "追丢.马快" : "追丢", "追", w.Name, "逃", f.Name);
        }

        public static string LostLine(Random rng) => T(rng, "追丢台词");

        public static string CounterEscapeText(DantiaoGeneral w, DantiaoGeneral f, Random rng) => T(rng, "回身逃脱", "追", w.Name, "逃", f.Name);

        public static string CaughtText(PursuitOutcome outcome, DantiaoGeneral w, DantiaoGeneral f, Random rng)
        {
            var key = outcome == PursuitOutcome.Kill ? "追上.斩杀" : outcome == PursuitOutcome.Capture ? "追上.生擒" : "追上.负伤";
            return DantiaoText.PickFor(rng, key, w, "追", w.Name, "逃", f.Name);
        }

        public static string DesperateText(DantiaoGeneral g, Random rng) => T(rng, "死战", "名", g.Name);

        public static string DesperateLine(Random rng) => T(rng, "死战台词");

        public static string WinLine(Random rng) => T(rng, "获胜台词");

        public static string TauntLine(DantiaoGeneral target, Random rng)
        {
            var leader = target.Person.BelongedFaction?.Leader;
            var withLeader = leader != null && leader != target.Person;
            return DantiaoText.PickAny(rng, new[] { "骂阵台词", withLeader ? "骂阵台词.有主公" : null, target.Person.Age >= 55 ? "骂阵台词.老将" : null },
                "对方", target.Name, "主公", leader?.Name ?? "");
        }

        public static string AngryLine(Random rng) => T(rng, "激怒台词");

        public static string CalmLine(Random rng) => T(rng, "冷静台词");

        public static string RoarLine(Person p, Random rng) => T(rng, "大喝台词", "自称", SelfName(p));

        public static string PersuadeLine(DantiaoGeneral target, Random rng)
        {
            return T(rng, "说服台词", "主公", target.Person.BelongedFaction?.Leader?.Name ?? "其主");
        }

        #endregion
    }
}

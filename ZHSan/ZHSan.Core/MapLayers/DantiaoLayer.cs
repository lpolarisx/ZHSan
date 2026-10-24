using System;
using System.Collections.Generic;
using System.Linq;
using GameGlobal;
using GameManager;
using GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Platforms;

namespace WorldOfTheThreeKingdoms.GameScreens.ScreenLayers
{
    /// <summary>
    /// 单挑画面：参照《三国单挑程序》（泽政）重做。
    /// 左侧为P1（发起方），右侧为P2；结果码与Challenge.HandleChallengeResult一致。
    /// </summary>
    public class DantiaoLayer
    {
        public static List<Person> Persons = null;

        public TroopDamage damage = null;

        public int Result = 0;

        //单挑窗体大小（像素），背景按1倍、人物按2倍绘制
        const float VirtualHeight = 600f;
        const float PanelWidth = 1000f;
        const float TitleBarHeight = 26f;
        const float WorldWidth = 2300f;
        const float GyMin = 50f;
        const float GyMax = 260f;
        const int LogPageLines = 6;
        //双方头像、名字、体力条都排在窗体顶部一条，头像框按这个倍数缩小
        const float PortraitScale = 0.6f;
        const float TopBandHeight = 128f;
        //生命条比体力条短
        const float LifeBarScale = 0.8f;
        //台词气泡相对原图（319/337×96）的大小，文字和边距跟着缩
        const float BubbleScale = 0.75f;

        const string Root = @"Content\Textures\Resources\Dantiao\";
        const string SoundRoot = @"Content\Sound\Dantiao\";

        readonly Random rng = new Random();

        readonly DantiaoGeneral left, right;

        float scale = 1f;   //屏幕放不下窗体时才缩小
        Vector2 origin;     //窗体左上角的屏幕坐标
        readonly float viewWidth = PanelWidth;

        static Texture2D whitePixel;

        float camX;
        float horizonY = 220f;

        float alpha = 0f;
        float startDelay = 1f;  //等待云层合拢

        int round = 0;
        int speed = 1;
        bool paused;
        float slowTime;
        int focus;  //镜头：0双方，1左，2右
        DantiaoGeneral fleeing;

        readonly Queue<Func<float, bool>> steps = new Queue<Func<float, bool>>();
        Func<float, bool> currentStep;
        bool finished;
        int persuadeAttempts;

        readonly Dictionary<DantiaoGeneral, StrikeResult> lastHit = new Dictionary<DantiaoGeneral, StrikeResult>();
        readonly Dictionary<DantiaoGeneral, bool> lastHitChop = new Dictionary<DantiaoGeneral, bool>();

        string title = "";
        float overTime;
        bool closing;
        float closeTime;

        readonly List<string> logs = new List<string>();
        readonly List<string> logLines = new List<string>();
        int logBack = 0;  //往前翻的页数，0为最新
        bool storyVisible = true;
        float logWrapScale;

        class Blood
        {
            public float X, Gy, Time;
        }

        readonly List<Blood> bloods = new List<Blood>();

        /// <summary>
        /// 冷箭：从放箭处飞向目标，Miss时从头顶掠过
        /// </summary>
        class Arrow
        {
            public float FromX, FromGy, Time;
            public DantiaoGeneral Target;
            public bool Miss;
            public const float Duration = 0.35f;
            public bool Done => Time >= Duration;
        }

        readonly List<Arrow> arrows = new List<Arrow>();

        readonly HashSet<DantiaoGeneral> mustDie = new HashSet<DantiaoGeneral>();  //中了诈败的追兵，落马即死

        class Exchange
        {
            public DantiaoGeneral Attacker, Defender;
            public bool Chop;
        }

        public DantiaoLayer(Person leftPerson, Person rightPerson)
        {
            DantiaoText.Reload();
            left = new DantiaoGeneral(leftPerson, 0);
            right = new DantiaoGeneral(rightPerson, 1);

            UpdateScale();

            var center = WorldWidth / 2;
            left.Place(center - 430, 120, 1);
            right.Place(center + 430, 126, -1);
            camX = center - viewWidth / 2;
            horizonY = TargetHorizon();

            PlanIntro();

            Session.PlayMusic("Battle");
        }

        #region 流程

        void Do(Action action)
        {
            steps.Enqueue(dt =>
            {
                action();
                return true;
            });
        }

        void Wait(float time)
        {
            var remaining = time;
            steps.Enqueue(dt => (remaining -= dt) <= 0f);
        }

        void WaitUntil(Func<bool> condition, float timeout = 6f)
        {
            var elapsed = 0f;
            steps.Enqueue(dt => condition() || (elapsed += dt) >= timeout);
        }

        void RunScript(float dt)
        {
            for (int guard = 0; guard < 50; guard++)
            {
                if (currentStep == null)
                {
                    if (steps.Count == 0)
                    {
                        if (finished) return;
                        PlanNext();
                        if (steps.Count == 0) return;
                    }
                    currentStep = steps.Dequeue();
                }

                var step = currentStep;
                if (!step(dt)) return;
                if (currentStep == step) currentStep = null;
                dt = 0f;
            }
        }

        /// <summary>
        /// 终止当前安排，后续只执行结局
        /// </summary>
        void Interrupt()
        {
            steps.Clear();
            finished = true;
        }

        void PlanIntro()
        {
            Wait(0.6f);
            Do(() =>
            {
                left.MoveTo(left.X + 120, left.Gy, 90);
                PlaySound("Horse");
            });
            WaitUntil(() => left.Arrived);
            Do(() => Speak(left, DantiaoRules.OpeningLine(left.Person, rng)));
            Wait(2.2f);
            Do(() =>
            {
                right.MoveTo(right.X - 120, right.Gy, 90);
                PlaySound("Horse");
            });
            WaitUntil(() => right.Arrived);
            Do(() => Speak(right, DantiaoRules.ReplyLine(right.Person, rng)));
            Wait(2.2f);
        }

        void PlanNext()
        {
            if (round >= 2 && TryPlanSpecial()) return;

            var distance = Math.Abs(left.X - right.X);

            if (distance > 300)
            {
                if (rng.NextDouble() < 0.75) PlanCharge(); else PlanMelee();
            }
            else
            {
                if (rng.NextDouble() < 0.3) PlanSeparate(); else PlanMelee();
            }
        }

        /// <summary>
        /// 两马对冲，交错时各出一招，冲过后勒马回转
        /// </summary>
        void PlanCharge()
        {
            bool chopLeft = false, chopRight = false, pendingLeft = false, pendingRight = false;

            Do(() =>
            {
                var mid = Clamp((left.X + right.X) / 2, 700, WorldWidth - 700);
                var lane = Clamp((left.Gy + right.Gy) / 2 + Range(-60, 60), GyMin + 30, GyMax - 30);
                var dir = left.X < right.X ? 1 : -1;
                left.MoveTo(mid + dir * 320, lane - 6, 240);
                right.MoveTo(mid - dir * 320, lane + 6, 240);
                PlaySound("Horse");
            });
            WaitUntil(() => Math.Abs(left.X - right.X) < 130);
            Do(() =>
            {
                round++;
                chopLeft = rng.NextDouble() < 0.5;
                chopRight = rng.NextDouble() < 0.5;
                left.Attack(chopLeft);
                right.Attack(chopRight);
                pendingLeft = pendingRight = true;
                PlayVoice(rng.NextDouble() < 0.5 ? left : right, "Ha");
            });
            WaitUntil(() =>
            {
                if (pendingLeft && left.ImpactReady)
                {
                    left.ImpactReady = false;
                    pendingLeft = false;
                    if (!IsDefeated(left) || rng.NextDouble() < 0.15) DoStrike(left, right, chopLeft, true);
                }
                if (pendingRight && right.ImpactReady)
                {
                    right.ImpactReady = false;
                    pendingRight = false;
                    if (!IsDefeated(right) || rng.NextDouble() < 0.15) DoStrike(right, left, chopRight, true);
                }
                return !pendingLeft && !pendingRight;
            });
            Do(() => CheckDefeat());
            WaitUntil(() => left.Arrived && right.Arrived);
            Do(() =>
            {
                left.FaceTo(right);
                right.FaceTo(left);
            });
            Wait(0.5f);
            Do(CheckEvents);
        }

        /// <summary>
        /// 近身缠斗，你来我往数合
        /// </summary>
        void PlanMelee()
        {
            Do(() =>
            {
                var mid = Clamp((left.X + right.X) / 2, 300, WorldWidth - 300);
                var lane = Clamp((left.Gy + right.Gy) / 2 + Range(-50, 50), GyMin + 20, GyMax - 20);
                var leftX = left.X <= right.X ? mid - 56 : mid + 56;
                var approach = Math.Abs(left.X - right.X) > 300 ? 200 : 120;
                left.MoveTo(leftX, lane, approach);
                right.MoveTo(2 * mid - leftX, lane + 4, approach);
            });
            WaitUntil(() => left.Arrived && right.Arrived, 12f);
            Do(() =>
            {
                left.FaceTo(right);
                right.FaceTo(left);
            });
            Wait(0.3f);

            var count = rng.Next(2, 5);
            DantiaoGeneral lastAttacker = null;
            for (int i = 0; i < count; i++)
            {
                var ex = new Exchange();
                Do(() =>
                {
                    if (finished) return;
                    round++;
                    ex.Attacker = PickAttacker(lastAttacker);
                    ex.Defender = ex.Attacker == left ? right : left;
                    ex.Chop = rng.NextDouble() < 0.55;
                    lastAttacker = ex.Attacker;
                    ex.Attacker.Attack(ex.Chop);
                    if (rng.NextDouble() < 0.3) PlayVoice(ex.Attacker, "Ha");
                });
                WaitUntil(() =>
                {
                    if (ex.Attacker == null) return true;
                    if (!ex.Attacker.ImpactReady) return false;
                    ex.Attacker.ImpactReady = false;
                    DoStrike(ex.Attacker, ex.Defender, ex.Chop, false);
                    return true;
                });
                WaitUntil(() => ex.Attacker == null || !ex.Attacker.IsAttacking);
                Do(() => CheckDefeat());
                Wait(Range(0.2f, 0.5f));
            }

            Do(() =>
            {
                //换个位置再战
                var choice = rng.Next(3);
                if (choice == 0)
                {
                    var dy = left.Gy > (GyMin + GyMax) / 2 ? -70 : 70;
                    left.MoveTo(left.X, Clamp(left.Gy + dy, GyMin, GyMax), 70, true);
                    right.MoveTo(right.X, Clamp(right.Gy + dy, GyMin, GyMax), 70, true);
                }
                else if (choice == 1)
                {
                    var back = rng.NextDouble() < 0.5 ? left : right;
                    var other = back == left ? right : left;
                    back.MoveTo(Clamp(back.X + (back.X < other.X ? -110 : 110), 150, WorldWidth - 150), back.Gy, 80, true);
                }
            });
            WaitUntil(() => left.Arrived && right.Arrived);
            Do(CheckEvents);
        }

        /// <summary>
        /// 拨马分开，准备下一次对冲
        /// </summary>
        void PlanSeparate()
        {
            Do(() =>
            {
                var mid = Clamp((left.X + right.X) / 2, 600, WorldWidth - 600);
                var dir = left.X < right.X ? 1 : -1;
                left.MoveTo(mid - dir * 330, left.Gy, 170);
                right.MoveTo(mid + dir * 330, right.Gy, 170);
                PlaySound("Horse");
            });
            WaitUntil(() => left.Arrived && right.Arrived);
            Do(() =>
            {
                left.FaceTo(right);
                right.FaceTo(left);
            });
            Wait(0.4f);
        }

        DantiaoGeneral PickAttacker(DantiaoGeneral last)
        {
            var p = 0.5f + (DantiaoRules.Effective(left) - DantiaoRules.Effective(right)) * DantiaoRules.GapFactor * 0.006f;
            if (last == left) p -= 0.15f;
            else if (last == right) p += 0.15f;
            return rng.NextDouble() < Clamp(p, 0.2f, 0.8f) ? left : right;
        }

        #region 演义招式：骂阵、大喝、拖刀计/回马枪、喘息

        int tauntCount, feintCount, restCount;
        int loggedRound;    //评书里已写过回合数的回合，同一回合后面的招式不再重复
        readonly HashSet<DantiaoGeneral> roared = new HashSet<DantiaoGeneral>();
        readonly HashSet<DantiaoGeneral> desperateTried = new HashSet<DantiaoGeneral>();  //每人只判定一次是否死战

        bool TryPlanSpecial()
        {
            var distance = Math.Abs(left.X - right.X);
            var first = rng.Next(2) == 0 ? left : right;

            //双方都打累了，各自歇马
            if (restCount < 1 && left.Stamina < 45f && right.Stamina < 45f && rng.NextDouble() < 0.5)
            {
                PlanRest();
                return true;
            }

            foreach (var a in new[] { first, Other(first) })
            {
                var b = Other(a);
                var p = a.Person;

                if (!roared.Contains(a) && p.Braveness >= 8 && rng.NextDouble() < 0.08)
                {
                    PlanRoar(a, b);
                    return true;
                }

                //智力低、勇猛高的更爱骂阵
                if (tauntCount < 2 && rng.NextDouble() < 0.04 + p.Braveness * 0.006 + (70 - p.Intelligence) * 0.001)
                {
                    PlanTaunt(a, b);
                    return true;
                }

                if (round >= 5 && feintCount < 2 && distance <= 300 && rng.NextDouble() < DantiaoRules.FeintChance(a))
                {
                    PlanFeint(a, b);
                    return true;
                }
            }

            return false;
        }

        void PlanTaunt(DantiaoGeneral a, DantiaoGeneral b)
        {
            tauntCount++;

            Do(() =>
            {
                a.Stop();
                b.Stop();
                a.FaceTo(b);
                b.FaceTo(a);
            });
            Wait(0.3f);
            Do(() =>
            {
                a.SetPose(DantiaoPose.Taunt, 2.2f);
                Speak(a, DantiaoRules.TauntLine(b, rng), 2.4f);
            });
            Wait(2.5f);
            Do(() =>
            {
                //冷静低的容易被激怒
                if (rng.NextDouble() < 0.75 - b.Person.Calmness * 0.06)
                {
                    b.Rage = 3;
                    b.SetPose(DantiaoPose.Raise, 0.8f);
                    PlayVoice(b, "Ha");
                    Speak(b, DantiaoRules.AngryLine(rng), 1.8f);
                    Log(T("骂阵.激怒", "骂", a.Name, "被骂", b.Name));
                }
                else
                {
                    Speak(b, DantiaoRules.CalmLine(rng), 1.8f);
                    Log(T("骂阵.无效", "骂", a.Name, "被骂", b.Name));
                }
            });
            Wait(1.8f);
        }

        void PlanRoar(DantiaoGeneral a, DantiaoGeneral b)
        {
            roared.Add(a);

            Do(() =>
            {
                a.Stop();
                a.FaceTo(b);
            });
            Wait(0.3f);
            Do(() =>
            {
                a.SetPose(DantiaoPose.Raise, 1.2f);
                PlayVoice(a, "Ha");
                Speak(a, DantiaoRules.RoarLine(a.Person, rng), 1.6f);
            });
            Wait(0.5f);
            Do(() =>
            {
                var loss = Clamp((a.Person.Braveness - b.Person.Calmness) * 2.5f + Range(0f, 4f), 0f, 15f);
                if (loss >= 5f)
                {
                    b.Stamina = Math.Max(1f, b.Stamina - loss);
                    b.SetPose(DantiaoPose.Recoil, 0.5f);
                    b.Nudge(b.X < a.X ? -20 : 20);
                    Log(T("大喝.震慑", "喝", a.Name, "对方", b.Name));
                }
                else
                {
                    Log(T("大喝.无效", "喝", a.Name, "对方", b.Name));
                }
            });
            Wait(1.2f);
        }

        /// <summary>
        /// 诈败：拨马便走，诱敌来追，回身突袭。劈砍为拖刀计，突刺为回马枪
        /// </summary>
        void PlanFeint(DantiaoGeneral a, DantiaoGeneral b)
        {
            feintCount++;
            //使刀的拖刀计，使枪矛的回马枪，其余随机
            var chop = a.WeaponKind == "刀" ? true : a.WeaponKind == "枪" ? false : rng.NextDouble() < 0.5;
            var seen = false;

            Do(() =>
            {
                var dir = a.X < b.X ? -1 : 1;
                var target = Clamp(a.X + dir * 280, 150, WorldWidth - 150);
                a.MoveTo(target, a.Gy, 150);
                PlaySound("Horse");
                //和真败走用同一套说法，看不出是诈
                Log(DantiaoRules.FleeText(a, rng));
                Speak(a, DantiaoRules.FleeLine(rng), 1.2f);

                seen = rng.NextDouble() < DantiaoRules.SeeThroughChance(a, b);
                if (!seen)
                {
                    b.MoveTo(target - dir * 70, a.Gy + 3, 200);
                }
            });
            Wait(0.5f);
            Do(() =>
            {
                if (!seen) Speak(b, DantiaoRules.PursueLine(rng), 1.2f);
            });
            WaitUntil(() => a.Arrived, 5f);
            Do(() =>
            {
                a.FaceTo(b);
                if (seen)
                {
                    b.Stop();
                    Log(T("识破", "追", b.Name, "逃", a.Name));
                    Speak(b, T("识破台词"), 1.6f);
                }
            });
            Wait(0.3f);
            Do(() =>
            {
                if (seen) return;
                round++;
                a.Attack(chop);
                PlayVoice(a, "Ha");
                loggedRound = round;
                Log(DantiaoText.PickFor(rng, chop ? "诈败.反击.砍" : "诈败.反击.刺", a, "逃", a.Name, "追", b.Name, "回合", round));
            });
            WaitUntil(() =>
            {
                if (seen) return true;
                if (!a.ImpactReady) return false;
                a.ImpactReady = false;
                b.Stop();
                var r = DoStrike(a, b, chop, false, 0.4f);
                //追得太急，中了计的可能当场被斩
                if (r.Outcome == StrikeOutcome.Hit && rng.NextDouble() < DantiaoRules.FeintKillChance(a, b))
                {
                    mustDie.Add(b);
                    b.Life = 0f;
                }
                return true;
            });
            Do(() => CheckDefeat());
            WaitUntil(() => !a.IsAttacking && b.Arrived);
            Do(CheckEvents);
        }

        void PlanRest()
        {
            restCount++;

            Do(() =>
            {
                var mid = Clamp((left.X + right.X) / 2, 600, WorldWidth - 600);
                var dir = left.X < right.X ? 1 : -1;
                left.MoveTo(mid - dir * 300, left.Gy, 110);
                right.MoveTo(mid + dir * 300, right.Gy, 110);
                Log(DantiaoRules.RestText(rng));
            });
            WaitUntil(() => left.Arrived && right.Arrived);
            Do(() =>
            {
                left.FaceTo(right);
                right.FaceTo(left);
            });
            Wait(0.3f);
            Do(() =>
            {
                left.SetPose(DantiaoPose.Pant, 2f);
                right.SetPose(DantiaoPose.Pant, 2f);
            });
            Wait(2.1f);
            Do(() =>
            {
                left.Stamina = Math.Min(100f, left.Stamina + 10f);
                right.Stamina = Math.Min(100f, right.Stamina + 10f);
                Speak(rng.Next(2) == 0 ? left : right, DantiaoRules.RestLine(rng), 1.2f);
            });
            Wait(1f);
        }

        #endregion

        StrikeResult DoStrike(DantiaoGeneral a, DantiaoGeneral d, bool chop, bool charge, float surprise = 0f)
        {
            var r = DantiaoRules.Strike(a, d, chop, charge, rng, surprise);
            a.Stamina = Math.Max(0f, a.Stamina - r.AttackerStaminaLoss);
            d.Stamina = Math.Max(0f, d.Stamina - r.DefenderStaminaLoss);

            var away = d.X < a.X ? -1 : 1;
            PlaySound($"Swing{a.WeaponSound}" + (rng.Next(2) == 0 ? "a" : "b"));

            switch (r.Outcome)
            {
                case StrikeOutcome.Dodge:
                    d.Nudge(away * 26);
                    break;
                case StrikeOutcome.Parry:
                    PlaySound(rng.Next(2) == 0 ? "Clash1" : "Clash2");
                    if (!d.IsAttacking) d.SetPose(DantiaoPose.Guard, 0.3f);
                    d.Nudge(away * 10);
                    a.Nudge(-away * 6);
                    break;
                case StrikeOutcome.Hit:
                    d.Life = Math.Max(0f, d.Life - r.Damage);
                    lastHit[d] = r;
                    lastHitChop[d] = chop;
                    if (!d.IsAttacking) d.SetPose(DantiaoPose.Recoil, 0.35f);
                    d.Nudge(away * 16);
                    bloods.Add(new Blood() { X = d.X, Gy = d.Gy });
                    PlaySound("Hit" + rng.Next(1, 4));
                    PlayVoice(d, "Heng");
                    slowTime = r.Critical ? 0.9f : 0.5f;
                    break;
            }

            var firstInRound = loggedRound != round;
            if (firstInRound)
            {
                loggedRound = round;
                //每满十合点一句战况
                if (round > 10 && round % 10 == 1)
                {
                    Log(DantiaoRules.MilestoneText(round - 1, rng));
                }
            }
            //回马枪、拖刀计前面已经写过出招，这里只写结果
            Log(DantiaoRules.StrikeText(round, a, d, chop, charge, r, rng, firstInRound, surprise == 0f));
            return r;
        }

        bool IsDefeated(DantiaoGeneral g) => g.Life <= 0f || g.Stamina <= 0f;

        void CheckDefeat()
        {
            if (finished) return;

            //生命耗尽才落马；体力耗尽只是打不动了，拨马逃走
            var leftDown = left.Life <= 0f;
            var rightDown = right.Life <= 0f;

            if (leftDown && rightDown)
            {
                FinishBothDown();
            }
            else if (leftDown || rightDown)
            {
                FinishFall(leftDown ? left : right);
            }
            else if (left.Stamina <= 0f && right.Stamina <= 0f)
            {
                FinishDraw();
            }
            else if (left.Stamina <= 0f || right.Stamina <= 0f)
            {
                FinishFlee(left.Stamina <= 0f ? left : right);
            }
            else if (round >= DantiaoRules.MaxRounds)
            {
                FinishDraw();
            }
        }

        bool RollKilled(DantiaoGeneral g)
        {
            if (g.Life > 0f) return false;
            if (mustDie.Contains(g)) return true;
            lastHit.TryGetValue(g, out var hit);
            return DantiaoRules.IsKilled(hit?.Damage ?? 20f, hit?.Critical ?? false, rng);
        }

        bool RollBeheaded(DantiaoGeneral g)
        {
            lastHit.TryGetValue(g, out var hit);
            lastHitChop.TryGetValue(g, out var chop);
            return hit != null && chop && hit.Damage >= 20f && rng.NextDouble() < 0.5;
        }

        void FinishFall(DantiaoGeneral loser)
        {
            Interrupt();
            var winner = Other(loser);
            var killed = RollKilled(loser);
            var beheaded = killed && RollBeheaded(loser);

            Result = killed ? (loser == left ? 3 : 4) : (loser == left ? 2 : 1);

            Do(() =>
            {
                winner.Stop();
                loser.FallOff(loser.X < winner.X ? -1 : 1);
                PlaySound("Fall");
                if (killed)
                {
                    Log(DantiaoText.PickFor(rng, beheaded ? "落马.斩首" : "落马.斩杀", winner, "胜", winner.Name, "败", loser.Name));
                }
                else
                {
                    Log(T("落马.负伤", "胜", winner.Name, "败", loser.Name));
                }
            });
            Wait(0.7f);
            Do(() =>
            {
                if (beheaded)
                {
                    loser.SetPose(DantiaoPose.Beheaded);
                    PlaySound("Slash");
                    bloods.Add(new Blood() { X = loser.X, Gy = loser.Gy + 30 });
                }
                winner.FaceTo(loser);
                winner.SetPose(DantiaoPose.Raise);
            });
            Wait(0.4f);
            Do(() => Victory(winner, $"{winner.Name}获胜！"));
        }

        void FinishBothDown()
        {
            Interrupt();
            var leftKilled = RollKilled(left);
            var rightKilled = RollKilled(right);

            Result = leftKilled && rightKilled ? -4 : leftKilled ? -2 : rightKilled ? -3 : -1;

            Do(() =>
            {
                left.FallOff(left.X < right.X ? -1 : 1);
                right.FallOff(right.X < left.X ? -1 : 1);
                PlaySound("Fall");
                Log(T("双双落马"));
                if (leftKilled) Log(T("伤重不治", "名", left.Name));
                if (rightKilled) Log(T("伤重不治", "名", right.Name));
            });
            Wait(1.1f);
            Do(() =>
            {
                Log(T("结局.两败俱伤", "回合", round));
                ShowTitle("两败俱伤");
            });
        }

        void FinishDraw()
        {
            Interrupt();
            Result = -1;

            Do(() =>
            {
                left.Stop();
                right.Stop();
                left.FaceTo(right);
                right.FaceTo(left);
                Speak(left, T("平局台词"));
            });
            Wait(1.6f);
            Do(() =>
            {
                Speak(right, T("平局台词"));
                Log(T("结局.平局", "回合", round));
                ShowTitle("双方平局");
            });
        }

        void Victory(DantiaoGeneral winner, string text)
        {
            Log(T("结局.获胜", "胜", winner.Name, "回合", round));
            Speak(winner, DantiaoRules.WinLine(rng));
            ShowTitle(text);
        }

        void ShowTitle(string text)
        {
            title = text;
            overTime = 0f;
            finished = true;
        }

        /// <summary>
        /// 每轮交手后检查逃跑、生擒、说服
        /// </summary>
        void CheckEvents()
        {
            left.Rage = Math.Max(0, left.Rage - 1);
            right.Rage = Math.Max(0, right.Rage - 1);

            if (finished || round < 4) return;

            //伤重时勇猛暴躁的可能奋起死战，死战后不再逃跑
            foreach (var g in new[] { left, right })
            {
                if (!desperateTried.Contains(g) && DantiaoRules.Danger(g) >= 0.4f)
                {
                    desperateTried.Add(g);
                    if (rng.NextDouble() < DantiaoRules.DesperateChance(g))
                    {
                        PlanDesperate(g);
                        return;
                    }
                }
            }

            foreach (var g in rng.Next(2) == 0 ? new[] { left, right } : new[] { right, left })
            {
                if (rng.NextDouble() < DantiaoRules.FleeChance(g, Other(g)))
                {
                    FinishFlee(g);
                    return;
                }
            }

            foreach (var a in new[] { left, right })
            {
                var d = Other(a);
                //生擒较少见：对方几乎力竭、自己明显占优时才偶尔尝试
                if (d.Stamina < 25f && a.Stamina > d.Stamina + 20f && rng.NextDouble() < 0.15)
                {
                    PlanCapture(a, d);
                    return;
                }
            }

            if (persuadeAttempts < 2)
            {
                foreach (var p in new[] { left, right })
                {
                    var t = Other(p);
                    if ((t.Stamina < 50f || t.Life < 60f) && DantiaoRules.CanPersuade(p, t)
                        && rng.NextDouble() < 0.08 + (p.Person.Glamour - 60) * 0.003)
                    {
                        PlanPersuade(p, t);
                        return;
                    }
                }
            }
        }

        void PlanDesperate(DantiaoGeneral g)
        {
            Do(() =>
            {
                g.Desperate = true;
                //血性一起，又多了几分气力
                g.Stamina = Math.Min(100f, g.Stamina + 15f);
                g.FaceTo(Other(g));
                g.SetPose(DantiaoPose.Raise, 0.5f);
                PlayVoice(g, "Ha");
                Log(DantiaoRules.DesperateText(g, rng));
                Speak(g, DantiaoRules.DesperateLine(rng), 1.8f);
            });
            Wait(1.6f);
        }

        /// <summary>
        /// 败走：对方按体力、性格决定追不追，追上了可能斩杀、生擒或打落马下
        /// </summary>
        void FinishFlee(DantiaoGeneral g)
        {
            Interrupt();
            var winner = Other(g);
            var dir = g.X < winner.X ? -1 : 1;
            Result = g == left ? 5 : 6;

            Do(() =>
            {
                winner.Stop();
                Log(DantiaoRules.FleeText(g, rng));
                Speak(g, DantiaoRules.FleeLine(rng), 1.5f);
                PlaySound("Horse");
                if (rng.NextDouble() < DantiaoRules.ShootChance(winner))
                {
                    PlanShoot(winner, g, dir);
                }
                else if (rng.NextDouble() < DantiaoRules.PursueChance(winner, g))
                {
                    PlanPursuit(winner, g, dir);
                }
                else
                {
                    PlanLetGo(winner, g, dir);
                }
            });
        }

        void PlanLetGo(DantiaoGeneral w, DantiaoGeneral g, int dir)
        {
            Do(() =>
            {
                fleeing = g;
                g.MoveTo(dir < 0 ? -500 : WorldWidth + 500, g.Gy, 260);
            });
            Wait(0.8f);
            Do(() =>
            {
                w.FaceTo(g);
                Log(DantiaoRules.LetGoText(w, rng));
                Speak(w, DantiaoRules.LetGoLine(w, rng), 2f);
            });
            WaitUntil(() => Math.Abs(g.X - w.X) > viewWidth, 4f);
            Do(() => FinishEscaped(g));
        }

        void PlanPursuit(DantiaoGeneral w, DantiaoGeneral g, int dir)
        {
            var caught = rng.NextDouble() < DantiaoRules.CatchChance(w, g);

            Do(() =>
            {
                fleeing = null;
                //会被追上的，多半已跑不快；跑到场边也就无路可走了
                if (caught)
                {
                    g.MoveTo(dir < 0 ? 60 : WorldWidth - 60, g.Gy, 170);
                }
                else
                {
                    g.MoveTo(dir < 0 ? -500 : WorldWidth + 500, g.Gy, 260);
                }
            });
            Wait(0.5f);
            Do(() =>
            {
                Log(DantiaoRules.PursueText(w, rng));
                Speak(w, DantiaoRules.PursueLine(rng), 1.5f);
                PlaySound("Horse");
            });

            if (caught)
            {
                WaitUntil(() =>
                {
                    w.MoveTo(g.X - dir * 10, g.Gy + 3, 330);
                    return Math.Abs(w.X - g.X) < 75f;
                }, 6f);
                Do(() => ResolvePursuit(w, g, dir));
            }
            else
            {
                Do(() => w.MoveTo(Clamp(w.X + dir * 450, 100, WorldWidth - 100), w.Gy, 200));
                WaitUntil(() => w.Arrived, 3f);
                Do(() =>
                {
                    fleeing = g;
                    Log(DantiaoRules.LostText(w, g, rng));
                    Speak(w, DantiaoRules.LostLine(rng), 1.8f);
                });
                WaitUntil(() => Math.Abs(g.X - w.X) > viewWidth, 4f);
                Do(() => FinishEscaped(g));
            }
        }

        void ResolvePursuit(DantiaoGeneral w, DantiaoGeneral g, int dir)
        {
            var outcome = DantiaoRules.RollPursuit(w, g, rng);
            var chop = rng.NextDouble() < 0.55;
            w.Stop();
            g.Stop();

            if (outcome == PursuitOutcome.Escape)
            {
                //逃将回身一击逼退追兵，夺路而走
                g.FaceTo(w);
                Wait(0.25f);
                Do(() =>
                {
                    g.Attack(chop);
                    PlayVoice(g, "Ha");
                });
                WaitUntil(() => g.ImpactReady, 1.5f);
                Do(() =>
                {
                    g.ImpactReady = false;
                    PlaySound(rng.Next(2) == 0 ? "Clash1" : "Clash2");
                    w.SetPose(DantiaoPose.Guard, 0.3f);
                    w.Nudge(-dir * 24);
                    Log(DantiaoRules.CounterEscapeText(w, g, rng));
                });
                Wait(0.5f);
                Do(() =>
                {
                    fleeing = g;
                    g.MoveTo(dir < 0 ? -500 : WorldWidth + 500, g.Gy, 260);
                    Speak(w, DantiaoRules.LostLine(rng), 1.8f);
                });
                WaitUntil(() => Math.Abs(g.X - w.X) > viewWidth, 4f);
                Do(() => FinishEscaped(g));
                return;
            }

            if (outcome == PursuitOutcome.Capture)
            {
                Do(() =>
                {
                    w.SetPose(DantiaoPose.Raise, 0.4f);
                    PlayVoice(w, "Ha");
                });
                Wait(0.35f);
            }
            else
            {
                Do(() =>
                {
                    w.Attack(chop);
                    PlayVoice(w, "Ha");
                });
                WaitUntil(() => w.ImpactReady, 1.5f);
            }

            Do(() =>
            {
                w.ImpactReady = false;
                g.FallOff(dir);
                PlaySound("Fall");
                if (outcome != PursuitOutcome.Capture)
                {
                    g.Life = Math.Max(0f, g.Life - (outcome == PursuitOutcome.Kill ? 100f : 15f));
                    bloods.Add(new Blood() { X = g.X, Gy = g.Gy });
                    PlaySound("Hit" + rng.Next(1, 4));
                }
                Result = outcome == PursuitOutcome.Kill ? (g == left ? 3 : 4)
                    : outcome == PursuitOutcome.Capture ? (g == left ? 7 : 8)
                    : (g == left ? 2 : 1);
                Log(DantiaoRules.CaughtText(outcome, w, g, rng));
            });
            Wait(0.9f);
            Do(() =>
            {
                w.FaceTo(g);
                if (outcome == PursuitOutcome.Capture)
                {
                    Log(T("结局.生擒", "胜", w.Name, "败", g.Name, "回合", round));
                    Speak(w, T("生擒台词"));
                    ShowTitle($"生擒{g.Name}！");
                }
                else
                {
                    w.SetPose(DantiaoPose.Raise);
                    Victory(w, $"{w.Name}获胜！");
                }
            });
        }

        /// <summary>
        /// 败走时善射的一方放冷箭
        /// </summary>
        void PlanShoot(DantiaoGeneral w, DantiaoGeneral g, int dir)
        {
            var outcome = DantiaoRules.RollShot(w, g, rng);
            Arrow arrow = null;

            Do(() =>
            {
                //箭落下之前镜头要把双方都框进来
                fleeing = null;
                g.MoveTo(dir < 0 ? -500 : WorldWidth + 500, g.Gy, 180);
            });
            Wait(0.6f);
            Do(() =>
            {
                w.FaceTo(g);
                w.SetPose(DantiaoPose.Raise, 0.6f);
                Log(T("冷箭.张弓", "射", w.Name, "逃", g.Name));
                Speak(w, T("冷箭台词"), 1.2f);
            });
            Wait(0.45f);
            Do(() =>
            {
                PlaySound($"Swing{w.WeaponSound}a");
                arrow = new Arrow() { FromX = w.X, FromGy = w.Gy, Target = g, Miss = outcome == ShotOutcome.Miss };
                arrows.Add(arrow);
            });
            WaitUntil(() => arrow == null || arrow.Done, 1.5f);
            Do(() =>
            {
                if (outcome == ShotOutcome.Miss)
                {
                    //沉稳的神射手故意射盔缨示警
                    Log(T(w.Person.Calmness >= 7 && DantiaoText.Has("冷箭.盔缨") ? "冷箭.盔缨" : "冷箭.射空", "射", w.Name, "逃", g.Name));
                    fleeing = g;
                    g.MoveTo(dir < 0 ? -500 : WorldWidth + 500, g.Gy, 260);
                    return;
                }

                bloods.Add(new Blood() { X = g.X, Gy = g.Gy });
                PlaySound("Hit" + rng.Next(1, 4));
                PlayVoice(g, "Heng");

                if (outcome == ShotOutcome.Graze)
                {
                    g.Life = Math.Max(1f, g.Life - 10f);
                    g.Nudge(dir * 12);
                    Log(T("冷箭.带箭", "射", w.Name, "逃", g.Name));
                    fleeing = g;
                    return;
                }

                g.FallOff(dir);
                PlaySound("Fall");
                g.Life = outcome == ShotOutcome.Kill ? 0f : Math.Max(1f, g.Life - 15f);
                Result = outcome == ShotOutcome.Kill ? (g == left ? 3 : 4) : (g == left ? 2 : 1);
                Log(T(outcome == ShotOutcome.Kill ? "冷箭.射杀" : "冷箭.落马", "射", w.Name, "逃", g.Name));
            });

            if (outcome == ShotOutcome.Kill || outcome == ShotOutcome.Fall)
            {
                Wait(0.9f);
                Do(() =>
                {
                    w.SetPose(DantiaoPose.Raise);
                    Victory(w, $"{w.Name}获胜！");
                });
            }
            else
            {
                WaitUntil(() => Math.Abs(g.X - w.X) > viewWidth, 4f);
                Do(() => FinishEscaped(g));
            }
        }

        void FinishEscaped(DantiaoGeneral g)
        {
            g.Fled = true;
            Result = g == left ? 5 : 6;
            Log(T("结局.败走", "逃", g.Name, "回合", round));
            ShowTitle($"{g.Name}败走！");
        }

        void PlanCapture(DantiaoGeneral a, DantiaoGeneral d)
        {
            Do(() =>
            {
                d.Stop();
                a.MoveTo(Clamp(d.X + (a.X < d.X ? -45 : 45), 100, WorldWidth - 100), d.Gy + 3, 200);
                PlaySound("Horse");
            });
            WaitUntil(() => a.Arrived, 3f);
            Do(() =>
            {
                a.FaceTo(d);
                a.SetPose(DantiaoPose.Raise, 0.4f);
                PlayVoice(a, "Ha");
            });
            Wait(0.35f);
            Do(() =>
            {
                if (rng.NextDouble() < DantiaoRules.CaptureChance(a, d))
                {
                    Interrupt();
                    Result = d == left ? 7 : 8;
                    d.FallOff(a.X > d.X ? 1 : -1);
                    PlaySound("Fall");
                    Log(T("生擒.成功", "攻", a.Name, "守", d.Name));
                    Wait(0.8f);
                    Do(() =>
                    {
                        Log(T("结局.生擒", "胜", a.Name, "败", d.Name, "回合", round));
                        Speak(a, T("生擒台词"));
                        ShowTitle($"生擒{d.Name}！");
                    });
                }
                else
                {
                    d.SetPose(DantiaoPose.Recoil, 0.35f);
                    d.Nudge(a.X < d.X ? 20 : -20);
                    a.Stamina = Math.Max(0f, a.Stamina - 8f);
                    Log(T("生擒.挣脱", "攻", a.Name, "守", d.Name));
                    Wait(0.6f);
                }
            });
        }

        void PlanPersuade(DantiaoGeneral p, DantiaoGeneral t)
        {
            persuadeAttempts++;

            Do(() =>
            {
                left.Stop();
                right.Stop();
                p.FaceTo(t);
                t.FaceTo(p);
                var line = DantiaoRules.PersuadeLine(t, rng);
                Speak(p, line, 2.5f);
            });
            Wait(2.6f);
            Do(() =>
            {
                if (rng.NextDouble() < DantiaoRules.PersuadeChance(p, t))
                {
                    Interrupt();
                    Result = t == left ? 9 : 10;
                    Speak(t, T("降服台词"));
                    Log(T("说服.成功", "降", t.Name, "说", p.Name));
                    Wait(1.8f);
                    Do(() =>
                    {
                        p.SetPose(DantiaoPose.Raise);
                        Log(T("结局.说服", "降", t.Name, "说", p.Name, "回合", round));
                        ShowTitle($"{t.Name}被说服！");
                    });
                }
                else
                {
                    Speak(t, T("拒降台词"), 1.8f);
                    Wait(1.6f);
                }
            });
        }

        DantiaoGeneral Other(DantiaoGeneral g) => g == left ? right : left;

        string T(string key, params object[] vars) => DantiaoText.Pick(rng, key, vars);

        void Speak(DantiaoGeneral g, string text, float time = 2.2f)
        {
            if (String.IsNullOrEmpty(text)) return;
            g.Say(text, time);
            Log($"{g.Name}：“{text}”");
        }

        void Log(string text)
        {
            if (String.IsNullOrEmpty(text)) return;
            logs.Add("* " + text);
            logWrapScale = 0f;  //触发重新换行
        }

        #endregion

        #region 更新

        public void Update(float gameTime)
        {
            UpdateScale();
            HandleInput();

            if (closing)
            {
                closeTime += gameTime;
                alpha = Math.Max(0f, 1f - closeTime / 0.8f);
                if (closeTime >= 0.8f)
                {
                    Close();
                }
                return;
            }

            if (startDelay > 0f)
            {
                startDelay -= gameTime;
                return;
            }

            if (!String.IsNullOrEmpty(title))
            {
                overTime += gameTime;
            }

            if (paused) return;

            alpha = Math.Min(1f, alpha + gameTime / 0.8f);

            var dt = gameTime * speed;
            if (slowTime > 0f)
            {
                slowTime -= gameTime;
                dt *= 0.3f;
            }

            RunScript(dt);

            left.Update(dt);
            right.Update(dt);

            foreach (var b in bloods) b.Time += dt;
            bloods.RemoveAll(b => b.Time > 0.45f);
            foreach (var a in arrows) a.Time += dt;
            arrows.RemoveAll(a => a.Done);

            UpdateCamera(dt);
        }

        void UpdateScale()
        {
            var resY = Session.ResolutionY > 0 ? Session.ResolutionY : 720;
            var resX = Session.ResolutionX > 0 ? Session.ResolutionX : 1280;
            var totalH = VirtualHeight + TitleBarHeight + 8;
            scale = Math.Min(1f, Math.Min((resX - 8) / PanelWidth, resY / totalH));
            origin = new Vector2((float)Math.Floor((resX - PanelWidth * scale) / 2), (float)Math.Floor((resY - (VirtualHeight - TitleBarHeight) * scale) / 2));
        }

        float TargetHorizon()
        {
            var gy = (left.Gy + right.Gy) / 2;
            return Clamp(VirtualHeight * 0.56f - gy, 160f, 290f);
        }

        void UpdateCamera(float dt)
        {
            float midX, midGy;

            if (fleeing != null)
            {
                var stay = Other(fleeing);
                midX = stay.X;
                midGy = stay.Gy;
            }
            else if (focus == 1)
            {
                midX = left.X;
                midGy = left.Gy;
            }
            else if (focus == 2)
            {
                midX = right.X;
                midGy = right.Gy;
            }
            else
            {
                midX = (left.X + right.X) / 2;
                midGy = (left.Gy + right.Gy) / 2;
            }

            var targetX = Clamp(midX - viewWidth / 2, 0f, Math.Max(0f, WorldWidth - viewWidth));
            var targetH = Clamp(VirtualHeight * 0.56f - midGy, 160f, 290f);
            var k = Math.Min(1f, dt * 3f);
            camX += (targetX - camX) * k;
            horizonY += (targetH - horizonY) * k;
        }

        bool KeyPressed(Keys key)
        {
            return InputManager.KeyBoardState.IsKeyDown(key) && !InputManager.KeyBoardStatePre.IsKeyDown(key);
        }

        Vector2 MouseVirtual => (new Vector2(InputManager.PoX, InputManager.PoY) - origin) / scale;

        Box StoryButtonRect => new Box(14, VirtualHeight - 50, 96, 43);

        //头像框：P1在顶部左端，P2在顶部右端
        Box LeftPortraitRect => new Box(12, 8, 169 * PortraitScale, 188 * PortraitScale);

        Box RightPortraitRect => new Box(viewWidth - 12 - 169 * PortraitScale, 8, 169 * PortraitScale, 188 * PortraitScale);

        Box PagePrevRect => new Box(122, VirtualHeight - 42, 44, 26);

        Box PageNextRect => new Box(248, VirtualHeight - 42, 44, 26);

        void HandleInput()
        {
            //ESC会被记为返回键，单挑期间吞掉，免得退出后误触主界面
            var esc = KeyPressed(Keys.Escape);
            _ = InputManager.IsBackPressed;

            if (closing || !Platform.IsActive) return;

            var mouse = MouseVirtual;
            var leftClick = InputManager.IsPressed;
            var rightClick = InputManager.NowMouse.RightButton == ButtonState.Pressed && InputManager.MouseStatePre.RightButton != ButtonState.Pressed;
            var onUI = false;

            if (leftClick)
            {
                if (StoryButtonRect.Contains(mouse))
                {
                    storyVisible = !storyVisible;
                    onUI = true;
                }
                else if (storyVisible && PagePrevRect.Contains(mouse))
                {
                    logBack = Math.Min(PageCount - 1, logBack + 1);
                    onUI = true;
                }
                else if (storyVisible && PageNextRect.Contains(mouse))
                {
                    logBack = Math.Max(0, logBack - 1);
                    onUI = true;
                }
            }

            if (!String.IsNullOrEmpty(title))
            {
                if (overTime > 0.8f && (leftClick && !onUI || rightClick || esc || KeyPressed(Keys.Space) || KeyPressed(Keys.Enter)))
                {
                    closing = true;
                    closeTime = 0f;
                }
                return;
            }

            if (KeyPressed(Keys.Space)) paused = !paused;
            if (KeyPressed(Keys.Left)) speed = Math.Min(4, speed * 2);
            if (KeyPressed(Keys.Right)) speed = Math.Max(1, speed / 2);
            if (KeyPressed(Keys.Tab)) focus = (focus + 1) % 3;
        }

        void Close()
        {
            var screen = Session.MainGame.mainGameScreen;
            screen.dantiaoLayer = null;

            if (damage == null)
            {
                screen.ReturnToMainMenu();
            }
            else
            {
                screen.cloudLayer.IsStart = false;
                screen.cloudLayer.IsVisible = false;
                screen.cloudLayer.Reverse = false;

                damage.ChallengeHappened = true;
                damage.ChallengeStarted = false;
                damage.ChallengeResult = Result;
                damage.ChallengeSourcePerson = left.Person;
                damage.ChallengeDestinationPerson = right.Person;

                screen.EnableUpdate = true;
            }

            Persons = null;
        }

        #endregion

        #region 绘制

        Vector2 ToScreen(float x, float gy) => new Vector2(x - camX, horizonY + gy);

        Color Fade(Color c) => c * alpha;

        Vector2 Screen(Vector2 pos) => origin + pos * scale;

        /// <summary>
        /// 按窗体范围裁剪后绘制，坐标取整保证像素清晰
        /// </summary>
        void DrawClipped(Texture2D tex, Rectangle source, Vector2 pos, SpriteEffects effect, float depth)
        {
            var x = (float)Math.Round(pos.X);
            var y = (float)Math.Round(pos.Y);
            var x0 = Math.Max(x, 0f);
            var y0 = Math.Max(y, 0f);
            var x1 = Math.Min(x + source.Width, PanelWidth);
            var y1 = Math.Min(y + source.Height, VirtualHeight);
            if (x1 <= x0 || y1 <= y0) return;

            var cutLeft = (int)(x0 - x);
            var cutTop = (int)(y0 - y);
            var w = (int)(x1 - x0);
            var h = (int)(y1 - y0);
            var srcX = effect == SpriteEffects.FlipHorizontally ? source.X + source.Width - cutLeft - w : source.X + cutLeft;
            var src = new Rectangle(srcX, source.Y + cutTop, w, h);

            Session.Current.SpriteBatch.Draw(tex, Screen(new Vector2(x0, y0)), src, Fade(Color.White), 0f, Vector2.Zero, scale, effect, depth);
        }

        void DrawSprite(Texture2D tex, Rectangle source, Vector2 pos, SpriteEffects effect, float depth)
        {
            if (tex == null || tex.IsDisposed) return;
            DrawClipped(tex, source, pos, effect, depth);
        }

        void DrawImage(string name, Vector2 pos, Rectangle? source, float depth)
        {
            var tex = CacheManager.LoadTexture(Root + name);
            if (tex == null || tex.IsDisposed) return;
            DrawClipped(tex, source ?? tex.Bounds, pos, SpriteEffects.None, depth);
        }

        void FillRect(Box box, Color color, float depth, bool screenSpace = false)
        {
            if (whitePixel == null || whitePixel.IsDisposed)
            {
                whitePixel = new Texture2D(Session.Current.SpriteBatch.GraphicsDevice, 1, 1);
                whitePixel.SetData(new[] { Color.White });
            }
            var pos = screenSpace ? new Vector2(box.X, box.Y) : Screen(new Vector2(box.X, box.Y));
            var size = screenSpace ? new Vector2(box.Width, box.Height) : new Vector2(box.Width, box.Height) * scale;
            Session.Current.SpriteBatch.Draw(whitePixel, new Rectangle((int)pos.X, (int)pos.Y, (int)Math.Ceiling(size.X), (int)Math.Ceiling(size.Y)), null, color, 0f, Vector2.Zero, SpriteEffects.None, depth);
        }

        float FontScale(float px) => px * scale / CacheManager.FontPair.Size;

        void DrawText(string text, Vector2 pos, float px, Color color, float depth)
        {
            CacheManager.DrawString(null, text, Screen(pos), Fade(color), 0f, Vector2.Zero, FontScale(px), SpriteEffects.None, depth);
        }

        float TextWidth(string text, float px)
        {
            var bounds = CacheManager.CalculateTextBounds(null, text, Vector2.Zero, FontScale(px));
            if (bounds == null || bounds.Count == 0) return 0f;
            return (bounds.Max(b => b.X2) - bounds.Min(b => b.X)) / scale;
        }

        string WrapText(string text, float width, float px)
        {
            var wrapped = CacheManager.AutoWrap(null, text, width * scale, FontScale(px)) ?? text;
            var lines = new List<string>();
            foreach (var line in wrapped.Replace("\r\n", "\n").TrimEnd('\n').Split('\n'))
            {
                //只剩标点的行并回上一行
                if (lines.Count > 0 && IsPunctuationLine(line))
                {
                    lines[^1] += line.Trim();
                }
                else
                {
                    lines.Add(line);
                }
            }
            return String.Join("\n", lines);
        }

        static bool IsPunctuationLine(string line)
        {
            var trimmed = line.Trim();
            return trimmed.Length > 0 && trimmed.All(c => "。，！？、”’）".Contains(c));
        }

        void DrawTextCentered(string text, float centerX, float y, float px, Color color, float depth)
        {
            DrawText(text, new Vector2(centerX - TextWidth(text, px) / 2, y), px, color, depth);
        }

        public void Draw()
        {
            if (startDelay > 0f && !closing) return;

            UpdateScale();

            DrawWindow();
            DrawBackground();

            left.Draw(ToScreen, DrawSprite);
            right.Draw(ToScreen, DrawSprite);

            DrawBloods();
            DrawArrows();
            DrawBubble(left);
            DrawBubble(right);
            DrawHover();
            DrawPanels();
            DrawLog();
            DrawStatus();
        }

        /// <summary>
        /// 窗体外框、标题栏，以及窗体外的暗色遮罩
        /// </summary>
        void DrawWindow()
        {
            var resX = Session.ResolutionX > 0 ? Session.ResolutionX : 1280;
            var resY = Session.ResolutionY > 0 ? Session.ResolutionY : 720;
            FillRect(new Box(0, 0, resX, resY), Color.Black * (0.55f * alpha), 0.09998f, true);

            var bar = TitleBarHeight;
            FillRect(new Box(-4, -bar - 4, PanelWidth + 8, VirtualHeight + bar + 8), new Color(60, 40, 20) * alpha, 0.0999f);
            FillRect(new Box(-2, -bar - 2, PanelWidth + 4, bar), new Color(110, 70, 30) * alpha, 0.09985f);
            DrawText("单　挑", new Vector2(10, -bar + 2), 18, new Color(255, 230, 170), 0.0998f);
        }

        void DrawBackground()
        {
            //天空（远景，视差最小）
            var skyY = horizonY + 12 - 348f;
            var skyOff = -(camX * 0.25f % 1200f);
            for (var x = skyOff; x < viewWidth; x += 1200f)
            {
                DrawImage(@"Map\Sky.png", new Vector2(x, skyY), null, 0.0995f);
            }

            //远山树林
            var treeOff = -(camX * 0.6f % 1000f);
            for (var x = treeOff; x < viewWidth; x += 1000f)
            {
                DrawImage(@"Map\Trees.png", new Vector2(x, horizonY - 40), null, 0.0985f);
            }

            //地面
            var groundOff = -(camX % 2300f);
            for (var x = groundOff; x < viewWidth; x += 2300f)
            {
                DrawImage(@"Map\Ground.png", new Vector2(x, horizonY), null, 0.099f);
            }
        }

        void DrawBloods()
        {
            foreach (var b in bloods)
            {
                var frame = Math.Min(2, (int)(b.Time / 0.15f));
                var pos = ToScreen(b.X, b.Gy) + new Vector2(-26, -95);
                DrawImage(@"Eff\Blood.png", pos, new Rectangle(frame * 52, 0, 52, 52), 0.03f);
            }
        }

        void DrawArrows()
        {
            foreach (var a in arrows)
            {
                //射空的从头顶掠过
                var height = a.Miss ? 125f : 70f;
                var k = Math.Min(1f, a.Time / Arrow.Duration);
                var from = ToScreen(a.FromX, a.FromGy) - new Vector2(0, 75f);
                var to = ToScreen(a.Target.X, a.Target.Gy) - new Vector2(0, height);
                var pos = Vector2.Lerp(from, to, k);
                if (pos.X < 0 || pos.X > PanelWidth || pos.Y < 0 || pos.Y > VirtualHeight) continue;

                var angle = (float)Math.Atan2(to.Y - from.Y, to.X - from.X);
                FillLine(pos, angle, 30f, 2f, Fade(new Color(90, 60, 30)), 0.028f);
                FillLine(pos, angle, 6f, 4f, Fade(Color.DimGray), 0.0279f);
            }
        }

        /// <summary>
        /// 从pos往angle反方向画一条线（箭头在pos）
        /// </summary>
        void FillLine(Vector2 pos, float angle, float length, float thickness, Color color, float depth)
        {
            Session.Current.SpriteBatch.Draw(whitePixel, Screen(pos), null, color, angle, new Vector2(1f, 0.5f), new Vector2(length, thickness) * scale, SpriteEffects.None, depth);
        }

        void DrawBubble(DantiaoGeneral g)
        {
            //结局标题出现后台词只留在评书里，免得遮挡标题
            if (g.SayTime <= 0f || String.IsNullOrEmpty(g.SayText) || !String.IsNullOrEmpty(title)) return;

            var image = g.Side == 0 ? @"System\Talk1.png" : @"System\Talk2.png";
            var width = (g.Side == 0 ? 319f : 337f) * BubbleScale;
            var height = 96f * BubbleScale;
            var anchor = ToScreen(g.ScreenX, g.Gy);
            //底边在头顶上方；不高过顶部的头像体力条，免得被挡住
            var pos = new Vector2((float)Math.Round(Clamp(anchor.X - width / 2, 6f, viewWidth - width - 6f)), (float)Math.Round(Math.Max(TopBandHeight + 4f, anchor.Y - 109f - height)));

            var tex = CacheManager.LoadTexture(Root + image);
            if (tex == null || tex.IsDisposed) return;
            Session.Current.SpriteBatch.Draw(tex, Screen(pos), null, Fade(Color.White), 0f, Vector2.Zero, BubbleScale * scale, SpriteEffects.None, 0.026f);

            const float px = 16f;
            const float lineHeight = 19f;
            var textArea = width - 70f * BubbleScale;
            var progress = Math.Min(1f, (g.SayTotal - g.SayTime) / 0.6f);
            var shown = g.SayText.Substring(0, (int)Math.Ceiling(g.SayText.Length * progress));
            //按完整台词排版，逐字显示时位置不跳动
            var full = WrapText(g.SayText, textArea, px);
            var lines = full.Split('\n');
            var textWidth = lines.Max(l => TextWidth(l, px));
            DrawText(WrapText(shown, textArea, px), pos + new Vector2((width - textWidth) / 2, (height - lines.Length * lineHeight) / 2), px, Color.Black, 0.025f);
        }

        void DrawHover()
        {
            var mouse = MouseVirtual;
            foreach (var g in new[] { left, right })
            {
                if (!g.Mounted && g.Fled) continue;
                var s = ToScreen(g.ScreenX, g.Gy);
                if (new Box(s.X - 40, s.Y - 110, 80, 110).Contains(mouse))
                {
                    DrawTextCentered(g.Name, s.X, s.Y - 128, 18, Color.Yellow, 0.024f);
                }
            }
        }

        void DrawPanels()
        {
            //左：P1，红色，头像在最左，名字和武力在体力条上方，左对齐
            DrawPortrait(left, LeftPortraitRect);
            var leftBars = new Vector2((float)Math.Round(LeftPortraitRect.Right + 10), 46);
            DrawText($"{left.Name}　武力：{left.Force}", leftBars + new Vector2(6, -28), 20, Color.Red, 0.018f);
            DrawBars(left, leftBars, false);

            //右：P2，蓝色，与P1左右镜像，文字右对齐
            DrawPortrait(right, RightPortraitRect);
            var rightBars = new Vector2((float)Math.Round(RightPortraitRect.X - 10 - 290), 46);
            var rightText = $"{right.Name}　武力：{right.Force}";
            DrawText(rightText, new Vector2(rightBars.X + 290 - 6 - TextWidth(rightText, 20), rightBars.Y - 28), 20, Color.Blue, 0.018f);
            DrawBars(right, rightBars, true);

            //回合数放在双方生命条中间，生命条短，这一行空间宽
            if (round > 0)
            {
                DrawTextCentered($"第 {round} 回合", viewWidth / 2, 74, 28, Color.Red, 0.018f);
            }
        }

        void DrawPortrait(DantiaoGeneral g, Box frame)
        {
            var pos = new Vector2((float)Math.Round(frame.X), (float)Math.Round(frame.Y));
            var topLeft = Screen(pos + new Vector2(8, 8) * PortraitScale);
            var rect = new Rectangle((int)topLeft.X, (int)topLeft.Y, (int)(154 * PortraitScale * scale), (int)(173 * PortraitScale * scale));
            CacheManager.DrawZhsanAvatar(g.Person, rect, 0.021f, PortraitSize.Medium, Fade(Color.White));

            var tex = CacheManager.LoadTexture(Root + @"System\Frame.png");
            if (tex == null || tex.IsDisposed) return;
            Session.Current.SpriteBatch.Draw(tex, Screen(pos), null, Fade(Color.White), 0f, Vector2.Zero, PortraitScale * scale, SpriteEffects.None, 0.020f);
        }

        void DrawBars(DantiaoGeneral g, Vector2 pos, bool mirror)
        {
            DrawBar(pos, 1, g.Stamina / 100f, mirror, 1f);
            DrawBar(pos + new Vector2(0, 32), 2, g.Life / 100f, mirror, LifeBarScale);
        }

        /// <summary>
        /// 画一条体力/生命条。length是相对原图290宽的长度倍数，缩短的条靠头像一侧对齐
        /// </summary>
        void DrawBar(Vector2 pos, int row, float ratio, bool mirror, float length)
        {
            var tex = CacheManager.LoadTexture(Root + @"System\Bars.png");
            if (tex == null || tex.IsDisposed) return;

            //镜像时整条左右翻转，从右往左填充
            var effect = mirror ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            ratio = Clamp(ratio, 0f, 1f);
            var size = new Vector2(scale * length, scale);
            var x = mirror ? pos.X + 290 * (1 - length) : pos.X;
            Session.Current.SpriteBatch.Draw(tex, Screen(new Vector2(x, pos.Y)), new Rectangle(0, 0, 290, 24), Fade(Color.White), 0f, Vector2.Zero, size, effect, 0.019f);
            var w = (int)Math.Round(290 * ratio);
            if (w <= 0) return;
            var fillX = mirror ? x + (290 - w) * length : x;
            Session.Current.SpriteBatch.Draw(tex, Screen(new Vector2(fillX, pos.Y)), new Rectangle(0, row * 24, w, 24), Fade(Color.White), 0f, Vector2.Zero, size, effect, 0.0185f);
        }

        int PageCount => Math.Max(1, (logLines.Count + LogPageLines - 1) / LogPageLines);

        void WrapLogs()
        {
            var fs = FontScale(17);
            if (logWrapScale == fs) return;
            logWrapScale = fs;
            logLines.Clear();
            var width = (viewWidth - 40f) * scale;
            foreach (var log in logs)
            {
                var wrapped = CacheManager.AutoWrap(null, log, width, fs) ?? log;
                foreach (var line in wrapped.Replace("\r\n", "\n").Split('\n').Where(l => !String.IsNullOrWhiteSpace(l)))
                {
                    //只剩标点的行并回上一行
                    if (logLines.Count > 0 && IsPunctuationLine(line))
                    {
                        logLines[^1] += line.Trim();
                    }
                    else
                    {
                        logLines.Add(line);
                    }
                }
            }
        }

        void DrawLog()
        {
            var storySrc = StoryButtonRect.Contains(MouseVirtual) ? new Rectangle(0, 43, 96, 43) : new Rectangle(0, 0, 96, 43);
            DrawImage(@"System\Story.png", new Vector2(StoryButtonRect.X, StoryButtonRect.Y), storySrc, 0.019f);

            if (!storyVisible) return;

            WrapLogs();

            var pages = PageCount;
            var back = Math.Min(logBack, pages - 1);
            var end = logLines.Count - back * LogPageLines;
            var lines = logLines.Skip(Math.Max(0, end - LogPageLines)).Take(Math.Min(end, LogPageLines)).ToList();
            //不显示被截断的半条
            while (lines.Count > 1 && !lines[0].StartsWith("*"))
            {
                lines.RemoveAt(0);
            }
            var page = pages - 1 - back;

            var top = VirtualHeight - 56 - LogPageLines * 23;
            for (int i = 0; i < lines.Count; i++)
            {
                DrawText(lines[i], new Vector2(20, top + i * 23), 17, Color.Black, 0.017f);
            }

            DrawText("上页", new Vector2(PagePrevRect.X, PagePrevRect.Y), 18, page > 0 ? Color.DarkBlue : Color.Gray, 0.017f);
            DrawTextCentered($"{page + 1}/{pages} 页", (PagePrevRect.Right + PageNextRect.X) / 2, PagePrevRect.Y, 18, Color.Blue, 0.017f);
            DrawText("下页", new Vector2(PageNextRect.X, PageNextRect.Y), 18, page < pages - 1 ? Color.DarkBlue : Color.Gray, 0.017f);
        }

        void DrawStatus()
        {
            if (!String.IsNullOrEmpty(title))
            {
                DrawTextCentered(title, viewWidth / 2, 150, 50, Color.Red, 0.012f);
                if (overTime > 0.8f)
                {
                    DrawTextCentered("（按“鼠标右键/空格/ESC”键退出）", viewWidth / 2, 212, 20, Color.Blue, 0.012f);
                }
                return;
            }

            //操作提示放在标题栏右侧，顶部留给双方头像体力条
            var hint = "空格 暂停　← 加速　→ 减速　Tab 切换视角" + (speed > 1 ? $"　×{speed}" : "");
            DrawText(hint, new Vector2(viewWidth - 10 - TextWidth(hint, 15), -TitleBarHeight + 4), 15, new Color(255, 230, 170), 0.0998f);

            if (paused)
            {
                DrawTextCentered("暂停", viewWidth / 2, 130, 40, Color.DarkRed, 0.012f);
            }
        }

        #endregion

        struct Box
        {
            public float X, Y, Width, Height;

            public Box(float x, float y, float width, float height)
            {
                X = x;
                Y = y;
                Width = width;
                Height = height;
            }

            public float Right => X + Width;

            public bool Contains(Vector2 p) => p.X >= X && p.X < X + Width && p.Y >= Y && p.Y < Y + Height;
        }

        static float Clamp(float v, float min, float max) => Math.Max(min, Math.Min(max, v));

        float Range(float min, float max) => min + (float)rng.NextDouble() * (max - min);

        void PlaySound(string name)
        {
            Platform.Current.PlayEffect(SoundRoot + name);
        }

        void PlayVoice(DantiaoGeneral g, string kind)
        {
            PlaySound(g.Voice + kind);
        }
    }
}

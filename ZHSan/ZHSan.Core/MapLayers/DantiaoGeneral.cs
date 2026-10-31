using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GameManager;
using GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Platforms;

namespace WorldOfTheThreeKingdoms.GameScreens.ScreenLayers
{
    public enum DantiaoPose
    {
        Ride,       //骑马（待机/移动）
        Chop,       //劈砍
        Thrust,     //突刺
        Guard,      //格挡
        Recoil,     //中招
        Raise,      //举兵器（得胜/大喝）
        Taunt,      //指着对方骂阵
        Pant,       //喘息
        Airborne,   //被打飞落马
        Lying,      //倒地
        Beheaded    //首级被斩
    }

    /// <summary>
    /// 曹操传格式的S形象：atk（64格，12帧）、mov（48格，11帧）、spc（48格，5帧），
    /// 竖排，每个方向依次为正面、背面、侧面（朝左）。载入时去掉紫红底色并按最近邻放大2倍
    /// </summary>
    public class DantiaoUnitSprite
    {
        public const string Root = @"Content\Textures\Resources\Dantiao\Unit\";
        public const int Zoom = 2;
        public const int MovCell = 48 * Zoom;
        public const int AtkCell = 64 * Zoom;
        //atk格比mov格大16像素，以中心对齐
        public const int AtkOffset = (AtkCell - MovCell) / 2;

        static readonly Dictionary<string, DantiaoUnitSprite> cache = new Dictionary<string, DantiaoUnitSprite>();

        public Texture2D Atk, Mov, Spc;

        /// <summary>
        /// 按武将ID取形象，没有则按性别用默认形象
        /// </summary>
        public static DantiaoUnitSprite Get(Person person)
        {
            var dir = HasSprite(person.ID.ToString()) ? person.ID.ToString() : person.Sex ? "DefaultFemale" : "Default";

            lock (cache)
            {
                if (!cache.TryGetValue(dir, out var sprite) || sprite.Mov == null || sprite.Mov.IsDisposed)
                {
                    sprite = new DantiaoUnitSprite()
                    {
                        Atk = Load(dir, "atk"),
                        Mov = Load(dir, "mov"),
                        Spc = Load(dir, "spc")
                    };
                    cache[dir] = sprite;
                }
                return sprite;
            }
        }

        static bool HasSprite(string dir)
        {
            return FindFile(dir, "mov") != null;
        }

        static string FindFile(string dir, string kind)
        {
            foreach (var ext in new[] { ".png", ".bmp" })
            {
                var path = Platform.Current.GetMODFile(Root + dir + @"\" + kind + ext);
                if (Platform.Current.FileExists(path)) return path;
            }
            return null;
        }

        static Texture2D Load(string dir, string kind)
        {
            var path = FindFile(dir, kind);
            if (path == null) return null;

            try
            {
                var device = Session.Current.SpriteBatch.GraphicsDevice;
                using var stream = new MemoryStream(Platform.Current.LoadFile(path));
                using var raw = Texture2D.FromStream(device, stream);

                var w = raw.Width;
                var h = raw.Height;
                var src = new Color[w * h];
                raw.GetData(src);

                var dst = new Color[w * Zoom * h * Zoom];
                for (int y = 0; y < h; y++)
                {
                    for (int x = 0; x < w; x++)
                    {
                        var c = src[y * w + x];
                        //紫红底色(247,0,255)和纯蓝底色(0,0,255)视为透明。纯蓝要完全相等，人物身上有相近的深蓝
                        if (c.R >= 230 && c.G <= 20 && c.B >= 230 || c.R == 0 && c.G == 0 && c.B == 255) c = Color.Transparent;
                        for (int dy = 0; dy < Zoom; dy++)
                        {
                            for (int dx = 0; dx < Zoom; dx++)
                            {
                                dst[(y * Zoom + dy) * w * Zoom + x * Zoom + dx] = c;
                            }
                        }
                    }
                }

                var tex = new Texture2D(device, w * Zoom, h * Zoom);
                tex.SetData(dst);
                return tex;
            }
            catch
            {
                return null;
            }
        }
    }

    /// <summary>
    /// 单挑中的一名武将：属性、动作、移动与绘制
    /// </summary>
    public class DantiaoGeneral
    {
        //mov帧里马蹄落地处，作为武将的站位锚点
        static readonly Vector2 Foot = new Vector2(24 * DantiaoUnitSprite.Zoom, 47 * DantiaoUnitSprite.Zoom);

        //侧面帧序号（朝左）
        const int MovSideWalk = 4;      //4、5两帧
        const int MovSideGuard = 8;
        const int MovFront = 0;
        const int MovBack = 2;
        const int MovPant = 9;          //9、10两帧
        const int SpcTaunt = 0;
        const int SpcSideHit = 2;
        const int SpcFallen = 3;
        const int SpcRaise = 4;

        //(atk帧, 时长)，ImpactIndex为命中判定所在帧
        static readonly (int Frame, float Time)[] ChopFrames = { (8, 0.24f), (9, 0.08f), (10, 0.08f), (11, 0.22f) };
        static readonly (int Frame, float Time)[] ThrustFrames = { (8, 0.20f), (10, 0.10f), (11, 0.26f) };
        const int ChopImpact = 2;
        const int ThrustImpact = 1;

        public Person Person { get; }
        public int Side { get; }  //0：P1（左），1：P2（右）
        public string Name => Person.Name;
        public int Force { get; }
        public float Life = 100f;
        public float Stamina = 100f;
        public int Rage;  //被骂阵激怒后剩余的回合数
        public bool Desperate;  //伤重时奋起死战：出手更狠，不再逃跑

        public float HorseRate { get; }
        public string Weapon { get; }  //兵器的叫法，如“刀”“矛”，认不出时为“兵器”
        public string WeaponKind { get; }  //兵器类别，如“刀”“枪”，认不出时为null
        public bool IsArcher { get; }  //善射，对方败走时可能放冷箭
        public string Voice { get; }
        public int WeaponSound { get; }

        public float X, Gy;
        public int Facing;  //1朝右，-1朝左（素材原始朝左）
        public float TargetX, TargetGy, MoveSpeed;
        public bool Moving;
        public bool KeepFacing;  //后退时保持朝向
        float speed;

        public bool Mounted = true;

        public DantiaoPose Pose = DantiaoPose.Ride;
        float poseTime;
        float poseHold;
        bool impactFired;
        public bool ImpactReady;

        float turnTime;
        bool turnFront;
        float animTime;

        float offsetX;  //闪避/受击时的短暂位移
        Vector2 airVelocity;

        public string SayText = "";
        public float SayTime;
        public float SayTotal;

        public bool Fled;

        public bool Reserve;  //在场边待命的候补
        public bool Out;      //已退场（败走、被救回），头像变灰

        public DantiaoGeneral(Person person, int side)
        {
            Person = person;
            Side = side;
            Force = Math.Max(1, person.ChallengeStrength);
            WeaponSound = person.ID % 3;
            Voice = person.Sex ? "Woman" : "Man";
            HorseRate = GetHorseRate(person);
            (Weapon, WeaponKind) = DantiaoText.WeaponOf(person);
            IsArcher = DantiaoText.IsArcher(person);
        }

        /// <summary>
        /// 按坐骑宝物名称决定马速
        /// </summary>
        static float GetHorseRate(Person person)
        {
            var horse = person.Treasures.FirstOrDefault(t => t.Influences.Values.Any(x => x.Kind.ID == 5110));
            if (horse == null) return 1f;

            var name = horse.Name ?? "";
            if (name.Contains("赤兔")) return 1.3f;
            if (name.Contains("爪黄") || name.Contains("的卢") || name.Contains("绝影")) return 1.2f;
            return 1.15f;
        }

        public bool IsAttacking => Pose == DantiaoPose.Chop || Pose == DantiaoPose.Thrust;

        public bool IsDown => Pose == DantiaoPose.Airborne || Pose == DantiaoPose.Lying || Pose == DantiaoPose.Beheaded;

        public bool Arrived => !Moving;

        public float ScreenX => X + offsetX;

        public void Place(float x, float gy, int facing)
        {
            X = TargetX = x;
            Gy = TargetGy = gy;
            Facing = facing;
            Moving = false;
            speed = 0f;
        }

        public void MoveTo(float x, float gy, float moveSpeed, bool keepFacing = false)
        {
            TargetX = x;
            TargetGy = gy;
            MoveSpeed = moveSpeed * HorseRate;
            KeepFacing = keepFacing;
            Moving = true;
        }

        public void Stop()
        {
            Moving = false;
            speed = 0f;
            TargetX = X;
            TargetGy = Gy;
        }

        public void Face(int facing)
        {
            if (facing != 0 && facing != Facing && Mounted)
            {
                StartTurn();
            }
        }

        public void FaceTo(DantiaoGeneral other)
        {
            Face(other.X >= X ? 1 : -1);
        }

        void StartTurn()
        {
            if (turnTime > 0f) return;
            turnTime = 0.24f;
            turnFront = !turnFront;
        }

        public void Attack(bool chop)
        {
            SetPose(chop ? DantiaoPose.Chop : DantiaoPose.Thrust);
        }

        public void SetPose(DantiaoPose pose, float hold = 0f)
        {
            //说话时不用正面举兵器的帧，保持侧身骑马
            if (IsFrontPose(pose) && SayTime > 0f)
            {
                pose = DantiaoPose.Ride;
            }

            Pose = pose;
            poseTime = 0f;
            poseHold = hold;
            impactFired = false;
            ImpactReady = false;
        }

        public void Nudge(float dx)
        {
            offsetX = dx;
        }

        public void Say(string text, float time = 2.2f)
        {
            SayText = text;
            SayTime = SayTotal = time;
            if (IsFrontPose(Pose))
            {
                SetPose(DantiaoPose.Ride);
            }
        }

        /// <summary>
        /// spc图里正面举兵器的两帧：骂阵（第0帧）和举兵器（第4帧）
        /// </summary>
        static bool IsFrontPose(DantiaoPose pose) => pose == DantiaoPose.Raise || pose == DantiaoPose.Taunt;

        /// <summary>
        /// 落马：被打得往后跌出，人仰马翻
        /// </summary>
        public void FallOff(int awayDirection)
        {
            Mounted = false;
            Moving = false;
            airVelocity = new Vector2(awayDirection * 70f, 0f);
            SetPose(DantiaoPose.Airborne);
        }

        /// <summary>
        /// 落马后被援将救起，重新上马
        /// </summary>
        public void Remount()
        {
            Mounted = true;
            airVelocity = Vector2.Zero;
            SetPose(DantiaoPose.Ride);
        }

        public void Update(float dt)
        {
            if (SayTime > 0f) SayTime = Math.Max(0f, SayTime - dt);

            animTime += dt;

            if (offsetX != 0f)
            {
                offsetX *= Math.Max(0f, 1f - dt * 6f);
                if (Math.Abs(offsetX) < 0.5f) offsetX = 0f;
            }

            if (turnTime > 0f)
            {
                var before = turnTime;
                turnTime = Math.Max(0f, turnTime - dt);
                if (before > 0.12f && turnTime <= 0.12f)
                {
                    Facing = -Facing;
                }
            }

            //逃跑时虽未落马也要继续移动
            if (Mounted || Fled)
            {
                UpdateMove(dt);
            }

            UpdatePose(dt);
        }

        void UpdateMove(float dt)
        {
            if (!Moving) return;

            var delta = new Vector2(TargetX - X, TargetGy - Gy);
            var dist = delta.Length();

            if (dist < 1.5f)
            {
                X = TargetX;
                Gy = TargetGy;
                Moving = false;
                speed = 0f;
                return;
            }

            const float accel = 420f;
            speed = Math.Min(speed + accel * dt, MoveSpeed);
            speed = Math.Min(speed, (float)Math.Sqrt(2 * accel * dist) + 20f);

            var step = Math.Min(dist, speed * dt);
            X += delta.X / dist * step;
            Gy += delta.Y / dist * step;

            if (!KeepFacing && Math.Abs(delta.X) > 4f)
            {
                Face(delta.X > 0 ? 1 : -1);
            }
        }

        void UpdatePose(float dt)
        {
            poseTime += dt;

            switch (Pose)
            {
                case DantiaoPose.Chop:
                case DantiaoPose.Thrust:
                    var frames = Pose == DantiaoPose.Chop ? ChopFrames : ThrustFrames;
                    var impactAt = frames.Take(Pose == DantiaoPose.Chop ? ChopImpact : ThrustImpact).Sum(f => f.Time);
                    if (!impactFired && poseTime >= impactAt)
                    {
                        impactFired = true;
                        ImpactReady = true;
                    }
                    if (poseTime >= frames.Sum(f => f.Time))
                    {
                        Pose = DantiaoPose.Ride;
                    }
                    break;
                case DantiaoPose.Guard:
                case DantiaoPose.Recoil:
                    if (poseTime >= (poseHold > 0f ? poseHold : 0.3f))
                    {
                        Pose = DantiaoPose.Ride;
                    }
                    break;
                case DantiaoPose.Raise:
                case DantiaoPose.Taunt:
                case DantiaoPose.Pant:
                    if (poseHold > 0f && poseTime >= poseHold)
                    {
                        Pose = DantiaoPose.Ride;
                    }
                    break;
                case DantiaoPose.Airborne:
                    X += airVelocity.X * dt;
                    airVelocity.X *= Math.Max(0f, 1f - dt * 3f);
                    if (poseTime >= 0.38f)
                    {
                        SetPose(DantiaoPose.Lying);
                    }
                    break;
            }
        }

        /// <summary>
        /// 当前帧：所用图（0 mov、1 atk、2 spc）与帧序号
        /// </summary>
        (int Sheet, int Frame) CurrentFrame()
        {
            switch (Pose)
            {
                case DantiaoPose.Chop:
                case DantiaoPose.Thrust:
                    var frames = Pose == DantiaoPose.Chop ? ChopFrames : ThrustFrames;
                    var t = 0f;
                    foreach (var f in frames)
                    {
                        t += f.Time;
                        if (poseTime < t) return (1, f.Frame);
                    }
                    return (1, frames[^1].Frame);
                case DantiaoPose.Guard: return (0, MovSideGuard);
                case DantiaoPose.Recoil: return (2, SpcSideHit);
                case DantiaoPose.Raise: return (2, SpcRaise);
                case DantiaoPose.Taunt: return (2, SpcTaunt);
                case DantiaoPose.Pant: return (0, MovPant + (int)(animTime / 0.4f) % 2);
                case DantiaoPose.Airborne: return (2, SpcSideHit);
                case DantiaoPose.Lying:
                case DantiaoPose.Beheaded: return (2, SpcFallen);
            }

            if (turnTime > 0f) return (0, turnFront ? MovFront : MovBack);

            //跑得快时换帧也快；站着不动时缓缓踏步
            var step = Moving && speed > 150f ? 0.13f : Moving && speed > 5f ? 0.2f : 0.45f;
            return (0, MovSideWalk + (int)(animTime / step) % 2);
        }

        public Rectangle Bounds(Vector2 screen)
        {
            return new Rectangle((int)(screen.X - 40), (int)(screen.Y - 90), 80, 90);
        }

        /// <summary>
        /// 绘制武将。toScreen：世界坐标（x, 纵深）转虚拟屏幕坐标
        /// </summary>
        public void Draw(Func<float, float, Vector2> toScreen, Action<Texture2D, Rectangle, Vector2, SpriteEffects, float> draw)
        {
            if (Fled) return;

            var sprite = DantiaoUnitSprite.Get(Person);
            var (sheet, frame) = CurrentFrame();
            var flip = Facing > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            var pos = toScreen(ScreenX, Gy) - Foot;
            var depth = 0.05f - (int)Gy * 0.00005f - Side * 0.00002f;

            const int mov = DantiaoUnitSprite.MovCell;
            const int atk = DantiaoUnitSprite.AtkCell;

            switch (sheet)
            {
                case 1:
                    draw(sprite.Atk, new Rectangle(0, frame * atk, atk, atk), pos - new Vector2(DantiaoUnitSprite.AtkOffset), flip, depth);
                    break;
                case 2:
                    draw(sprite.Spc, new Rectangle(0, frame * mov, mov, mov), pos, flip, depth);
                    break;
                default:
                    draw(sprite.Mov, new Rectangle(0, frame * mov, mov, mov), pos, flip, depth);
                    break;
            }
        }
    }
}

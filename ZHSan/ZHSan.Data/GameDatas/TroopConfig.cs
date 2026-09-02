
using System.Collections.Generic;
using GameEnums;
using Microsoft.Xna.Framework;

namespace GameDatas;

public class TroopConfig : BaseConfig
{
    /// <summary>
    /// 队长ID
    /// </summary>
    public int LeaderId { get; set; }

    /// <summary>
    /// 可控
    /// </summary>
    public bool Controllable { get; set; } = true;

    /// <summary>
    /// 状态
    /// </summary>
    public TroopStatus Status { get; set; }

    /// <summary>
    /// 方向
    /// </summary>
    public TroopDirection Direction { get; set; } = TroopDirection.East;

    /// <summary>
    /// 委任
    /// </summary>
    public bool Auto { get; set; }

    /// <summary>
    /// 粮草
    /// </summary>
    public int Food { get; set; }

    /// <summary>
    /// 出发地
    /// </summary>
    public int StartingArchitectureString { get; set; }

    /// <summary>
    /// 部队武将
    /// </summary>
    public string PersonsString { get; set; }

    /// <summary>
    /// 位置
    /// </summary>
    public Point Position { get; set; }

    /// <summary>
    /// 目标位置
    /// </summary>
    public Point Destination { get; set; }

    /// <summary>
    /// 真实目标位置
    /// </summary>
    public Point RealDestination { get; set; }

    /// <summary>
    /// 第一层路径
    /// </summary>
    public List<Point> FirstTierPath { get; set; }

    /// <summary>
    /// 第二层路径
    /// </summary>
    public List<Point> SecondTierPath { get; set; }

    /// <summary>
    /// 第三层路径
    /// </summary>
    public List<Point> ThirdTierPath { get; set; }

    public int FirstIndex { get; set; }

    public int SecondIndex { get; set; }

    public int ThirdIndex { get; set; }

    /// <summary>
    /// 编队ID
    /// </summary>
    public int MilitaryID { get; set; }

    /// <summary>
    /// 施展默认种类
    /// </summary>
    public TroopCastDefaultKind CastDefaultKind { get; set; }

    /// <summary>
    /// 施展目标种类
    /// </summary>
    public TroopCastTargetKind CastTargetKind { get; set; }

    /// <summary>
    /// 攻击默认种类
    /// </summary>
    public TroopAttackDefaultKind AttackDefaultKind { get; set; }

    /// <summary>
    /// 攻击目标种类
    /// </summary>
    public TroopAttackTargetKind AttackTargetKind { get; set; }

    /// <summary>
    /// 目标部队
    /// </summary>
    public int TargetTroopID { get; set; }

    /// <summary>
    /// 目标建筑
    /// </summary>
    public int TargetArchitectureID { get; set; }

    /// <summary>
    /// 意愿部队
    /// </summary>
    public int WillTroopID { get; set; }

    /// <summary>
    /// 意愿建筑
    /// </summary>
    public int WillArchitectureID { get; set; }

    /// <summary>
    /// 当前战法
    /// </summary>
    public int CurrentCombatMethodID { get; set; } = -1;

    /// <summary>
    /// 当前计略
    /// </summary>
    public int CurrentStratagemID { get; set; } = -1;

    /// <summary>
    /// 自施展位置
    /// </summary>
    public Point SelfCastPosition { get; set; }

    /// <summary>
    /// 混乱剩余天数
    /// </summary>
    public int ChaosDayLeft { get; set; }

    /// <summary>
    /// 切断粮道剩余天数
    /// </summary>
    public int CutRoutewayDays { get; set; }

    /// <summary>
    /// 俘虏列表
    /// </summary>
    public string CaptivesString { get; set; }

    /// <summary>
    /// 近期战斗过
    /// </summary>
    public int RecentlyFighting { get; set; }

    /// <summary>
    /// 出发点技术攻防加成
    /// </summary>
    public int TechnologyIncrement { get; set; }

    /// <summary>
    /// 事件影响
    /// </summary>
    public string EventInfluencesString { get; set; }

    /// <summary>
    /// 当前特技
    /// </summary>
    public int CurrentStuntIDString { get; set; }

    /// <summary>
    /// 特技剩余天数
    /// </summary>
    public int StuntDayLeft { get; set; }

    /// <summary>
    /// 资金
    /// </summary>
    public int Fund { get; set; }

    /// <summary>
    /// 命令
    /// </summary>
    public string Order { get; set; }

    /// <summary>
    /// 手动控制
    /// </summary>
    public bool ManualControl { get; set; }

    /// <summary>
    /// 强制目标部队
    /// </summary>
    public int ForceTroopTargetId { get; set; }

    /// <summary>
    /// 快速战斗
    /// </summary>
    public bool QuickBattling { get; set; }

    /// <summary>
    /// 可使用的计略
    /// </summary>
    public List<int> AllowedStrategems { get; set; }

    /// <summary>
    /// 俘虏概率
    /// </summary>
    public int CaptureChance { get; set; }

    /// <summary>
    /// 是否将目标位置重设
    /// </summary>
    public bool IsTargetPositionReset { get; set; }

    /// <summary>
    /// 当前动画类型
    /// </summary>
    public TileAnimationKind CurrentTileAnimationKind { get; set; } = TileAnimationKind.无;

    /// <summary>
    /// 被摧毁
    /// </summary>
    public bool Destroyed { get; set; }

    /// <summary>
    /// 影响
    /// </summary>
    public TroopEffect Effect { get; set; }

    /// <summary>
    /// 友军行为类型
    /// </summary>
    public FriendlyActionKind FriendlyAction = FriendlyActionKind.Ignore;

    /// <summary>
    /// 有路径
    /// </summary>
    public bool HasPath { get; set; }

    /// <summary>
    /// 是否需执行战斗动作
    /// </summary>
    public bool HasToDoCombatAction { get; set; }

    /// <summary>
    /// 敌军行为类型
    /// </summary>
    public HostileActionKind HostileAction { get; set; } = HostileActionKind.EvadeEffect;

    /// <summary>
    /// 命令位置
    /// </summary>
    public Point OrderPosition { get; set; }

    /// <summary>
    /// 士气
    /// </summary>
    public int Morale { get; set; }
    
    /// <summary>
    /// 移动动画帧
    /// </summary>
    public List<Point> MoveAnimationFrames { get; set; }

    /// <summary>
    /// 已移动
    /// </summary>
    public bool Moved { get; set; }

    /// <summary>
    /// 操作完成
    /// </summary>
    public bool OperationDone { get; set; }

    /// <summary>
    /// 朝向位置
    /// </summary>
    public Point OrientationPosition { get; set; }
    
    /// <summary>
    /// 暴击防御倍数
    /// </summary>
    public int OutburstDefenceMultiple { get; set; } = 1;

    /// <summary>
    /// 暴击且不进入混乱
    /// </summary>
    public bool OutburstNeverBeIntoChaos { get; set; }

    /// <summary>
    /// 暴击进攻倍数
    /// </summary>
    public int OutburstOffenceMultiple { get; set; } = 1;

    /// <summary>
    /// 暴击防止致命一击
    /// </summary>
    public bool OutburstPreventCriticalStrike { get; set; }

    /// <summary>
    /// 预操作
    /// </summary>
    public TroopPreAction PreAction { get; set; }

    /// <summary>
    /// 上一个位置
    /// </summary>
    public Point PreviousPosition { get; set; }

    /// <summary>
    /// 士兵数
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// 队列结束
    /// </summary>
    public bool QueueEnded { get; set; }

    /// <summary>
    /// 显示路径
    /// </summary>
    public bool ShowPath { get; set; }
    
    /// <summary>
    /// 模拟
    /// </summary>
    public bool Simulating { get; set; }

    /// <summary>
    /// 步骤未完成
    /// </summary>
    public bool StepNotFinished { get; set; }

    /// <summary>
    /// 等待深度混乱帧数
    /// </summary>
    public int WaitForDeepChaosFrameCount { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public TroopStatus TroopStatus { get; set; }

    public bool StuntMustSurround { get; set; }

    // public int targetArchitectureID { get; set; } = -1;

    // public int targetTroopID { get; set; } = -1;

    // public int willArchitectureID { get; set; } = -1;

    // public int willTroopID { get; set; } = -1;
    
    public int AutoCombatMethodID { get; set; } = -1;
}
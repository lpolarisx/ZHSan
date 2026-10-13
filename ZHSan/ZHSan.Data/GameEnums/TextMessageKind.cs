
namespace GameEnums;

/// <summary>
/// 语言类型
/// </summary>
public enum TextMessageKind
{
    /// <summary>
    /// 无
    /// </summary>
    None = 0,

    /// <summary>
    /// 暴击
    /// </summary>
    Critical = 1,

    /// <summary>
    /// 暴击建筑
    /// </summary>
    CriticalArchitecture = 2,

    /// <summary>
    /// 被暴击
    /// </summary>
    BeCritical = 3,

    /// <summary>
    /// 包围
    /// </summary>
    Surround = 4,

    /// <summary>
    /// 击破
    /// </summary>
    Rout = 5,

    /// <summary>
    /// 单挑主动胜利
    /// </summary>
    DualActiveWin = 6,

    /// <summary>
    /// 单挑被动胜利
    /// </summary>
    DualPassiveWin = 7,

    /// <summary>
    /// 论战主动胜利
    /// </summary>
    ControversyActiveWin = 8,

    /// <summary>
    /// 论战被动胜利
    /// </summary>
    ControversyPassiveWin = 9,

    /// <summary>
    /// 进入混乱
    /// </summary>
    Chaos = 10,

    /// <summary>
    /// 深度混乱
    /// </summary>
    DeepChaos = 11,

    /// <summary>
    /// 施展计略致乱
    /// </summary>
    CastDeepChaos = 12,

    /// <summary>
    /// 从混乱中恢复
    /// </summary>
    RecoverChaos = 13,

    /// <summary>
    /// 中计
    /// </summary>
    TrappedByStratagem = 14,

    /// <summary>
    /// 被计略帮助
    /// </summary>
    HelpedByStratagem = 15,

    /// <summary>
    /// 抵抗敌意计略
    /// </summary>
    ResistHarmfulStratagem = 16,

    /// <summary>
    /// 抵抗友好计略
    /// </summary>
    ResistHelpfulStratagem = 17,

    /// <summary>
    /// 抵抗攻击
    /// </summary>
    AntiAttack = 18,

    /// <summary>
    /// 攻破城墙
    /// </summary>
    BreakWall = 19,

    /// <summary>
    /// 愤怒
    /// </summary>
    Angry = 20,

    /// <summary>
    /// 沉静
    /// </summary>
    Calm = 21,

    /// <summary>
    /// 开始工作
    /// </summary>
    StartWork = 22,

    /// <summary>
    /// 修习技能成功
    /// </summary>
    StudySkillSuccess = 23,

    /// <summary>
    /// 修习技能失败
    /// </summary>
    StudySkillFailure = 24,

    /// <summary>
    /// 修习特技成功
    /// </summary>
    StudyStuntSuccess = 25,

    /// <summary>
    /// 修习特技失败
    /// </summary>
    StudyStuntFailure = 26,

    /// <summary>
    /// 修习称号成功
    /// </summary>
    StudyTitleSuccess = 27,

    /// <summary>
    /// 修习称号失败
    /// </summary>
    StudyTitleFailure = 28,

    /// <summary>
    /// 被录用
    /// </summary>
    HiredPerson = 29,

    /// <summary>
    /// 被褒奖
    /// </summary>
    Rewarded = 30,

    /// <summary>
    /// 被获得宝物
    /// </summary>
    BeAwardedTreasure = 31,

    /// <summary>
    /// 被没收宝物
    /// </summary>
    BeConfiscatedTreasure = 32,

    /// <summary>
    /// 发现宝物
    /// </summary>
    TreasureFound = 33,

    /// <summary>
    /// 情报成功
    /// </summary>
    InformationSuccess = 34,

    /// <summary>
    /// 情报失败
    /// </summary>
    InformationFailure = 35,

    /// <summary>
    /// 搜索资金
    /// </summary>
    SearchFundFound = 36,

    /// <summary>
    /// 搜索军粮
    /// </summary>
    SearchFoodFound = 37,

    /// <summary>
    /// 搜索技巧点
    /// </summary>
    SearchTechniqueFound = 38,
    
    /// <summary>
    /// 搜索间谍
    /// </summary>
    SearchSpyFound = 39,

    /// <summary>
    /// 搜索未发现武将
    /// </summary>
    SearchPersonFound = 40,

    /// <summary>
    /// 下野
    /// </summary>
    LeaveFaction = 41,

    /// <summary>
    /// 逃狱
    /// </summary>
    CaptiveEscape = 42,

    /// <summary>
    /// 出征
    /// </summary>
    StartCampaign = 43,

    /// <summary>
    /// 部队移动
    /// </summary>
    TroopMoveTo = 44,

    /// <summary>
    /// 运输队返回
    /// </summary>
    TransportReturn = 45,

    /// <summary>
    /// 被扩散火伤
    /// </summary>
    GetSpreadBurnt = 46,

    /// <summary>
    /// 使用战法
    /// </summary>
    UseCombatMethod = 47,

    /// <summary>
    /// 准备使用战法
    /// </summary>
    SetCombatMethod = 48,

    /// <summary>
    /// 使用特技
    /// </summary>
    UseStunt = 49,

    /// <summary>
    /// (无势力)使用友好计略
    /// </summary>
    NoFactionUseStratagemFriendly = 50,

    /// <summary>
    /// (无势力)使用敌对计略
    /// </summary>
    NoFactionUseStratagemHostile = 51,

    /// <summary>
    /// 攻心
    /// </summary>
    UseStratagem0 = 52,

    /// <summary>
    /// 扰乱
    /// </summary>
    UseStratagem1 = 53,

    /// <summary>
    /// 侦查
    /// </summary>
    UseStratagem2 = 54,

    /// <summary>
    /// 埋伏
    /// </summary>
    UseStratagem3 = 55,

    /// <summary>
    /// 火攻
    /// </summary>
    UseStratagem4 = 56,

    /// <summary>
    /// 镇静
    /// </summary>
    UseStratagem5 = 57,

    /// <summary>
    /// 灭火
    /// </summary>
    UseStratagem6 = 58,

    /// <summary>
    /// 鼓舞
    /// </summary>
    UseStratagem7 = 59,

    /// <summary>
    /// 点火
    /// </summary>
    UseStratagem8 = 60,

    /// <summary>
    /// 医治
    /// </summary>
    UseStratagem9 = 61,

    /// <summary>
    /// 伪报
    /// </summary>
    UseStratagem10 = 62,

    /// <summary>
    /// 挑衅
    /// </summary>
    UseStratagem11 = 63,

    /// <summary>
    /// 准备使用计略
    /// </summary>
    SetStratagem = 64,

    /// <summary>
    /// 开始埋伏
    /// </summary>
    StartAmbush = 65,

    /// <summary>
    /// 中止埋伏
    /// </summary>
    StopAmbush = 66,

    /// <summary>
    /// 发动埋伏
    /// </summary>
    Ambush = 67,

    /// <summary>
    /// 被发动埋伏
    /// </summary>
    BeAmbush = 68,

    /// <summary>
    /// 发现埋伏
    /// </summary>
    DiscoverAmbush = 69,

    /// <summary>
    /// 被发现埋伏
    /// </summary>
    BeDiscoverAmbush = 70,

    /// <summary>
    /// 部队捕获武将
    /// </summary>
    TroopNewCaptive = 71,

    /// <summary>
    /// 开始截断粮道
    /// </summary>
    StartCutRouteway = 72,

    /// <summary>
    /// 中止截断粮道
    /// </summary>
    StopCutRouteway = 73,

    /// <summary>
    /// 成功截断粮道
    /// </summary>
    CutRoutewaySuccess = 74,

    /// <summary>
    /// 失败截断粮道
    /// </summary>
    CutRoutewayFail = 75,

    /// <summary>
    /// 死亡
    /// </summary>
    Died = 76,

    /// <summary>
    /// 在单挑中死亡
    /// </summary>
    DiedInChallenge = 77,

    /// <summary>
    /// 因上任死亡成为君主
    /// </summary>
    DiedChangeFaction = 78,

    /// <summary>
    /// 建立义兄弟
    /// </summary>
    CreateBrother = 79,

    /// <summary>
    /// 建立义姊妹
    /// </summary>
    CreateSister = 80,

    /// <summary>
    /// 建立配偶
    /// </summary>
    CreateSpouse = 81,

    /// <summary>
    /// 被纳妃
    /// </summary>
    TakePrincess = 82,

    /// <summary>
    /// 被宠幸
    /// </summary>
    Hougong = 83,

    /// <summary>
    /// 君主自己发现怀孕
    /// </summary>
    SelfFoundPregnant = 84,

    /// <summary>
    /// 武将发现怀孕
    /// </summary>
    CoupleFoundPregnant = 87,

    /// <summary>
    /// 妃子发现怀孕
    /// </summary>
    FoundPregnant = 86,

    /// <summary>
    /// 父亲子女出生
    /// </summary>
    ChildrenBorn = 87,

    /// <summary>
    /// 子女出生
    /// </summary>
    BeChildrenBorn = 88,

    /// <summary>
    /// 被夺妻
    /// </summary>
    BeTakenSpouse = 89,

    /// <summary>
    /// 子女加入
    /// </summary>
    ChildJoin = 90,

    /// <summary>
    /// 子女自行加入
    /// </summary>
    ChildJoinSelfTalk = 91,

    /// <summary>
    /// 女性配偶加入
    /// </summary>
    FemaleSpouseJoin = 92,

    /// <summary>
    /// 男性配偶加入
    /// </summary>
    MaleSpouseJoin = 93,

    /// <summary>
    /// 亲善
    /// </summary>
    EnhanceDiplomaticRelation= 94,

    /// <summary>
    /// 外交征讨
    /// </summary>
    EncircleDiplomaticRelation = 95,

    /// <summary>
    /// 断交指令
    /// </summary>
    BreakDiplomaticRelation = 96,

    /// <summary>
    /// 断交
    /// </summary>
    ResetDiplomaticRelation = 97,

    /// <summary>
    /// 同盟
    /// </summary>
    CreateAlly = 98,

    /// <summary>
    /// 同盟失败
    /// </summary>
    CreateAllyFailed = 99,

    /// <summary>
    /// 停战
    /// </summary>
    Truce = 100,

    /// <summary>
    /// 停战失败
    /// </summary>
    TruceFailed = 101,

    /// <summary>
    /// 势力灭亡被俘
    /// </summary>
    AsLeaderCaught = 102,

    /// <summary>
    /// 释放俘虏
    /// </summary>
    ReleaseCaptive = 103,

    /// <summary>
    /// 处斩俘虏
    /// </summary>
    KillCaptive = 104,

    /// <summary>
    /// 流放武将
    /// </summary>
    ReleaseSelfPerson = 105,

    /// <summary>
    /// 得到控制权
    /// </summary>
    GetTurn = 106,

    /// <summary>
    /// 设施完成
    /// </summary>
    FacilityCompleted = 107,

    /// <summary>
    /// (君主)占领建筑
    /// </summary>
    LeaderOccupy = 108,

    /// <summary>
    /// 发生天灾
    /// </summary>
    DisasterHappened = 109,

    /// <summary>
    /// 势力技巧完成
    /// </summary>
    FactionTechniqueFinished = 110,

    /// <summary>
    /// 建筑遭受攻击
    /// </summary>
    ArchitectureUnderAttack = 111,

    /// <summary>
    /// 君主受封升官
    /// </summary>
    RiseEmperorClass = 112,

    /// <summary>
    /// 禅位
    /// </summary>
    BecomeEmperorLegally = 113,

    /// <summary>
    /// 称帝
    /// </summary>
    BecomeEmperorIllegally = 114,

    /// <summary>
    /// 称帝后果
    /// </summary>
    SelfBecomeInfluenceConsequence = 115,

    /// <summary>
    /// 自立
    /// </summary>
    CreateNewFaction = 116,

    /// <summary>
    /// 转君主保留名称
    /// </summary>
    ChangeLeaderKeepName = 117,

    /// <summary>
    /// 转君主更改名称
    /// </summary>
    ChangeLeaderChangeName = 118,

    /// <summary>
    /// 统一天下
    /// </summary>
    EndWithUnite = 119,

    /// <summary>
    /// 君主自立升官
    /// </summary>
    SelfRiseEmperorClass = 120,

    /// <summary>
    /// 被成功伪报
    /// </summary>
    Rumour = 121,

    /// <summary>
    /// 被成功挑衅
    /// </summary>
    Attract = 122,

    /// <summary>
    /// 武将加入
    /// </summary>
    PersonJoin  = 123,

    /// <summary>
    /// 取得基本兵种
    /// </summary>
    ObtainMilitaryKind = 124,

    /// <summary>
    /// 赐婚
    /// </summary>
    MakeMarriage = 125,

    /// <summary>
    /// 立储
    /// </summary>
    SelectPrince = 126,

    /// <summary>
    /// 招贤
    /// </summary>
    ZhaoXian = 127,

    /// <summary>
    /// 任命县令
    /// </summary>
    AppointMayor = 128,

    /// <summary>
    /// 劝降
    /// </summary>
    QuanXiang = 129,

    /// <summary>
    /// 劝降失败
    /// </summary>
    QuanXiangFailed = 130,

    /// <summary>
    /// 割地
    /// </summary>
    GeDi = 131,

    /// <summary>
    /// 合併對付玩家
    /// </summary>
    AIMergeAgainstPlayer = 132,

    /// <summary>
    /// 被宠幸(厭惡)
    /// </summary>
    HougongHate = 133,

    /// <summary>
    /// 找到宝物
    /// </summary>
    PersonTreasureFound = 134,
}
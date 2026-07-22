using GameObjects.Influences;
using GameObjects.Conditions;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using GameEnums;
using GameDatas;

namespace GameObjects.TroopDetail;

/// <summary>
/// 计略
/// </summary>
public class Stratagem : GameObject
{
    #region DataMember

    /// <summary>
    /// 消耗战意
    /// </summary>
    public int Combativity { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// 动画
    /// </summary>
    public TileAnimationKind AnimationKind { get; set; }

    /// <summary>
    /// 影响列表
    /// </summary>
    public string InfluencesString { get; set; }

    /// <summary>
    /// 使用条件列表
    /// </summary>
    public string CastConditionsString { get; set; }

    public string AIConditionWeightSelfString { get; set; }

    public string AIConditionWeightEnemyString { get; set; }

    public bool ArchitectureTarget { get; set; }

    public int CastDefaultString { get; set; }

    public int CastTargetString { get; set; }

    public int Chance { get; set; }

    public bool Friendly { get; set; }

    public bool Self { get; set; }

    public int TechniquePoint { get; set; }

    public bool RequireInfluenceToUse { get; set; }

    #endregion

    public Stratagem(StratagemConfig config)
    {
        ID = config.Id;
        Name = config.Name;
        Combativity = config.Combativity;
        Description = config.Description;
        AnimationKind = config.AnimationKind;
        InfluencesString = config.InfluencesString;
        CastConditionsString = config.CastConditionsString;
        AIConditionWeightSelfString = config.AIConditionWeightSelfString;
        AIConditionWeightEnemyString = config.AIConditionWeightEnemyString;
        ArchitectureTarget = config.ArchitectureTarget;
        CastDefaultString = config.CastDefaultString;
        CastTargetString = config.CastTargetString;
        Chance = config.Chance;
        Friendly = config.Friendly;
        Self = config.Self;
        TechniquePoint = config.TechniquePoint;
        RequireInfluenceToUse = config.RequireInfluenceToUse;
    }

    public List<Influence> Influences { get; set; } = new();

    public List<Condition> CastConditions { get; set; } = new();
    
    public Dictionary<Condition, float> AIConditionWeightSelf = new();

    public Dictionary<Condition, float> AIConditionWeightEnemy = new();

    public void Apply(Troop troop)
    {
        foreach (var influence in Influences)
        {
            influence.ApplyInfluence(troop, Applier.Stratagem, 0);
        }
    }

    public int GetCredit(Troop source, Troop destination)
    {
        if (!source.HasStratagem(ID)) return 0;

        int num = 0;
        foreach (var influence in Influences)
        {
            num += influence.GetCredit(source, destination);
        }
        return num;
    }

    public bool IsCastable(Troop troop)
    {
        return Condition.CheckConditionList(CastConditions, troop);
    }

    public int GetCreditWithPosition(Troop source, out Point? position)
    {
        position = new Point(0, 0);

        int num = 0;
        List<Point?> list = new List<Point?>();
        foreach (var influence in Influences)
        {
            Point? nullable = null;
            num += influence.GetCreditWithPosition(source, out nullable);
            list.Add(nullable);
        }
        if (list.Count > 0)
        {
            position = list[0];
        }
        return num;
    }

    public bool IsValid(Troop troop)
    {
        foreach (var influence in Influences)
        {
            if (!influence.IsVaild(troop)) return false;
        }

        return true;
    }
}
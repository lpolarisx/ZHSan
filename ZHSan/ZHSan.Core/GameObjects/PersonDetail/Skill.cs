using GameDatas;
using GameEnums;
using GameGlobal;
using GameObjects.Conditions;
using GameObjects.Influences;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GameObjects.PersonDetail;

/// <summary>
/// 技能
/// </summary>
public class Skill : GameObject
{
    #region DataMember

    /// <summary>
    /// 显示行
    /// </summary>
    public int DisplayRow { get; set; }

    /// <summary>
    /// 显示列
    /// </summary>
    public int DisplayCol { get; set; }

    /// <summary>
    /// 类别
    /// </summary>
    public int Kind { get; set; }

    /// <summary>
    /// 等级
    /// </summary>
    public int Level { get; set; }

    /// <summary>
    /// 战斗
    /// </summary>
    public bool Combat { get; set; }

    /// <summary>
    /// 影响列表
    /// </summary>
    public string InfluencesString { get; set; }

    /// <summary>
    /// 条件列表
    /// </summary>
    public string ConditionTableString { get; set; }

    /// <summary>
    /// 不同生成武将类型获得机率
    /// </summary>
    public int[] GenerationChance { get; set; } = new int[10];

    /// <summary>
    /// 此技能的相关能力、0-4为武统智政魅
    /// </summary>
    public int RelatedAbility { get; set; }

    #endregion

    public Skill(SkillConfig config)
    {
        ID = config.Id;
        Name = config.Name;
        DisplayRow = config.DisplayRow;
        DisplayCol = config.DisplayCol;
        Kind = config.Kind;
        Level = config.Level;
        Combat = config.Combat;
        InfluencesString = config.InfluencesString;
        ConditionTableString = config.ConditionTableString;
        GenerationChance = config.GenerationChance;
        RelatedAbility = config.RelatedAbility;
    }

    public Dictionary<int, Influence> Influences { get; set; } = new();

    public List<Condition> Conditions { get; set; } = new();

    public List<Condition> GenerateConditions = new();

    public int GetRelatedAbility(Person person)
    {
        switch (RelatedAbility)
        {
            case 0: return person.Strength;
            case 1: return person.Command;
            case 2: return person.Intelligence;
            case 3: return person.Politics;
            case 4: return person.Glamour;
        }
        return 0;
    }

    public MilitaryType MilitaryTypeOnly
    {
        get
        {
            foreach (var influence in Influences.Values)
            {
                if (influence.KindId == 290)
                {
                    return (MilitaryType)Enum.Parse(typeof(MilitaryType), influence.Parameter);
                }
            }
            return MilitaryType.Other;
        }
    }

    public virtual bool CanLearn(Person person)
    {
        return Condition.CheckConditionList(Conditions, person);
    }

    public int ConditionCount => Conditions.Count;

    public string Description => string.Join("•", Influences.Values.Select(x => x.Description));

    public int InfluenceCount => Influences.Count;

    public int Merit => Level * 5;

    public string Prerequisite => StaticMethods.SaveNameToString(Conditions);

    private int? subOfficerMerit = null;
    public int SubOfficerMerit
    {
        get
        {
            if (subOfficerMerit == null)
            {
                int subofficerInfluences = 0;
                foreach (var influence in Influences.Values)
                {
                    if (influence.KindId == 281) break;

                    if (influence.Kind.Combat)
                    {
                        subofficerInfluences++;
                    }
                }

                subOfficerMerit = (int)(Merit * (double)subofficerInfluences / Influences.Count);
            }
            
            return subOfficerMerit.Value;
        }
    }

    public bool CanBeChosenForGenerated(Person person)
    {
        if (Conditions.Any(x => x.Kind.ID == 902)) return false;

        return Condition.CheckConditionList(GenerateConditions, person);
    }

    public bool CanBeBorn(Person person)
    {
        if (Conditions.Any(x => x.Kind.ID == 901)) return false;

        return Condition.CheckConditionList(GenerateConditions, person);
    }
}
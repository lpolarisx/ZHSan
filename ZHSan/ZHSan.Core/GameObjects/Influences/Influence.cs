using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using GameManager;
using GameEnums;
using GameDatas;

namespace GameObjects.Influences;

public class Influence : GameObject
{
    #region DataMember

    /// <summary>
    /// 影响类型
    /// </summary>
    public InfluenceKind Kind { get; set; }

    public int KindId { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    public string Description { get; set; }

    private string parameter;
    private int? intParameter;
    private float? floatParameter;

    /// <summary>
    /// 参数1
    /// </summary>
    public string Parameter
    {
        get => parameter;
        set
        {
            parameter = value;
            intParameter = null;
            floatParameter = null;
        }
    }

    private string parameter2;
    private int? intParameter2;
    private float? floatParameter2;

    /// <summary>
    /// 参数2
    /// </summary>
    public string Parameter2
    {
        get => parameter2;
        set
        {
            parameter2 = value;
            intParameter2 = null;
            floatParameter2 = null;
        }
    }

    #endregion

    public Influence(InfluenceConfig config)
    {
        ID = config.Id;
        Name = config.Name;
        KindId = config.KindId;
        Description = config.Description;
        Parameter = config.Parameter;
        Parameter2 = config.Parameter2;
    }

    /// <summary>
    /// 获取参数1解析的int值
    /// </summary>
    /// <returns></returns>
    public int GetIntParam()
    {
        if (!intParameter.HasValue)
        {
            intParameter = int.TryParse(parameter, out int v) ? v : 0;
        }
        return intParameter.Value;
    }

    /// <summary>
    /// 获取参数1解析的float值
    /// </summary>
    /// <returns></returns>
    public float GetFloatParam()
    {
        if (!floatParameter.HasValue)
        {
            floatParameter = float.TryParse(parameter, out float v) ? v : 0;
        }
        return floatParameter.Value;
    }

    /// <summary>
    /// 获取参数2解析的int值
    /// </summary>
    /// <returns></returns>
    public int GetIntParam2()
    {
        if (!intParameter2.HasValue)
        {
            intParameter2 = int.TryParse(parameter2, out int v) ? v : 0;
        }
        return intParameter2.Value;
    }

    /// <summary>
    /// 获取参数2解析的float值
    /// </summary>
    /// <returns></returns>
    public float GetFloatParam2()
    {
        if (!floatParameter2.HasValue)
        {
            floatParameter2 = float.TryParse(parameter2, out float v) ? v : 0;
        }
        return floatParameter2.Value;
    }

    public HashSet<ApplyArchitecture> ApplyArchitectures { get; set; } = new();

    public HashSet<ApplyPerson> ApplyPersons { get; set; } = new();

    public HashSet<ApplyFaction> ApplyFactions { get; set; } = new();

    public HashSet<ApplyTroop> ApplyTroops { get; set; } = new();

    public void Init()
    {
        ApplyArchitectures = new();
        ApplyPersons = new();
        ApplyFactions = new();
        ApplyTroops = new();
    }

    public static void ApplyInfluenceListToPerson(IEnumerable<Influence> influences, Person person, Applier applier, int id)
    {
        bool flag = false;
        bool flag2 = false;
        foreach (var influence in influences)
        {
            if ((influence.Type != InfluenceType.Prerequisite) && (influence.Type != InfluenceType.Exclusive))
            {
                if (!flag || flag2)
                {
                    influence.ApplyInfluence(person, applier, id);
                }
                continue;
            }
            if (!(flag || (influence.Type != InfluenceType.Exclusive)))
            {
                flag = true;
            }
            if (influence.IsVaild(person))
            {
                if (influence.Type == InfluenceType.Exclusive)
                {
                    flag2 = true;
                    continue;
                }
            }
            else if (influence.Type == InfluenceType.Prerequisite)
            {
                break;
            }
        }
    }

    public static void ApplyInfluenceList<T>(IEnumerable<Influence> influences, T target, Applier applier, int id) where T : GameObject
    {
        foreach (var influence in influences)
        {
            influence.ApplyInfluence(target, applier, id);
        }
    }

    public void ApplyInfluence(GameObject target, Applier applier, int id)
    {
        switch (target)
        {
            case Faction faction:
                Kind.ApplyFromEntry(faction, this, applier, id);
                break;
            case Architecture arch:
                Kind.ApplyFromEntry(arch, this, applier, id);
                break;
            case Person person:
                Kind.ApplyFromEntry(person, this, applier, id);
                break;
            case Troop troop:
                Kind.ApplyFromEntry(troop, this, applier, id);
                break;
            default:
                throw new NotSupportedException($"不支持的影响对象: {target.GetType().Name}");
        }
    }


    public void ApplyInfluence(Architecture arch, Applier applier, int id)
    {
        Kind.ApplyFromEntry(arch, this, applier, id);
    }

    public void ApplyInfluence(Faction faction, Applier applier, int applierID)
    {
        Kind.ApplyFromEntry(faction, this, applier, applierID);
    }

    public void ApplyInfluence(Person person, Applier applier, int applierID)
    {
        Kind.ApplyFromEntry(person, this, applier, applierID);
    }

    public void ApplyInfluence(Troop troop, Applier applier, int applierID)
    {
        Kind.ApplyFromEntry(troop, this, applier, applierID);
    }

    public void DoWork(Architecture architecture)
    {
        Kind.DoWork(this, architecture);
    }

    public int GetCredit(Troop source, Troop destination)
    {
        return Kind.GetCredit(this, source, destination);
    }

    public int GetCreditWithPosition(Troop source, out Point? position)
    {
        position = new Point(0, 0);
        return Kind.GetCreditWithPosition(source, out position);
    }

    public bool IsVaild(Person person)
    {
        return Kind.IsVaild(this, person);
    }


    public bool IsVaild(Troop troop)
    {
        return Kind.IsVaild(this, troop);
    }

    public static void PurifyInfluenceList<T>(IEnumerable<Influence> influences, T target, Applier applier, int id) where T : GameObject
    {
        foreach (var influence in influences)
        {
            influence.PurifyInfluence(target, applier, id);
        }
    }

    public void PurifyInfluence(GameObject target, Applier applier, int id)
    {
        switch (target)
        {
            case Faction faction:
                Kind.PurifyFromEntry(faction, this, applier, id);
                break;
            case Architecture arch:
                Kind.PurifyFromEntry(arch, this, applier, id);
                break;
            case Person person:
                Kind.PurifyFromEntry(person, this, applier, id);
                break;
            case Troop troop:
                Kind.PurifyFromEntry(troop, this, applier, id);
                break;
            default:
                throw new NotSupportedException($"不支持的影响对象: {target.GetType().Name}");
        }
    }

    public void PurifyInfluence(Architecture architecture, Applier applier, int applierID)
    {
        Kind.PurifyFromEntry(architecture, this, applier, applierID);
    }

    public void PurifyInfluence(Faction faction, Applier applier, int applierID)
    {
        Kind.PurifyFromEntry(faction, this, applier, applierID);
    }

    public void PurifyInfluence(Person person, Applier applier, int applierID)
    {
        Kind.PurifyFromEntry(person, this, applier, applierID);
    }

    public void TroopDestroyed(Troop troop)
    {
        ApplyTroops.RemoveWhere((x) => { return x.troop == troop; });
    }

    public void PurifyInfluence(Troop troop, Applier applier, int applierID)
    {
        Kind.PurifyFromEntry(troop, this, applier, applierID);
    }

    public double AIFacilityValue(Architecture arch)
    {
        return Kind.AIFacilityValue(this, arch);
    }

    public override string ToString() => Description;

    public bool TroopLeaderValid => Kind.TroopLeaderValid;

    public InfluenceType Type => Kind.Type;

    public double AIPersonValue
    {
        get
        {
            var commonData = Session.Current.Scenario.GameCommonData;

            var i1 = GetIntParam();
            var f1 = GetFloatParam();
            var i2 = GetIntParam2();
            var f2 = GetFloatParam2();

            var value = Kind.AIPersonValue;
            var pow = Kind.AIPersonValuePow;

            double v;
            switch (Kind.ID)
            {
                case 320:
                    {
                        if (commonData.AllCombatMethods.TryGetValue(i1, out var combatMethod))
                        {
                            return value * combatMethod.Combativity * pow;
                        }
                        else
                        {
                            return 0;
                        }
                    }
                case 330:
                    {
                        if (commonData.AllStunts.TryGetValue(i1, out var stunt))
                        {
                            return value * stunt.Combativity * pow;
                        }
                        else
                        {
                            return 0;
                        }
                    }
                case 860:
                    {
                        if (commonData.AllStratagems.TryGetValue(i1, out var stratagem))
                        {
                            return value * stratagem.Combativity * pow;
                        }
                        else
                        {
                            return 0;
                        }
                    }
                case 800:
                case 802:
                case 804:
                case 824:
                case 832:
                    return value * (f1 - 1);
                case 801:
                case 803:
                case 805:
                case 825:
                case 833:
                    return value * (1 - f1);
                case 200:
                case 220:
                    if (i2 == 0) return value * Math.Pow(f1, pow);

                    v = value * Math.Pow(i2, pow);
                    switch (i1)
                    {
                        case 1:
                        case 2:
                            return v * 2;
                        case 3:
                        case 5:
                            return v;
                        case 6:
                            return v * 1.5;
                        case 8:
                        case 9:
                        case 10:
                            return v * 0.5;
                        default:
                            return 0;
                    }
                case 352:
                    return value * Math.Min(f1, f2 - 0.5) * Math.Pow(f1, pow);
                case 6140:
                    v = value * Math.Pow(f1, pow);
                    if (f1 >= 100)
                    {
                        v *= 1.2;
                    }

                    if (f1 > 110)
                    {
                        v *= 1.5;
                    }
                    return v * i2;
                case 6350:
                    return value * Math.Pow(i2 - 1, f1 / 100.0) * Math.Pow(f1, pow);
                case 6360:
                    return value * (Math.Max(i2, 100) / 100.0) * Math.Pow(f1, pow);
                case 6420:
                case 6430:
                case 6450:
                    return value * Math.Pow(i2, pow);
                case 6700:
                case 6705:
                case 6710:
                case 6715:
                case 6720:
                case 6725:
                case 6730:
                case 6735:
                case 6740:
                case 6745:
                case 6760:
                    v = value * Math.Pow(f1, pow);

                    if (i2 == 0)
                    {
                        return v;
                    }
                    else
                    {
                        return v * Math.Pow(i2, 1.2);
                    }
                case 6750:
                case 6755:
                    v = value * Math.Pow(f1, pow);

                    if (i2 == 0)
                    {
                        return v;
                    }
                    else
                    {
                        return v * Math.Pow(i2 / 1000, 1.2);
                    }
                default:
                    if (f1 == 0 && pow <= 0)
                    {
                        return pow == 0 ? value : value * 10;
                    }
                    else
                    {
                        return value * Math.Pow(f1, pow);
                    }
            }
        }
    }
}
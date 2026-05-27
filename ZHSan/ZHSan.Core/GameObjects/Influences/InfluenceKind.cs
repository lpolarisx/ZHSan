using Microsoft.Xna.Framework;
using System.Runtime.Serialization;

namespace GameObjects.Influences;

[DataContract]
public class InfluenceKind : GameObject
{
    /// <summary>
    /// 种类
    /// </summary>
    [DataMember]
    public InfluenceType Type { get; set; }

    /// <summary>
    /// 战斗
    /// </summary>
    [DataMember]
    public bool Combat { get; set; }

    /// <summary>
    /// 武将AI值
    /// </summary>
    [DataMember]
    public float AIPersonValue { get; set; }

    /// <summary>
    /// 武将AI值乘幂
    /// </summary>
    [DataMember]
    public float AIPersonValuePow { get; set; }

    /// <summary>
    /// 主将有效
    /// </summary>
    [DataMember]
    public bool TroopLeaderValid { get; set; }

    public virtual void DoWork(Architecture architecture)
    {
    }

    public virtual int GetCredit(Influence influence, Troop source, Troop destination)
    {
        return 0;
    }

    public virtual int GetCreditWithPosition(Troop source, out Point? position)
    {
        position = new Point(0, 0);
        return 0;
    }

    public virtual void InitializeParameter(string parameter)
    {
    }

    public virtual void InitializeParameter2(string parameter)
    {
    }

    public virtual bool IsVaild(Influence influence, Person person)
    {
        return true;
    }

    public virtual bool IsVaild(Influence influence, Troop troop)
    {
        return true;
    }

    public virtual double AIFacilityValue(Architecture a)
    {
        return 0;
    }

    #region 应用影响

    public virtual void ApplyInfluenceKind(Influence influence, Architecture architecture)
    {
    }

    public void ApplyInfluenceKind(Architecture architecture, Influence influence, Applier applier, int applierID)
    {
        if (Type == InfluenceType.建筑 || Type == InfluenceType.建筑战斗)
        {
            if (influence.appliedArch.Add(new ApplyingArchitecture(architecture, applier, applierID)))
            {
                ApplyInfluenceKind(influence, architecture);
            }
        }
        else if (Type == InfluenceType.个人)
        {
            foreach (Person person in architecture.Persons)
            {
                ApplyInfluenceKind(person, influence, applier, applierID, false);
            }
        }
    }

    public virtual void ApplyInfluenceKind(Faction faction)
    {
    }

    public void ApplyInfluenceKind(Faction faction, Influence influence, Applier applier, int applierID)
    {
        if (Type == InfluenceType.势力)
        {
            if (influence.appliedFaction.Add(new ApplyingFaction(faction, applier, applierID)))
            {
                ApplyInfluenceKind(faction);
            }
        }
        else if (Type == InfluenceType.建筑 || Type == InfluenceType.建筑战斗)
        {
            foreach (Architecture arch in faction.Architectures)
            {
                ApplyInfluenceKind(arch, influence, applier, applierID);
            }
        }
        else if (Type == InfluenceType.战斗 || Type == InfluenceType.建筑战斗)
        {
            foreach (Troop troop in faction.Troops)
            {
                ApplyInfluenceKind(troop, influence, applier, applierID);
            }
        }
        else if (Type == InfluenceType.个人)
        {
            foreach (Person person in faction.Persons)
            {
                ApplyInfluenceKind(person, influence, applier, applierID, false);
            }
        }
    }

    public virtual void ApplyInfluenceKind(Influence influence, Person person)
    {
    }

    public void ApplyInfluenceKind(Person person, Influence influence, Applier applier, int applierID, bool excludePersonal)
    {
        if (Type == InfluenceType.个人)
        {
            if (influence.appliedPerson.Add(new ApplyingPerson(person, applier, applierID)))
            {
                ApplyInfluenceKind(influence, person);
            }
        }
        else if (Type == InfluenceType.战斗 || Type == InfluenceType.建筑战斗)
        {
            if (person.LocationTroop != null)
            {
                ApplyInfluenceKind(person.LocationTroop, influence, applier, applierID);
            }
        }
        if (Type == InfluenceType.建筑 || Type == InfluenceType.建筑战斗)
        {
            if (person.LocationArchitecture != null)
            {
                ApplyInfluenceKind(person.LocationArchitecture, influence, applier, applierID);
            }
        }
    }

    public virtual void ApplyInfluenceKind(Influence influence, Troop troop)
    {
    }

    public void ApplyInfluenceKind(Troop troop, Influence influence, Applier applier, int applierID)
    {
        if (Type == InfluenceType.战斗 || Type == InfluenceType.建筑战斗)
        {
            if (influence.appliedTroop.Add(new ApplyingTroop(troop, applier, applierID)) || (ID >= 390 && ID <= 399) || ID == 720 || ID == 721)
            {
                troop.InfluencesApplying.Add(influence);
                ApplyInfluenceKind(influence, troop);
            }
        }
    }

    #endregion

    #region 移除影响

    public virtual void PurifyInfluenceKind(Influence influence, Architecture architecture)
    {
    }

    public void PurifyInfluenceKind(Architecture architecture, Influence influence, Applier applier, int applierID)
    {
        if (Type == InfluenceType.建筑 || Type == InfluenceType.建筑战斗)
        {
            if (influence.appliedArch.Remove(new ApplyingArchitecture(architecture, applier, applierID)))
            {
                PurifyInfluenceKind(influence, architecture);
            }
        }
        else if (Type == InfluenceType.个人)
        {
            foreach (Person person in architecture.Persons)
            {
                PurifyInfluenceKind(person, influence, applier, applierID, false);
            }
        }
    }

    public virtual void PurifyInfluenceKind(Faction faction)
    {
    }

    public void PurifyInfluenceKind(Faction faction, Influence influence, Applier applier, int applierID)
    {
        if (Type == InfluenceType.势力)
        {
            if (influence.appliedFaction.Remove(new ApplyingFaction(faction, applier, applierID)))
            {
                PurifyInfluenceKind(faction);
            }

            foreach (Architecture arch in faction.Architectures)
            {
                PurifyInfluenceKind(arch, influence, applier, applierID);
            }

            foreach (Troop troop in faction.Troops)
            {
                PurifyInfluenceKind(troop, influence, applier, applierID);
            }
        }
    }

    public virtual void PurifyInfluenceKind(Influence influence, Person person)
    {
    }

    public void PurifyInfluenceKind(Person person, Influence influence, Applier applier, int applierID, bool excludePersonal)
    {
        if (Type == InfluenceType.个人)
        {
            if (influence.appliedPerson.Remove(new ApplyingPerson(person, applier, applierID)))
            {
                PurifyInfluenceKind(influence, person);
            }
        }
        else if (Type == InfluenceType.战斗 || Type == InfluenceType.建筑战斗)
        {
            if (person.LocationTroop != null)
            {
                PurifyInfluenceKind(person.LocationTroop, influence, applier, applierID);
            }
        }
        else if (Type == InfluenceType.建筑 || Type == InfluenceType.建筑战斗)
        {
            if (person.LocationArchitecture != null)
            {
                PurifyInfluenceKind(person.LocationArchitecture, influence, applier, applierID);
            }
        }
    }

    public virtual void PurifyInfluenceKind(Influence influence, Troop troop)
    {
    }

    public void PurifyInfluenceKind(Troop troop, Influence influence, Applier applier, int applierID)
    {
        if (Type == InfluenceType.战斗 || Type == InfluenceType.建筑战斗)
        {
            if (influence.appliedTroop.Remove(new ApplyingTroop(troop, applier, applierID)))
            {
                PurifyInfluenceKind(influence, troop);
            }
        }
    }

    #endregion
}
using System;
using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind6300 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Person person)
    {
        person.pregnantChance += influence.GetIntParam();
    }

    public override void PurifyInfluenceKind(Influence influence, Person person)
    {
        person.pregnantChance -= influence.GetIntParam();
    }

    public override double AIFacilityValue(Influence influence, Architecture arch)
    {
        return Math.Max(arch.Meinvkongjian, arch.GetConcubines().Count) * (influence.GetIntParam() / 100.0) * 10;
    }
}
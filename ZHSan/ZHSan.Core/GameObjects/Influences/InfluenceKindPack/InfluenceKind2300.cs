using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind2300 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Faction faction)
    {
        faction.IncrementOfCombativityCeiling += influence.GetIntParam();
    }

    public override void PurifyInfluenceKind(Influence influence, Faction faction)
    {
        faction.IncrementOfCombativityCeiling -= influence.GetIntParam();
    }
}
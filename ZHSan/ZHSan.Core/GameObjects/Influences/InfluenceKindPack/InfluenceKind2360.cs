using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind2360 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Faction faction)
    {
        faction.IncrementOfResistStratagemChance += influence.GetIntParam();
    }

    public override void PurifyInfluenceKind(Influence influence, Faction faction)
    {
        faction.IncrementOfResistStratagemChance -= influence.GetIntParam();
    }
}
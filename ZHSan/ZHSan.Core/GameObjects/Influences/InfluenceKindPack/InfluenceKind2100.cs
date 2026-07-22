using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind2100 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Faction faction)
    {
        faction.AddTechniqueMilitaryKind(influence.GetIntParam());
    }

    public override void PurifyInfluenceKind(Influence influence, Faction faction)
    {
        faction.RemoveTechniuqeMilitaryKind(influence.GetIntParam());
    }
}
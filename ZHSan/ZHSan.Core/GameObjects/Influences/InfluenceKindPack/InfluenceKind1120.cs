using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind1120 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Architecture architecture)
    {
        architecture.AddMilitaryKind(influence.GetIntParam());
    }

    public override void PurifyInfluenceKind(Influence influence, Architecture architecture)
    {
        architecture.RemoveMilitaryKind(influence.GetIntParam());
    }
}
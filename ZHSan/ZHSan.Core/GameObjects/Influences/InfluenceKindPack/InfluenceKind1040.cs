using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind1040 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Architecture architecture)
    {
        architecture.TroopershipAvailable = true;
    }

    public override void PurifyInfluenceKind(Influence influence, Architecture architecture)
    {
        architecture.TroopershipAvailable = false;
    }
}
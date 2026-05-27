using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind251 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Troop troop)
    {
        troop.IncrementOfMovability += influence.GetIntParam();
    }

    public override void PurifyInfluenceKind(Influence influence, Troop troop)
    {
        troop.IncrementOfMovability -= influence.GetIntParam();
    }
}
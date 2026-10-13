using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind6555 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Troop troop)
    {
        troop.TroopLostInViewArea += influence.GetIntParam();
    }

    public override void PurifyInfluenceKind(Influence influence, Troop troop)
    {
        troop.TroopLostInViewArea -= influence.GetIntParam();
    }
}
using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind6540 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Troop troop)
    {
        troop.InjuryRecoverInViewArea += influence.GetIntParam();
    }

    public override void PurifyInfluenceKind(Influence influence, Troop troop)
    {
        troop.InjuryRecoverInViewArea -= influence.GetIntParam();
    }
}
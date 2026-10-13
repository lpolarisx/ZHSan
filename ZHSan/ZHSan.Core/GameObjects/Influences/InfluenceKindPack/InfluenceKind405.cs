using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind405 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Troop troop)
    {
        troop.ChanceDecrementOfCriticalStrike += influence.GetIntParam();
    }

    public override void ApplyInfluenceKind(Influence influence, Architecture arch)
    {
        arch.ChanceDecrementOfCriticalStrike += influence.GetIntParam();
    }

    public override void PurifyInfluenceKind(Influence influence, Troop troop)
    {
        troop.ChanceDecrementOfCriticalStrike -= influence.GetIntParam();
    }

    public override void PurifyInfluenceKind(Influence influence, Architecture arch)
    {
        arch.ChanceDecrementOfCriticalStrike -= influence.GetIntParam();
    }
}
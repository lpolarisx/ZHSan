using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind6110 : InfluenceKind
{
    private int increment;

    public override void ApplyInfluenceKind(Influence influence, Architecture arch)
    {
        arch.captureChance += influence.GetIntParam();
    }

    public override void PurifyInfluenceKind(Influence influence, Architecture arch)
    {
        arch.captureChance -= influence.GetIntParam();
    }

    public override double AIFacilityValue(Influence influence, Architecture arch)
    {
        var frontLine = arch.FrontLine ? 1 : 0.2;
        
        return influence.GetIntParam() * 10 / (double)arch.BelongedFaction.PersonCount * frontLine;
    }
}
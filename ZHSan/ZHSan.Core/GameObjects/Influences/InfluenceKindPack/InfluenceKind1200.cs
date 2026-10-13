using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind1200 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Architecture architecture)
    {
        var type = influence.GetIntParam();
        var rate = influence.GetFloatParam2();
        
        switch (type)
        {
            case 0:
                architecture.RateIncrementOfNewBubingTroopOffence += rate;
                break;
            case 1:
                architecture.RateIncrementOfNewNubingTroopOffence += rate;
                break;
            case 2:
                architecture.RateIncrementOfNewQibingTroopOffence += rate;
                break;
            case 3:
                architecture.RateIncrementOfNewShuijunTroopOffence += rate;
                break;
            case 4:
                architecture.RateIncrementOfNewQixieTroopOffence += rate;
                break;
        }
    }

    public override void PurifyInfluenceKind(Influence influence, Architecture architecture)
    {
        var type = influence.GetIntParam();
        var rate = influence.GetFloatParam2();

        switch (type)
        {
            case 0:
                architecture.RateIncrementOfNewBubingTroopOffence -= rate;
                break;
            case 1:
                architecture.RateIncrementOfNewNubingTroopOffence -= rate;
                break;
            case 2:
                architecture.RateIncrementOfNewQibingTroopOffence -= rate;
                break;
            case 3:
                architecture.RateIncrementOfNewShuijunTroopOffence -= rate;
                break;
            case 4:
                architecture.RateIncrementOfNewQixieTroopOffence -= rate;
                break;
        }
    }

    public override double AIFacilityValue(Influence influence, Architecture arch)
    {
        var frontLineFirst = arch.FrontLine ? 1 : 0.001;
        var frontLineSecond = arch.FrontLine ? 2 : 1;
        var hostileLine = arch.HostileLine ? 2 : 1;
        var criticalHostile = arch.CriticalHostile ? 2 : 1;

        return influence.GetFloatParam2() * 4 * frontLineFirst * frontLineSecond * hostileLine * criticalHostile;
    }
}
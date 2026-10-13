using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind1100 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Architecture architecture)
    {
        var type = influence.GetIntParam();
        var rate = 1 - influence.GetFloatParam2();

        switch (type)
        {
            case 0:
                architecture.RateOfNewBubingMilitaryFundCost -= rate;
                break;
            case 1:
                architecture.RateOfNewNubingMilitaryFundCost -= rate;
                break;
            case 2:
                architecture.RateOfNewQibingMilitaryFundCost -= rate;
                break;
            case 3:
                architecture.RateOfNewShuijunMilitaryFundCost -= rate;
                break;
            case 4:
                architecture.RateOfNewQixieMilitaryFundCost -= rate;
                break;
        }
    }

    public override void PurifyInfluenceKind(Influence influence, Architecture architecture)
    {
        var type = influence.GetIntParam();
        var rate = 1 - influence.GetFloatParam2();

        switch (type)
        {
            case 0:
                architecture.RateOfNewBubingMilitaryFundCost += rate;
                break;
            case 1:
                architecture.RateOfNewNubingMilitaryFundCost += rate;
                break;
            case 2:
                architecture.RateOfNewQibingMilitaryFundCost += rate;
                break;
            case 3:
                architecture.RateOfNewShuijunMilitaryFundCost += rate;
                break;
            case 4:
                architecture.RateOfNewQixieMilitaryFundCost += rate;
                break;
        }
    }

    public override double AIFacilityValue(Influence influence, Architecture arch)
    {
        var frontLine = arch.FrontLine ? 2 : 1;
        var hostileLine = arch.HostileLine ? 2 : 1;
        var criticalHostile = arch.CriticalHostile ? 2 : 1;

        return influence.GetFloatParam2() * frontLine * hostileLine * criticalHostile;
    }
}
using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind1300 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Architecture architecture)
    {
        var type = influence.GetIntParam();
        var rate = influence.GetFloatParam2();

        switch (type)
        {
            case 0:
                architecture.RateIncrementOfNewBubingTroopDefence += rate;
                break;
            case 1:
                architecture.RateIncrementOfNewNubingTroopDefence += rate;
                break;
            case 2:
                architecture.RateIncrementOfNewQibingTroopDefence += rate;
                break;
            case 3:
                architecture.RateIncrementOfNewShuijunTroopDefence += rate;
                break;
            case 4:
                architecture.RateIncrementOfNewQixieTroopDefence += rate;
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
                architecture.RateIncrementOfNewBubingTroopDefence -= rate;
                break;
            case 1:
                architecture.RateIncrementOfNewNubingTroopDefence -= rate;
                break;
            case 2:
                architecture.RateIncrementOfNewQibingTroopDefence -= rate;
                break;
            case 3:
                architecture.RateIncrementOfNewShuijunTroopDefence -= rate;
                break;
            case 4:
                architecture.RateIncrementOfNewQixieTroopDefence -= rate;
                break;
        }
    }
}
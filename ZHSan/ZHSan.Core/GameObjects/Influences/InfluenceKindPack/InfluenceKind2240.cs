using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind2240 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Faction faction)
    {
        var type = influence.GetIntParam();
        var rate = influence.GetFloatParam2();

        switch (type)
        {
            case 0:
                faction.OffenceRateOfBubing += rate;
                break;
            case 1:
                faction.OffenceRateOfNubing += rate;
                break;
            case 2:
                faction.OffenceRateOfQibing += rate;
                break;
            case 3:
                faction.OffenceRateOfShuijun += rate;
                break;
            case 4:
                faction.OffenceRateOfQixie += rate;
                break;
        }
    }

    public override void PurifyInfluenceKind(Influence influence, Faction faction)
    {
        var type = influence.GetIntParam();
        var rate = influence.GetFloatParam2();

        switch (type)
        {
            case 0:
                faction.OffenceRateOfBubing -= rate;
                break;
            case 1:
                faction.OffenceRateOfNubing -= rate;
                break;
            case 2:
                faction.OffenceRateOfQibing -= rate;
                break;
            case 3:
                faction.OffenceRateOfShuijun -= rate;
                break;
            case 4:
                faction.OffenceRateOfQixie -= rate;
                break;
        }
    }
}
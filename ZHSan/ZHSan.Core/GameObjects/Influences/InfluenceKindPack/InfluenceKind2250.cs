using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind2250 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Faction faction)
    {
        var type = influence.GetIntParam();
        var rate = influence.GetFloatParam2();

        switch (type)
        {
            case 0:
                faction.DefenceRateOfBubing += rate;
                break;
            case 1:
                faction.DefenceRateOfNubing += rate;
                break;
            case 2:
                faction.DefenceRateOfQibing += rate;
                break;
            case 3:
                faction.DefenceRateOfShuijun += rate;
                break;
            case 4:
                faction.DefenceRateOfQixie += rate;
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
                faction.DefenceRateOfBubing -= rate;
                break;
            case 1:
                faction.DefenceRateOfNubing -= rate;
                break;
            case 2:
                faction.DefenceRateOfQibing -= rate;
                break;
            case 3:
                faction.DefenceRateOfShuijun -= rate;
                break;
            case 4:
                faction.DefenceRateOfQixie -= rate;
                break;
        }
    }
}
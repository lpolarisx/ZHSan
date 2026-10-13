using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind2010 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Faction faction)
    {
        var type = influence.GetIntParam();
        var rate = influence.GetFloatParam2();

        switch (type)
        {
            case 0:
                faction.OffenceRateWhileCombatMethodOfBubing += rate;
                break;
            case 1:
                faction.OffenceRateWhileCombatMethodOfNubing += rate;
                break;
            case 2:
                faction.OffenceRateWhileCombatMethodOfQibing += rate;
                break;
            case 3:
                faction.OffenceRateWhileCombatMethodOfShuijun += rate;
                break;
            case 4:
                faction.OffenceRateWhileCombatMethodOfQixie += rate;
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
                faction.OffenceRateWhileCombatMethodOfBubing -= rate;
                break;
            case 1:
                faction.OffenceRateWhileCombatMethodOfNubing -= rate;
                break;
            case 2:
                faction.OffenceRateWhileCombatMethodOfQibing -= rate;
                break;
            case 3:
                faction.OffenceRateWhileCombatMethodOfShuijun -= rate;
                break;
            case 4:
                faction.OffenceRateWhileCombatMethodOfQixie -= rate;
                break;
        }
    }
}
using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind2000 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Faction faction)
    {
        var type = influence.GetIntParam();
        var rate = influence.GetFloatParam2();

        switch (type)
        {
            case 0:
                faction.DefenceRateWhileCombatMethodOfBubing += rate;
                break;
            case 1:
                faction.DefenceRateWhileCombatMethodOfNubing += rate;
                break;
            case 2:
                faction.DefenceRateWhileCombatMethodOfQibing += rate;
                break;
            case 3:
                faction.DefenceRateWhileCombatMethodOfShuijun += rate;
                break;
            case 4:
                faction.DefenceRateWhileCombatMethodOfQixie += rate;
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
                faction.DefenceRateWhileCombatMethodOfBubing -= rate;
                break;
            case 1:
                faction.DefenceRateWhileCombatMethodOfNubing -= rate;
                break;
            case 2:
                faction.DefenceRateWhileCombatMethodOfQibing -= rate;
                break;
            case 3:
                faction.DefenceRateWhileCombatMethodOfShuijun -= rate;
                break;
            case 4:
                faction.DefenceRateWhileCombatMethodOfQixie -= rate;
                break;
        }
    }
}
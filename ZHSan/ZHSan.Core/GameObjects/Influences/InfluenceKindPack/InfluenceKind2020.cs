using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind2020 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Faction faction)
    {
        var type = influence.GetIntParam();
        var increment = influence.GetIntParam2();

        switch (type)
        {
            case 0:
                faction.CriticalStrikeChanceIncrementWhileCombatMethodOfBubing += increment;
                break;
            case 1:
                faction.CriticalStrikeChanceIncrementWhileCombatMethodOfNubing += increment;
                break;
            case 2:
                faction.CriticalStrikeChanceIncrementWhileCombatMethodOfQibing += increment;
                break;
            case 3:
                faction.CriticalStrikeChanceIncrementWhileCombatMethodOfShuijun += increment;
                break;
            case 4:
                faction.CriticalStrikeChanceIncrementWhileCombatMethodOfQixie += increment;
                break;
        }
    }

    public override void PurifyInfluenceKind(Influence influence, Faction faction)
    {
        var type = influence.GetIntParam();
        var increment = influence.GetIntParam2();

        switch (type)
        {
            case 0:
                faction.CriticalStrikeChanceIncrementWhileCombatMethodOfBubing -= increment;
                break;
            case 1:
                faction.CriticalStrikeChanceIncrementWhileCombatMethodOfNubing -= increment;
                break;
            case 2:
                faction.CriticalStrikeChanceIncrementWhileCombatMethodOfQibing -= increment;
                break;
            case 3:
                faction.CriticalStrikeChanceIncrementWhileCombatMethodOfShuijun -= increment;
                break;
            case 4:
                faction.CriticalStrikeChanceIncrementWhileCombatMethodOfQixie -= increment;
                break;
        }
    }
}
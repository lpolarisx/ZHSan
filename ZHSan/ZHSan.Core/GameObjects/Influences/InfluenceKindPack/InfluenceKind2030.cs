using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind2030 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Faction faction)
    {
        var type = influence.GetIntParam();
        var increment = influence.GetIntParam2();

        switch (type)
        {
            case 0:
                faction.AntiCriticalStrikeChanceIncrementWhileCombatMethodOfBubing += increment;
                break;
            case 1:
                faction.AntiCriticalStrikeChanceIncrementWhileCombatMethodOfNubing += increment;
                break;
            case 2:
                faction.AntiCriticalStrikeChanceIncrementWhileCombatMethodOfQibing += increment;
                break;
            case 3:
                faction.AntiCriticalStrikeChanceIncrementWhileCombatMethodOfShuijun += increment;
                break;
            case 4:
                faction.AntiCriticalStrikeChanceIncrementWhileCombatMethodOfQixie += increment;
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
                faction.AntiCriticalStrikeChanceIncrementWhileCombatMethodOfBubing -= increment;
                break;
            case 1:
                faction.AntiCriticalStrikeChanceIncrementWhileCombatMethodOfNubing -= increment;
                break;
            case 2:
                faction.AntiCriticalStrikeChanceIncrementWhileCombatMethodOfQibing -= increment;
                break;
            case 3:
                faction.AntiCriticalStrikeChanceIncrementWhileCombatMethodOfShuijun -= increment;
                break;
            case 4:
                faction.AntiCriticalStrikeChanceIncrementWhileCombatMethodOfQixie -= increment;
                break;
        }
    }
}
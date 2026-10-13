using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind2200 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Faction faction)
    {
        var type = influence.GetIntParam();
        var increment = influence.GetIntParam2();

        switch (type)
        {
            case 0:
                faction.AntiArrowChanceIncrementOfBubing += increment;
                break;
            case 1:
                faction.AntiArrowChanceIncrementOfNubing += increment;
                break;
            case 2:
                faction.AntiArrowChanceIncrementOfQibing += increment;
                break;
            case 3:
                faction.AntiArrowChanceIncrementOfShuijun += increment;
                break;
            case 4:
                faction.AntiArrowChanceIncrementOfQixie += increment;
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
                faction.AntiArrowChanceIncrementOfBubing -= increment;
                break;
            case 1:
                faction.AntiArrowChanceIncrementOfNubing -= increment;
                break;
            case 2:
                faction.AntiArrowChanceIncrementOfQibing -= increment;
                break;
            case 3:
                faction.AntiArrowChanceIncrementOfShuijun -= increment;
                break;
            case 4:
                faction.AntiArrowChanceIncrementOfQixie -= increment;
                break;
        }
    }
}
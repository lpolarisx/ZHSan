using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind2220 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Faction faction)
    {
        var type = influence.GetIntParam();
        var increment = influence.GetIntParam2();

        switch (type)
        {
            case 0:
                faction.NoCounterChanceIncrementOfBubing += increment;
                break;
            case 1:
                faction.NoCounterChanceIncrementOfNubing += increment;
                break;
            case 2:
                faction.NoCounterChanceIncrementOfQibing += increment;
                break;
            case 3:
                faction.NoCounterChanceIncrementOfShuijun += increment;
                break;
            case 4:
                faction.NoCounterChanceIncrementOfQixie += increment;
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
                faction.NoCounterChanceIncrementOfBubing -= increment;
                break;
            case 1:
                faction.NoCounterChanceIncrementOfNubing -= increment;
                break;
            case 2:
                faction.NoCounterChanceIncrementOfQibing -= increment;
                break;
            case 3:
                faction.NoCounterChanceIncrementOfShuijun -= increment;
                break;
            case 4:
                faction.NoCounterChanceIncrementOfQixie -= increment;
                break;
        }
    }
}
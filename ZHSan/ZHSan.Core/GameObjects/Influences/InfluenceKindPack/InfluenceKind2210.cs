using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind2210 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Faction faction)
    {
        var type = influence.GetIntParam();
        var increment = influence.GetIntParam2();

        switch (type)
        {
            case 0:
                faction.OffenceRadiusIncrementOfBubing += increment;
                break;
            case 1:
                faction.OffenceRadiusIncrementOfNubing += increment;
                break;
            case 2:
                faction.OffenceRadiusIncrementOfQibing += increment;
                break;
            case 3:
                faction.OffenceRadiusIncrementOfShuijun += increment;
                break;
            case 4:
                faction.OffenceRadiusIncrementOfQixie += increment;
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
                faction.OffenceRadiusIncrementOfBubing -= increment;
                break;
            case 1:
                faction.OffenceRadiusIncrementOfNubing -= increment;
                break;
            case 2:
                faction.OffenceRadiusIncrementOfQibing -= increment;
                break;
            case 3:
                faction.OffenceRadiusIncrementOfShuijun -= increment;
                break;
            case 4:
                faction.OffenceRadiusIncrementOfQixie -= increment;
                break;
        }
    }
}
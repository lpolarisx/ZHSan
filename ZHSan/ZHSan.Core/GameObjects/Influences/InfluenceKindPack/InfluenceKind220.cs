using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind220 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Troop troop)
    {
        var terrain = influence.GetIntParam();

        var value = influence.GetIntParam2();

        switch (terrain)
        {
            case 1:
                troop.CriticalChanceIncrementOfPlain += value;
                break;
            case 2:
                troop.CriticalChanceIncrementOfGrassland += value;
                break;
            case 3:
                troop.CriticalChanceIncrementOfForrest += value;
                break;
            case 4:
                troop.CriticalChanceIncrementOfMarsh += value;
                break;
            case 5:
                troop.CriticalChanceIncrementOfMountain += value;
                break;
            case 6:
                troop.CriticalChanceIncrementOfWater += value;
                break;
            case 7:
                troop.CriticalChanceIncrementOfRidge += value;
                break;
            case 8:
                troop.CriticalChanceIncrementOfWasteland += value;
                break;
            case 9:
                troop.CriticalChanceIncrementOfDesert += value;
                break;
            case 10:
                troop.CriticalChanceIncrementOfCliff += value;
                break;
        }
    }

    public override void PurifyInfluenceKind(Influence influence, Troop troop)
    {
        var terrain = influence.GetIntParam();

        var value = influence.GetIntParam2();

        switch (terrain)
        {
            case 1:
                troop.CriticalChanceIncrementOfPlain -= value;
                break;

            case 2:
                troop.CriticalChanceIncrementOfGrassland -= value;
                break;

            case 3:
                troop.CriticalChanceIncrementOfForrest -= value;
                break;

            case 4:
                troop.CriticalChanceIncrementOfMarsh -= value;
                break;

            case 5:
                troop.CriticalChanceIncrementOfMountain -= value;
                break;

            case 6:
                troop.CriticalChanceIncrementOfWater -= value;
                break;

            case 7:
                troop.CriticalChanceIncrementOfRidge -= value;
                break;

            case 8:
                troop.CriticalChanceIncrementOfWasteland -= value;
                break;

            case 9:
                troop.CriticalChanceIncrementOfDesert -= value;
                break;

            case 10:
                troop.CriticalChanceIncrementOfCliff -= value;
                break;
        }
    }
}
using System;
using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind200 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Troop troop)
    {
        var terrain = influence.GetIntParam();

        var value = influence.GetIntParam2();

        switch (terrain)
        {
            case 1:
                troop.DecrementOfPlainAdaptability = Math.Max(troop.DecrementOfPlainAdaptability, value);
                break;
            case 2:
                troop.DecrementOfGrasslandAdaptability = Math.Max(troop.DecrementOfGrasslandAdaptability, value);
                break;
            case 3:
                troop.DecrementOfForrestAdaptability = Math.Max(troop.DecrementOfForrestAdaptability, value);
                break;
            case 4:
                troop.DecrementOfMarshAdaptability = Math.Max(troop.DecrementOfMarshAdaptability, value);
                break;
            case 5:
                troop.DecrementOfMountainAdaptability = Math.Max(troop.DecrementOfMountainAdaptability, value);
                break;
            case 6:
                troop.DecrementOfWaterAdaptability = Math.Max(troop.DecrementOfWaterAdaptability, value);
                break;
            case 7:
                troop.DecrementOfRidgeAdaptability = Math.Max(troop.DecrementOfRidgeAdaptability, value);
                break;
            case 8:
                troop.DecrementOfWastelandAdaptability = Math.Max(troop.DecrementOfWastelandAdaptability, value);
                break;
            case 9:
                troop.DecrementOfDesertAdaptability = Math.Max(troop.DecrementOfDesertAdaptability, value);
                break;
            case 10:
                troop.DecrementOfCliffAdaptability = Math.Max(troop.DecrementOfCliffAdaptability, value);
                break;
        }
    }

    public override void PurifyInfluenceKind(Influence influence, Troop troop)
    {
        var terrain = influence.GetIntParam();

        switch (terrain)
        {
            case 1:
                troop.DecrementOfPlainAdaptability = 0;
                break;
            case 2:
                troop.DecrementOfGrasslandAdaptability = 0;
                break;
            case 3:
                troop.DecrementOfForrestAdaptability = 0;
                break;
            case 4:
                troop.DecrementOfMarshAdaptability = 0;
                break;
            case 5:
                troop.DecrementOfMountainAdaptability = 0;
                break;
            case 6:
                troop.DecrementOfWaterAdaptability = 0;
                break;
            case 7:
                troop.DecrementOfRidgeAdaptability = 0;
                break;
            case 8:
                troop.DecrementOfWastelandAdaptability = 0;
                break;
            case 9:
                troop.DecrementOfDesertAdaptability = 0;
                break;
            case 10:
                troop.DecrementOfCliffAdaptability = 0;
                break;
        }
    }
}
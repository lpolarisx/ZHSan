using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind1530 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Troop troop)
    {
        var terrain = condition.GetIntParam();

        var troopCount = condition.GetIntParam2();

        switch (terrain)
        {
            case 1:
                return troop.ViewingPlainHostileTroopCount < troopCount;

            case 2:
                return troop.ViewingGrasslandHostileTroopCount < troopCount;

            case 3:
                return troop.ViewingForestHostileTroopCount < troopCount;

            case 4:
                return troop.ViewingMarshHostileTroopCount < troopCount;

            case 5:
                return troop.ViewingMountainHostileTroopCount < troopCount;

            case 6:
                return troop.ViewingWaterHostileTroopCount < troopCount;

            case 7:
                return troop.ViewingRidgeHostileTroopCount < troopCount;

            case 8:
                return troop.ViewingWastelandHostileTroopCount < troopCount;

            case 9:
                return troop.ViewingDesertHostileTroopCount < troopCount;

            case 10:
                return troop.ViewingCliffHostileTroopCount < troopCount;
        }
        return false;
    }
}
using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind1510 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Troop troop)
    {
        var terrain = condition.GetIntParam();

        var troopCount = condition.GetIntParam2();

        switch (terrain)
        {
            case 1:
                return troop.ViewingPlainFriendlyTroopCount < troopCount;

            case 2:
                return troop.ViewingGrasslandFriendlyTroopCount < troopCount;

            case 3:
                return troop.ViewingForestFriendlyTroopCount < troopCount;

            case 4:
                return troop.ViewingMarshFriendlyTroopCount < troopCount;

            case 5:
                return troop.ViewingMountainFriendlyTroopCount < troopCount;

            case 6:
                return troop.ViewingWaterFriendlyTroopCount < troopCount;

            case 7:
                return troop.ViewingRidgeFriendlyTroopCount < troopCount;

            case 8:
                return troop.ViewingWastelandFriendlyTroopCount < troopCount;

            case 9:
                return troop.ViewingDesertFriendlyTroopCount < troopCount;

            case 10:
                return troop.ViewingCliffFriendlyTroopCount < troopCount;
        }
        return false;
    }
}
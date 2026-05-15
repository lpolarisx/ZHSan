using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind1725 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Troop troop)
    {
        var army = troop.Army;

        var result = army.FollowedLeader != null && 
                     army.FollowedLeader.LocationArchitecture != null &&
                     army.Leader.LocationArchitecture.ID != condition.GetIntParam();

        return result;
    }
}
using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind1735 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Troop troop)
    {
        var army = troop.Army;

        var result = army.FollowedLeader != null && 
                     army.FollowedLeader.BelongedFaction != null &&
                     army.Leader.BelongedFaction.ID != condition.GetIntParam();

        return result;
    }
}
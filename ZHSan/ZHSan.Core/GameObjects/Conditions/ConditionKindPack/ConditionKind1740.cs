using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind1740 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Troop troop)
    {
        var leader = troop.Army.FollowedLeader;

        return leader != null && leader.ID == condition.GetIntParam();
    }
}
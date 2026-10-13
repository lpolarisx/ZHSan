using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind1605 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Troop troop)
    {
        var army = troop.Army;

        var faction = troop.BelongedFaction;

        var result = army.FollowedLeader != null && faction != null && army.FollowedLeader.ID != faction.LeaderID;

        return result;
    }
}
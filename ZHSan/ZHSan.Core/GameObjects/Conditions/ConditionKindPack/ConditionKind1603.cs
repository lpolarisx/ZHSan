using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind1603 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Troop troop)
    {
        return troop.Leader != troop.BelongedFaction.Leader;
    }
}
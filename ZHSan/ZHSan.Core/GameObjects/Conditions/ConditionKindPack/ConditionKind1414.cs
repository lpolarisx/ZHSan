using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind1414 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Troop troop)
    {
        var leader = troop.Leader;

        return leader.Braveness <= leader.Calmness;
    }
}
using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind1300 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Troop troop)
    {
        return (int)troop.Army.Kind.Type == condition.GetIntParam();
    }
}
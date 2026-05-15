using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind1210 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Troop troop)
    {
        return troop.Quantity > 0 && (float)troop.InjuryQuantity / troop.Quantity >= condition.GetFloatParam();
    }
}
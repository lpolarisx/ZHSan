using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind1280 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Troop troop)
    {
        return troop.ViewingHostileTroopCount >= condition.GetIntParam();
    }
}
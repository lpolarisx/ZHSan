using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind1003 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Troop troop)
    {
        return troop.Status != TroopStatus.埋伏;
    }
}
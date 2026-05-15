using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind3060 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Faction faction)
    {
        return faction.BaseMilitaryKinds.MilitaryKinds.ContainsKey(condition.GetIntParam());
    }
}
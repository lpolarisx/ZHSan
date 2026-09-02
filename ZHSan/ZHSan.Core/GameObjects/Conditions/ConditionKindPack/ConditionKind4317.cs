using System.Runtime.Serialization;
using GameEnums;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind4317 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        return person.OutsideTask == OutsideTaskKind.技能;
    }
}
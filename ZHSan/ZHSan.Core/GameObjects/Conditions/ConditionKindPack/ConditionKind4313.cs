using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind4313 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        return person.OutsideTask == OutsideTaskKind.破坏;
    }
}
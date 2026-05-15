using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind4324 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        return person.OutsideTask == OutsideTaskKind.劝降;
    }
}
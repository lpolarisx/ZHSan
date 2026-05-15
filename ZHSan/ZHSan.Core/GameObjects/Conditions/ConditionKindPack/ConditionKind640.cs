using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind640 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        return person.Stunts.GetStunt(condition.GetIntParam()) != null;
    }
}
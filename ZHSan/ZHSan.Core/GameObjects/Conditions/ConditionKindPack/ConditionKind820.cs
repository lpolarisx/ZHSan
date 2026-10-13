using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind820 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        return person.NumberOfChildren < condition.GetIntParam();
    }
}
using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind4735 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        return person.UntiredMerit - markedPerson.UntiredMerit < condition.GetIntParam();
    }
}
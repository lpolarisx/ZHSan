using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind4645 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        return person.PoliticsExperience - markedPerson.PoliticsExperience < condition.GetIntParam();
    }
}
using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind4650 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        return person.GlamourExperience - markedPerson.GlamourExperience >= condition.GetIntParam();
    }
}
using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind4690 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        return person.ShuijunExperience - markedPerson.ShuijunExperience >= condition.GetIntParam();
    }
}
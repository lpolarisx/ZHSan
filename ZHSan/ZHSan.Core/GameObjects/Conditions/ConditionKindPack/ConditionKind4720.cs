using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind4720 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        return person.TacticsExperience - markedPerson.TacticsExperience >= condition.GetIntParam();
    }
}
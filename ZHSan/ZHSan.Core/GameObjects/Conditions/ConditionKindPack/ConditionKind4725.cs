using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind4725 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        return person.TacticsExperience - markedPerson.TacticsExperience < condition.GetIntParam();
    }
}
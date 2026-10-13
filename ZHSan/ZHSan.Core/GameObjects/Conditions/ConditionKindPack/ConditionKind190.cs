using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind190 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        return person.GlamourIncludingExperience >= condition.GetIntParam();
    }
}
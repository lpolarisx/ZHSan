using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind140 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        return person.GlamourExperience >= condition.GetIntParam();
    }
}
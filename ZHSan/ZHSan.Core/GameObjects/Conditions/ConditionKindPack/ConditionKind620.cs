using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind620 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        return person.Skills.GetSkill(condition.GetIntParam()) != null;
    }
}
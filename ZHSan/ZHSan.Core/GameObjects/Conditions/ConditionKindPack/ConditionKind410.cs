using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind410 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        return person.TacticsExperience >= condition.GetIntParam();
    }
}
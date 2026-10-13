using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind130 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        return person.PoliticsExperience >= condition.GetIntParam();
    }
}
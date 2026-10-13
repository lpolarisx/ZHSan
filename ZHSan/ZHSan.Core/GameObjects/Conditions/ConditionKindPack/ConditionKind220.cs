using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind220 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        return person.QibingExperience >= condition.GetIntParam();
    }
}
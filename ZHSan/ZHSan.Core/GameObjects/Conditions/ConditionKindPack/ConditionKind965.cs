using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind965 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        return person.Sex && !person.huaiyun;
    }
}
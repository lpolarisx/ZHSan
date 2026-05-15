using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind4145 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        return person.OfficerKillCount < condition.GetIntParam();
    }
}
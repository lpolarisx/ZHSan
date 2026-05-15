using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind811 : ConditionKind
{

    public override bool CheckConditionKind(Condition condition, Person person)
    {
        return person.ArrivingDays > 0;
    }
}
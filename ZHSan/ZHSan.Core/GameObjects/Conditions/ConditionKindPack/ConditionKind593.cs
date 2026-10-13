using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind593 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        return person.RoutCount < person.RoutedCount;
    }
}
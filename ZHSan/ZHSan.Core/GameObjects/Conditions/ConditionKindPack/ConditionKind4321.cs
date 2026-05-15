using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind4321 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        return person.OutsideTask == OutsideTaskKind.结盟;
    }
}
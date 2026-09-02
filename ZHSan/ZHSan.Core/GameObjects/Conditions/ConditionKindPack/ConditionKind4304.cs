using System.Runtime.Serialization;
using GameEnums;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind4304 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        return person.WorkKind == ArchitectureWorkKind.统治;
    }
}
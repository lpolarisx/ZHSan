using System.Runtime.Serialization;
using GameEnums;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind4307 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        return person.WorkKind == ArchitectureWorkKind.补充;
    }
}
using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind4300 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        return person.OutsideTask == OutsideTaskKind.无 && person.WorkKind == GameGlobal.ArchitectureWorkKind.无;
    }
}
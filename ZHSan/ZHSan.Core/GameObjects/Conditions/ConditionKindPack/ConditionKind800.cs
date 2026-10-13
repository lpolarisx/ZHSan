using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind800 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        return person.BelongedFaction == null;
    }
}
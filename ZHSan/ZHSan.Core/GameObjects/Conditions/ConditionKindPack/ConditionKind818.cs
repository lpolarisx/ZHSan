using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind818 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        var result = (person.BelongedFaction != null && person.BelongedFaction.PrinceID != person.ID) || person.BelongedFaction == null;

        return result;
    }
}
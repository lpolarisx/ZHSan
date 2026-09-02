using GameManager;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind535 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        var personId = condition.GetIntParam();
        var otherPerson = Session.Current.Scenario.AllPersons.GetValueOrDefault(personId);

        var result = !(person.BelongedFactionWithPrincess != null && otherPerson.BelongedFactionWithPrincess != null && person.BelongedFactionWithPrincess == otherPerson.BelongedFactionWithPrincess);

        return result;
    }
}
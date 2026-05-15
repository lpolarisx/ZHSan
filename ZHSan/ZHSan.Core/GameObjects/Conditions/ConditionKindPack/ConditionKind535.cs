using GameManager;
using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind535 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        Person p1 = Session.Current.Scenario.Persons.GetGameObject(condition.GetIntParam()) as Person;

        var result = !(person.BelongedFactionWithPrincess != null && p1.BelongedFactionWithPrincess != null && person.BelongedFactionWithPrincess == p1.BelongedFactionWithPrincess);

        return result;
    }
}
using GameManager;
using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind690 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        if (person.ID == -1)
        {
            person = Session.Current.Scenario.Persons[GameObject.Random(Session.Current.Scenario.Persons.Count)] as Person;
        }

        var result = person.LocationArchitecture != null && person.LocationArchitecture.BelongedFaction == person.BelongedFaction;

        return result;
    }
}
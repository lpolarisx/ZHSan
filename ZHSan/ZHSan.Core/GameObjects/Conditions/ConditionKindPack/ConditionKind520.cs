using GameManager;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind520 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        var personId = condition.GetIntParam();
        var otherPerson = Session.Current.Scenario.AllPersons.GetValueOrDefault(personId);

        var result = person.LocationArchitecture != null && otherPerson.LocationArchitecture != null && person.LocationArchitecture == otherPerson.LocationArchitecture;

        return result;
    }
}
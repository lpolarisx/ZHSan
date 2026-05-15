using GameManager;
using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind525 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        Person p1 = Session.Current.Scenario.Persons.GetGameObject(condition.GetIntParam()) as Person;

        var result = !(person.LocationArchitecture != null && p1.LocationArchitecture != null && person.LocationArchitecture == p1.LocationArchitecture);

        return result;
    }
}
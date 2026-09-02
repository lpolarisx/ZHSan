using GameGlobal;
using GameManager;
using System.Linq;
using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind690 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        if (person.ID == -1)
        {
            person = StaticMethods.GetRandomItem(Session.Current.Scenario.AllPersons.Values.ToList());
        }

        var result = person.LocationArchitecture != null && person.LocationArchitecture.BelongedFaction == person.BelongedFaction;

        return result;
    }
}
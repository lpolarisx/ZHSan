using GameManager;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind840 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        HashSet<Person> relatedPersons = new HashSet<Person>();
        foreach (Person p in Session.Current.Scenario.Persons)
        {
            if (p.Father == person)
            {
                relatedPersons.Add(p.Mother);
            }
            if (p.Mother == person)
            {
                relatedPersons.Add(p.Father);
            }
        }
        return relatedPersons.Count < condition.GetIntParam();
    }
}
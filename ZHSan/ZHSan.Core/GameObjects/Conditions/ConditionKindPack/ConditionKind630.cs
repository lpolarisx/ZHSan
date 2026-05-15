using GameManager;
using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind630 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        return person.RealTitles.Contains(Session.Current.Scenario.GameCommonData.AllTitles.GetTitle(condition.GetIntParam()));
    }
}
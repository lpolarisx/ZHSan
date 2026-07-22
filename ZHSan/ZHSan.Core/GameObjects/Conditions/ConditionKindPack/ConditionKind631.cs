using GameManager;
using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind631 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        var titleId = condition.GetIntParam();

        return Session.Current.Scenario.GameCommonData.AllTitles.TryGetValue(titleId, out var title) && title.CanLearn(person);
    }
}
using GameManager;
using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind636 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        return !Session.Current.Scenario.GameCommonData.AllTitles.GetTitle(condition.GetIntParam()).CanLearn(person);
    }
}
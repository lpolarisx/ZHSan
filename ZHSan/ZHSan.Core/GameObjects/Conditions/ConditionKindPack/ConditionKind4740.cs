using GameManager;
using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind4740 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        var kindId = condition.GetIntParam();

        var current = person.getTitleOfKind(Session.Current.Scenario.GameCommonData.AllTitleKinds.GetTitleKind(kindId));
        var marked = markedPerson.getTitleOfKind(Session.Current.Scenario.GameCommonData.AllTitleKinds.GetTitleKind(kindId));

        int t1 = current?.Level ?? 0;
        int t2 = marked?.Level ?? 0;

        return t1 - t2 >= condition.GetIntParam2();
    }
}
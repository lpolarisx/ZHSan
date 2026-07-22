using GameManager;
using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind4745 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        var kindId = condition.GetIntParam();
        var current = person.GetTitleOfKind(kindId);
        var marked = markedPerson.GetTitleOfKind(kindId);

        int t1 = current?.Level ?? 0;
        int t2 = marked?.Level ?? 0;

        return t1 - t2 < condition.GetIntParam2();
    }
}
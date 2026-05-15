using System.Runtime.Serialization;
using GameObjects.PersonDetail;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind806 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        return person.Status == PersonStatus.Princess;
    }
}
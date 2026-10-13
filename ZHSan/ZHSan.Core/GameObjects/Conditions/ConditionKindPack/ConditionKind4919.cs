using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind4919 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        return (person.Father != null || person.Mother != null) && (person.Father == markedPerson.Father || person.Mother == markedPerson.Mother);
    }
}

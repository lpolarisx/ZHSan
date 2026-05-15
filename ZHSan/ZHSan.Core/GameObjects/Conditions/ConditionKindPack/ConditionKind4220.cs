using System.Runtime.Serialization;
using GameObjects.PersonDetail;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind4220 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        var arch = person.LocationArchitecture;

        var result = arch != null && arch.ID == condition.GetIntParam() && person.Status == PersonStatus.Princess;

        return result;
    }
}
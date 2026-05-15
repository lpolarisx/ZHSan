using System.Linq;
using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind680 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        return person.RealTitles.Any(x => x.Kind.ID == condition.GetIntParam());
    }
}
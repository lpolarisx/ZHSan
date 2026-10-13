using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind2835 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Architecture arch)
    {
        var personId = condition.GetIntParam();

        foreach (var person in arch.GetConcubines())
        {
            if (person.ID == personId)
            {
                return false;
            }
        }

        return true;
    }
}
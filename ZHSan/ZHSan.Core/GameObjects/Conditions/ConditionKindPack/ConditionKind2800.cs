using System.Runtime.Serialization;
using GameObjects.PersonDetail;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind2800 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Architecture arch)
    {
        var personId = condition.GetIntParam();

        foreach (Person person in arch.Persons)
        {
            if (person.ID == personId && person.Status == PersonStatus.Normal)
            {
                return true;
            }
        }

        return false;
    }
}
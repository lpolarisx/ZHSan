using System.Runtime.Serialization;
using GameObjects.PersonDetail;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind2815 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Architecture arch)
    {
        var personId = condition.GetIntParam();

        foreach (var person in arch.GetNoFactionPersons())
        {
            if (person.ID == personId && person.Status == PersonStatus.NoFaction)
            {
                return false;
            }
        }

        return true;
    }
}
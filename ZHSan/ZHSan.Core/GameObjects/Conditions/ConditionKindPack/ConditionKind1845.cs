using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind1845 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Troop troop)
    {
        var id = condition.GetIntParam();

        foreach (Person p in troop.Persons)
        {
            if (p.ID == id)
            {
                return false;
            }
        }
        
        return true;
    }
}
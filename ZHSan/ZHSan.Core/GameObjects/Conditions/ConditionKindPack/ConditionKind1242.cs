using GameManager;
using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind1242 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Troop troop)
    {
        GameObjectList viewingArchitecturesByPosition = Session.Current.Scenario.GetViewingArchitecturesByPosition(troop.Position);
        foreach (Architecture architecture in viewingArchitecturesByPosition)
        {
            if (!troop.IsFriendly(architecture.BelongedFaction))
            {
                return true;
            }
        }
        
        return false;
    }
}
using GameManager;
using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind1243 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Troop troop)
    {
        var architectures = Session.Current.Scenario.GetViewingArchitecturesByPosition(troop.Position);

        foreach (var architecture in architectures)
        {
            if (!troop.IsFriendly(architecture.BelongedFaction))
            {
                return false;
            }
        }

        return true;
    }
}
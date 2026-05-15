using GameManager;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind2755 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Architecture arch)
    {
        int hostile = 0;
        foreach (Point point in arch.ArchitectureArea.Area)
        {
            Troop troopByPosition = Session.Current.Scenario.GetTroopByPosition(point);
            if (troopByPosition != null && !troopByPosition.IsFriendly(arch.BelongedFaction))
            {
                hostile++;
            }
        }

        return hostile < condition.GetIntParam();
    }
}
using GameManager;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind2740 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Architecture arch)
    {
        int friendly = 0;
        foreach (Point point in arch.ArchitectureArea.Area)
        {
            Troop troopByPosition = Session.Current.Scenario.GetTroopByPosition(point);
            if (troopByPosition != null && troopByPosition.IsFriendly(arch.BelongedFaction))
            {
                friendly++;
            }
        }

        return friendly >= condition.GetIntParam();
    }
}

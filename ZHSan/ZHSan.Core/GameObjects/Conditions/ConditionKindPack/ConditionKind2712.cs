using GameManager;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind2712 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Architecture arch)
    {
        int hostile = 0;
        int friendly = 0;
        foreach (Point point in arch.LongViewArea.Area)
        {
            Troop troopByPosition = Session.Current.Scenario.GetTroopByPosition(point);
            if (troopByPosition != null)
            {
                if (troopByPosition.IsFriendly(arch.BelongedFaction))
                {
                    friendly++;
                }
                else
                {
                    hostile++;
                }
            }
        }
        
        return friendly > 0 && hostile > 0 && friendly >= hostile;
    }
}
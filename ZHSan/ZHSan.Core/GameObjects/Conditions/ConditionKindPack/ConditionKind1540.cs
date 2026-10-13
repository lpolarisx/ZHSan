using GameManager;
using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind1540 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Troop troop)
    {
        return (int)Session.Current.Scenario.GetTerrainKindByPosition(troop.Position) == condition.GetIntParam();
    }
}
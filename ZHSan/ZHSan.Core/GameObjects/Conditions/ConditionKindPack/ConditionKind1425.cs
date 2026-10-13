using GameManager;
using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind1425 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Troop troop)
    {
        var arch = Session.Current.Scenario.GetArchitectureByPositionNoCheck(troop.Position);

        return arch != null && arch.ID != condition.GetIntParam();
    }
}
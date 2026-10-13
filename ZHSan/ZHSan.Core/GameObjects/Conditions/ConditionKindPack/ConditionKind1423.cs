using GameManager;
using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind1423 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Troop troop)
    {
        var arch = Session.Current.Scenario.GetArchitectureByPositionNoCheck(troop.Position);
        
        return arch == null || troop.IsFriendly(arch.BelongedFaction);
    }
}
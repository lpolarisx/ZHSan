using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind3051 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Faction faction)
    {
        return !faction.IsAlien;
    }
}
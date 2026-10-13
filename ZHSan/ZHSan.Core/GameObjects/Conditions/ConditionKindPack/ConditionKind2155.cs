using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind2155 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Architecture arch)
    {
        return arch.MoraleCeiling > 0 && arch.Morale / arch.MoraleCeiling < condition.GetFloatParam();
    }
}
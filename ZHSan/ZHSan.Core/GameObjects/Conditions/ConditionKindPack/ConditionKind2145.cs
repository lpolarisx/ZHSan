using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind2145 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Architecture arch)
    {
        return arch.EnduranceCeiling > 0 && arch.Endurance / arch.EnduranceCeiling < condition.GetFloatParam();
    }
}
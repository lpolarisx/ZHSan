using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind2115 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Architecture arch)
    {
        return arch.CommerceCeiling > 0 && arch.Commerce / arch.CommerceCeiling < condition.GetFloatParam();
    }
}
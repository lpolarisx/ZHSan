using System.Linq;
using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind2300 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Architecture arch)
    {
        var requiredCount = condition.GetIntParam();
        var kindId = condition.GetIntParam2();
        
        var count = arch.Facilities.Count(x => x.KindID == kindId);
        
        return count >= requiredCount;
    }
}
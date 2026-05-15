using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind2545 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Architecture arch)
    {
        var count = condition.GetIntParam();
        var kindId = condition.GetIntParam2();

        int result = 0;
        foreach (Military m in arch.Militaries)
        {
            if (m.KindID == kindId)
            {
                result++;
            }
        }

        return result < count;
    }

    public override bool CheckConditionKind(Condition condition, Faction faction)
    {
        var count = condition.GetIntParam();
        var kindId = condition.GetIntParam2();

        int result = 0;
        foreach (Military m in faction.Militaries)
        {
            if (m.KindID == kindId)
            {
                result++;
            }
        }

        return result < count;
    }
}
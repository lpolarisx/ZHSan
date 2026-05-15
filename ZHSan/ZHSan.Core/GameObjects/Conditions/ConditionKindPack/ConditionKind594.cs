using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind594 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        // 总战斗次数 = 击破次数 + 被击破次数
        var totalBattles = person.RoutCount + person.RoutedCount;

        var result = totalBattles > 0 && (float)person.RoutCount / totalBattles >= condition.GetFloatParam();

        return result;
    }
}
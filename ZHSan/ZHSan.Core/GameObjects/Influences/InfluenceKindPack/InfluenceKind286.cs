using System.Runtime.Serialization;
using GameManager;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind286 : InfluenceKind
{
    public override bool IsVaild(Influence influence, Person person)
    {
        var conditionId = influence.GetIntParam();

        if (Session.Current.Scenario.GameCommonData.AllConditions.TryGetValue(conditionId, out var condition))
        {
            return condition.CheckCondition(person);
        }

        return false;
    }
}
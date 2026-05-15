using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind918 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        var result = person.BelongedFactionWithPrincess != null && 
                     (person.Father != null || person.Mother != null) && 
                     (person.BelongedFactionWithPrincess.Leader.Father == person.Father || person.BelongedFactionWithPrincess.Leader.Mother == person.Mother);

        return result;
    }
}
using System.Runtime.Serialization;

namespace GameObjects.ArchitectureDetail.EventEffect;

[DataContract]
public class EventEffect215 : EventEffectKind
{
    public override void ApplyEffectKind(EventEffect eventEffect, Person person, Event e)
    {
        person.huaiyun = true;
        person.huaiyuntianshu = 0;

        var princess = person.BelongedFactionWithPrincess;

        if (princess != null)
        {
            var leader = princess.Leader;

            person.suoshurenwu = princess.LeaderID;
            leader.suoshurenwu = person.ID;

            if (!person.suoshurenwuList.Contains(leader))
            {
                person.suoshurenwuList.Add(leader);
            }

            if (!leader.suoshurenwuList.Contains(person))
            {
                leader.suoshurenwuList.Add(person);
            }
        }
        else
        {
            person.suoshurenwu = person.ID;
        }
    }
}
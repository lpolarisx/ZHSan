using System.Runtime.Serialization;

namespace GameObjects.ArchitectureDetail.EventEffect;

[DataContract]
public class EventEffect1310 : EventEffectKind
{
    public override void ApplyEffectKind(EventEffect eventEffect, Architecture arch, Event e)
    {
        var remainingDays = arch.zainan.shengyutianshu;

        remainingDays += eventEffect.GetIntParam();
        if (remainingDays <= 0)
        {
            arch.youzainan = false;
            arch.tingzhizhenzai();
        }
    }
}
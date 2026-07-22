using GameGlobal;
using GameManager;
using System.Runtime.Serialization;

namespace GameObjects.ArchitectureDetail.EventEffect;

[DataContract]
public class EventEffect1300 : EventEffectKind
{
    public override void ApplyEffectKind(EventEffect eventEffect, Architecture arch, Event e)
    {
        var kindId = eventEffect.GetIntParam();
        if (Session.Current.Scenario.GameCommonData.AllDisasterKinds.TryGetValue(kindId, out var disasterKind))
        {
            var disaster = arch.zainan;
            disaster.DisasterKind = disasterKind;

            disaster.shengyutianshu = disasterKind.MinDuration + StaticMethods.Random(disasterKind.MaxDuration - disasterKind.MinDuration);
            arch.youzainan = true;

            //发生灾难时不能补充
            foreach (Military military in arch.Militaries)
            {
                military.StopRecruitment();
            }
        }
    }
}
using GameManager;
using System.Runtime.Serialization;

namespace GameObjects.ArchitectureDetail.EventEffect;

[DataContract]
public class EventEffect1200 : EventEffectKind
{
    public override void ApplyEffectKind(EventEffect eventEffect, Architecture arch, Event e)
    {
        var facilityLevelId = eventEffect.GetIntParam();

        if (Session.Current.Scenario.GameCommonData.AllFacilityKindLevels.TryGetValue(facilityLevelId, out var facilityLevel))
        {
            arch.BuildFacility(facilityLevel);
        }
    }
}
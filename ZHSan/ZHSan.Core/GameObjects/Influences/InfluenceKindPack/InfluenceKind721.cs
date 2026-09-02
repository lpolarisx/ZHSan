using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind721 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Troop troop)
    {
        var maxDays = influence.GetIntParam();

        var enemy = troop.OrientationTroop;
        var inevitableSuccess = troop.InevitableRumourOnLowerIntelligence || troop.InevitableStratagemOnLowerIntelligence;

        if (troop.GetCurrentStratagemSuccess(enemy, inevitableSuccess, enemy.InvincibleAttract, enemy.InvincibleStratagemFromLowerIntelligence))
        {
            enemy.SetAttract(troop, troop.GenerateCastAttractDay(maxDays));
        }

        foreach (var troop2 in troop.AreaStratagemTroops)
        {
            if (troop.GetCurrentStratagemSuccess(troop2, inevitableSuccess, troop2.InvincibleAttract, troop2.InvincibleStratagemFromLowerIntelligence))
            {
                troop2.SetAttract(troop, troop.GenerateCastAttractDay(maxDays));
            }
        }
    }

    public override int GetCredit(Influence influence, Troop source, Troop destination)
    {
        var inevitableSuccess = source.InevitableAttractOnLowerIntelligence || source.InevitableStratagemOnLowerIntelligence;

        var sum = 0;
        int pureFightingForce = source.PureFightingForce;
        foreach (var troop in source.GetAreaStratagemTroops(destination))
        {
            if (troop.FightingForce > source.FightingForce) return -100;

            int num3 = source.GetStratagemSuccessChanceCredit(troop, inevitableSuccess, troop.InvincibleAttract, troop.InvincibleStratagemFromLowerIntelligence);
            if (num3 > 0)
            {
                num3 = (num3 + (100 - troop.Morale) / 2 + (40 - troop.Army.Scales) * 5) * troop.PureFightingForce / pureFightingForce;
                sum += num3;
            }
        }
        return sum * (2 + source.IncrementOfAttractDay * 3) / 2;
    }
}
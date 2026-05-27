using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind391 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Troop troop)
    {
        var maxDays = influence.GetIntParam();

        var enemy = troop.OrientationTroop;
        var inevitableSuccess = troop.InevitableRaoluanOnLowerIntelligence || troop.InevitableStratagemOnLowerIntelligence;
        var invincible = enemy.OrientationTroop.NeverBeIntoChaos || enemy.OrientationTroop.OutburstNeverBeIntoChaos || enemy.InvincibleRaoluan;

        if (troop.GetCurrentStratagemSuccess(enemy, inevitableSuccess, invincible, enemy.InvincibleStratagemFromLowerIntelligence))
        {
            troop.OrientationTroop.SetChaos(troop.GenerateCastChaosDay(maxDays));
        }

        foreach (Troop troop2 in troop.AreaStratagemTroops)
        {
            if (troop.GetCurrentStratagemSuccess(troop2, inevitableSuccess, troop2.NeverBeIntoChaos || troop2.OutburstNeverBeIntoChaos || troop2.InvincibleRaoluan, troop2.InvincibleStratagemFromLowerIntelligence))
            {
                troop2.SetChaos(troop.GenerateCastChaosDay(maxDays));
            }
        }
    }

    public override int GetCredit(Troop source, Troop destination)
    {
        var sum = 0;
        int pureFightingForce = source.PureFightingForce;

        var inevitableSuccess = source.InevitableRaoluanOnLowerIntelligence || source.InevitableStratagemOnLowerIntelligence;

        foreach (Troop troop in source.GetAreaStratagemTroops(destination, false))
        {
            int num3 = source.GetStratagemSuccessChanceCredit(troop, inevitableSuccess, troop.NeverBeIntoChaos || troop.OutburstNeverBeIntoChaos || troop.InvincibleRaoluan, troop.InvincibleStratagemFromLowerIntelligence);

            if (destination.ChaosDayLeft > 2)
            {
                num3 /= destination.ChaosDayLeft - 1;
            }

            // TODO: 整数运算？
            num3 *= (1 - troop.ChaosRecoverByViewArea / 100) * (1 - troop.ChaosRecoverInViewArea / 100);

            if (troop.ChaosLastOneDay)
            {
                num3 /= 5;
            }

            if (num3 > 0)
            {
                num3 = (num3 + (120 - troop.Morale) / 2 + (troop.Army.Scales - 5) * 5) * troop.PureFightingForce / pureFightingForce;
                sum += num3;
            }
        }
        return sum * (2 + source.IncrementOfChaosDay * 3) / 2;
    }
}
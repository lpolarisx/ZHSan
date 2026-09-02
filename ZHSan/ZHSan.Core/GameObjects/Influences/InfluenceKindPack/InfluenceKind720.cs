using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind720 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Troop troop)
    {
        var maxDays = influence.GetIntParam();

        var enemy = troop.OrientationTroop;
        var inevitableSuccess = troop.InevitableRumourOnLowerIntelligence || troop.InevitableStratagemOnLowerIntelligence;

        if (troop.GetCurrentStratagemSuccess(enemy, inevitableSuccess, enemy.NeverBeIntoChaos || enemy.OutburstNeverBeIntoChaos || enemy.InvincibleRumour, enemy.InvincibleStratagemFromLowerIntelligence))
        {
            enemy.SetRumour(troop.GenerateCastRumourDay(maxDays));
        }

        foreach (var troop2 in troop.AreaStratagemTroops)
        {
            if (troop.GetCurrentStratagemSuccess(troop2, inevitableSuccess, troop2.NeverBeIntoChaos || troop2.OutburstNeverBeIntoChaos || troop2.InvincibleRumour, troop2.InvincibleStratagemFromLowerIntelligence))
            {
                troop2.SetRumour(troop.GenerateCastRumourDay(maxDays));
            }
        }
    }

    public override int GetCredit(Influence influence, Troop source, Troop destination)
    {
        var inevitableSuccess = source.InevitableRumourOnLowerIntelligence || source.InevitableStratagemOnLowerIntelligence;

        var sum = 0;
        int pureFightingForce = source.PureFightingForce;
        foreach (var troop in source.GetAreaStratagemTroops(destination))
        {
            int num3 = source.GetStratagemSuccessChanceCredit(troop, inevitableSuccess, troop.InvincibleAttract, troop.InvincibleStratagemFromLowerIntelligence);
            if (num3 > 0)
            {
                num3 = (num3 + (troop.Army.Scales - 5) * 10) * troop.PureFightingForce / pureFightingForce;
                sum += num3;
            }
        }
        return sum * (2 + source.IncrementOfRumourDay * 3) / 2;
    }
}
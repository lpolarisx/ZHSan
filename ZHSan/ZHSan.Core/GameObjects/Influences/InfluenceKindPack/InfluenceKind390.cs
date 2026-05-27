using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind390 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Troop troop)
    {
        var baseDecrement = influence.GetIntParam();

        var enemy = troop.OrientationTroop;
        var inevitableSuccess = troop.InevitableGongxinOnLowerIntelligence || troop.InevitableStratagemOnLowerIntelligence;

        if (troop.GetCurrentStratagemSuccess(enemy, inevitableSuccess, enemy.InvincibleGongxin, enemy.InvincibleStratagemFromLowerIntelligence))
        {
            troop.ApplyGongxin(enemy, baseDecrement);
        }

        foreach (Troop troop2 in troop.AreaStratagemTroops)
        {
            if (troop.GetCurrentStratagemSuccess(troop2, inevitableSuccess, troop2.InvincibleGongxin, troop2.InvincibleStratagemFromLowerIntelligence))
            {
                troop.ApplyGongxin(troop2, baseDecrement);
            }
        }
    }

    public override int GetCredit(Influence influence, Troop source, Troop destination)
    {
        int sum = 0;
        int pureFightingForce = source.PureFightingForce;
        var inevitableSuccess = source.InevitableGongxinOnLowerIntelligence || source.InevitableStratagemOnLowerIntelligence;

        foreach (Troop troop in source.GetAreaStratagemTroops(destination, false))
        {
            int credit = source.GetStratagemSuccessChanceCredit(troop, inevitableSuccess, troop.InvincibleGongxin, troop.InvincibleStratagemFromLowerIntelligence);
            credit -= (destination.MoraleIncreaseByViewArea + destination.MoraleIncreaseInViewArea + destination.IncrementPerDayOfMorale - destination.MoraleDecreaseByViewArea - destination.MoraleDecreaseInViewArea) * 50;

            if (credit > 0)
            {
                credit = (credit + (120 - troop.Morale) / 2 + (troop.Army.Scales - 5) * 5) * troop.PureFightingForce / pureFightingForce;
                sum += credit;
            }
        }

        return (int)(sum * source.RateOfGongxin);
    }
}
using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind390 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Troop troop)
    {
        // 计略影响只在施计时有效，被特技等其他途径触发时没有当前计略/目标，直接跳过
        if (troop.CurrentStratagem == null || troop.OrientationTroop == null) return;

        var baseDecrement = influence.GetIntParam();

        var enemy = troop.OrientationTroop;
        var inevitableSuccess = troop.InevitableGongxinOnLowerIntelligence || troop.InevitableStratagemOnLowerIntelligence;

        if (troop.GetCurrentStratagemSuccess(enemy, inevitableSuccess, enemy.InvincibleGongxin, enemy.InvincibleStratagemFromLowerIntelligence))
        {
            troop.ApplyGongxin(enemy, baseDecrement);
        }

        foreach (var troop2 in troop.AreaStratagemTroops)
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

        foreach (var troop in source.GetAreaStratagemTroops(destination))
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
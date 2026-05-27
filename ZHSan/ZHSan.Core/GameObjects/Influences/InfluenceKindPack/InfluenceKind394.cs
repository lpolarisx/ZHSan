using GameManager;
using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind394 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Troop troop)
    {
        var scale = influence.GetIntParam();

        var enemy = troop.OrientationTroop;
        var inevitableSuccess = troop.InevitableHuogongOnLowerIntelligence || troop.InevitableStratagemOnLowerIntelligence;

        if (troop.GetCurrentStratagemSuccess(enemy, inevitableSuccess, enemy.InvincibleHuogong, enemy.InvincibleStratagemFromLowerIntelligence))
        {
            var terrain = Session.Current.Scenario.GetTerrainDetailByPositionNoCheck(enemy.Position);
            troop.OrientationTroop.SetOnFire(troop.GenerateFireDamageScale(scale, terrain));
        }

        foreach (Troop troop2 in troop.AreaStratagemTroops)
        {
            if (troop.GetCurrentStratagemSuccess(troop2, inevitableSuccess, troop2.InvincibleHuogong, troop2.InvincibleStratagemFromLowerIntelligence))
            {
                var terrain = Session.Current.Scenario.GetTerrainDetailByPositionNoCheck(troop2.Position);
                troop2.SetOnFire(troop.GenerateFireDamageScale(scale, terrain));
            }
        }
    }

    public override int GetCredit(Influence influence, Troop source, Troop destination)
    {
        if (!IsVaild(influence, destination)) return 0;
       
        var sum = 0;
        int pureFightingForce = source.PureFightingForce;

        var inevitableSuccess = source.InevitableHuogongOnLowerIntelligence || source.InevitableStratagemOnLowerIntelligence;

        foreach (Troop troop in source.GetAreaStratagemTroops(destination, false))
        {
            int num3 = source.GetStratagemSuccessChanceCredit(troop, inevitableSuccess, troop.InvincibleHuogong, troop.InvincibleStratagemFromLowerIntelligence);
            if (num3 > 0)
            {
                num3 = (int)((num3 + (5 - troop.Army.Scales) * 5) * (troop.FireDamageRate - 1) * troop.PureFightingForce) / pureFightingForce;
                if (troop.RateOfFireProtection > 0f)
                {
                    sum += (int)(num3 * troop.RateOfFireProtection);
                }
                else
                {
                    sum += num3;
                }
            }
        }
        return (int)(sum * source.RateOfFireDamage);
    }

    public override bool IsVaild(Influence influence, Troop troop)
    {
        return Session.Current.Scenario.IsFireVaild(troop.Position, true, troop.Army.Kind.Type);
    }
}
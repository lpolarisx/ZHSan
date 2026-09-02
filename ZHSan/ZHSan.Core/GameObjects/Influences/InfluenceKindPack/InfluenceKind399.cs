using System;
using System.Runtime.Serialization;
using GameEnums;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind399 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Troop troop)
    {
        var rate = influence.GetFloatParam();

        var friendly = troop.OrientationTroop;

        if (troop.GetCurrentStratagemSuccess(friendly, false, false, false))
        {
            friendly.PreAction = TroopPreAction.Recover;
            var recoverQuantity = (int)(rate * friendly.Army.Kind.MinScale);

            recoverQuantity = Math.Min(recoverQuantity, friendly.InjuryQuantity);

            recoverQuantity = friendly.IncreaseQuantity(recoverQuantity);
            friendly.InjuryQuantity -= recoverQuantity;
        }

        foreach (var troop2 in troop.AreaStratagemTroops)
        {
            if (troop.GetCurrentStratagemSuccess(troop2, false, false, false))
            {
                troop2.PreAction = TroopPreAction.Recover;
                var recoverQuantity = (int)(rate * troop2.Army.Kind.MinScale);
                
                recoverQuantity = Math.Min(recoverQuantity, troop2.InjuryQuantity);

                recoverQuantity = troop2.IncreaseQuantity(recoverQuantity);
                if (troop2.OrientationTroop != null)
                {
                    troop2.OrientationTroop.InjuryQuantity -= recoverQuantity;
                }
            }
        }
    }

    public override int GetCredit(Influence influence, Troop source, Troop destination)
    {
        if (!IsVaild(influence, destination)) return 0;

        var baseRate = influence.GetFloatParam();

        var sum = 0;
        int fightingForce = source.FightingForce;
        foreach (var troop in source.GetAreaStratagemTroops(destination, true))
        {
            var rate = (float)troop.InjuryQuantity / troop.Army.Kind.MinScale;
            int num4 = source.GetStratagemSuccessChanceCredit(troop, false, false, false);
            if (num4 > 0)
            {
                rate = Math.Min(rate, baseRate);

                sum += (int)(num4 * rate / baseRate * troop.FightingForce / fightingForce);
            }
        }
        return sum;
    }

    public override bool IsVaild(Influence influence, Troop troop)
    {
        return troop.InjuryQuantity > 0;
    }
}
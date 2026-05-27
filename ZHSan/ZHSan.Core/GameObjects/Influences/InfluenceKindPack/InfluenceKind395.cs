using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind395 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Troop troop)
    {
        if (troop.GetCurrentStratagemSuccess(troop.OrientationTroop, false, false, false))
        {
            troop.OrientationTroop.SetRecoverFromChaos();
        }
        
        foreach (Troop troop2 in troop.AreaStratagemTroops)
        {
            if (troop.GetCurrentStratagemSuccess(troop2, false, false, false))
            {
                troop2.SetRecoverFromChaos();
            }
        }
    }

    public override int GetCredit(Influence influence, Troop source, Troop destination)
    {
        if (!IsVaild(influence, destination)) return 0;

        var sum = 0;
        int pureFightingForce = source.PureFightingForce;
        foreach (Troop troop in source.GetAreaStratagemTroops(destination, true))
        {
            int num3 = source.GetStratagemSuccessChanceCredit(troop, false, false, false);
            if (num3 > 0)
            {
                num3 = num3 * troop.FightingForce / pureFightingForce;
                sum += num3;
            }
        }
        return sum * 2;
    }

    public override bool IsVaild(Influence influence, Troop troop)
    {
        return troop.Status == TroopStatus.混乱 || troop.Status == TroopStatus.挑衅 || troop.Status == TroopStatus.伪报;
    }
}
using Microsoft.Xna.Framework;
using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind393 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Troop troop)
    {
        troop.SetAmbush();
    }

    public override int GetCreditWithPosition(Troop source, out Point? position)
    {
        position = new Point(0, 0);

        int num = 0;
        if (!source.IsInHostileTroopView() && !source.IsInHostileArchitectureHighView())
        {
            num += 50 * source.HostileTroopInViewFightingForce / source.PureFightingForce;
            if (source.OnlyBeDetectedByHighLevelInformation)
            {
                num *= 3;
            }

            if (num > 0)
            {
                position = new Point?(source.Position);
            }
        }
        return num;
    }

    public override bool IsVaild(Influence influence, Troop troop)
    {
        return troop.AmbushAvail(influence.GetIntParam());
    }
}
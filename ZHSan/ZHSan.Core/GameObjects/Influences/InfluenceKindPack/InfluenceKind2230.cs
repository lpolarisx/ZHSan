using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind2230 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Faction faction)
    {
        var type = influence.GetIntParam();

        switch (type)
        {
            case 0:
                faction.AllowAttackAfterMoveOfBubing = true;
                break;
            case 1:
                faction.AllowAttackAfterMoveOfNubing = true;
                break;
            case 2:
                faction.AllowAttackAfterMoveOfQibing = true;
                break;
            case 3:
                faction.AllowAttackAfterMoveOfShuijun = true;
                break;
            case 4:
                faction.AllowAttackAfterMoveOfQixie = true;
                break;
        }
    }

    public override void PurifyInfluenceKind(Influence influence, Faction faction)
    {
        var type = influence.GetIntParam();

        switch (type)
        {
            case 0:
                faction.AllowAttackAfterMoveOfBubing = false;
                break;
            case 1:
                faction.AllowAttackAfterMoveOfNubing = false;
                break;
            case 2:
                faction.AllowAttackAfterMoveOfQibing = false;
                break;
            case 3:
                faction.AllowAttackAfterMoveOfShuijun = false;
                break;
            case 4:
                faction.AllowAttackAfterMoveOfQixie = false;
                break;
        }
    }
}
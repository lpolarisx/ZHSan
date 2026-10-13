using GameDatas;
using System.Runtime.Serialization;

namespace GameObjects.TroopDetail;

[DataContract]
public class CastTargetKind : GameObject
{
    public CastTargetKind(CastTargetKindConfig config)
    {
        ID = config.Id;
        Name = config.Name;
    }
}
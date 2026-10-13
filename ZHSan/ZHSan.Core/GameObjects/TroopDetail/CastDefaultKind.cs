using System.Runtime.Serialization;
using GameDatas;

namespace GameObjects.TroopDetail;

[DataContract]
public class CastDefaultKind : GameObject
{
    public CastDefaultKind(CastDefaultKindConfig config)
    {
        ID = config.Id;
        Name = config.Name;
    }
}
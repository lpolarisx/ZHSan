using System.Runtime.Serialization;
using GameDatas;

namespace GameObjects.TroopDetail;

[DataContract]
public class AttackTargetKind : GameObject
{
    public AttackTargetKind(AttackDefaultKindConfig config)
    {
        ID = config.Id;
        Name = config.Name;
    }
}
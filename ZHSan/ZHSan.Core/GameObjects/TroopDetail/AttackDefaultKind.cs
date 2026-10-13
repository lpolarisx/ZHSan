using GameDatas;

namespace GameObjects.TroopDetail;

public class AttackDefaultKind : GameObject
{
   public AttackDefaultKind(AttackDefaultKindConfig config)
    {
        ID = config.Id;
        Name = config.Name;
    }
}
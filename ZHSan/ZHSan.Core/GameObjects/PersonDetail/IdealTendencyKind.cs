using GameDatas;

namespace GameObjects.PersonDetail;

public class IdealTendencyKind : GameObject
{
    public int Offset { get; set; }

    public IdealTendencyKind(IdealTendencyKindConfig config)
    {
        ID = config.Id;
        Name = config.Name;
        Offset = config.Offset;
    }

    public override string ToString() => $"{base.Name} {Offset}";
}
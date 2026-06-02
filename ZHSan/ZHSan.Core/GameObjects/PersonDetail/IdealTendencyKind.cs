using System.Runtime.Serialization;

namespace GameObjects.PersonDetail;

[DataContract]
public class IdealTendencyKind : GameObject
{
    public override string ToString() => $"{Name} {Offset}";
    
    [DataMember]
    public int Offset { get; set; }
}
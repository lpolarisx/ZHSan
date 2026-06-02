using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GameObjects.PersonDetail;

[DataContract]
public class BiographyAdjectives : GameObject
{
    [DataMember]
    public int Strength { get; set; }

    [DataMember]
    public int Command { get; set; }

    [DataMember]
    public int Intelligence { get; set; }

    [DataMember]
    public int Politics { get; set; }

    [DataMember]
    public int Glamour { get; set; }

    [DataMember]
    public int Braveness { get; set; }

    [DataMember]
    public int Calmness { get; set; }

    [DataMember]
    public int PersonalLoyalty { get; set; }

    [DataMember]
    public int Ambition { get; set; }

    [DataMember]
    public bool Male { get; set; }

    [DataMember]
    public bool Female { get; set; }

    [DataMember]
    public List<string> Text { get; set; } = new();

    [DataMember]
    public List<string> SuffixText { get; set; } = new();
}
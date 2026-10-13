
using System.Runtime.Serialization;

namespace GameObjects;

[DataContract]
public class PersonIdDialog
{
    [DataMember]
    public int id { get; set; }

    [DataMember]
    public string dialog { get; set; }

    [DataMember]
    public string yesdialog { get; set; }
    
    [DataMember]
    public string nodialog { get; set; }
}
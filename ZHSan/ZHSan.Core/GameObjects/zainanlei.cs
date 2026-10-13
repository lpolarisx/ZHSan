using System.Runtime.Serialization;

namespace GameObjects
{
    [DataContract]
    public class zainanlei : GameObject
	{
        [DataMember]
        public int zainanleixing { get; set; }

        public DisasterKind DisasterKind { get; set; } = new();

        [DataMember]
        public int shengyutianshu { get; set; }

        public string SavezainantoString() => $"{zainanleixing} {shengyutianshu} ";
    }
}

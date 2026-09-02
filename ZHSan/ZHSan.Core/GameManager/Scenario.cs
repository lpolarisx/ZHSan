using System.Runtime.Serialization;
using Tools;

namespace GameManager
{
    [DataContract]
    public class Scenario
    {
        [DataMember]
        public string ID { get; set; }
        
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Path { get; set; }

        [DataMember]
        public string Time { get; set; }

        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public string Create { get; set; }

        [DataMember]
        public string Info { get; set; }

        [DataMember]
        public string First { get; set; }

        [DataMember]
        public string Desc { get; set; }

        [DataMember]
        public string IDs { get; set; }

        [DataMember]
        public string Names { get; set; }

        [DataMember]
        public string Players { get; set; }

        [DataMember]
        public string Player { get; set; }

        [DataMember]
        public string PlayTime { get; set; }

        [DataMember]
        public string LeaderPics { get; set; }

        [DataMember]
        public string LeaderNames { get; set; }

        [DataMember]
        public string Reputations { get; set; }

        [DataMember]
        public string ArchitectureCounts { get; set; }

        [DataMember]
        public string CapitalNames { get; set; }

        [DataMember]
        public string Populations { get; set; }

        [DataMember]
        public string MilitaryCounts { get; set; }

        [DataMember]
        public string Funds { get; set; }

        [DataMember]
        public string Foods { get; set; }

        [DataMember]
        public string Mod { get; set; }

        public string GameTime
        {
            get
            {
                if (int.TryParse(PlayTime, out var playTime))
                {
                    return (playTime / 60 / 60) + ":" + (playTime / 60 % 60);
                }
                return "";
            }
        }

        public string Summary
        {
            get
            {
                string prefix = $"存档{ID}:    ";
                string autoSaveTag = ID == "0" ? "(自动保存) " : "";

                if (string.IsNullOrEmpty(Title))
                {
                    return autoSaveTag + prefix + "空 白 存 档";
                }

                string[] parts = { prefix, Info, Title, Mod, Time.ToSeasonDate(), Create.ToSeasonShortTime(), $"({GameTime})" };
                return autoSaveTag + string.Join("  ", parts);
            }
        }
    }
}

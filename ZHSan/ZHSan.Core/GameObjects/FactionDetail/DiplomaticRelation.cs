using GameDatas;
using GameManager;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GameObjects.FactionDetail
{
    [DataContract]
    public class DiplomaticRelation : GameObject
    {
        [DataMember]
        public int RelationFaction1ID { get; set; }

        [DataMember]
        public int RelationFaction2ID { get; set; }

        [DataMember]
        public int Relation { get; set; }

        [DataMember]
        public int Truce { get; set; }

        public DiplomaticRelation(DiplomaticRelationConfig config)
        {
            RelationFaction1ID = config.RelationFaction1ID;
            RelationFaction2ID = config.RelationFaction2ID;
            Relation = config.Relation;
            Truce = config.Truce;
        }

        public DiplomaticRelationConfig ToConfig()
        {
            return new DiplomaticRelationConfig
            {
                RelationFaction1ID = RelationFaction1ID,
                RelationFaction2ID = RelationFaction2ID,
                Relation = Relation,
                Truce = Truce,
            };
        }

        public DiplomaticRelation(int faction1ID, int faction2ID, int relation)
        {
            RelationFaction1ID = faction1ID;
            RelationFaction2ID = faction2ID;
            Relation = relation;
        }

        public Faction GetDiplomaticFaction(int factionID)
        {
            if (factionID == RelationFaction1ID)
            {
                return RelationFaction2;
            }
            if (factionID == RelationFaction2ID)
            {
                return RelationFaction1;
            }
            return null;
        }

        public int GetTheOtherFactionID(int factionID)
        {
            if (factionID == RelationFaction1ID)
            {
                return RelationFaction2ID;
            }
            
            return RelationFaction1ID;
        }

        public Faction RelationFaction1 => Session.Current.Scenario.Factions.GetValueOrDefault(RelationFaction1ID);

        public string RelationFaction1String => RelationFaction1?.Name ?? "----";

        public Faction RelationFaction2 => Session.Current.Scenario.Factions.GetValueOrDefault(RelationFaction2ID);

        public string RelationFaction2String => RelationFaction2?.Name ?? "----";
    }
}
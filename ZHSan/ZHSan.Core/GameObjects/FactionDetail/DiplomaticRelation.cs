using GameDatas;
using GameManager;
using System;
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

        // 本月谈判未成的次数，每月初清零
        [DataMember]
        public int NegotiationCount { get; set; }

        // AI再次主动遣使提议之前还要等的月数
        [DataMember]
        public int ProposalCooldown { get; set; }

        // 解除同盟后还要等几个月才能再结盟
        [DataMember]
        public int AllianceCooldown { get; set; }

        /// <summary>
        /// 双方是否同盟：关系达到结盟阈值
        /// </summary>
        public bool IsAlliance => Relation >= Session.GlobalVariables.FriendlyDiplomacyThreshold;

        /// <summary>
        /// 关系下降。同盟期间最多降到结盟阈值，同盟只能由解除同盟来解除，不会因为一两件小事悄悄散伙
        /// </summary>
        public void Worsen(int amount)
        {
            int floor = IsAlliance ? Session.GlobalVariables.FriendlyDiplomacyThreshold : int.MinValue;
            Relation = Math.Max(floor, Relation - amount);
        }

        public DiplomaticRelation(DiplomaticRelationConfig config)
        {
            RelationFaction1ID = config.RelationFaction1ID;
            RelationFaction2ID = config.RelationFaction2ID;
            Relation = config.Relation;
            Truce = config.Truce;
            NegotiationCount = config.NegotiationCount;
            ProposalCooldown = config.ProposalCooldown;
            AllianceCooldown = config.AllianceCooldown;
        }

        public DiplomaticRelationConfig ToConfig()
        {
            return new DiplomaticRelationConfig
            {
                RelationFaction1ID = RelationFaction1ID,
                RelationFaction2ID = RelationFaction2ID,
                Relation = Relation,
                Truce = Truce,
                NegotiationCount = NegotiationCount,
                ProposalCooldown = ProposalCooldown,
                AllianceCooldown = AllianceCooldown,
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
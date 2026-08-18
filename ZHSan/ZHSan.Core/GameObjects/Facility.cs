using GameDatas;
using GameManager;
using GameObjects.ArchitectureDetail;
using GameObjects.Conditions;
using GameObjects.Influences;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GameObjects
{
    /// <summary>
    /// 设施
    /// </summary>
    public class Facility : GameObject
    {
        /// <summary>
        /// 种类ID
        /// </summary>
        public int KindID => facilityLevel.KindId;

        public int LevelId { get; set; }

        /// <summary>
        /// 耐久
        /// </summary>
        public int Endurance { get; private set; }

        public Facility(FacilityConfig config)
        {
            ID = config.Id;
            LevelId = config.LevelId;
            facilityLevel = Session.Current.Scenario.GameCommonData.AllFacilityKindLevels.GetValueOrDefault(config.LevelId);
            Endurance = config.Endurance;
        }

        public FacilityConfig ToConfig()
        {
            return new FacilityConfig
            {
                Id = ID,
                LevelId = LevelId,
                Endurance = Endurance,
            };
        }

        public Facility(int id, FacilityKindLevel level)
        {
            ID = id;
            Endurance = level.Endurance;
            facilityLevel = level;
        }

        private FacilityKindLevel facilityLevel;

        /// <summary>
        /// 耐久下降
        /// </summary>
        /// <param name="decrement"></param>
        public void DecreaseEndurance(int decrement)
        {
            Endurance -= decrement;
        }

        /// <summary>
        /// 耐久恢复
        /// </summary>
        /// <param name="extraInc"></param>
        public void RecoverEndurance(int extraInc)
        {
            // 耐久小于上限时才需要恢复
            if (Endurance < EnduranceCeiling)
            {
                int increase = EnduranceCeiling / facilityLevel.Days / 2 + extraInc;

                Endurance += Math.Max(1, increase);

                Endurance = Math.Max(Endurance, EnduranceCeiling);
            }
        }

        public void DoWork(Architecture architecture)
        {
            foreach (var influence in facilityLevel.Influences)
            {
                influence.DoWork(architecture);
            }
        }

        public int DaysText => facilityLevel.DaysText;

        public string Description => facilityLevel.Description;

        public int EnduranceCeiling => facilityLevel.Endurance;

        public List<Influence> Influences => facilityLevel.Influences;
        
        public int MaintenanceCost => facilityLevel.MaintenanceCost;

        public new string Name => facilityLevel.Name;

        public int PositionOccupied => facilityLevel.PositionOccupied;

        public int ArchitectureLimit => facilityLevel.ArchitectureLimit;

        public int FactionLimit => facilityLevel.FactionLimit;

        public bool IsDemolishable => facilityLevel.IsDemolishable;

        public Dictionary<Condition, float> AIBuildConditionWeight => facilityLevel.AIBuildConditionWeight;

        public int rongna => facilityLevel.ConcubineCapacity;

        public bool IsProfitable => facilityLevel.IsProfitable;

        public float AILevel => facilityLevel.AILevel;

        public float AiValue(Architecture architecture) => facilityLevel.AiValue(architecture);

        public int TechnologyNeeded => facilityLevel.TechnologyNeeded;

        public int PointCost => facilityLevel.PointCost;

        public int FundCost => facilityLevel.FundCost;

        public string ConditionString => facilityLevel.ConditionString;
    }

    public class FacilityFactory
    {
        public Facility Create(FacilityKindLevel facilityLevel)
        {
            var id = Session.Current.Scenario.Facilities.Keys.Max() + 1;

            var facility = new Facility(id, facilityLevel);

            return facility;
        }
    }
}
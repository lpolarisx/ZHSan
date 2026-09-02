using GameDatas;
using GameEnums;
using GameManager;
using GameObjects.ArchitectureDetail;
using GameObjects.SectionDetail;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace GameObjects;

public class Section : GameObject
{
    public string ArchitecturesString { get; set; }

    public List<Architecture> Architectures { get; set; } = new();

    public Faction BelongedFaction { get; set; }

    public Faction OrientationFaction;

    public int OrientationFactionID { get; set; }

    public Section OrientationSection;

    public int OrientationSectionID { get; set; }

    public State OrientationState;

    public int OrientationStateID { get; set; }

    public Architecture OrientationArchitecture;

    public int OrientationArchitectureID { get; set; }

    public Section() {}

    public Section(SectionConfig config)
    {
        ID = config.Id;
        Name = config.Name;
        AIDetailIDString = config.AIDetailIDString;
        OrientationFactionID = config.OrientationFactionID;
        OrientationSectionID = config.OrientationSectionID;
        OrientationStateID = config.OrientationStateID;
        OrientationArchitectureID = config.OrientationArchitectureID;
        ArchitecturesString = config.ArchitecturesString;
    }

    public SectionConfig ToConfig()
    {
        return new SectionConfig
        {
            Id = ID,
            Name = Name,
            AIDetailIDString = AIDetailIDString,
            OrientationFactionID = OrientationFactionID,
            OrientationSectionID = OrientationSectionID,
            OrientationStateID = OrientationStateID,
            OrientationArchitectureID = OrientationArchitectureID,
            ArchitecturesString = ArchitecturesString,
        };
    }

    public void EnsureSectionArchitecture()
    {
        foreach (var architecture in Session.Current.Scenario.Architectures.Values)
        {
            if (architecture.BelongedSection == this && !Architectures.Contains(architecture))
            {
                Architectures.Add(architecture);
            }
        }
    }

    public void AddArchitecture(Architecture architecture)
    {
        if (!Architectures.Contains(architecture))
        {
            architecture.BelongedSection = this;
            Architectures.Add(architecture);
        }
    }

    public void AI()
    {
    }

    public void AIIntraTransfer()
    {
        if (Architectures.Count > 1)
        {
            BelongedFaction.AITransferPlanning(Architectures);
        }
    }

    public void AIInterTransfer()
    {
        if (OrientationSection == null) return;

        var allowFoodOrFundTransfer = AIDetail.AllowFoodTransfer || AIDetail.AllowFundTransfer;
        var allowMilitaryTransfer = AIDetail.AllowMilitaryTransfer;

        if (allowFoodOrFundTransfer || allowMilitaryTransfer)
        {
            BelongedFaction.AllocationTransfer(Architectures, OrientationSection.Architectures, allowFoodOrFundTransfer, false, allowMilitaryTransfer);

            if (GameObject.GetChance(10))
            {
                BelongedFaction.FullTransfer(Architectures, OrientationSection.Architectures, allowFoodOrFundTransfer, false, allowMilitaryTransfer);
            }
        }
    }

    public int GetFrontScale()
    {
        if (ArchitectureCount == 0) return 0;

        int num = 0;
        foreach (var architecture in Architectures)
        {
            if (architecture.FrontLine)
            {
                num++;
            }
        }

        return num / ArchitectureCount * 100;
    }

    public int GetHostileScale()
    {
        if (ArchitectureCount == 0) return 0;

        int num = 0;
        foreach (var architecture in Architectures)
        {
            if (architecture.HostileLine)
            {
                num++;
            }
        }

        return num / ArchitectureCount * 100;
    }

    public List<Architecture> GetOtherArchitectureList(Architecture architecture)
    {
        return Architectures.Where(x => x != architecture).ToList();
    }

    public bool HasArchitecture(Architecture architecture)
    {
        return Architectures.Contains(architecture);
    }

    public void RefreshSectionName()
    {
        var architecture = MaxPopulationArchitecture;

        Name = architecture != null ? $"{architecture.Name}军区" : "----";
    }

    public void RemoveArchitecture(Architecture architecture)
    {
        Architectures.Remove(architecture);
        architecture.BelongedSection = null;
    }

    [DataMember]
    public int AIDetailIDString { get; set; }

    public SectionAIDetail AIDetail { get; set; }

    public string AIDetailString => AIDetail?.Name ?? "————";

    public int ArchitectureCount => Architectures.Count;

    public int ArchitectureScale => Architectures.Sum(x => x.AreaCount);

    public int Army
    {
        get
        {
            int num = 0;
            foreach (var architecture in Architectures)
            {
                num += architecture.ArmyQuantity;
            }

            foreach (Troop troop in BelongedFaction.Troops)
            {
                if (!troop.Destroyed && HasArchitecture(troop.StartingArchitecture))
                {
                    num += troop.Quantity;
                }
            }

            return num;
        }
    }

    public int ArmyScale
    {
        get
        {
            int num = 0;
            foreach (var architecture in Architectures)
            {
                num += architecture.ArmyScale;
            }

            foreach (Troop troop in BelongedFaction.Troops)
            {
                if (!troop.Destroyed && HasArchitecture(troop.StartingArchitecture))
                {
                    num += troop.Army.Scales;
                }
            }

            return num;
        }
    }

    public string FactionString => BelongedFaction.Name;

    public int Food => Architectures.Sum(x => x.Food);

    public int Fund => Architectures.Sum(x => x.Fund);

    public Architecture MaxPopulationArchitecture
    {
        get
        {
            var architecture = Architectures.OrderByDescending(x => x.Population).FirstOrDefault();

            return architecture;
        }
    }

    public int MilitaryCount => Architectures.Sum(x => x.MilitaryCount);

    public string OrientationString
    {
        get
        {
            var defaultStr = "----";

            if (AIDetail != null)
            {
                switch (AIDetail.OrientationKind)
                {
                    case SectionOrientationKind.None:
                        return defaultStr;

                    case SectionOrientationKind.Section:
                        return OrientationSection?.Name ?? defaultStr;

                    case SectionOrientationKind.Faction:
                        return OrientationFaction?.Name ?? defaultStr;

                    case SectionOrientationKind.State:
                        return OrientationState?.Name ?? defaultStr;

                    case SectionOrientationKind.Architecture:
                        return OrientationArchitecture?.Name ?? defaultStr;
                }
            }

            return defaultStr;
        }
    }

    public List<Person> Persons
    {
        get
        {
            var result = new List<Person>();
            foreach (var architecture in Architectures)
            {
                foreach (Person person in architecture.Persons)
                {
                    result.Add(person);
                }
            }

            foreach (Troop troop in BelongedFaction.Troops)
            {
                if (troop.StartingArchitecture.BelongedSection == this)
                {
                    foreach (Person person in troop.Persons)
                    {
                        result.Add(person);
                    }
                }
            }
            return result;
        }
    }

    public int PersonCount
    {
        get
        {
            int num = 0;
            foreach (var architecture in Architectures)
            {
                num += architecture.GetAllPersons().Count;
            }

            foreach (Troop troop in BelongedFaction.Troops)
            {
                if (troop.StartingArchitecture.BelongedSection == this)
                {
                    num += troop.PersonCount;
                }
            }
            return num;
        }
    }

    public int Population => Architectures.Sum(x => x.Population);

    public int TroopCount
    {
        get
        {
            int num = 0;
            foreach (Troop troop in BelongedFaction.Troops)
            {
                if (!troop.Destroyed && troop.StartingArchitecture.BelongedSection == this)
                {
                    num++;
                }
            }
            return num;
        }
    }
}
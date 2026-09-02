using System.Collections.Generic;
using GameDatas;
using GameGlobal;

namespace GameObjects.ArchitectureDetail;

public class State : GameObject
{
    public List<Architecture> Architectures { get; set; } = new();

    public List<State> ContactStates { get; set; } = new();

    public string ContactStatesString { get; set; }

    public Region LinkedRegion;

    public Architecture StateAdmin;

    public int StateAdminID { get; set; }

    public State(StateConfig config)
    {
        ID = config.Id;
        Name = config.Name;
        StateAdminID = config.StateAdminID;
        ContactStatesString = config.ContactStatesString;
    }

    public StateConfig ToConfig()
    {
        return new StateConfig
        {
            Id = ID,
            Name = Name,
            StateAdminID = StateAdminID,
            ContactStatesString = ContactStatesString,
        };
    }

    public int GetFactionScale(Faction faction)
    {
        var architectureCount = Architectures.Count;

        if (architectureCount == 0) return 0;

        int num = 0;
        foreach (var architecture in Architectures)
        {
            if (architecture.BelongedFaction == null || faction == architecture.BelongedFaction)
            {
                num++;
            }
        }
        return num * 100 / architectureCount;
    }

    public int GetSectionScale(Section section)
    {
        var sectionCount = section.ArchitectureCount;

        if (Architectures.Count == 0 || sectionCount <= 0) return 0;

        int num = 0;
        foreach (var architecture in Architectures)
        {
            if (architecture.BelongedSection == section)
            {
                num++;
            }

            if (num >= sectionCount)
            {
                return 100;
            }
        }

        return num * 100 / sectionCount;
    }
    public override string ToString() => $"{Name} {LinkedRegionString}";

    public string ContactStatesDisplayString => StaticMethods.SaveNameToString(ContactStates);

    public string LinkedRegionString => LinkedRegion?.Name ?? "----";

    public string StateAdminString => StateAdmin?.Name ?? "----";
}
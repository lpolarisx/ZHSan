using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GameObjects.ArchitectureDetail;

[DataContract]
public class State : GameObject
{
    public ArchitectureList Architectures = new ArchitectureList();

    public StateList ContactStates = new StateList();

    [DataMember]
    public string ContactStatesString;

    public Region LinkedRegion;

    public Architecture StateAdmin;

    [DataMember]
    public int StateAdminID;

    public void Init()
    {
        Architectures = new ArchitectureList();

        ContactStates = new StateList();
    }

    public int GetFactionScale(Faction faction)
    {
        var count = Architectures.Count;
        if (count <= 0) return 0;

        int num = 0;
        foreach (Architecture architecture in Architectures)
        {
            if (architecture.BelongedFaction == null || faction == architecture.BelongedFaction)
            {
                num++;
            }
        }

        return num * 100 / count;
    }

    public int GetSectionScale(Section section)
    {
        var count = section.ArchitectureCount; 
        if (Architectures.Count <= 0 || count <= 0) return 0;

        int num = 0;
        foreach (Architecture architecture in Architectures)
        {
            if (architecture.BelongedSection == section)
            {
                num++;
            }

            if (num >= count)
            {
                return 100;
            }
        }
        return num * 100 / count;
    }

    public List<string> LoadContactStatesFromString(StateList contactStates, string dataString)
    {
        List<string> errorMsg = new List<string>();
        char[] separator = new char[] { ' ', '\n', '\r', '\t' };
        string[] strArray = dataString.Split(separator, StringSplitOptions.RemoveEmptyEntries);
        this.ContactStates.Clear();
        try
        {
            foreach (string str in strArray)
            {
                State gameObject = contactStates.GetGameObject(int.Parse(str)) as State;
                if (gameObject != null)
                {
                    this.ContactStates.Add(gameObject);
                }
                else
                {
                    errorMsg.Add("州域ID" + str + "不存在");
                }
            }
        }
        catch
        {
            errorMsg.Add("连接州域一栏应为半型空格分隔的州域ID");
        }
        return errorMsg;
    }

    public override string ToString() => $"{Name} {LinkedRegionString}";

    public string ContactStatesDisplayString
    {
        get
        {
            string str = "";
            foreach (State state in this.ContactStates)
            {
                str = str + state.Name + " ";
            }
            return str;
        }
    }

    public string LinkedRegionString => LinkedRegion?.Name ?? "----";

    public string StateAdminString => StateAdmin?.Name ?? "----";
}
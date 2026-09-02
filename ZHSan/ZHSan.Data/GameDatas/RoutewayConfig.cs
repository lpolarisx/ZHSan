
using System.Collections.Generic;

namespace GameDatas;

public class RoutewayConfig : BaseConfig
{
    public bool HasSupportedLegion { get; set; }

    public LinkedList<RoutePointConfig> RoutePoints { get; set; } = new();

    public int StartArchitectureString { get; set; }

    public int EndArchitectureString { get; set; }

    public int DestinationArchitectureString { get; set; }

    public int BelongedFactionString { get; set; }

    public bool Developing { get; set; }

    public bool AvoidWater { get; set; }

    public bool Building { get; set; }

    public int InefficiencyDays { get; set; }

    public int LastActivePointIndex { get; set; } = -1;

    public bool RemoveAfterClose { get; set; }

    public bool ShowArea { get; set; }
}
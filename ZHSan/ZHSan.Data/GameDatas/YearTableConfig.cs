
namespace GameDatas;

public class YearTableConfig
{
    public int Id { get; set; }

    public GameDateConfig Date { get; set; }

    public string Content { get; set; }

    public string FactionsString { get; set; }

    public bool IsGloballyKnown { get; set; }
}
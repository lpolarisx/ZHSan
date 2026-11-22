namespace GameObjects;

public class FundPack
{
    public int Days;
    public int Fund;
    public int SourceArchitectureID;

    public FundPack(int fund, int days, int sourceArchitectureId = -1)
    {
        Fund = fund;
        Days = days;
        SourceArchitectureID = sourceArchitectureId;
    }
}
namespace GameObjects;

public class FoodPack
{
    public int Days;
    public int Food;
    public int SourceArchitectureID;

    public FoodPack(int food, int days, int sourceArchitectureId = -1)
    {
        Food = food;
        Days = days;
        SourceArchitectureID = sourceArchitectureId;
    }
}
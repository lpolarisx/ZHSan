namespace GameObjects.PersonDetail;

public class PersonGenerateParam
{
    public Architecture FoundLocation { get; private set; }

    public Person Finder { get; private set; }

    public bool InGame { get; private set; }

    public PersonGeneratorType PreferredType { get; private set; }

    public bool IsAI { get; private set; }

    public PersonGenerateParam(Architecture foundLocation, Person finder, bool inGame, PersonGeneratorType preferredType, bool isAI)
    {
        FoundLocation = foundLocation;
        Finder = finder;
        InGame = inGame;
        PreferredType = preferredType;
        IsAI = isAI;
    }
}
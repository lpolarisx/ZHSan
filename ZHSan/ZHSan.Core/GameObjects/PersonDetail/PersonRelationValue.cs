namespace GameObjects.PersonDetail;

public class PersonRelationValue : GameObject
{
    public Person Source { get; private set; }
    public Person Person { get; private set; }
    public int RelationValue { get; private set; }

    public PersonRelationValue(Person source, Person person, int relationValue)
    {
        Source = source;
        Person = person;
        RelationValue = relationValue;
    }

    public override bool Equals(object obj)
    {
        if (obj is not PersonRelationValue) return false;

        PersonRelationValue other = (PersonRelationValue)obj;
        return Source.Equals(other.Source) && Person.Equals(other.Person);
    }

    public override int GetHashCode()
    {
        return Source.GetHashCode() * 31 + Person.GetHashCode();
    }

    public new string Name => Person.Name;

    public bool HasStrain => Source.HasStrainTo(Person);

    public bool HasCloseStrain => Source.HasCloseStrainTo(Person);

    public bool IsSpouse => Source.Spouse == Person;

    public bool IsBrother => Source.Brothers.GameObjects.Contains(Person);
    public bool IsClose => Source.Closes(Person);

    public bool IsHate => Source.Hates(Person);
}
namespace GameObjects.PersonDetail;

public class PersonRelationValue : GameObject
{
    public Person Source { get; set; }

    public Person Person { get; set; }
    
    public int RelationValue { get; set; }

    public new string Name => Person.Name;

    public bool HasStrain => Source.HasStrainTo(Person);

    public bool HasCloseStrain => Source.HasCloseStrainTo(Person);

    public bool IsSpouse => Source.Spouse == Person;

    public bool IsBrother => Source.Brothers.GameObjects.Contains(Person);

    public bool IsClose => Source.Closes(Person);

    public bool IsHate => Source.Hates(Person);

    public PersonRelationValue(Person source, Person person, int relationValue)
    {
        Source = source;
        Person = person;
        RelationValue = relationValue;
    }

    public override bool Equals(object obj)
    {
        if (!(obj is PersonRelationValue)) return false;
        PersonRelationValue other = (PersonRelationValue)obj;
        return this.Source.Equals(other.Source) && this.Person.Equals(other.Person);
    }

    public override int GetHashCode()
    {
        return this.Source.GetHashCode() * 31 + this.Person.GetHashCode();
    }
}

using System.Runtime.Serialization;
using GameDatas;

namespace GameObjects;

[DataContract]
public class PersonIDRelation
{
    [DataMember]
    public int PersonID1 { get; set; }

    [DataMember]
    public int PersonID2 { get; set; }

    [DataMember]
    public int Relation { get; set; }

    public PersonIDRelation() {}

    public PersonIDRelation(PersonRelationConfig config)
    {
        PersonID1 = config.PersonID1;
        PersonID2 = config.PersonID2;
        Relation = config.Relation;
    }

    public PersonRelationConfig ToConfig()
    {
        return new PersonRelationConfig
        {
            PersonID1 = PersonID1,
            PersonID2 = PersonID2,
            Relation = Relation,
        };
    }
}
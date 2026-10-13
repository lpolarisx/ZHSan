
namespace GameObjects.FactionDetail;

public class DiplomaticRelationDisplay : GameObject
{
    public string FactionName { get; private set; }

    public DiplomaticRelation LinkedDiplomaticRelation { get; private set; } 

    public DiplomaticRelationDisplay(DiplomaticRelation diplomaticRelation, string factionName)
    {
        LinkedDiplomaticRelation = diplomaticRelation;
        FactionName = factionName;
    }
    
    public int Relation => LinkedDiplomaticRelation.Relation;

    public int Truce => LinkedDiplomaticRelation.Truce;
}
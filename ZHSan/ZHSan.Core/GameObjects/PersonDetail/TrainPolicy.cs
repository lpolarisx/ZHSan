using System.Collections.Generic;
using GameDatas;

namespace GameObjects.PersonDetail;

/// <summary>
/// 培育方针
/// </summary>
public class TrainPolicy : GameObject
{
    public string Description { get; set; }

    public float Command { get; set; }

    public float Strength { get; set; }

    public float Intelligence { get; set; }

    public float Politics { get; set; }

    public float Glamour { get; set; }

    public float Skill { get; set; }

    public float Stunt { get; set; }

    public float Title { get; set; }

    public TrainPolicy(TrainPolicyConfig config)
    {
        ID = config.Id;
        Name = config.Name;
        Description = config.Description;
        Command = config.Command;
        Strength = config.Strength;
        Intelligence = config.Intelligence;
        Politics = config.Politics;
        Glamour = config.Glamour;
        Skill = config.Skill;
        Stunt = config.Stunt;
        Title = config.Title;
    }

    public Dictionary<int, float> Weighting
    {
        get
        {
            Dictionary<int, float> dict = new Dictionary<int, float>
            {
                { 1, this.Command },
                { 2, this.Strength },
                { 3, this.Intelligence },
                { 4, this.Politics },
                { 5, this.Glamour },
                { 6, this.Skill },
                { 7, this.Stunt },
                { 8, this.Title }
            };
            return dict;
        }
    }

    public float WeightSum => Command + Strength + Intelligence + Politics + Glamour + Skill + Stunt + Title;
}
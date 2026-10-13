namespace GameEvents;

/// <summary>
/// 人物-事件订阅器
/// </summary>
public class PersonSubscriber
{
    public PersonSubscriber()
    {
        EventManager.Instance.Subscribe<EntertainEvent>(OnEntertain);
    }

    private void OnEntertain(EntertainEvent e)
    {
        e.invitees.Add(e.inviter);

        foreach (var person in e.invitees)
        {
            person.Drink();
        }
    }
}
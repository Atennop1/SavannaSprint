using UnityEngine;

public class BonusesActivator : MonoBehaviour
{
    [SerializeField] private AudioSource _powerUpSource;
    [SerializeField] private BonusHandlersDatabase _database;

    public void ActivateBonus(Bonus bonus)
    {
        _powerUpSource.Play();
        foreach (var handler in _database.Handlers)
        {
            if (handler.Type != bonus.Type) 
                continue;
            
            handler.Init(bonus);
            (handler as TemporaryBonusHandler)?.SelfInit(1);
            handler.Activate();
        }
    }

    private void Start()
    {
        _powerUpSource.volume = 1 * MusicPlayer.Instance.Volume;
    }
}

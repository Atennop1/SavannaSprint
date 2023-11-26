using UnityEngine;

public class BonusObject : MonoBehaviour
{
    [SerializeField] private BonusType _type;

    private Bonus GetBonus()
    {
        return _type switch
        {
            BonusType.Shield => new ShieldBonus(_type),
            BonusType.Magnet => new MagnetBonus(_type),
            _ => new TemporaryBonus(_type)
        };
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out BonusesActivator activator)) 
            return;
        
        activator.ActivateBonus(GetBonus());
        gameObject.SetActive(false);
    }
}

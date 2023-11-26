using UnityEngine;

public abstract class BonusHandler : MonoBehaviour
{
    [field: SerializeField] public Player Player { get; private set; }
    [field: SerializeField] public BonusType Type { get; private set; }
    
    public Bonus Bonus { get; private set; }

    public void Init(Bonus bonus)
    {
        Bonus = bonus;
    }

    public void Activate()
    {
        Bonus.Init(Player);
        ConcreteBonusInit();
        StartCoroutine(Bonus.Activate());
    }

    protected virtual void ConcreteBonusInit() { }
}

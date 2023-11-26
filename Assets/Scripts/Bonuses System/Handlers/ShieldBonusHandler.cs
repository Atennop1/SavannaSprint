using UnityEngine;

public class ShieldBonusHandler : BonusHandler
{
    [Space]
    [SerializeField] private GameObject _shield;

    protected override void ConcreteBonusInit()
    {
        (Bonus as ShieldBonus)?.SelfInit(_shield);
    }
}

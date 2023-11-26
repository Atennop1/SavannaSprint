using UnityEngine;

public class MagnetBonusHandler : TemporaryBonusHandler
{
    [Space]
    [SerializeField] private GameObject _coinsDetector;
    [SerializeField] private ParticleSystem _bonusParticles;

    protected override void ConcreteBonusInit()
    {
        base.ConcreteBonusInit();
        (Bonus as MagnetBonus)?.SelfInit(_bonusParticles, _coinsDetector);
    }
}
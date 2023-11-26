using UnityEngine;
using System.Collections;

public class MagnetBonus : TemporaryBonus
{
    private GameObject _coinsDetector;
    private ParticleSystem _particles;

    public MagnetBonus(BonusType type) : base(type) { }

    public void SelfInit(ParticleSystem particles, GameObject coinDetector)
    {
        _coinsDetector = coinDetector;
        _particles = particles;
    }

    protected override IEnumerator TemplateActivate()
    {
        if (_particles != null)
            _particles.Play();

        _coinsDetector.SetActive(true);
        yield return Player.StartCoroutine(base.TemplateActivate());
    }

    protected override void TerminationCallback()
    {
        if (_particles != null)
            _particles.Stop();
            
        _coinsDetector.SetActive(false);
        base.TerminationCallback();
    }

    protected override void IncorrectTerminationCallback()
    {
        _particles?.Stop();
        _coinsDetector.SetActive(false);
        base.IncorrectTerminationCallback();
    }
}
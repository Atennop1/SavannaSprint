using System.Collections;
using UnityEngine;

public class PlayerAnimations : MonoCache, IPlayerPart
{
    [SerializeField] private Player _player;

    [field: SerializeField] public Animator PlayerAnimator { get; private set; }
    [SerializeField] private Animator _shieldAnimator;
    
    public IEnumerator Main()
    {
        PlayerAnimator.Play("Reborn");
        yield return new WaitForSeconds(1.5f);
        PlayerAnimator.Play("Run");

        if (_player.BonusHandlersDatabase.IsHandlerActive(BonusType.Shield))
            _shieldAnimator.gameObject.SetActive(true);
    }

    public IEnumerator Change()
    {
        if (_shieldAnimator.gameObject.activeInHierarchy)
            _shieldAnimator.SetTrigger("Crush");
        
        PlayerAnimator.Play("Lose");
        yield return new WaitForFixedUpdate();
    }

    public IEnumerator Lose()
    {
        PlayerAnimator.Play("Lose");
        yield return new WaitForFixedUpdate();
    }

    public IEnumerator Reborn(bool ad)
    {
        PlayerAnimator.Play("Reborn");
        yield return new WaitForSeconds(1.5f);
        PlayerAnimator.Play("Run");
    }

    public IEnumerator ShieldLose()
    {
        _shieldAnimator.SetTrigger("Crush");
        yield return new WaitForSeconds(0.7f);
        _shieldAnimator.gameObject.SetActive(false);
    }

    public void SetupSkin(Skin skin)
    {
        skin.SetupSkinAnimator(PlayerAnimator);
    }

    public void SetShieldAnimatorIsCtrl(bool key)
    {
        if (_shieldAnimator.gameObject.activeInHierarchy)
            _shieldAnimator.SetBool("IsCtrl", key);
    }
}

using System.Collections;
using UnityEngine;

public class PlayerAnimations : MonoCache
{
    [SerializeField] private PlayerController _player;

    [field: SerializeField] public Animator PlayerAnimator { get; private set; }
    [field: SerializeField] public Animator ShieldAnimator { get; private set; }

    public override void OnTick()
    {
        if (_player.PlayerState != PlayerState.Death)
        {
            if (!_player.GameOver.isGameOver && (_player.PlayerState == PlayerState.Run || _player.PlayerState == PlayerState.Jump))
                if (ShieldAnimator.gameObject.activeInHierarchy)
                    ShieldAnimator.SetTrigger("isNotCtrl");

            if (_player.PlayerState == PlayerState.Ctrl)
                if (ShieldAnimator.gameObject.activeInHierarchy)
                    ShieldAnimator.SetTrigger("isCtrl");
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if ((collision.gameObject.tag == "Lose" || collision.gameObject.tag == "RampLose") && !_player.GameOver.isGameOver)
            if (!_player.GameManager.isShield)
                PlayerAnimator.Play("Lose");
    }
    
    public IEnumerator Change()
    {
        if (ShieldAnimator.gameObject.activeInHierarchy)
            ShieldAnimator.SetTrigger("crush");
        
        PlayerAnimator.Play("Lose");
        yield return new WaitForFixedUpdate();
    }

    public IEnumerator Lose()
    {
        PlayerAnimator.Play("Lose");
        yield return new WaitForFixedUpdate();
    }

    public IEnumerator Reborn()
    {
        PlayerAnimator.Play("Reborn");
        yield return new WaitForFixedUpdate();
    }

    public IEnumerator StartMethod()
    {
        PlayerAnimator.Play("Reborn");
        yield return new WaitForSeconds(1.5f);
        PlayerAnimator.Play("Run");

        if (_player.GameManager.isShield)
            ShieldAnimator.gameObject.SetActive(true);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimations2D : MonoCache
{
    [SerializeField] private PlayerController2D _player;

    [field: SerializeField] public Animator ShieldAnimator { get; private set; }
    [field: SerializeField] public Animator PlayerAnimator { get; private set; }
    
    public override void OnTick()
    {
        if (_player.PlayerState != PlayerState.Death && !_player.GameOver.isGameOver && Time.timeScale != 0)
        {
            if ((_player.PlayerState == PlayerState.Run || _player.PlayerState == PlayerState.Jump))
            {
                if (_player.PlayerState == PlayerState.Run)
                    PlayerAnimator.Play("Run");
                else
                    PlayerAnimator.Play("Jump");

                if (ShieldAnimator.gameObject.activeInHierarchy)
                    ShieldAnimator.SetTrigger("isNotCtrl");
            }

            if (_player.PlayerState == PlayerState.Ctrl)
            {
                PlayerAnimator.Play("Ctrl");
                if (ShieldAnimator.gameObject.activeInHierarchy)
                    ShieldAnimator.SetTrigger("isCtrl");
            }
        }
    }

    public IEnumerator StartMethod()
    {
        PlayerAnimator.Play("Reborn");

        yield return new WaitForSeconds(1.5f);

        PlayerAnimator.Play("Run");
        if (_player.GameManager.isShield)
            ShieldAnimator.gameObject.SetActive(true);
    }

    public IEnumerator Reborn()
    {
        PlayerAnimator.Play("Reborn");

        yield return new WaitForSeconds(1.6f);

        PlayerAnimator.Play("Run");
    }

    public IEnumerator Change()
    {
        if (ShieldAnimator.gameObject.activeInHierarchy)
            ShieldAnimator.SetTrigger("crush");

        PlayerAnimator.Play("Lose");

        yield return null;
    }
}

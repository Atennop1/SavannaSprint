using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimations2D : MonoCache
{
    public Animator shieldAnimator;
    public Animator playerAnimator;
    
    public override void OnTick()
    {
        if (PlayerController2D.playerState != PlayerState.Death && !GameOverScript.isGameOver && Time.timeScale != 0)
        {
            if ((PlayerController2D.playerState == PlayerState.Run || PlayerController2D.playerState == PlayerState.Jump))
            {
                if (PlayerController2D.playerState == PlayerState.Run)
                    playerAnimator.Play("Run");
                else
                    playerAnimator.Play("Jump");

                if (shieldAnimator.gameObject.activeInHierarchy)
                    shieldAnimator.SetTrigger("isNotCtrl");
            }

            if (PlayerController2D.playerState == PlayerState.Ctrl)
            {
                playerAnimator.Play("Ctrl");
                if (shieldAnimator.gameObject.activeInHierarchy)
                    shieldAnimator.SetTrigger("isCtrl");
            }
        }
    }

    public IEnumerator StartMethod()
    {
        playerAnimator.Play("Reborn");

        yield return new WaitForSeconds(1.5f);

        playerAnimator.Play("Run");
        if (GameManager.isShield)
            shieldAnimator.gameObject.SetActive(true);
    }

    public IEnumerator Reborn()
    {
        playerAnimator.Play("Reborn");

        yield return new WaitForSeconds(1.6f);

        playerAnimator.Play("Run");
    }

    public IEnumerator Change()
    {
        if (shieldAnimator.gameObject.activeInHierarchy)
            shieldAnimator.SetTrigger("crush");

        playerAnimator.Play("Lose");

        yield return null;
    }
}

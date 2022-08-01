using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimations : MonoCache
{
    public Animator playerAnimator;
    public Animator shieldAnimator;

    public override void OnTick()
    {
        if (PlayerController.playerState != PlayerState.Death)
        {
            if (!GameOverScript.isGameOver && (PlayerController.playerState == PlayerState.Run || PlayerController.playerState == PlayerState.Jump))
                if (shieldAnimator.gameObject.activeInHierarchy)
                    shieldAnimator.SetTrigger("isNotCtrl");

            if (PlayerController.playerState == PlayerState.Ctrl)
                if (shieldAnimator.gameObject.activeInHierarchy)
                    shieldAnimator.SetTrigger("isCtrl");
        }
    }
    public void OnCollisionEnter(Collision collision)
    {
        if ((collision.gameObject.tag == "Lose" || collision.gameObject.tag == "RampLose") && !GameOverScript.isGameOver)
        {
            if (!GameManager.isShield)
                playerAnimator.Play("Lose");
        }
    }
    public IEnumerator Change()
    {
        if (shieldAnimator.gameObject.activeInHierarchy)
            shieldAnimator.SetTrigger("crush");
        playerAnimator.Play("Lose");
        yield return new WaitForFixedUpdate();
    }

    public IEnumerator Lose()
    {
        playerAnimator.Play("Lose");
        yield return new WaitForFixedUpdate();
    }

    public IEnumerator Reborn()
    {
        playerAnimator.Play("Reborn");
        yield return new WaitForFixedUpdate();
    }

    public IEnumerator StartMethod()
    {
        playerAnimator.Play("Reborn");
        yield return new WaitForSeconds(1.5f);
        playerAnimator.Play("Run");

        if (GameManager.isShield)
            shieldAnimator.gameObject.SetActive(true);
    }
}

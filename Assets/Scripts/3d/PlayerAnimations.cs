using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimations : MonoCache
{
    [field: SerializeField] public Animator PlayerAnimator { get; private set; }
    [field: SerializeField] public Animator ShieldAnimator { get; private set; }

    public override void OnTick()
    {
        if (PlayerController.playerState != PlayerState.Death)
        {
            if (!GameOverScript.isGameOver && (PlayerController.playerState == PlayerState.Run || PlayerController.playerState == PlayerState.Jump))
                if (ShieldAnimator.gameObject.activeInHierarchy)
                    ShieldAnimator.SetTrigger("isNotCtrl");

            if (PlayerController.playerState == PlayerState.Ctrl)
                if (ShieldAnimator.gameObject.activeInHierarchy)
                    ShieldAnimator.SetTrigger("isCtrl");
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if ((collision.gameObject.tag == "Lose" || collision.gameObject.tag == "RampLose") && !GameOverScript.isGameOver)
        {
            if (!GameManager.isShield)
                PlayerAnimator.Play("Lose");
        }
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

        if (GameManager.isShield)
            ShieldAnimator.gameObject.SetActive(true);
    }
}

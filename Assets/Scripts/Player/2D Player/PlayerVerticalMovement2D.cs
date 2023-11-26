using UnityEngine;
using System.Collections;

public class PlayerVerticalMovement2D : PlayerVerticalMovement
{
    private bool _isJumping;

    protected override void OnTick()
    {
        if (Player.CurrentState == PlayerState.Death || Player.CurrentState == PlayerState.None ||
            Player.CurrentState == PlayerState.Changing || Player.GameOver.IsGameOver || Time.timeScale == 0) return;
        
        if (Player.Input.ActiveUp && Player.CurrentState == PlayerState.Run && !_isJumping)
        {
            PlayerSlowMotion.StopSlowMotion();
            StartActionCoroutine(JumpCoroutine());
            Player.CurrentState = PlayerState.Jump;
        }

        if (Player.CurrentState == PlayerState.Jump) return;
            
        Player.CurrentState = Player.Input.ActiveDown ? PlayerState.Ctrl : PlayerState.Run;
        PlayerAnimations.PlayerAnimator.Play(Player.Input.ActiveDown ? "Ctrl" : "Run");
        PlayerAnimations.SetShieldAnimatorIsCtrl(Player.Input.ActiveDown);
        SetActionColliders(!Player.Input.ActiveDown);

        if (Player.Input.ActiveDown)
            PlayerSlowMotion.StopSlowMotion();
    }

    private IEnumerator JumpCoroutine()
    {
        _isJumping = true;
        PlayJumpSource();
        ForwardMovement.RunDust.Stop();
        PlayerRigidbody.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);

        yield return null;

        Player.CurrentState = PlayerState.Jump;
        PlayerAnimations.PlayerAnimator.Play("Jump");
        PlayerAnimations.SetShieldAnimatorIsCtrl(false);

        var wait = new WaitForFixedUpdate();
        for (var i = 0; i < 60; i++)
        {
            if (Player.Input.ActiveUp && Player.CurrentState == PlayerState.Jump)
                PlayerRigidbody.AddForce(Vector3.up * JumpForce / 25, ForceMode.Impulse);
            else break;

            yield return wait;
        }
        
        _isJumping = false;
    }
}

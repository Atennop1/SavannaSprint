using System.Collections;
using UnityEngine;

public class PlayerVerticalMovement3D : PlayerVerticalMovement
{
    [SerializeField] private float _ctrlForce;

    protected override void OnTick()
    {
        if (Player.CurrentState == PlayerState.Death || Player.CurrentState == PlayerState.None ||
            Player.CurrentState == PlayerState.Changing) return;
        
        if (Player.Input.ActiveUp && !Player.GameOver.IsGameOver && Time.timeScale != 0 && (Player.CurrentState == PlayerState.Run || Player.CurrentState == PlayerState.Ctrl))
        {
            PlayerSlowMotion.StopSlowMotion();
            PlayerAnimations.SetShieldAnimatorIsCtrl(false);

            PlayerAnimations.PlayerAnimator.Play("Jump");
            PlayerRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            PlayerRigidbody.velocity = Vector3.up * JumpForce;

            PlayJumpSource();
            ForwardMovement.RunDust.Stop();
            Player.CurrentState = PlayerState.Jump;

            if (TryStopActionCoroutine())
                SetActionColliders(true);
        }
            
        if (Player.Input.ActiveDown && !Player.GameOver.IsGameOver && Time.timeScale != 0 && Player.CurrentState != PlayerState.Ramp)
        {
            PlayerSlowMotion.StopSlowMotion();
            StartActionCoroutine(Ctrl());
        }

        if (PlayerRigidbody.velocity.y > 1 && Player.CurrentState == PlayerState.Ramp)
            PlayerRigidbody.velocity = new Vector3(PlayerRigidbody.velocity.x, 1, PlayerRigidbody.velocity.z);
    }

    private IEnumerator Ctrl()
    {
        PlayerAnimations.SetShieldAnimatorIsCtrl(true);
        PlayerAnimations.PlayerAnimator.Play("Ctrl");
        
        Player.CurrentState = Player.CurrentState == PlayerState.Jump ? PlayerState.CtrlJump : PlayerState.Ctrl;
        PlayerRigidbody.AddForce(Vector3.down * _ctrlForce, ForceMode.Impulse);
        
        SetActionColliders(false);
        yield return new WaitForSeconds(1);
        SetActionColliders(true);

        Player.CurrentState = PlayerState.Run;
        PlayerAnimations.PlayerAnimator.Play("Run");
        PlayerAnimations.SetShieldAnimatorIsCtrl(false);
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);
        if (!collision.gameObject.TryGetComponent(out RampTrigger _) || !TryStopActionCoroutine()) 
            return;
        
        PlayerAnimations.PlayerAnimator.Play("Run");
        PlayerAnimations.SetShieldAnimatorIsCtrl(false);
    }
}

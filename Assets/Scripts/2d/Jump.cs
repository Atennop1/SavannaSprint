using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class Jump : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [field: SerializeField] public PlayerController2D Player { get; private set; }
    [SerializeField] private Rigidbody _playerRigidbody;

    [Space]
    [SerializeField] private float _jumpForce;
    [SerializeField] private AudioSource _jumpSound;

    public bool IsJump { get; private set; }
    public static bool CanDust { get; private set; }

    private Coroutine _jumpCoroutine;

    public void OnPointerDown(PointerEventData a)
    {
        if (PlayerController2D.playerState == PlayerState.Run && !GameOverScript.isGameOver && Time.timeScale != 0 && Player.canJump)
        {
            if (Obstacle.isShowing)
                Obstacle.StopSlowMotion();

            //ctrlCol.SetActive(false);
            //runCol.SetActive(true);

            _playerRigidbody.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
            _jumpSound.volume = 0.5f * SingletonManager.soundVolume;
            _jumpSound.Play();

            IsJump = true;
            CanDust = true;

            Player.PlayerMovementNonControlable.RunDust.Stop();
            PlayerController2D.playerState = PlayerState.Jump;
            Player.PlayerAnimations.PlayerAnimator.Play("Jump");

            if (_jumpCoroutine != null)
                StopCoroutine(_jumpCoroutine);
            _jumpCoroutine = StartCoroutine(JumpCoroutine());
        }
    }

    public void OnPointerUp(PointerEventData a)
    {
        IsJump = false;
    }

    private IEnumerator JumpCoroutine()
    {
        yield return new WaitForSeconds(0.1f);
        for (int i = 0; i < 79; i++)
        {
            yield return new WaitForFixedUpdate();
            if (IsJump)
            {
                _playerRigidbody.AddForce(Vector3.up * _jumpForce / 25, ForceMode.Impulse);
                CanDust = true;
            }
            else break;
        }
        IsJump = false;
    }
}

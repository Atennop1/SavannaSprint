using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Jump : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public PlayerController2D player;
    [SerializeField] private float jumpForce;

    public AudioSource jumpSound;

    public GameObject ctrlCol;
    public GameObject runCol;

    public static bool canDust = false;
    private Coroutine jumpCoroutine;

    [HideInInspector] public bool isJump;
    [HideInInspector] public Rigidbody playerRb;

    public void OnPointerDown(PointerEventData a)
    {
        if (PlayerController2D.playerState == PlayerState.Run && !GameOverScript.isGameOver && Time.timeScale != 0 && player.canJump)
        {
            if (Obstacle.isShowing)
                Obstacle.StopSlowMotion();

            ctrlCol.SetActive(false);
            runCol.SetActive(true);

            playerRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jumpSound.volume = 0.5f * SingletonManager.soundVolume;
            jumpSound.Play();

            isJump = true;
            canDust = true;

            player.playerMovementNonControlable.runDust.Stop();
            PlayerController2D.playerState = PlayerState.Jump;
            player.playerAnimations.playerAnimator.Play("Jump");

            if (jumpCoroutine != null)
                StopCoroutine(jumpCoroutine);
            jumpCoroutine = StartCoroutine(JumpCoroutine());
        }
    }
    public void OnPointerUp(PointerEventData a)
    {
        isJump = false;
    }
    IEnumerator JumpCoroutine()
    {
        yield return new WaitForSeconds(0.1f);
        for (int i = 0; i < 79; i++)
        {
            yield return new WaitForFixedUpdate();
            if (isJump)
            {
                playerRb.AddForce(Vector3.up * jumpForce / 25, ForceMode.Impulse);
                canDust = true;
            }
            else break;
        }
        isJump = false;
    }
}

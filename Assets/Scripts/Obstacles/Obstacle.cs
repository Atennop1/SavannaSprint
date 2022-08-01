using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Obstacle : MonoBehaviour
{
    [SerializeField] private string obstacleAnimName;
    [SerializeField] private Collider slowMotionCollider;

    [Space]
    [SerializeField] private bool canMoveRight;
    [SerializeField] private bool canMoveLeft;
    [SerializeField] private bool canMoveUp;
    [SerializeField] private bool canMoveDown;

    public static bool isShowing;
    public static Collider cantMoveCollider;

    private static Image image;
    private static PlayerController player;
    private static PlayerController2D player2d;
    private static float timer;

    [HideInInspector] public bool isGuided;

    public void Start()
    {
        isShowing = false;
        player2d = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController2D>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        isGuided = PlayerPrefs.HasKey("isGuided" + obstacleAnimName + (player == null ? "2D" : "3D"));

        if (SceneManager.GetActiveScene().name == "3d World")
            image = player.guideImage;
        else
            image = player2d.guideImage;

        StopSlowMotion();
    }
    public void SlowMotionStart()
    {
        if (!isGuided)
        {
            isShowing = true;
            StartCoroutine(SlowMotion());

            image.enabled = true;
            slowMotionCollider.enabled = false;

            Animator imageAnimator = image.GetComponent<Animator>();
            if (obstacleAnimName == "Wall")
            {
                if (transform.position.x == -3.3f)
                {
                    imageAnimator.Play("Right");
                    canMoveRight = true;
                }
                else if (transform.position.x == 3.3f)
                {
                    imageAnimator.Play("Left");
                    canMoveLeft = true;
                }
                else
                {
                    imageAnimator.Play("Wall");
                    canMoveLeft = true;
                    canMoveRight = true;
                }
            }
            else
                imageAnimator.Play(obstacleAnimName);

            if (player != null)
            {
                player.playerMovementControlable.canMoveDown = canMoveDown;
                player.playerMovementControlable.canMoveLeft = canMoveLeft;
                player.playerMovementControlable.canMoveRight = canMoveRight;
                player.playerMovementControlable.canMoveUp = canMoveUp;
            }
            else
            {
                player2d.canCtrl = canMoveDown;
                player2d.canJump = canMoveUp;
            }

            PlayerPrefs.SetInt("isGuided" + obstacleAnimName + (player == null ? "2D" : "3D"), 1);
            isGuided = true;
        }
    }
    private IEnumerator SlowMotion()
    {
        if (!isGuided)
        {
            timer = 0;
            float slowMotionTime;
            if (player != null)
                slowMotionTime = 0.25f / (PlayerMovementNonControlable.speed / 15);
            else
                slowMotionTime = 0.25f / (PlayerMovementNonControlable2D.speed / -10);

            while (timer < slowMotionTime)
            {
                timer += Time.deltaTime;
                Time.timeScale = Mathf.Lerp(1, 0.001f, timer * (1 / slowMotionTime));
                yield return new WaitForFixedUpdate();
            }
        }
    }
    public static void StopSlowMotion()
    {
        timer = 1;
        Time.timeScale = 1;
        isShowing = false;
        image.enabled = false;

        if (player != null)
        {
            player.playerMovementControlable.canMoveDown = true;
            player.playerMovementControlable.canMoveLeft = true;
            player.playerMovementControlable.canMoveRight = true;
            player.playerMovementControlable.canMoveUp = true;
        }
        else
        {
            player2d.canJump = true;
            player2d.canCtrl = true;
        }
    }
    public static void CantMove()
    {
        cantMoveCollider.enabled = false;
        if (player != null)
        {
            player.playerMovementControlable.canMoveDown = false;
            player.playerMovementControlable.canMoveLeft = false;
            player.playerMovementControlable.canMoveRight = false;
            player.playerMovementControlable.canMoveUp = false;
        }
        else
        {
            player2d.canJump = false;
            player2d.canCtrl = false;
            if (PlayerController2D.playerState == PlayerState.Ctrl)
            {
                player2d.ctrl.isClicked = false;
                PlayerController2D.playerState = PlayerState.Run;
            }
            player2d.jump.isJump = false;
        }
    }
}

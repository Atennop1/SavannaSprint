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
    private static PlayerController _player;
    private static PlayerController2D _player2d;
    private static float timer;

    [HideInInspector] public bool isGuided;

    public void Init(PlayerController player, PlayerController2D player2d)
    {
        _player = player;
        _player2d = player2d;
    }

    public void Start()
    {
        isShowing = false;
        isGuided = PlayerPrefs.HasKey("isGuided" + obstacleAnimName + (_player == null ? "2D" : "3D"));

        if (SceneManager.GetActiveScene().name == "3d World")
            image = _player.GuideImage;
        else
            image = _player2d.GuideImage;

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

            if (_player != null)
            {
                _player.PlayerMovementControlable.canMoveDown = canMoveDown;
                _player.PlayerMovementControlable.canMoveLeft = canMoveLeft;
                _player.PlayerMovementControlable.canMoveRight = canMoveRight;
                _player.PlayerMovementControlable.canMoveUp = canMoveUp;
            }
            else
            {
                _player2d.canCtrl = canMoveDown;
                _player2d.canJump = canMoveUp;
            }

            PlayerPrefs.SetInt("isGuided" + obstacleAnimName + (_player == null ? "2D" : "3D"), 1);
            isGuided = true;
        }
    }
    private IEnumerator SlowMotion()
    {
        if (!isGuided)
        {
            timer = 0;
            float slowMotionTime;
            if (_player != null)
                slowMotionTime = 0.25f / (_player.PlayerMovementNonControlable.Speed / 15);
            else
                slowMotionTime = 0.25f / (_player2d.PlayerMovementNonControlable.speed / -10);

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

        if (_player != null)
        {
            _player.PlayerMovementControlable.canMoveDown = true;
            _player.PlayerMovementControlable.canMoveLeft = true;
            _player.PlayerMovementControlable.canMoveRight = true;
            _player.PlayerMovementControlable.canMoveUp = true;
        }
        else
        {
            _player2d.canJump = true;
            _player2d.canCtrl = true;
        }
    }
    public static void CantMove()
    {
        cantMoveCollider.enabled = false;
        if (_player != null)
        {
            _player.PlayerMovementControlable.canMoveDown = false;
            _player.PlayerMovementControlable.canMoveLeft = false;
            _player.PlayerMovementControlable.canMoveRight = false;
            _player.PlayerMovementControlable.canMoveUp = false;
        }
        else
        {
            _player2d.canJump = false;
            _player2d.canCtrl = false;

            if (_player2d.PlayerState == PlayerState.Ctrl)
                _player2d.PlayerState = PlayerState.Run;

            //player2d.jump.isJump = false;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementControlable : MonoCache
{
    [SerializeField] private float jumpForce;
    [SerializeField] private float ctrlForce;

    [Space]
    [SerializeField] private GameObject ctrlCol;
    [SerializeField] private GameObject runCol;

    [Space]
    [SerializeField] private AudioSource jumpSource;

    [Space]
    public ParticleSystem jumpDust;
    public ParticleSystem runDust;



    private PlayerController player;
    private Rigidbody playerRigidbody;

    private float movementPointFinish;
    private float movementPointStart;
    private readonly float lineChangeSpeed = 30;
    private readonly float lineDistance = 3.3f;

    private Coroutine ctrlCoroutine;
    public Coroutine moveHorizontalCoroutine;

    [HideInInspector] public bool canMoveRight;
    [HideInInspector] public bool canMoveLeft;
    [HideInInspector] public bool canMoveUp;
    [HideInInspector] public bool canMoveDown;
    [HideInInspector] public bool canDust;

    public void Start()
    {
        player = PlayerController.instance;
        canMoveDown = canMoveLeft = canMoveRight = canMoveUp = true;
        playerRigidbody = GetComponent<Rigidbody>();
    }

    public override void OnTick()
    {
        if (PlayerController.playerState != PlayerState.Death && PlayerController.playerState != PlayerState.None && PlayerController.playerState != PlayerState.Changing)
        {
            if (SwipeController.swipeLeft && !GameOverScript.isGameOver && Time.timeScale != 0 && movementPointFinish > -lineDistance && canMoveLeft)
            {
                MoveHorizontal(-lineChangeSpeed);
                Obstacle.StopSlowMotion();
            }
            if (SwipeController.swipeRight && !GameOverScript.isGameOver && Time.timeScale != 0 && movementPointFinish < lineDistance && canMoveRight)
            {
                MoveHorizontal(lineChangeSpeed);
                Obstacle.StopSlowMotion();
            }

            if (SwipeController.swipeUp && !GameOverScript.isGameOver && Time.timeScale != 0 && canMoveUp)
            {
                if (PlayerController.playerState == PlayerState.Run || PlayerController.playerState == PlayerState.Ctrl)
                {
                    Obstacle.StopSlowMotion();
                    if (player.playerAnimations.shieldAnimator.gameObject.activeInHierarchy)
                        player.playerAnimations.shieldAnimator.SetTrigger("isNotCtrl");

                    if (moveHorizontalCoroutine != null)
                    {
                        StopCoroutine(moveHorizontalCoroutine);
                        transform.position = new Vector3(movementPointFinish, transform.position.y, transform.position.z);
                    }

                    player.playerAnimations.playerAnimator.Play("Jump");
                    playerRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
                    playerRigidbody.velocity = Vector3.up * jumpForce;
                    jumpSource.Play();
                    runDust.Stop();
                    canDust = true;

                    PlayerController.playerState = PlayerState.Jump;
                    if (ctrlCoroutine != null)
                    {
                        ctrlCol.SetActive(false);
                        runCol.SetActive(true);
                        StopCoroutine(ctrlCoroutine);
                    }
                }
            }
            if (SwipeController.swipeDown && !GameOverScript.isGameOver && Time.timeScale != 0 && canMoveDown)
            {
                if (PlayerController.playerState != PlayerState.Ramp)
                {
                    Obstacle.StopSlowMotion();

                    if (ctrlCoroutine != null)
                        StopCoroutine(ctrlCoroutine);
                    ctrlCoroutine = StartCoroutine(Ctrl());
                }
            }

            jumpDust.gameObject.transform.position = new Vector3(transform.position.x, 0.3f, transform.position.z);
            runDust.gameObject.transform.position = new Vector3(transform.position.x, 0.3f, transform.position.z);

            if (playerRigidbody.velocity.y > 1 && PlayerController.playerState == PlayerState.Ramp)
                playerRigidbody.velocity = new Vector3(playerRigidbody.velocity.x, 1, playerRigidbody.velocity.z);
        }
    }

    private void MoveHorizontal(float speed)
    {
        movementPointStart = movementPointFinish;
        movementPointFinish += Mathf.Sign(speed) * lineDistance;

        if (moveHorizontalCoroutine != null)
            StopCoroutine(moveHorizontalCoroutine);

        moveHorizontalCoroutine = StartCoroutine(MoveCoroutine(speed));
    }
    public IEnumerator MoveCoroutine(float vectorX)
    {
        if (PlayerController.playerState == PlayerState.Jump)
            canDust = true;

        playerRigidbody.constraints = RigidbodyConstraints.FreezeRotation;

        while (Mathf.Abs(movementPointStart - transform.position.x) < lineDistance && !GameOverScript.isGameOver && !(PlayerController.playerState == PlayerState.Changing) && PlayerController.playerState != PlayerState.Death)
        {
            yield return new WaitForFixedUpdate();
            playerRigidbody.velocity = new Vector3(vectorX * 1.2f, -12, 0);
            float x = Mathf.Clamp(transform.position.x, Mathf.Min(movementPointStart, movementPointFinish), Mathf.Max(movementPointStart, movementPointFinish));
            transform.position = new Vector3(x, transform.position.y, transform.position.z);
        }

        playerRigidbody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX;
        transform.position = new Vector3(movementPointFinish, transform.position.y, transform.position.z);
        playerRigidbody.velocity = Vector3.zero;
    }
    public IEnumerator Ctrl()
    {
        if (player.playerAnimations.shieldAnimator.gameObject.activeInHierarchy)
            player.playerAnimations.shieldAnimator.SetTrigger("isCtrl");

        player.playerAnimations.playerAnimator.Play("Ctrl");

        playerRigidbody.AddForce(Vector3.down * ctrlForce, ForceMode.Impulse);
        PlayerController.playerState = PlayerState.Ctrl;
        ctrlCol.SetActive(true);
        runCol.SetActive(false);

        yield return new WaitForSeconds(1);

        ctrlCol.SetActive(false);
        runCol.SetActive(true);
        PlayerController.playerState = PlayerState.Run;

        if (player.playerAnimations.shieldAnimator.gameObject.activeInHierarchy)
            player.playerAnimations.shieldAnimator.SetTrigger("isNotCtrl");

        player.playerAnimations.playerAnimator.Play("Run");
    }
    public IEnumerator StartMethod()
    {
        yield return new WaitForSeconds(1.5f);

        runDust.Play();
        jumpSource.volume = 0.5f * SingletonManager.soundVolume;
    }
    public IEnumerator Reborn()
    {
        transform.position = new Vector3(movementPointFinish, 0.3f, transform.position.z);

        yield return new WaitForSeconds(1.6f);

        if (moveHorizontalCoroutine != null)
            StopCoroutine(moveHorizontalCoroutine);

        runDust.Play();
    }
    public IEnumerator Lose()
    {
        if (ctrlCoroutine != null)
            StopCoroutine(ctrlCoroutine);

        ctrlCol.SetActive(false);
        runCol.SetActive(true);

        runDust.Stop();
        yield return null;
    }
    public IEnumerator Change()
    {
        runDust.Stop();
        yield return null;
    }
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Road"))
        {
            if (PlayerController.playerState != PlayerState.Death && PlayerController.playerState != PlayerState.None)
                runDust.Play();
        }
        if ((collision.gameObject.CompareTag("Lose") || collision.gameObject.CompareTag("RampLose")) && !GameOverScript.isGameOver)
        {
            if (!GameManager.isShield)
            {
                runDust.Stop();
                canDust = true;
            }
        }
        if (collision.gameObject.CompareTag("Ramp"))
        {
            if (ctrlCoroutine != null)
            {
                if (player.playerAnimations.shieldAnimator.gameObject.activeInHierarchy)
                    player.playerAnimations.shieldAnimator.SetTrigger("isNotCtrl");
            }
        }
    }
    public void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ramp"))
            runDust.Stop();

        if (collision.gameObject.CompareTag("NotLose"))
            canDust = true;
    }
}

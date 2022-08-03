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

    private float movementPointStart;
    private float movementPointFinish;
    private readonly float lineChangeSpeed = 30;
    private readonly float lineDistance = 3.3f;

    private Coroutine ctrlCoroutine;
    public Coroutine moveHorizontalCoroutine;
    private LinePosition _position;

    [HideInInspector] public bool canMoveRight;
    [HideInInspector] public bool canMoveLeft;
    [HideInInspector] public bool canMoveUp;
    [HideInInspector] public bool canMoveDown;
    [HideInInspector] public bool canDust;

    public void Start()
    {
        _position = LinePosition.Center;
        player = PlayerController.instance;
        canMoveDown = canMoveLeft = canMoveRight = canMoveUp = true;
        playerRigidbody = GetComponent<Rigidbody>();
    }

    public override void OnTick()
    {
        if (PlayerController.playerState != PlayerState.Death && PlayerController.playerState != PlayerState.None && PlayerController.playerState != PlayerState.Changing)
        {
            if (SwipeController.swipeLeft && !GameOverScript.isGameOver && Time.timeScale != 0 && canMoveLeft && _position != LinePosition.Left)
            {
                if (_position == LinePosition.Center)
                    MoveHorizontal(-lineChangeSpeed, -3.3f);
                else if (_position == LinePosition.Right)
                    MoveHorizontal(-lineChangeSpeed, 0);
                
                Obstacle.StopSlowMotion();
            }

            if (SwipeController.swipeRight && !GameOverScript.isGameOver && Time.timeScale != 0 && canMoveRight && _position != LinePosition.Right)
            {
                if (_position == LinePosition.Center)
                    MoveHorizontal(lineChangeSpeed, 3.3f);
                else if (_position == LinePosition.Left)
                    MoveHorizontal(lineChangeSpeed, 0);

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
                        StopCoroutine(moveHorizontalCoroutine);

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

    private void MoveHorizontal(float speed, float moveTo)
    {
        if (moveHorizontalCoroutine != null)
            StopCoroutine(moveHorizontalCoroutine);

        moveHorizontalCoroutine = StartCoroutine(MoveCoroutine(speed, moveTo));
    }

    public void CancelMoveHorizontal()
    {
        Debug.Log("Point start: " + movementPointStart);
        if (moveHorizontalCoroutine != null)
            MoveHorizontal(movementPointStart - transform.position.x < 0 ? -lineChangeSpeed / 2.2f : lineChangeSpeed / 2.2f, movementPointStart);
    }
    public IEnumerator MoveCoroutine(float speed, float moveTo)
    {
        if (PlayerController.playerState == PlayerState.Jump)
            canDust = true;

        playerRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        _position = (LinePosition)(moveTo / 3.3f);
        movementPointStart = transform.position.x;
        movementPointFinish = moveTo;

        while (transform.position.x != movementPointFinish && !GameOverScript.isGameOver && PlayerController.playerState != PlayerState.Changing && PlayerController.playerState != PlayerState.Death)
        {
            yield return new WaitForFixedUpdate();
            playerRigidbody.velocity = new Vector3(speed * 1.2f, -12, 0);
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

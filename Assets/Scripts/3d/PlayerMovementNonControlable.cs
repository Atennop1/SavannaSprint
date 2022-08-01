using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementNonControlable : MonoCache
{
    [SerializeField] private SpawnManager spawnManager;
    [SerializeField] private GameManager gameManager;
    private PlayerController player;

    private readonly float scoreSpeed = 0.2f;
    private readonly float maxSpeed = 60;
    public static float speed = 15;
    private float curScoreSpeed;

    private Rigidbody playerRigidbody;

    public void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody>();
        spawnManager.InitValues3D();
        curScoreSpeed = scoreSpeed * (15 / speed);
        player = PlayerController.instance;
    }
    public IEnumerator Change()
    {
        playerRigidbody.constraints = RigidbodyConstraints.FreezeAll;
        PlayerController.playerState = PlayerState.Changing;
        yield return new WaitForFixedUpdate();
    }
    public IEnumerator Lose()
    {
        PlayerController.playerState = PlayerState.Death;
        playerRigidbody.constraints = RigidbodyConstraints.FreezeAll;

        yield return new WaitForFixedUpdate();
    }
    public IEnumerator Reborn()
    {
        playerRigidbody.constraints = RigidbodyConstraints.None;
        playerRigidbody.freezeRotation = true;

        yield return new WaitForSeconds(1.6f);

        PlayerController.playerState = PlayerState.Run;

        player.playerAnimations.playerAnimator.Play("Run");
        GameOverScript.isGameOver = false;
        StartCoroutine(Move());
        StartCoroutine(SpeedAdder());
        StartCoroutine(ScoreAdder());
    }
    public IEnumerator StartMethod()
    {
        yield return new WaitForSeconds(1.5f);

        PlayerController.playerState = PlayerState.Run;
        curScoreSpeed = scoreSpeed;
        StartCoroutine(Move());
        StartCoroutine(SpeedAdder());
        StartCoroutine(ScoreAdder());
    }
    public IEnumerator Move()
    {
        while (!GameOverScript.isGameOver && !(PlayerController.playerState == PlayerState.Changing))
        {
            transform.Translate(transform.forward * -speed * Time.fixedDeltaTime);
            if (PlayerController.playerState == PlayerState.Ramp)
                transform.Translate(transform.up * 2 * Time.fixedDeltaTime * speed);
            yield return new WaitForFixedUpdate();
        }
    }
    public IEnumerator SpeedAdder()
    {
        while ((speed < maxSpeed) && !GameOverScript.isGameOver && PlayerController.playerState != PlayerState.Death)
        {
            yield return new WaitForSeconds(1);
            GameManager.speedAdderIterations++;

            spawnManager.UpdateValues3D();
            curScoreSpeed = scoreSpeed * (10 / speed);
        }
    }
    public IEnumerator ScoreAdder()
    {
        while (!GameOverScript.isGameOver && !(PlayerController.playerState == PlayerState.Changing))
        {
            if (GameManager.isX2)
                yield return new WaitForSeconds(curScoreSpeed / 2);
            else
                yield return new WaitForSeconds(curScoreSpeed);

            GameManager.score += 1;
            gameManager.UpdateText();

            if (GameManager.score >= 6666 && !PlayerPrefsSafe.HasKey("ActiveSkin3DDemon"))
                PlayerPrefsSafe.SetInt("isUnlocked3DDemon", 1);
            if (GameManager.score >= 5000 && !PlayerPrefsSafe.HasKey("ActiveSkin3DKnight"))
                PlayerPrefsSafe.SetInt("isUnlocked3DKnight", 1);
        }
    }
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("NotLose"))
        {
            PlayerController.playerState = PlayerState.Run;
            player.playerAnimations.playerAnimator.Play("Run");
        }

        if (collision.gameObject.CompareTag("Road"))
        {
            if ((PlayerController.playerState == PlayerState.Jump || PlayerController.playerState == PlayerState.Ctrl) && player.playerMovementControlable.canDust)
            {
                player.playerMovementControlable.canDust = false;
                player.playerMovementControlable.jumpDust.Play();
                if (PlayerController.playerState != PlayerState.Ctrl)
                {
                    PlayerController.playerState = PlayerState.Run;
                    player.playerAnimations.playerAnimator.Play("Run");
                }
            }
        }
        if (collision.gameObject.CompareTag("Ramp"))
        {
            PlayerController.playerState = PlayerState.Ramp;
            playerRigidbody.velocity = new Vector3(playerRigidbody.velocity.x, 0, 1);
        }
    }
    public void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ramp"))
        {
            playerRigidbody.velocity = new Vector3(playerRigidbody.velocity.x, 0, 1);
            player.playerAnimations.playerAnimator.Play("Jump");
            PlayerController.playerState = PlayerState.Jump;
        }
        if (collision.gameObject.CompareTag("NotLose"))
        {
            PlayerController.playerState = PlayerState.Jump;
            player.playerAnimations.playerAnimator.Play("Jump");
        }
    }
}

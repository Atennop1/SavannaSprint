using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementNonControlable2D : MonoCache
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private SpawnManager spawnManager;
    [SerializeField] private PlayerController2D _player;

    [Space]
    public ParticleSystem runDust;
    public ParticleSystem jumpDust;

    private float curScoreSpeed;
    private float scoreSpeed = 0.2f;
    private readonly float maxSpeed = -40;
    public static float speed = -10;

    public void Awake()
    {
        spawnManager.InitValues2D();
        curScoreSpeed = -scoreSpeed * (10 / speed);
    }

    public override void OnTick()
    {
        jumpDust.gameObject.transform.position = new Vector3(transform.position.x, 0.3f, transform.position.z);
        runDust.gameObject.transform.position = new Vector3(transform.position.x, 0.2f, transform.position.z);
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Road") && PlayerController2D.playerState != PlayerState.Death && PlayerController2D.playerState != PlayerState.None && PlayerController2D.playerState != PlayerState.Ctrl)
        {
            runDust.Play();
            if (Jump.canDust && PlayerController2D.playerState != PlayerState.Run)
            {
                jumpDust.Play();
                Jump.canDust = false;
            }

            PlayerController2D.playerState = PlayerState.Run;
            
            if (_player.ctrl.isClicked)
                _player.ctrl.OnPointerDown(null);

        }
    }

    public void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Road") && PlayerController2D.playerState == PlayerState.Jump)
            runDust.Stop();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Coin"))
            gameManager.UpdateText();
    }

    public IEnumerator StartMethod()
    {
        gameManager.UpdateText();

        yield return new WaitForSeconds(1.5f);

        runDust.Play();
        curScoreSpeed = scoreSpeed;

        StartCoroutine(Move());
        StartCoroutine(ScoreAdder());
        StartCoroutine(SpeedAdder());
    }

    public IEnumerator Reborn()
    {
        runDust.Stop();

        yield return new WaitForSeconds(1.6f);

        GameOverScript.isGameOver = false;

        StartCoroutine(Move());
        StartCoroutine(ScoreAdder());
        StartCoroutine(SpeedAdder());
        runDust.Play();
    }

    public IEnumerator Lose()
    {
        runDust.Stop();
        yield return null;
    }

    public IEnumerator Change()
    {
        runDust.Stop();
        yield return null;
    }

    public IEnumerator ScoreAdder()
    {
        while (!GameOverScript.isGameOver && PlayerController2D.playerState != PlayerState.Changing)
        {
            if (GameManager.isX2)
                yield return new WaitForSeconds(curScoreSpeed / 2);
            else
                yield return new WaitForSeconds(curScoreSpeed);

            GameManager.score += 1;
            gameManager.UpdateText();

            if (GameManager.score >= 6666 && !PlayerPrefsSafe.HasKey("ActiveSkin2DDemon"))
                PlayerPrefsSafe.SetInt("isUnlocked2DDemon", 1);

            if (GameManager.score >= 5000 && !PlayerPrefsSafe.HasKey("ActiveSkin2DKnight"))
                PlayerPrefsSafe.SetInt("isUnlocked2DKnight", 1);
        }
    }

    public IEnumerator Move()
    {
        while (!GameOverScript.isGameOver && PlayerController2D.playerState != PlayerState.Changing)
        {
            yield return new WaitForFixedUpdate();
            transform.Translate(transform.forward * speed * Time.deltaTime);
        }
    }

    public IEnumerator SpeedAdder()
    {
        while (maxSpeed < speed && !GameOverScript.isGameOver && PlayerController2D.playerState != PlayerState.Death && PlayerController2D.playerState != PlayerState.None)
        {
            yield return new WaitForSeconds(1);
            GameManager.speedAdderIterations++;

            spawnManager.UpdateValues2D();
            curScoreSpeed = -scoreSpeed * (10 / speed);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementNonControlable2D : MonoCache
{
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private SpawnManager _spawnManager;
    [SerializeField] private PlayerController2D _player;

    [field: SerializeField, Space] public ParticleSystem RunDust { get; private set; }
    [field: SerializeField] public ParticleSystem JumpDust { get; private set; }

    private float maxSpeed = -40;
    public static float speed = -10;

    private float _currentScoreSpeed;
    private float _scoreSpeed = 0.2f;

    public void Awake()
    {
        _spawnManager.InitValues2D();
        _currentScoreSpeed = -_scoreSpeed * (10 / speed);
    }

    public override void OnTick()
    {
        JumpDust.gameObject.transform.position = new Vector3(transform.position.x, 0.3f, transform.position.z);
        RunDust.gameObject.transform.position = new Vector3(transform.position.x, 0.2f, transform.position.z);
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Road") && PlayerController2D.playerState != PlayerState.Death && PlayerController2D.playerState != PlayerState.None && PlayerController2D.playerState != PlayerState.Ctrl)
        {
            RunDust.Play();
            if (Jump.CanDust && PlayerController2D.playerState != PlayerState.Run)
            {
                JumpDust.Play();
                //Jump.CanDust = false;
            }

            PlayerController2D.playerState = PlayerState.Run;
            
            //if (_player.ctrl.IsClicked)
            //    _player.ctrl.OnPointerDown(null);
        }
    }

    public void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Road") && PlayerController2D.playerState == PlayerState.Jump)
            RunDust.Stop();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Coin"))
            _gameManager.UpdateText();
    }

    public IEnumerator StartMethod()
    {
        _gameManager.UpdateText();

        yield return new WaitForSeconds(1.5f);

        RunDust.Play();
        _currentScoreSpeed = _scoreSpeed;

        StartCoroutine(Move());
        StartCoroutine(ScoreAdder());
        StartCoroutine(SpeedAdder());
    }

    public IEnumerator Reborn()
    {
        RunDust.Stop();

        yield return new WaitForSeconds(1.6f);

        GameOverScript.isGameOver = false;

        StartCoroutine(Move());
        StartCoroutine(ScoreAdder());
        StartCoroutine(SpeedAdder());
        RunDust.Play();
    }

    public IEnumerator Lose()
    {
        RunDust.Stop();
        yield return null;
    }

    public IEnumerator Change()
    {
        RunDust.Stop();
        yield return null;
    }

    public IEnumerator ScoreAdder()
    {
        while (!GameOverScript.isGameOver && PlayerController2D.playerState != PlayerState.Changing)
        {
            if (GameManager.isX2)
                yield return new WaitForSeconds(_currentScoreSpeed / 2);
            else
                yield return new WaitForSeconds(_currentScoreSpeed);

            GameManager.score += 1;
            _gameManager.UpdateText();

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

            _spawnManager.UpdateValues2D();
            _currentScoreSpeed = -_scoreSpeed * (10 / speed);
        }
    }
}

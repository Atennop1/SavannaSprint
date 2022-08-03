using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementNonControlable : MonoCache
{
    [SerializeField] private SpawnManager _spawnManager;
    [SerializeField] private GameManager _gameManager;
    private PlayerController _player;

    private float _scoreSpeed = 0.2f;
    private float _currentScoreSpeed;

    private float _maxSpeed = 60;
    public static float _speed = 15;

    private Rigidbody _playerRigidbody;

    public void Awake()
    {
        _playerRigidbody = GetComponent<Rigidbody>();
        _spawnManager.InitValues3D();
        _currentScoreSpeed = _scoreSpeed * (15 / _speed);
        _player = PlayerController.instance;
    }
    public IEnumerator Change()
    {
        _playerRigidbody.constraints = RigidbodyConstraints.FreezeAll;
        PlayerController.playerState = PlayerState.Changing;
        yield return new WaitForFixedUpdate();
    }
    public IEnumerator Lose()
    {
        PlayerController.playerState = PlayerState.Death;
        _playerRigidbody.constraints = RigidbodyConstraints.FreezeAll;

        yield return new WaitForFixedUpdate();
    }
    public IEnumerator Reborn()
    {
        _playerRigidbody.constraints = RigidbodyConstraints.None;
        _playerRigidbody.freezeRotation = true;

        yield return new WaitForSeconds(1.6f);

        PlayerController.playerState = PlayerState.Run;

        _player.PlayerAnimations.PlayerAnimator.Play("Run");
        GameOverScript.isGameOver = false;
        StartCoroutine(Move());
        StartCoroutine(SpeedAdder());
        StartCoroutine(ScoreAdder());
    }
    public IEnumerator StartMethod()
    {
        yield return new WaitForSeconds(1.5f);

        PlayerController.playerState = PlayerState.Run;
        _currentScoreSpeed = _scoreSpeed;
        StartCoroutine(Move());
        StartCoroutine(SpeedAdder());
        StartCoroutine(ScoreAdder());
    }
    public IEnumerator Move()
    {
        while (!GameOverScript.isGameOver && !(PlayerController.playerState == PlayerState.Changing))
        {
            transform.Translate(transform.forward * -_speed * Time.fixedDeltaTime);
            if (PlayerController.playerState == PlayerState.Ramp)
                transform.Translate(transform.up * Time.fixedDeltaTime * _speed);
            yield return new WaitForFixedUpdate();
        }
    }
    public IEnumerator SpeedAdder()
    {
        while ((_speed < _maxSpeed) && !GameOverScript.isGameOver && PlayerController.playerState != PlayerState.Death)
        {
            yield return new WaitForSeconds(1);
            GameManager.speedAdderIterations++;

            _spawnManager.UpdateValues3D();
            _currentScoreSpeed = _scoreSpeed * (10 / _speed);
        }
    }
    public IEnumerator ScoreAdder()
    {
        while (!GameOverScript.isGameOver && !(PlayerController.playerState == PlayerState.Changing))
        {
            if (GameManager.isX2)
                yield return new WaitForSeconds(_currentScoreSpeed / 2);
            else
                yield return new WaitForSeconds(_currentScoreSpeed);

            GameManager.score += 1;
            _gameManager.UpdateText();

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
            _player.PlayerAnimations.PlayerAnimator.Play("Run");
        }

        if (collision.gameObject.CompareTag("Road"))
        {
            if ((PlayerController.playerState == PlayerState.Jump || PlayerController.playerState == PlayerState.Ctrl) && _player.PlayerMovementControlable.canDust)
            {
                _player.PlayerMovementControlable.canDust = false;
                //_player.PlayerMovementControlable._jumpDust.Play();
                if (PlayerController.playerState != PlayerState.Ctrl)
                {
                    PlayerController.playerState = PlayerState.Run;
                    _player.PlayerAnimations.PlayerAnimator.Play("Run");
                }
            }
        }
        if (collision.gameObject.CompareTag("Ramp"))
        {
            PlayerController.playerState = PlayerState.Ramp;
            _playerRigidbody.velocity = new Vector3(_playerRigidbody.velocity.x, 0, 1);
        }
    }
    public void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ramp"))
        {
            _playerRigidbody.velocity = new Vector3(_playerRigidbody.velocity.x, 0, 1);
            _player.PlayerAnimations.PlayerAnimator.Play("Jump");
            PlayerController.playerState = PlayerState.Jump;
        }
        if (collision.gameObject.CompareTag("NotLose"))
        {
            PlayerController.playerState = PlayerState.Jump;
            _player.PlayerAnimations.PlayerAnimator.Play("Jump");
        }
    }
}

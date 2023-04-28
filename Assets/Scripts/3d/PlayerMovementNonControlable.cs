using System.Collections;
using UnityEngine;

public class PlayerMovementNonControlable : MonoCache
{
    [SerializeField] private PlayerController _player;
    [SerializeField] private SpawnManager _spawnManager;
    [SerializeField] private GameManager _gameManager;

    private float _scoreSpeed = 0.2f;
    private float _currentScoreSpeed;

    private float _maxSpeed = 60;
    public float Speed { get; private set; } = 15;

    private Rigidbody _playerRigidbody;

    public void Awake()
    {
        _playerRigidbody = GetComponent<Rigidbody>();
        _spawnManager.InitValues3D();
        _currentScoreSpeed = _scoreSpeed * (15 / Speed);
    }
    public IEnumerator Change()
    {
        _playerRigidbody.constraints = RigidbodyConstraints.FreezeAll;
        _player.PlayerState = PlayerState.Changing;
        yield return new WaitForFixedUpdate();
    }
    public IEnumerator Lose()
    {
        _player.PlayerState = PlayerState.Death;
        _playerRigidbody.constraints = RigidbodyConstraints.FreezeAll;

        yield return new WaitForFixedUpdate();
    }
    public IEnumerator Reborn()
    {
        _playerRigidbody.constraints = RigidbodyConstraints.None;
        _playerRigidbody.freezeRotation = true;

        yield return new WaitForSeconds(1.6f);

        _player.PlayerState = PlayerState.Run;

        _player.PlayerAnimations.PlayerAnimator.Play("Run");
        _player.GameOver.isGameOver = false;
        StartCoroutine(Move());
        StartCoroutine(SpeedAdder());
        StartCoroutine(ScoreAdder());
    }
    public IEnumerator StartMethod()
    {
        yield return new WaitForSeconds(1.5f);

        _player.PlayerState = PlayerState.Run;
        _currentScoreSpeed = _scoreSpeed;
        StartCoroutine(Move());
        StartCoroutine(SpeedAdder());
        StartCoroutine(ScoreAdder());
    }
    public IEnumerator Move()
    {
        while (!_player.GameOver.isGameOver && !(_player.PlayerState == PlayerState.Changing))
        {
            transform.Translate(transform.forward * -Speed * Time.fixedDeltaTime);
            if (_player.PlayerState == PlayerState.Ramp)
                transform.Translate(transform.up * Time.fixedDeltaTime * Speed);
            yield return new WaitForFixedUpdate();
        }
    }
    public IEnumerator SpeedAdder()
    {
        while ((Speed < _maxSpeed) && !_player.GameOver.isGameOver && _player.PlayerState != PlayerState.Death)
        {
            yield return new WaitForSeconds(1);
            _player.GameManager.speedAdderIterations++;

            _spawnManager.UpdateValues3D();
            _currentScoreSpeed = _scoreSpeed * (10 / Speed);
        }
    }
    public IEnumerator ScoreAdder()
    {
        while (!_player.GameOver.isGameOver && !(_player.PlayerState == PlayerState.Changing))
        {
            if (_player.GameManager.isX2)
                yield return new WaitForSeconds(_currentScoreSpeed / 2);
            else
                yield return new WaitForSeconds(_currentScoreSpeed);

            _player.GameManager.score += 1;
            _gameManager.UpdateText();

            if (_player.GameManager.score >= 6666 && !PlayerPrefsSafe.HasKey("ActiveSkin3DDemon"))
                PlayerPrefsSafe.SetInt("isUnlocked3DDemon", 1);
            if (_player.GameManager.score >= 5000 && !PlayerPrefsSafe.HasKey("ActiveSkin3DKnight"))
                PlayerPrefsSafe.SetInt("isUnlocked3DKnight", 1);
        }
    }
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("NotLose"))
        {
            _player.PlayerState = PlayerState.Run;
            _player.PlayerAnimations.PlayerAnimator.Play("Run");
        }

        if (collision.gameObject.CompareTag("Road"))
        {
            if ((_player.PlayerState == PlayerState.Jump || _player.PlayerState == PlayerState.Ctrl) && _player.PlayerMovementControlable.canDust)
            {
                _player.PlayerMovementControlable.canDust = false;
                //_player.PlayerMovementControlable._jumpDust.Play();
                if (_player.PlayerState != PlayerState.Ctrl)
                {
                    _player.PlayerState = PlayerState.Run;
                    _player.PlayerAnimations.PlayerAnimator.Play("Run");
                }
            }
        }
        if (collision.gameObject.CompareTag("Ramp"))
        {
            _player.PlayerState = PlayerState.Ramp;
            _playerRigidbody.velocity = new Vector3(_playerRigidbody.velocity.x, 0, 1);
        }
    }
    public void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ramp"))
        {
            _playerRigidbody.velocity = new Vector3(_playerRigidbody.velocity.x, 0, 1);
            _player.PlayerAnimations.PlayerAnimator.Play("Jump");
            _player.PlayerState = PlayerState.Jump;
        }
        if (collision.gameObject.CompareTag("NotLose"))
        {
            _player.PlayerState = PlayerState.Jump;
            _player.PlayerAnimations.PlayerAnimator.Play("Jump");
        }
    }
}

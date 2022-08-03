using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementControlable : MonoCache
{
    [SerializeField] private float _jumpForce;
    [SerializeField] private float _ctrlForce;

    [Space]
    [SerializeField] private GameObject _ctrlCollider;
    [SerializeField] private GameObject _runCollider;

    [Space]
    [SerializeField] private AudioSource _jumpSource;

    [Space]
    [SerializeField] private ParticleSystem _jumpDust;
    [SerializeField] private ParticleSystem _runDust;

    private PlayerController _player;
    private Rigidbody _playerRigidbody;

    private float _horizontalMovementStartPointX;
    private float _horizontalMovementFinishPointX;

    private float _lineChangeSpeed = 30;
    private float _lineDistance = 3.3f;

    private Coroutine _ctrlCoroutine;
    public Coroutine _moveHorizontalCoroutine;
    private LinePosition _position;

    [HideInInspector] public bool canMoveRight;
    [HideInInspector] public bool canMoveLeft;
    [HideInInspector] public bool canMoveUp;
    [HideInInspector] public bool canMoveDown;
    [HideInInspector] public bool canDust;

    public void Start()
    {
        _position = LinePosition.Center;
        _player = PlayerController.instance;
        canMoveDown = canMoveLeft = canMoveRight = canMoveUp = true;
        _playerRigidbody = GetComponent<Rigidbody>();
    }

    public override void OnTick()
    {
        if (PlayerController.playerState != PlayerState.Death && PlayerController.playerState != PlayerState.None && PlayerController.playerState != PlayerState.Changing)
        {
            if (SwipeController.SwipeLeft && !GameOverScript.isGameOver && Time.timeScale != 0 && canMoveLeft && _position != LinePosition.Left)
            {
                if (_position == LinePosition.Center)
                    MoveHorizontal(-_lineChangeSpeed, -3.3f);
                else if (_position == LinePosition.Right)
                    MoveHorizontal(-_lineChangeSpeed, 0);
                
                Obstacle.StopSlowMotion();
            }

            if (SwipeController.SwipeRight && !GameOverScript.isGameOver && Time.timeScale != 0 && canMoveRight && _position != LinePosition.Right)
            {
                if (_position == LinePosition.Center)
                    MoveHorizontal(_lineChangeSpeed, 3.3f);
                else if (_position == LinePosition.Left)
                    MoveHorizontal(_lineChangeSpeed, 0);

                Obstacle.StopSlowMotion();
            }

            if (SwipeController.SwipeUp && !GameOverScript.isGameOver && Time.timeScale != 0 && canMoveUp)
            {
                if (PlayerController.playerState == PlayerState.Run || PlayerController.playerState == PlayerState.Ctrl)
                {
                    Obstacle.StopSlowMotion();
                    if (_player.PlayerAnimations.ShieldAnimator.gameObject.activeInHierarchy)
                        _player.PlayerAnimations.ShieldAnimator.SetTrigger("isNotCtrl");

                    if (_moveHorizontalCoroutine != null)
                        StopCoroutine(_moveHorizontalCoroutine);

                    _player.PlayerAnimations.PlayerAnimator.Play("Jump");
                    _playerRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
                    _playerRigidbody.velocity = Vector3.up * _jumpForce;
                    _jumpSource.Play();
                    _runDust.Stop();
                    canDust = true;

                    PlayerController.playerState = PlayerState.Jump;
                    if (_ctrlCoroutine != null)
                    {
                        _ctrlCollider.SetActive(false);
                        _runCollider.SetActive(true);
                        StopCoroutine(_ctrlCoroutine);
                    }
                }
            }
            if (SwipeController.SwipeDown && !GameOverScript.isGameOver && Time.timeScale != 0 && canMoveDown)
            {
                if (PlayerController.playerState != PlayerState.Ramp)
                {
                    Obstacle.StopSlowMotion();

                    if (_ctrlCoroutine != null)
                        StopCoroutine(_ctrlCoroutine);
                    _ctrlCoroutine = StartCoroutine(Ctrl());
                }
            }

            _jumpDust.gameObject.transform.position = new Vector3(transform.position.x, 0.3f, transform.position.z);
            _runDust.gameObject.transform.position = new Vector3(transform.position.x, 0.3f, transform.position.z);

            if (_playerRigidbody.velocity.y > 1 && PlayerController.playerState == PlayerState.Ramp)
                _playerRigidbody.velocity = new Vector3(_playerRigidbody.velocity.x, 1, _playerRigidbody.velocity.z);
        }
    }

    private void MoveHorizontal(float speed, float moveTo)
    {
        if (_moveHorizontalCoroutine != null)
            StopCoroutine(_moveHorizontalCoroutine);

        _moveHorizontalCoroutine = StartCoroutine(MoveCoroutine(speed, moveTo));
    }

    public void CancelMoveHorizontal()
    {
        Debug.Log("Point start: " + _horizontalMovementStartPointX);
        if (_moveHorizontalCoroutine != null)
            MoveHorizontal(_horizontalMovementStartPointX - transform.position.x < 0 ? -_lineChangeSpeed / 2.2f : _lineChangeSpeed / 2.2f, _horizontalMovementStartPointX);
    }
    public IEnumerator MoveCoroutine(float speed, float moveTo)
    {
        if (PlayerController.playerState == PlayerState.Jump)
            canDust = true;

        _playerRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        _position = (LinePosition)(moveTo / 3.3f);
        _horizontalMovementStartPointX = transform.position.x;
        _horizontalMovementFinishPointX = moveTo;

        while (transform.position.x != _horizontalMovementFinishPointX && !GameOverScript.isGameOver && PlayerController.playerState != PlayerState.Changing && PlayerController.playerState != PlayerState.Death)
        {
            yield return new WaitForFixedUpdate();
            _playerRigidbody.velocity = new Vector3(speed * 1.2f, -12, 0);
            float x = Mathf.Clamp(transform.position.x, Mathf.Min(_horizontalMovementStartPointX, _horizontalMovementFinishPointX), Mathf.Max(_horizontalMovementStartPointX, _horizontalMovementFinishPointX));
            transform.position = new Vector3(x, transform.position.y, transform.position.z);
        }

        _playerRigidbody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX;
        transform.position = new Vector3(_horizontalMovementFinishPointX, transform.position.y, transform.position.z);
        _playerRigidbody.velocity = Vector3.zero;
    }
    public IEnumerator Ctrl()
    {
        if (_player.PlayerAnimations.ShieldAnimator.gameObject.activeInHierarchy)
            _player.PlayerAnimations.ShieldAnimator.SetTrigger("isCtrl");

        _player.PlayerAnimations.PlayerAnimator.Play("Ctrl");

        _playerRigidbody.AddForce(Vector3.down * _ctrlForce, ForceMode.Impulse);
        PlayerController.playerState = PlayerState.Ctrl;
        _ctrlCollider.SetActive(true);
        _runCollider.SetActive(false);

        yield return new WaitForSeconds(1);

        _ctrlCollider.SetActive(false);
        _runCollider.SetActive(true);
        PlayerController.playerState = PlayerState.Run;

        if (_player.PlayerAnimations.ShieldAnimator.gameObject.activeInHierarchy)
            _player.PlayerAnimations.ShieldAnimator.SetTrigger("isNotCtrl");

        _player.PlayerAnimations.PlayerAnimator.Play("Run");
    }
    public IEnumerator StartMethod()
    {
        yield return new WaitForSeconds(1.5f);

        _runDust.Play();
        _jumpSource.volume = 0.5f * SingletonManager.soundVolume;
    }
    public IEnumerator Reborn()
    {
        transform.position = new Vector3(_horizontalMovementFinishPointX, 0.3f, transform.position.z);

        yield return new WaitForSeconds(1.6f);

        if (_moveHorizontalCoroutine != null)
            StopCoroutine(_moveHorizontalCoroutine);

        _runDust.Play();
    }
    public IEnumerator Lose()
    {
        if (_ctrlCoroutine != null)
            StopCoroutine(_ctrlCoroutine);

        _ctrlCollider.SetActive(false);
        _runCollider.SetActive(true);

        _runDust.Stop();
        yield return null;
    }
    public IEnumerator Change()
    {
        _runDust.Stop();
        yield return null;
    }
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Road"))
        {
            if (PlayerController.playerState != PlayerState.Death && PlayerController.playerState != PlayerState.None)
                _runDust.Play();
        }
        if ((collision.gameObject.CompareTag("Lose") || collision.gameObject.CompareTag("RampLose")) && !GameOverScript.isGameOver)
        {
            if (!GameManager.isShield)
            {
                _runDust.Stop();
                canDust = true;
            }
        }
        if (collision.gameObject.CompareTag("Ramp"))
        {
            if (_ctrlCoroutine != null)
            {
                if (_player.PlayerAnimations.ShieldAnimator.gameObject.activeInHierarchy)
                    _player.PlayerAnimations.ShieldAnimator.SetTrigger("isNotCtrl");
            }
        }
    }
    public void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ramp"))
            _runDust.Stop();

        if (collision.gameObject.CompareTag("NotLose"))
            canDust = true;
    }
}

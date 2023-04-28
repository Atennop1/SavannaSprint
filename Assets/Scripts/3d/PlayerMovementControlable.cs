using System.Collections;
using UnityEngine;

public class PlayerMovementControlable : MonoCache
{
    [SerializeField] private SwipeController _swipeController;

    [Space]
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
    private Coroutine _moveHorizontalCoroutine;
    private LinePosition _position;

    [HideInInspector] public bool canMoveRight;
    [HideInInspector] public bool canMoveLeft;
    [HideInInspector] public bool canMoveUp;
    [HideInInspector] public bool canMoveDown;
    [HideInInspector] public bool canDust;

    public void Start()
    {
        _position = LinePosition.Center;
        canMoveDown = canMoveLeft = canMoveRight = canMoveUp = true;
        _playerRigidbody = GetComponent<Rigidbody>();
    }

    public override void OnTick()
    {
        if (_player.PlayerState != PlayerState.Death && _player.PlayerState != PlayerState.None && _player.PlayerState != PlayerState.Changing)
        {
            if (_swipeController.SwipeLeft && !_player.GameOver.isGameOver && Time.timeScale != 0 && canMoveLeft && _position != LinePosition.Left)
            {
                if (_position == LinePosition.Center)
                    MoveHorizontal(-_lineChangeSpeed, -3.3f);
                else if (_position == LinePosition.Right)
                    MoveHorizontal(-_lineChangeSpeed, 0);
                
                Obstacle.StopSlowMotion();
            }

            if (_swipeController.SwipeRight && !_player.GameOver.isGameOver && Time.timeScale != 0 && canMoveRight && _position != LinePosition.Right)
            {
                if (_position == LinePosition.Center)
                    MoveHorizontal(_lineChangeSpeed, 3.3f);
                else if (_position == LinePosition.Left)
                    MoveHorizontal(_lineChangeSpeed, 0);

                Obstacle.StopSlowMotion();
            }

            if (_swipeController.SwipeUp && !_player.GameOver.isGameOver && Time.timeScale != 0 && canMoveUp)
            {
                if (_player.PlayerState == PlayerState.Run || _player.PlayerState == PlayerState.Ctrl)
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

                    _player.PlayerState = PlayerState.Jump;
                    if (_ctrlCoroutine != null)
                    {
                        _ctrlCollider.SetActive(false);
                        _runCollider.SetActive(true);
                        StopCoroutine(_ctrlCoroutine);
                    }
                }
            }
            if (_swipeController.SwipeDown && !_player.GameOver.isGameOver && Time.timeScale != 0 && canMoveDown)
            {
                if (_player.PlayerState != PlayerState.Ramp)
                {
                    Obstacle.StopSlowMotion();

                    if (_ctrlCoroutine != null)
                        StopCoroutine(_ctrlCoroutine);
                    _ctrlCoroutine = StartCoroutine(Ctrl());
                }
            }

            _jumpDust.gameObject.transform.position = new Vector3(transform.position.x, 0.3f, transform.position.z);
            _runDust.gameObject.transform.position = new Vector3(transform.position.x, 0.3f, transform.position.z);

            if (_playerRigidbody.velocity.y > 1 && _player.PlayerState == PlayerState.Ramp)
                _playerRigidbody.velocity = new Vector3(_playerRigidbody.velocity.x, 1, _playerRigidbody.velocity.z);
        }
    }

    private void MoveHorizontal(float speed, float moveTo)
    {
        StopMoveHorizontalCoroutine();
        _moveHorizontalCoroutine = StartCoroutine(MoveCoroutine(speed, moveTo));
    }

    public void StopMoveHorizontalCoroutine()
    {
        if (_moveHorizontalCoroutine != null)
            StopCoroutine(_moveHorizontalCoroutine);
    }

    public void CancelMoveHorizontal()
    {
        if (_moveHorizontalCoroutine != null)
            MoveHorizontal(_horizontalMovementStartPointX - transform.position.x < 0 ? -_lineChangeSpeed / 2.2f : _lineChangeSpeed / 2.2f, _horizontalMovementStartPointX);
    }

    public IEnumerator MoveCoroutine(float speed, float moveTo)
    {
        if (_player.PlayerState == PlayerState.Jump)
            canDust = true;

        _playerRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        _position = (LinePosition)(moveTo / 3.3f);
        _horizontalMovementStartPointX = transform.position.x;
        _horizontalMovementFinishPointX = moveTo;

        while (transform.position.x != _horizontalMovementFinishPointX && !_player.GameOver.isGameOver && _player.PlayerState != PlayerState.Changing && _player.PlayerState != PlayerState.Death)
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
        _player.PlayerState = PlayerState.Ctrl;
        _ctrlCollider.SetActive(true);
        _runCollider.SetActive(false);

        yield return new WaitForSeconds(1);

        _ctrlCollider.SetActive(false);
        _runCollider.SetActive(true);
        _player.PlayerState = PlayerState.Run;

        if (_player.PlayerAnimations.ShieldAnimator.gameObject.activeInHierarchy)
            _player.PlayerAnimations.ShieldAnimator.SetTrigger("isNotCtrl");

        _player.PlayerAnimations.PlayerAnimator.Play("Run");
    }
    
    public IEnumerator StartMethod()
    {
        yield return new WaitForSeconds(1.5f);

        _runDust.Play();
        _jumpSource.volume = 0.5f * SingletonManager.instance.soundVolume;
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
            if (_player.PlayerState != PlayerState.Death && _player.PlayerState != PlayerState.None)
                _runDust.Play();
        }
        if ((collision.gameObject.CompareTag("Lose") || collision.gameObject.CompareTag("RampLose")) && !_player.GameOver.isGameOver)
        {
            if (!_player.GameManager.isShield)
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

using System.Collections;
using UnityEngine;

public class PlayerHorizontalMovement : MonoCache, IPlayerPart
{
    [SerializeField] private Player _player;
    private Rigidbody _playerRigidbody;

    private float _horizontalMovementStartPointX;
    private float _horizontalMovementFinishPointX;

    private readonly float _lineChangeSpeed = 30;

    private Coroutine _moveHorizontalCoroutine;
    private LinePosition _position;
    
    private PlayerSlowMotion _playerSlowMotion;

    public void CancelMoveHorizontal()
    {
        MoveHorizontal(_horizontalMovementStartPointX - transform.position.x < 0 ? -_lineChangeSpeed / 2.2f : _lineChangeSpeed / 2.2f, _horizontalMovementStartPointX);
    }

    public IEnumerator Reborn(bool ad)
    {
        transform.position = new Vector3(_horizontalMovementFinishPointX, 0.3f, transform.position.z);

        yield return new WaitForSeconds(1.6f);

        if (_moveHorizontalCoroutine != null)
            StopCoroutine(_moveHorizontalCoroutine);
    }

    public IEnumerator Lose() { yield return null; }
    public IEnumerator Main() { yield return null; }
    public IEnumerator Change() { yield return null; }
    public void SetupSkin(Skin info) { }
    
    protected override void OnTick()
    {
        if (_player.CurrentState == PlayerState.Death || _player.CurrentState == PlayerState.None ||
            _player.CurrentState == PlayerState.Changing) return;
        
        if (_player.Input.ActiveLeft && !_player.GameOver.IsGameOver && Time.timeScale != 0 && _position != LinePosition.Left)
        {
            switch (_position)
            {
                case LinePosition.Center:
                    MoveHorizontal(-_lineChangeSpeed, -3.3f);
                    break;
                case LinePosition.Right:
                    MoveHorizontal(-_lineChangeSpeed, 0);
                    break;
            }

            _playerSlowMotion.StopSlowMotion();
        }

        if (!_player.Input.ActiveRight || _player.GameOver.IsGameOver || Time.timeScale == 0 ||
            _position == LinePosition.Right) return;
        
        switch (_position)
        {
            case LinePosition.Center:
                MoveHorizontal(_lineChangeSpeed, 3.3f);
                break;
            case LinePosition.Left:
                MoveHorizontal(_lineChangeSpeed, 0);
                break;
        }

        _playerSlowMotion.StopSlowMotion();
    }

    private void Awake()
    {
        _playerSlowMotion = _player.GetPlayerPart<PlayerSlowMotion>();
        _playerRigidbody = GetComponent<Rigidbody>();

        _position = LinePosition.Center;
    }

    private void MoveHorizontal(float speed, float moveTo)
    {
        StopMoveHorizontalCoroutine();
        _moveHorizontalCoroutine = StartCoroutine(MoveHorizontalCoroutine(speed, moveTo));
    }

    private void StopMoveHorizontalCoroutine()
    {
        if (_moveHorizontalCoroutine != null)
            StopCoroutine(_moveHorizontalCoroutine);
    }

    private IEnumerator MoveHorizontalCoroutine(float speed, float moveTo)
    {
        _playerRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        _position = (LinePosition)(Mathf.Round(moveTo / 3.3f));

        _horizontalMovementStartPointX = transform.position.x;
        _horizontalMovementFinishPointX = moveTo;

        var wait = new WaitForFixedUpdate();
        while (transform.localPosition.x != _horizontalMovementFinishPointX && !_player.GameOver.IsGameOver && _player.CurrentState != PlayerState.Changing && _player.CurrentState != PlayerState.Death)
        {
            yield return wait;
            _playerRigidbody.velocity = new Vector3(speed * 1.2f, -12, 0);
            float x = Mathf.Clamp(transform.localPosition.x, Mathf.Min(_horizontalMovementStartPointX, _horizontalMovementFinishPointX), Mathf.Max(_horizontalMovementStartPointX, _horizontalMovementFinishPointX));
            transform.localPosition = new Vector3(x, transform.localPosition.y, transform.localPosition.z);
        }

        _playerRigidbody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX;
        transform.localPosition = new Vector3(_horizontalMovementFinishPointX, transform.localPosition.y, transform.localPosition.z);
        _playerRigidbody.velocity = Vector3.zero;
    }
}

using UnityEngine;

public class CoinMove : MonoCache
{
    [SerializeField] private float _moveSpeed = 40f;

    private bool _canMove;
    private Player _player;
    private PlayerForwardMovement _forwardMovement;

    public void Init(Player player)
    {
        _player = player;
        _forwardMovement = _player.GetPlayerPart<PlayerForwardMovement>();
    }

    public override void OnDisable()
    {
        base.OnDisable();
        _canMove = false;
    }

    protected override void OnTick()
    {
        if (_canMove && (_player != null && _player.BonusHandlersDatabase.IsHandlerActive(BonusType.Magnet) && !_player.GameOver.IsGameOver && _player.CurrentState != PlayerState.Changing))
            transform.position = Vector3.MoveTowards(transform.position, _player.transform.position, _moveSpeed * Time.deltaTime * (_forwardMovement.Speed / 15));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out CoinsDetector _))
            _canMove = true;
    }
}
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerForwardMovement : MonoCache, IPlayerPart
{
    [SerializeField] private Player _player;
    [SerializeField] private ObstacleSpawner _obstacleSpawner;
    [SerializeField] private PlayerStatisticsView _statisticsView;

    [field: SerializeField] public ParticleSystem RunDust { get; private set; }

    private PlayerAnimations _playerAnimations;
    private Rigidbody _playerRigidbody;
    private bool _is3DMode;

    public float MaxSpeed { get; private set; }
    public float Speed { get; private set; }

    public IEnumerator Change()
    {
        RunDust.Stop();
        _playerRigidbody.constraints = RigidbodyConstraints.FreezeAll;
        yield return null;
    }

    public IEnumerator Lose()
    {
        RunDust.Stop();
        _playerRigidbody.constraints = RigidbodyConstraints.FreezeAll;
        yield return null;
    }

    public IEnumerator Reborn(bool ad)
    {
        _playerRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        _playerRigidbody.freezeRotation = true;

        yield return new WaitForSeconds(1.6f);

        RunDust.Play();
        StartCoroutine(Move());
        StartCoroutine(SpeedAdder());
    }

    public IEnumerator Main()
    {
        yield return new WaitForSeconds(1.5f);
        RunDust.Play();

        StartCoroutine(Move());
        StartCoroutine(SpeedAdder());
    }

    public void SetupSkin(Skin skin)
    {
        skin.SetupDust(RunDust.GetComponent<ParticleSystemRenderer>());
    }

    private IEnumerator Move()
    {
        var wait = new WaitForFixedUpdate();
        while (!_player.GameOver.IsGameOver && _player.CurrentState != PlayerState.Changing)
        {
            transform.Translate(transform.forward * (-Speed * Time.fixedDeltaTime));

            if (_player.CurrentState == PlayerState.Ramp)
                transform.Translate(transform.up * (Time.fixedDeltaTime * Speed));

            yield return wait;
        }
    }

    private IEnumerator SpeedAdder()
    {
        var wait = new WaitForSeconds(1);
        while (Speed < MaxSpeed && !_player.GameOver.IsGameOver && _player.CurrentState != PlayerState.Death)
        {
            if (_is3DMode)
            {
                Speed += 0.06f;
                _statisticsView.TempStatisticsModel.IncreaseSpeed(0.06f);
                _obstacleSpawner.UpdateValues3D();
            }
            else
            {
                Speed += 0.04f;
                _statisticsView.TempStatisticsModel.IncreaseSpeed(0.04f);
                _obstacleSpawner.UpdateValues2D();
            }

            yield return wait;
        }
    }


    private void Awake()
    {
        _is3DMode = SceneManager.GetActiveScene().name == "3d World";
        _playerRigidbody = GetComponent<Rigidbody>();
        _playerAnimations = _player.GetPlayerPart<PlayerAnimations>();
        MaxSpeed = _is3DMode ? 60 : 40;

        if (_is3DMode) _obstacleSpawner.InitValues3D();
        else _obstacleSpawner.InitValues2D();

        if (!_statisticsView.TempStatisticsModel.IsSpeedValid)
        {
            var totalSpeed = _is3DMode ? 15 : 12.5f;
            
            Speed = totalSpeed;
            _statisticsView.TempStatisticsModel.IncreaseSpeed(totalSpeed);
        }
        else
        {
            Speed = _statisticsView.TempStatisticsModel.Speed;
        }
    }

    private void FixedUpdate()
    {
        RunDust.gameObject.transform.position = new Vector3(transform.position.x, 0.3f, transform.position.z);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out RampTrigger _))
        {
            _player.CurrentState = PlayerState.Ramp;
            _playerRigidbody.velocity = new Vector3(_playerRigidbody.velocity.x, 0, 1);
        }
            
        if (collision.gameObject.TryGetComponent(out LoseTrigger _) && !_player.GameOver.IsGameOver && !_player.BonusHandlersDatabase.IsHandlerActive(BonusType.Shield))
            RunDust.Stop();
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out RampTrigger _))
        {
            _playerRigidbody.velocity = new Vector3(_playerRigidbody.velocity.x, 0, 1);
            _player.CurrentState = PlayerState.Run;
        }

        if (!collision.gameObject.TryGetComponent(out WallPlatformTrigger _)) 
            return;
        
        _playerAnimations.SetShieldAnimatorIsCtrl(false);
        _playerAnimations.PlayerAnimator.Play("Jump");
        RunDust.Stop();
        _player.CurrentState = PlayerState.Jump;
    }
}

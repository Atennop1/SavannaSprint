using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class PlayerVerticalMovement : MonoCache, IPlayerPart
{
    [field: SerializeField] public Player Player { get; private set; }
    [field: SerializeField] public float JumpForce { get; private set; }

    [Space]
    [SerializeField] private GameObject _ctrlCollider;
    [SerializeField] private GameObject _runCollider;
    
    [Space]
    [FormerlySerializedAs("_JumpSource")]
    [SerializeField] private AudioSource _jumpSource;
    [SerializeField] private ParticleSystem _jumpDust;

    private Coroutine _actionCoroutine;
    private bool _canDust;
    
    public PlayerAnimations PlayerAnimations { get; private set; }
    public PlayerSlowMotion PlayerSlowMotion { get; private set; }
    public PlayerForwardMovement ForwardMovement { get; private set; }

    public Rigidbody PlayerRigidbody { get; private set; }
    
    public IEnumerator Main()
    {
        _canDust = false;
        yield return new WaitForSeconds(1.5f);
        _jumpSource.volume = 0.5f * MusicPlayer.Instance.Volume;
    }

    public IEnumerator Lose()
    {
        TryStopActionCoroutine();
        SetActionColliders(true);
        yield return null;
    }

    public void SetupSkin(Skin skin) 
    { 
        skin.SetupDust(_jumpDust.GetComponent<ParticleSystemRenderer>());
    }

    public IEnumerator Reborn(bool ad) 
    { 
        _canDust = false;
        yield return null; 
    }
    
    public void PlayJumpSource()
    {
        _jumpSource.volume = 0.5f * MusicPlayer.Instance.Volume;
        _jumpSource.Play();
    }

    public IEnumerator Change() { yield return null; }

    protected void SetActionColliders(bool active)
    {
        _runCollider.SetActive(active);
        _ctrlCollider.SetActive(!active);
    }

    protected void StartActionCoroutine(IEnumerator routine)
    {
        TryStopActionCoroutine();
        _actionCoroutine = StartCoroutine(routine);
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out LoseTrigger _) && !Player.GameOver.IsGameOver && !Player.BonusHandlersDatabase.IsHandlerActive(BonusType.Shield))
            _canDust = true;

        if (collision.gameObject.TryGetComponent(out GroundTrigger _) && 
            (Player.CurrentState == PlayerState.Jump || Player.CurrentState == PlayerState.Ctrl) && _canDust)
        {
            _canDust = false;
            _jumpDust.Play();
        }

        if ((!collision.gameObject.TryGetComponent<GroundTrigger>(out _) &&
            !collision.gameObject.TryGetComponent<WallPlatformTrigger>(out _)) ||
            (Player.CurrentState != PlayerState.Jump && Player.CurrentState != PlayerState.CtrlJump)) return;
        
        ForwardMovement.RunDust.Play();

        if (Player.CurrentState == PlayerState.CtrlJump)
        {
            Player.CurrentState = PlayerState.Ctrl;
            PlayerAnimations.SetShieldAnimatorIsCtrl(true);
            return;
        }
            
        Player.CurrentState = PlayerState.Run;
        PlayerAnimations.PlayerAnimator.Play("Run");
        PlayerAnimations.SetShieldAnimatorIsCtrl(false);
    }

    protected bool TryStopActionCoroutine()
    {
        if (_actionCoroutine == null)
            return false;
        
        StopCoroutine(_actionCoroutine);
        return true;

    }

    private void Awake()
    {
        ForwardMovement = Player.GetPlayerPart<PlayerForwardMovement>();
        PlayerAnimations = Player.GetPlayerPart<PlayerAnimations>();
        PlayerSlowMotion = Player.GetPlayerPart<PlayerSlowMotion>();
        PlayerRigidbody = GetComponent<Rigidbody>();
    }

    private void OnCollisionExit(Collision collision)
    {
        if ((collision.gameObject.TryGetComponent(out GroundTrigger _) && Player.CurrentState == PlayerState.Jump) || collision.gameObject.TryGetComponent(out WallPlatformTrigger _))
            _canDust = true;
    }

    private void Update()
    {
        _jumpDust.gameObject.transform.position = new Vector3(transform.position.x, 0.3f, transform.position.z);
    }
}

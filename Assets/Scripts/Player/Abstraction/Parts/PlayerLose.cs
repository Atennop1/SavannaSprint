using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public abstract class PlayerLose : MonoCache, IPlayerPart
{
    [SerializeField] private Player _player;
    [SerializeField] private ParticleSystem _gameOverParticles;
    [field: SerializeField] protected PlayerStatisticsView StatisticsView { get; private set; }

    [Space]
    [SerializeField] private AudioSource _shieldSource;
    [SerializeField] private AudioSource _gameOverSource;

    [SerializeField] private AudioSource _teleportSource;
    [SerializeField] private ParticleSystem _teleportParticles;

    [Space]
    [SerializeField] private GameObject _gameOverPanel;
    [SerializeField] private GameObject _pauseButton;
    [field: SerializeField] protected Button RespawnButton { get; private set; }

    [field: SerializeField, Space] protected Text RebornCostText { get; private set; }
    [field: SerializeField] protected HorizontalLayoutGroup RebornCostLayout { get; private set; }

    protected PlayerStatisticsChanger _statisticsChanger { get; private set; }

    private Transform _obstacleKiller;
    private PlayerAnimations _playerAnimations;

    public IEnumerator Lose()
    {
        _pauseButton.gameObject.SetActive(false);
        TemplateCostLayoutSetup();

        yield return new WaitForSeconds(1.9f);

        _gameOverParticles.Play();
        _gameOverSource.Play();
        _gameOverPanel.SetActive(true);
    }

    public IEnumerator ShieldLose()
    {
        _player.BonusHandlersDatabase.GetHandler(BonusType.Shield).Bonus.StopRunningCoroutine();

        foreach (Transform child in _obstacleKiller)
            child.gameObject.SetActive(false);

        StartCoroutine(_playerAnimations.ShieldLose());
        _shieldSource.Play();
        yield return null;
    }

    public IEnumerator Main()
    {
        _pauseButton.gameObject.SetActive(false);
        yield return new WaitForSeconds(1.5f);
        _gameOverSource.volume = MusicPlayer.Instance.Volume;
        _pauseButton.gameObject.SetActive(true);
    }

    public IEnumerator Reborn(bool ad)
    {
        foreach (Transform child in _obstacleKiller)
            child.gameObject.SetActive(false);

        _gameOverPanel.SetActive(false);
        _pauseButton.gameObject.SetActive(true);

        yield return null;
    }

    public IEnumerator Change()
    { 
        _pauseButton.gameObject.SetActive(false);
        yield return new WaitForSeconds(1.6f);
        _teleportSource.Play();
        _teleportParticles.Play(); 
    }

    public void SetupSkin(Skin skin)
    {
        skin.SetupGameOverParticles(_gameOverParticles.GetComponent<ParticleSystemRenderer>());
    }

    protected virtual void TemplateCostLayoutSetup() { }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.TryGetComponent(out LoseTrigger _) || _player.GameOver.IsGameOver) 
            return;
        
        _obstacleKiller = collision.gameObject.transform.parent;

        StartCoroutine(!_player.BonusHandlersDatabase.IsHandlerActive(BonusType.Shield)
            ? _player.Lose()
            : ShieldLose());
    }

    private void Awake()
    {
        _statisticsChanger = _player.GetPlayerPart<PlayerStatisticsChanger>();
        _playerAnimations = _player.GetPlayerPart<PlayerAnimations>();
        _teleportSource.volume = 0.5f * MusicPlayer.Instance.Volume;
    }
}

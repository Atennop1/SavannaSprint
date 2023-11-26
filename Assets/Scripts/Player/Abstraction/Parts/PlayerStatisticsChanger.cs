using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerStatisticsChanger : MonoCache, IPlayerPart
{
    [SerializeField] private Player _player;
    [field: SerializeField] public PlayerStatisticsView StatisticsView { get; private set; }
    [SerializeField] private AudioSource _coinSource;

    private readonly float _scoreSpeed = 0.2f;
    private float _currentScoreSpeed;

    private PlayerForwardMovement _forwardMovement;
    private bool _is3DMode;

    public IEnumerator Reborn(bool ad)
    {
        if (!ad)
        {
            if (_is3DMode)
                StatisticsView.DecreaseOrangeCoins(500 * StatisticsView.TempStatisticsModel.LifesCount);
            else
                StatisticsView.DecreaseRedCoins(200 * StatisticsView.TempStatisticsModel.LifesCount);

            if (StatisticsView.TempStatisticsModel.LifesCount != 20)
                StatisticsView.TempStatisticsModel.IncreaseLifesCount();
        }

        yield return new WaitForSeconds(1.6f);
        StartCoroutine(SpeedAdder());
        StartCoroutine(ScoreAdder());
    }

    public IEnumerator Main()
    {
        StatisticsView.UpdateText();
        yield return new WaitForSeconds(1.5f);
        _coinSource.volume = 0.27f * MusicPlayer.Instance.Volume;
        _currentScoreSpeed = _scoreSpeed * (_forwardMovement.Speed / 15);

        StartCoroutine(SpeedAdder());
        StartCoroutine(ScoreAdder());
    }

    public void PickUpCoin()
    {
        if (_is3DMode)
        {
            StatisticsView.IncreaseOrangeCoins(_player.BonusHandlersDatabase.IsHandlerActive(BonusType.X2Coins) ? 2 : 1);
        }
        else
        {
            StatisticsView.IncreaseRedCoins(_player.BonusHandlersDatabase.IsHandlerActive(BonusType.X2Coins) ? 2 : 1);
        }

        _coinSource.Play();
    }

    public IEnumerator Change() { yield break; }
    public IEnumerator Lose() { yield return null; }
    public void SetupSkin(Skin info) { }

    private IEnumerator SpeedAdder()
    {
        while (_forwardMovement.MaxSpeed > _forwardMovement.Speed && !_player.GameOver.IsGameOver && _player.CurrentState != PlayerState.Death)
        {
            _currentScoreSpeed = _scoreSpeed * (10 / _forwardMovement.Speed);
            yield return new WaitForSeconds(1);
        }
    }

    private IEnumerator ScoreAdder()
    {
        while (!_player.GameOver.IsGameOver && _player.CurrentState != PlayerState.Changing)
        {
            StatisticsView.IncreaseScore(1);
            yield return new WaitForSeconds(_currentScoreSpeed / (_player.BonusHandlersDatabase.IsHandlerActive(BonusType.X2) ? 2 : 1));
        }
    }

    private void Awake()
    {
        _is3DMode = SceneManager.GetActiveScene().name == "3d World";
        _forwardMovement = _player.GetPlayerPart<PlayerForwardMovement>();
    }
}

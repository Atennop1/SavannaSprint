using UnityEngine;
using UnityEngine.UI;

public class GameOver : MonoCache
{
    [SerializeField] private Player _player;
    [SerializeField] private PlayerStatisticsView _statisticsView;

    [Space]
    [SerializeField] private Text _orangeCoinsText;
    [SerializeField] private Text _redCoinsText;

    [Space]
    [SerializeField] private Text _scoreText;
    [SerializeField] private Text _highScoreText;

    public bool IsGameOver { get; private set; }

    public void SetGameOver(bool active)
    {
        IsGameOver = _player != null && active && _player.CurrentState == PlayerState.Death;
        if (IsGameOver) Setup();
    }

    private void Setup()
    {
        _orangeCoinsText.text = _statisticsView.TempStatisticsModel.OrangeCoinsCount.ToString();
        _redCoinsText.text = _statisticsView.TempStatisticsModel.RedCoinsCount.ToString();

        _scoreText.text =  _statisticsView.StatisticsModel.Score.ToString();
        _highScoreText.text = "HI " + _statisticsView.StatisticsModel.HighScoreCount;
    }
}

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerStatisticsView : MonoBehaviour
{
    [Header("Numbers of Zeros")]
    [SerializeField] private int _numberOfZerosInScoreText;
    [SerializeField] private int _numberOfZerosInCoinsText;

    [Header("Text")]
    [SerializeField] private Text _scoreText;
    [SerializeField] private Text _orangeCoinsText;
    [SerializeField] private Text _redCoinsText;
    [SerializeField] private Text _maxScoreText;
    [SerializeField] private string _highScorePrefix;

    private SceneType _currentSceneType;

    public PlayerStatistics StatisticsModel { get; private set; }
    public TempPlayerStatistics TempStatisticsModel { get; private set; }

    public void UpdateText()
    {
        if (_scoreText != null)
            _scoreText.text = StatisticsModel.Score.ToString().PadLeft(_numberOfZerosInScoreText, '0');

        if (_maxScoreText != null)
            _maxScoreText.text = _highScorePrefix + StatisticsModel.HighScoreCount.ToString().PadLeft(_numberOfZerosInScoreText, '0');

        var redCoinsCount = _currentSceneType == SceneType.Game ? TempStatisticsModel.RedCoinsCount : StatisticsModel.RedCoinsCount;
        var orangeCoinsCount = _currentSceneType == SceneType.Game ? TempStatisticsModel.OrangeCoinsCount : StatisticsModel.OrangeCoinsCount;

        if (_redCoinsText != null)
            _redCoinsText.text = redCoinsCount.ToString().PadLeft(_numberOfZerosInCoinsText, '0');

        if (_orangeCoinsText != null)
            _orangeCoinsText.text = orangeCoinsCount.ToString().PadLeft(_numberOfZerosInCoinsText, '0');
    }

    public void IncreaseOrangeCoins(int count)
    {
        StatisticsModel?.IncreaseOrangeCoins(count);
        TempStatisticsModel?.IncreaseOrangeCoins(count);
        UpdateText();
    }

    public void DecreaseOrangeCoins(int count)
    {
        StatisticsModel?.DecreaseOrangeCoins(count);
        TempStatisticsModel?.DecreaseOrangeCoins(count);
        UpdateText();
    }

    public void IncreaseRedCoins(int count)
    {
        StatisticsModel?.IncreaseRedCoins(count);
        TempStatisticsModel?.IncreaseRedCoins(count);
        UpdateText();
    }

    public void DecreaseRedCoins(int count)
    {
        StatisticsModel?.DecreaseRedCoins(count);
        TempStatisticsModel?.DecreaseRedCoins(count);
        UpdateText();
    }

    public void IncreaseScore(int count)
    {
        StatisticsModel?.IncreaseScore(count);
        UpdateText();
    }

    public void Clear()
    {
        StatisticsModel?.Clear();
        TempStatisticsModel?.Clear();
    }

    private void OnEnable()
    {
        _currentSceneType = SceneManager.GetActiveScene().name == "Menu" ? SceneType.Menu : SceneType.Game;
        StatisticsModel = new PlayerStatistics();
        TempStatisticsModel = new TempPlayerStatistics();
        UpdateText();
    }
    
    private void OnDisable()
    {
        TempStatisticsModel.OnDisable();
        StatisticsModel.OnDisable();

        #if UNITY_EDITOR
            TempStatisticsModel.Clear();
            StatisticsModel.Clear();
        #endif
    }

    private enum SceneType
    {
        Menu,
        Game
    }
}

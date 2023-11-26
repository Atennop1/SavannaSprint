using UnityEngine;
using UnityEngine.UI;

public class MenuBonusesHandler : MonoBehaviour
{
    [field: SerializeField, Header("UI")] public Sprite RedCoinSprite { get; private set; }
    [field: SerializeField] public Sprite OrangeCoinSprite { get; private set; }

    [Header("Source")]
    [SerializeField] private AudioSource _notEnoughMoneySource;
    [SerializeField] private AudioSource _levelUpSource;
    [SerializeField] private AudioSource _maxLevelSource;

    [Space]
    [SerializeField] private Animator _notEnoughMoneyAnimator;
    [SerializeField] private PlayerStatisticsView _statisticsView;

    public bool TryDecreaseMoney(int value, bool is3DMode)
    {
        if (is3DMode && _statisticsView.StatisticsModel.OrangeCoinsCount >= value)
        {
            _statisticsView.StatisticsModel.DecreaseOrangeCoins(value);
            UpdateBalanceText();
            return true;
        }

        if (!is3DMode && _statisticsView.StatisticsModel.RedCoinsCount >= value)
        {
            _statisticsView.StatisticsModel.DecreaseRedCoins(value);
            UpdateBalanceText();
            return true;
        }

        _notEnoughMoneySource.volume = MusicPlayer.Instance.Volume;
        _notEnoughMoneySource.Play();

        _notEnoughMoneyAnimator.gameObject.SetActive(true);
        _notEnoughMoneyAnimator.Play("Money!");
        return false;
    }

    public void UpdateBalanceText()
    {
        _statisticsView.UpdateText();
    }

    public void CloseNotEnoughMoneyPanel()
    {
        _notEnoughMoneyAnimator.gameObject.SetActive(false);
    }

    public void PlayLevelUpSource()
    {
        _levelUpSource.volume = MusicPlayer.Instance.Volume;
        _levelUpSource.Play();
    }

    public void PlayMaxLevelSource()
    {
        _maxLevelSource.volume = MusicPlayer.Instance.Volume;
        _maxLevelSource.Play();
    }
}
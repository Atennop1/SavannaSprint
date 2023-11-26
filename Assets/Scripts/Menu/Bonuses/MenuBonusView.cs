using UnityEngine;
using UnityEngine.UI;

public class MenuBonusView : MonoBehaviour
{
    [SerializeField] private MenuBonusesHandler _handler;
    [SerializeField] private BonusType _type;

    [Header("UI")]
    [SerializeField] private Text _currentLevelText;
    [SerializeField] private Text _costText;
    [SerializeField] private Image _currencyImage;

    [Header("Objects")]
    [SerializeField] private Animator _thisAnimator;
    [SerializeField] private GameObject[] _levelGameObjects;

    private int _currentLevel;
    private bool _is3DMode;
    private Button _thisButton;

    public void Click()
    {
        if (_currentLevel >= 8) 
            return;
        
        if (!_handler.TryDecreaseMoney(_currentLevel * (_is3DMode ? 1000 : 500), _is3DMode))
            return;
        
        _currentLevel++;
        PlayerPrefsSafe.SetInt(_type.ToString().ToLower()[0] + _type.ToString().Substring(1), _currentLevel);
        UpdateButton();

        if (_currentLevel < 8) _handler.PlayLevelUpSource();
        else _handler.PlayMaxLevelSource();
    }

    public void ChangeCurrency(bool is3DMode)
    {
        if (is3DMode)
        {
            _is3DMode = true;
            _costText.text = $"{_currentLevel * 1000}";
            _currencyImage.sprite = _handler.OrangeCoinSprite;
        }
        else
        {
            _is3DMode = false;
            _costText.text = $"{_currentLevel * 500}";
            _currencyImage.sprite = _handler.RedCoinSprite;
        }
    }

    private void OnEnable()
    {
        _is3DMode = PlayerPrefs.GetInt("BonusesCurrency") == 1;
        _thisButton = GetComponent<Button>();
        _currentLevel = PlayerPrefsSafe.GetInt(_type.ToString().ToLower()[0] + _type.ToString().Substring(1));

        ChangeCurrency(_is3DMode);
        UpdateButton();
    }

    private void UpdateButton()
    {
        foreach (var level in _levelGameObjects)
        {
            if (level == _levelGameObjects[_currentLevel])
                break;

            level.SetActive(true);
        }

        if (_currentLevel < 8)
        {
            _currentLevelText.text = $"{_currentLevel}";
            _costText.text = $"{_currentLevel * (_is3DMode ? 1000 : 500)}";
        }
        else
        {
            _thisButton.enabled = false;
            _thisAnimator.SetTrigger("completed");
        }
    }
}

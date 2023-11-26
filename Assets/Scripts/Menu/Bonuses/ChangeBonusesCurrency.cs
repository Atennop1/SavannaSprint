using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeBonusesCurrency : MonoBehaviour
{
    [SerializeField] private Image _moneySprite;
    [SerializeField] private Sprite _redCoin;
    [SerializeField] private Sprite _orangeCoin;

    [SerializeField] private AudioSource _selectSource;
    [SerializeField] private List<MenuBonusView> _bonuses;
    
    public void Click()
    {
        _selectSource.Play();
        Start();
    }
    
    private void Start()
    {
        if (PlayerPrefs.GetInt("BonusesCurrency") == 0)
        {
            PlayerPrefs.SetInt("BonusesCurrency", 1);
            _moneySprite.sprite = _orangeCoin;
            
            foreach (var bonus in _bonuses)
                bonus.ChangeCurrency(true);
        }
        else
        {
            PlayerPrefs.SetInt("BonusesCurrency", 0);
            _moneySprite.sprite = _redCoin;
            
            foreach (var bonus in _bonuses)
                bonus.ChangeCurrency(false);
        }
    }
}

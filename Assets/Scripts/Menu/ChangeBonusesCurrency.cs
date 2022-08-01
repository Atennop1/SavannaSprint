using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeBonusesCurrency : MonoBehaviour
{
    [SerializeField] private List<LevelSystem> bonuses;
    [SerializeField] private Image moneySprite;
    [SerializeField] private Sprite redCoin;
    [SerializeField] private Sprite orangeCoin;
    [SerializeField] private AudioSource selectSource;
    
    public void Start()
    {
        if (PlayerPrefs.GetInt("BonusesCurrency") == 1)
            moneySprite.sprite = orangeCoin;
        else
            moneySprite.sprite = redCoin;
    }
    public void Click()
    {
        selectSource.Play();
        if (PlayerPrefs.GetInt("BonusesCurrency") == 0)
        {
            PlayerPrefs.SetInt("BonusesCurrency", 1);
            moneySprite.sprite = orangeCoin;
            foreach (LevelSystem bonus in bonuses)
                bonus.ChangeCurrency(true);
        }
        else
        {
            PlayerPrefs.SetInt("BonusesCurrency", 0);
            moneySprite.sprite = redCoin;
            foreach (LevelSystem bonus in bonuses)
                bonus.ChangeCurrency(false);
        }
    }
}

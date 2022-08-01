using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverScript : MonoCache
{
    public static bool isGameOver;

    [SerializeField] private Text coins;
    [SerializeField] private Text redCoins;
    [SerializeField] private Text score;
    [SerializeField] private Text highScore;

    public override void OnTick()
    {
        coins.text = "" + SingletonManager.instance.coins;
        redCoins.text = "" + SingletonManager.instance.redCoins;
        score.text =  "" + GameManager.score;
        highScore.text = "HI " + GameManager.maxScore;
    }
}

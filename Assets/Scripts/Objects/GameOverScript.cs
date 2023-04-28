using UnityEngine;
using UnityEngine.UI;

public class GameOverScript : MonoCache
{
    public bool isGameOver;

    [SerializeField] private Text coins;
    [SerializeField] private Text redCoins;
    [SerializeField] private Text score;
    [SerializeField] private Text highScore;

    [SerializeField] private GameManager _gameManager;

    public override void OnTick()
    {
        coins.text = "" + SingletonManager.instance.coins;
        redCoins.text = "" + SingletonManager.instance.redCoins;
        score.text =  "" + _gameManager.score;
        highScore.text = "HI " + _gameManager.maxScore;
    }
}

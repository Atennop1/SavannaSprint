using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class GameManager : MonoCache
{
    [Header("Objects")]
    [SerializeField] private AudioSource selectSource;

    [Space]
    [SerializeField] private HorizontalLayoutGroup rebornLayout;
    [SerializeField] private SceneChanger sceneChanger;

    [Header("Text")]
    [SerializeField] private Text scoreText;
    [SerializeField] private Text coinsText;
    [SerializeField] private Text maxScoreText;

    [Header("Panels")]
    [SerializeField] private GameObject scorePanel;
    [SerializeField] private GameObject coinsPanel;
    [SerializeField] private GameObject pauseMenu;

    [Header("Zero Count")]
    [SerializeField] private int zeroCountInScore;
    [SerializeField] private int zeroCountInCoins;

    private bool is3d;
    private bool isFocused = true;

    public static int speedAdderIterations;
    public static int lifesCount = 1;
    public static int score = 0;

    public static SafeInt maxScore;
    public static SafeInt allOrangeCoins;
    public static SafeInt allRedCoins;

    [HideInInspector] public PlayerController player;
    [HideInInspector] public PlayerController2D player2d;

    public static bool isMagnet;
    public static bool isX2;
    public static bool isShield;
    public static bool isX2Coins;

    void Awake()
    {
        player = PlayerController.instance;
        player2d = PlayerController2D.instance;

        if (SceneManager.GetActiveScene().name != "Menu")
            selectSource.volume = 0.5f * SingletonManager.soundVolume;

        if (MenuManager.IsShadows)
            QualitySettings.shadows = ShadowQuality.HardOnly;
        else
            QualitySettings.shadows = ShadowQuality.Disable;

        allOrangeCoins = PlayerPrefsSafe.GetInt("allOrangeCoins");
        allRedCoins = PlayerPrefsSafe.GetInt("allRedCoins");
        maxScore = PlayerPrefsSafe.GetInt("maxScore");

        is3d = SceneManager.GetActiveScene().name == "3d World";
    }
    public override void OnTick()
    {
        if (is3d)
        {
            if (lifesCount < 2)
                rebornLayout.padding.left = -156;
            else if (lifesCount >= 2 && lifesCount <= 19)
                rebornLayout.padding.left = -120;
            else
                rebornLayout.padding.left = -107;
        }
        else if (SceneManager.GetActiveScene().name != "Menu")
        {
            if (lifesCount < 5)
                rebornLayout.padding.left = -294;
            else
                rebornLayout.padding.left = -278;
        }
    }
    public void DataResetMethod()
    {
        StartCoroutine(DataReset());
    }
    public IEnumerator DataReset()
    {
        lifesCount = 1;
        isX2Coins = false;
        isMagnet = false;
        isX2 = false;
        isShield = false;

        PlayerPrefs.SetFloat("itemSpace2D", 17);
        PlayerPrefs.SetFloat("itemSpace", 30);

        PlayerMovementNonControlable.speed = 15;
        PlayerMovementNonControlable2D.speed = -10;

        yield return new WaitForSeconds(1);

        if (SingletonManager.instance != null)
        {
            SingletonManager.instance.coins = 0;
            SingletonManager.instance.redCoins = 0;
        }
        score = 0;
    }
    public void QuitToMenu()
    {
        selectSource.Play();
        GameOverScript.isGameOver = false;

        SceneChanger.levelToLoad = 0;

        sceneChanger.FadeToLevel();
        StartCoroutine(DataReset());
        Destroy(SingletonManager.instance.gameObject);
    }
    public void Restart()
    {
        selectSource.Play();
        GameOverScript.isGameOver = false;

        if (is3d)
            SceneChanger.levelToLoad = 2;
        else
            SceneChanger.levelToLoad = 1;

        sceneChanger.FadeToLevel();
        speedAdderIterations = 0;
        Destroy(SingletonManager.instance.gameObject);
        StartCoroutine(DataReset());
    }
    public void Pause()
    {
        if (isFocused)
            selectSource.Play();

        Time.timeScale = 0;
        isFocused = true;
        pauseMenu.SetActive(true);
        SingletonManager.instance.musicSource.Pause();
    }
    public void Resume()
    {
        selectSource.Play();
        Time.timeScale = 1;
        SingletonManager.instance.musicSource.UnPause();
        pauseMenu.SetActive(false);
    }
    public void UpdateText()
    {
        PlayerPrefsSafe.SetInt("allOrangeCoins", allOrangeCoins);
        PlayerPrefsSafe.SetInt("allRedCoins", allRedCoins);
        PlayerPrefsSafe.SetInt("maxScore", maxScore);

        scoreText.text = score.ToString().PadLeft(zeroCountInScore, '0');

        if (SceneManager.GetActiveScene().name == "2d World")
            coinsText.text = SingletonManager.instance.redCoins.ToString().PadLeft(zeroCountInCoins, '0');

        if (SceneManager.GetActiveScene().name == "3d World")
            coinsText.text = SingletonManager.instance.coins.ToString().PadLeft(zeroCountInCoins, '0');

        if (!is3d)
            maxScoreText.text = "HI " + maxScore.ToString().PadLeft(zeroCountInScore, '0');

        if (score > maxScore)
            maxScore = score;

        if (Social.localUser.authenticated)
        {
            Social.ReportScore(maxScore, GPS.leaderboard_best_runners, (bool success) => { });
            if (maxScore > 0)
                Social.ReportProgress(GPS.achievement_foundation_of_the_foundations, 101f, (bool success) => { });
            if (maxScore >= 5000)
                Social.ReportProgress(GPS.achievement_real_knight, 101f, (bool success) => { });
            if (maxScore >= 6666)
                Social.ReportProgress(GPS.achievement_demonic_runner, 101f, (bool success) => { });
        }
    }
    public void OnApplicationPause(bool pause)
    {
#if Unity_EDITOR
        if (pause && SceneManager.GetActiveScene().name != "Menu" && !GameOverScript.isGameOver && !Obstacle.isShowing)
        {
            if (!is3d)
            {
                if (PlayerController.playerState != PlayerState.Changing && PlayerController.playerState != PlayerState.Death && PlayerController.playerState != PlayerState.None)
                {
                    isFocused = false;
                    Pause();
                }
            }
            else
            {
                if (PlayerController.playerState != PlayerState.Changing && PlayerController2D.playerState != PlayerState.Death && PlayerController2D.playerState != PlayerState.None)
                {
                    isFocused = false;
                    Pause();
                }
            }
        }
#endif
    }
    public void OnApplicationFocus(bool focus)
    {
        OnApplicationPause(!focus);
    }
}

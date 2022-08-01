using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;

public class MenuManager : MonoCache
{
    [Header("Localization")]
    public Lean.Localization.LeanLocalization localization;

    [Header("Shaders")]
    [SerializeField] private Shader standart;
    [SerializeField] private Shader diffuse;

    [SerializeField] private Material godMaterial;
    [SerializeField] private Material backgroundMaterial;

    [Header("UI")]
    [SerializeField] private Toggle shaderToggle;
    [SerializeField] private Toggle shadowsToggle;
    [SerializeField] private Toggle fpsToggle;

    [Space]
    [SerializeField] private Slider volumeSlider;

    [Space]
    [SerializeField] private Dropdown roadDropdown;
    [SerializeField] private Dropdown languageDropdown;

    [Header("Source")]
    [SerializeField] private AudioSource menuMusicSource;
    [SerializeField] private AudioSource selectSource;
    [SerializeField] private AudioSource riddleSound;

    [Header("Objects")]
    [SerializeField] private Text volumeText;
    [SerializeField] private SceneChanger sceneChanger;

    public static bool IsShadows { get; private set; }
    public static Text highScore;
    public static Text redCoins;
    public static Text orangeCoins;

    [Space]
    [SerializeField] private GameObject ageScreen;
    [SerializeField] private GameObject riddleObject;

    [Space]
    [SerializeField] private SkinSystem riddleSkin;
    [SerializeField] private SkinSystem riddleSkin2d;

    [Space]
    [SerializeField] private List<GameObject> pieces;

    private static bool isSaving;
    private static Text logText;
    private static Text outputText;

    private static List<int> toSave;
    private static List<string> stringsToSave;

    private int shaderIntPP;
    private int shadowsIntPP;
    private int fpsIntPP;

    private bool canBeep;

    public void InitializeGPS()
    {
        PlayGamesPlatform.Activate();
        Social.localUser.Authenticate((success) => { });
    }
    #region SaveIntoCloud
    public static void OpenSave(bool saving)
    {
        if (logText)
            logText.text = "";
        if(Social.localUser.authenticated)
        {
            if (logText)
                logText.text += "User is aunthenticated\n";
            isSaving = saving;
            ((PlayGamesPlatform)Social.Active).SavedGame.OpenWithAutomaticConflictResolution("SaveFile",
                DataSource.ReadCacheOrNetwork,
                ConflictResolutionStrategy.UseLongestPlaytime,
                SaveGameOpen);
        }
        else
            if (logText)
                logText.text += "User isn't aunthenticated\n";
    }
    private static void SaveGameOpen(SavedGameRequestStatus status, ISavedGameMetadata data)
    {
        
        if (status == SavedGameRequestStatus.Success)
        {
            if (isSaving)
            {
                if (logText)
                    logText.text += "Status: successfull. Attempting to save...\n";
                byte[] myData = System.Text.ASCIIEncoding.ASCII.GetBytes(GetSaveString());
                SavedGameMetadataUpdate updateForMetadata = new SavedGameMetadataUpdate.Builder().Build();
                ((PlayGamesPlatform)Social.Active).SavedGame.CommitUpdate(data, updateForMetadata, myData, SaveCallback);
            }
            else
            {
                if (logText)
                    logText.text += "Status: successfull. Attempting to load...\n";
                ((PlayGamesPlatform)Social.Active).SavedGame.ReadBinaryData(data, LoadCallback);
            }
        }
        else
            if (logText)
                logText.text += $"Status: {status.ToString()}.\n";
    }
    public static string GetSaveString()
    {
        string dataToSave = "";

        foreach(int data in toSave)
            dataToSave += data.ToString() + "|";
        dataToSave = dataToSave.Remove(dataToSave.Length - 1);

        return dataToSave;
    }
    public static void LoadSaveString(string cloudData)
    {
        string[] cloudArray = cloudData.Split('|');
        for (int i = 0; i < toSave.Count; i++)
        {
            PlayerPrefsSafe.SetInt(stringsToSave[i], int.Parse(cloudArray[i]));
            Debug.Log(stringsToSave[i] + ": " + cloudArray[i]);
            if (cloudArray[i] == "0" && (stringsToSave[i] == "magnet" || stringsToSave[i] == "x2" || stringsToSave[i] == "x2Coins"))
                PlayerPrefsSafe.SetInt(stringsToSave[i], 1);
        }

        GameManager.maxScore = PlayerPrefsSafe.GetInt("maxScore");
        GameManager.allOrangeCoins = PlayerPrefsSafe.GetInt("allOrangeCoins");
        GameManager.allRedCoins = PlayerPrefsSafe.GetInt("allRedCoins");

        highScore.text = "HI " + PlayerPrefsSafe.GetInt("maxScore");
        redCoins.text = PlayerPrefsSafe.GetInt("allRedCoins").ToString();
        orangeCoins.text = PlayerPrefsSafe.GetInt("allOrangeCoins").ToString();

        if (outputText)
        {
            outputText.text = "";
            outputText.text += "High Score: " + PlayerPrefsSafe.GetInt("maxScore") + "\n";
            outputText.text += "Orange Coins: " + PlayerPrefsSafe.GetInt("allOrangeCoins") + "\n";
            outputText.text += "Red Coins: " + PlayerPrefsSafe.GetInt("allRedCoins") + "\n";
            outputText.text += "Riddle Skin: " + PlayerPrefsSafe.GetInt("isUnlocked3DRiddle") + "\n";
            outputText.text += "Magnet Level: " + PlayerPrefsSafe.GetInt("magnet") + "\n";
        }
    }
    private static void SaveCallback(SavedGameRequestStatus status, ISavedGameMetadata data)
    {
        if (status == SavedGameRequestStatus.Success)
            if (logText)
                logText.text += "File successfully saved\n";
        else
            if (logText)
                logText.text += "File failed to saved\n";
    }
    private static void LoadCallback(SavedGameRequestStatus status, byte[] data)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            if (logText)
                logText.text += "Load successful. Attempting to read data...\n";
            string loadedData = System.Text.ASCIIEncoding.ASCII.GetString(data);
            LoadSaveString(loadedData);
        }
    }
    #endregion
    public void Awake()
    {
        toSave = new List<int> {
            PlayerPrefsSafe.GetInt("allOrangeCoins"),
            PlayerPrefsSafe.GetInt("allRedCoins"),
            PlayerPrefsSafe.GetInt("maxScore"),

            PlayerPrefsSafe.GetInt("isUnlocked3DDemon"),
            PlayerPrefsSafe.GetInt("isUnlocked3DRiddle"),
            PlayerPrefsSafe.GetInt("isUnlocked3DGentle"),
            PlayerPrefsSafe.GetInt("isUnlocked3DGolden"),
            PlayerPrefsSafe.GetInt("isUnlocked3DMaster"),
            PlayerPrefsSafe.GetInt("isUnlocked3DSleepy"),
            PlayerPrefsSafe.GetInt("isUnlocked3DZombie"),
            PlayerPrefsSafe.GetInt("isUnlocked3DCyberpunk"),
            PlayerPrefsSafe.GetInt("isUnlocked3DKnight"),
            PlayerPrefsSafe.GetInt("isUnlocked3DHacker"),

            PlayerPrefsSafe.GetInt("isUnlocked2DDemon"),
            PlayerPrefsSafe.GetInt("isUnlocked2DRiddle"),
            PlayerPrefsSafe.GetInt("isUnlocked2DGentle"),
            PlayerPrefsSafe.GetInt("isUnlocked2DGolden"),
            PlayerPrefsSafe.GetInt("isUnlocked2DMaster"),
            PlayerPrefsSafe.GetInt("isUnlocked2DSleepy"),
            PlayerPrefsSafe.GetInt("isUnlocked2DZombie"),
            PlayerPrefsSafe.GetInt("isUnlocked2DCyberpunk"),
            PlayerPrefsSafe.GetInt("isUnlocked2DKnight"),
            PlayerPrefsSafe.GetInt("isUnlocked2DHacker"),

            PlayerPrefsSafe.GetInt("x2Coins"),
            PlayerPrefsSafe.GetInt("x2"),
            PlayerPrefsSafe.GetInt("magnet")
        };

        stringsToSave = new List<string>
        {
            "allOrangeCoins",
            "allRedCoins",
            "maxScore",

            "isUnlocked3DDemon",
            "isUnlocked3DRiddle",
            "isUnlocked3DGentle",
            "isUnlocked3DGolden",
            "isUnlocked3DMaster",
            "isUnlocked3DSleepy",
            "isUnlocked3DZombie",
            "isUnlocked3DCyberpunk",
            "isUnlocked3DKnight",
            "isUnlocked3DHacker",

            "isUnlocked2DDemon",
            "isUnlocked2DRiddle",
            "isUnlocked2DGentle",
            "isUnlocked2DGolden",
            "isUnlocked2DMaster",
            "isUnlocked2DSleepy",
            "isUnlocked2DZombie",
            "isUnlocked2DCyberpunk",
            "isUnlocked2DKnight",
            "isUnlocked2DHacker",

            "x2Coins",
            "x2",
            "magnet"
        };

        highScore = GameObject.Find("ScoreText").GetComponent<Text>();
        orangeCoins = GameObject.Find("MenuOrangeCoins").GetComponent<Text>();
        redCoins = GameObject.Find("MenuRedCoins").GetComponent<Text>();

        if (!PlayerPrefsSafe.HasKey("FirstTime"))
        {
            ageScreen.SetActive(true);
            PlayerPrefsSafe.SetInt("FirstTime", 1);
            PlayerPrefs.SetString("ActiveSkin2D", "Default");
            PlayerPrefs.SetString("ActiveSkin3D", "Default");
            PlayerPrefsSafe.SetInt("x2Coins", 1);
            PlayerPrefsSafe.SetInt("x2", 1);
            PlayerPrefsSafe.SetInt("magnet", 1);
            PlayerPrefs.SetString("Language", "English");
            PlayerPrefs.SetInt("BonusesCurrency", 1);
        }

        //outputText = GameObject.Find("Output").GetComponent<Text>();
        //logText = GameObject.Find("Log").GetComponent<Text>();

        if (!PlayGamesPlatform.Instance.IsAuthenticated())
            InitializeGPS();
    }
    private void Start()
    {
        if (Social.localUser.authenticated)
        {
            Social.ReportScore(GameManager.maxScore, GPS.leaderboard_best_runners, (bool success) => { });
            if (GameManager.maxScore > 0)
                Social.ReportProgress(GPS.achievement_foundation_of_the_foundations, 101f, (bool success) => { });

            if (GameManager.maxScore >= 5000)
                Social.ReportProgress(GPS.achievement_real_knight, 101f, (bool success) => { });

            if (GameManager.maxScore >= 6666)
                Social.ReportProgress(GPS.achievement_demonic_runner, 101f, (bool success) => { });

            if (PlayerPrefsSafe.HasKey("isUnlocked3DRiddle") && PlayerPrefsSafe.GetInt("isUnlocked3DRiddle") == 1)
                Social.ReportProgress(GPS.achievement_how_did_you_find_it, 101f, (bool success) => { });

            if ((PlayerPrefsSafe.HasKey("isUnlocked3DGolden") || PlayerPrefsSafe.HasKey("isUnlocked2DGolden")) && (PlayerPrefsSafe.GetInt("isUnlocked3DGolden") == 1 || PlayerPrefsSafe.GetInt("isUnlocked2DGolden") == 1))
                Social.ReportProgress(GPS.achievement_richer_than_midas, 101f, (bool success) => { });
        }

        if (PlayerPrefs.GetString("Language") == "Russian")
        {
            languageDropdown.value = 0;
            localization.CurrentLanguage = "Russian";
            PlayerPrefs.SetString("Language", "Russian");
        }
        else
        {
            languageDropdown.value = 1;
            localization.CurrentLanguage = "English";
            PlayerPrefs.SetString("Language", "English");
        }

        if (PlayerPrefsSafe.GetInt("isUnlocked3DRiddle") == 1)
        {
            riddleObject.transform.parent.GetComponent<Button>().interactable = false;
            riddleObject.SetActive(true);

            foreach (GameObject piece in pieces)
                Destroy(piece);
        }

        if (PlayerPrefs.HasKey("volume")) volumeSlider.value = PlayerPrefs.GetFloat("volume");
        else volumeSlider.value = 1f;

        if (PlayerPrefs.HasKey("shaderIntPP")) shaderIntPP = PlayerPrefs.GetInt("shaderIntPP");
        else shaderIntPP = 0;

        if (PlayerPrefs.HasKey("shadowsIntPP")) shadowsIntPP = PlayerPrefs.GetInt("shadowsIntPP");
        else shadowsIntPP = 0;

        if (PlayerPrefs.HasKey("fpsIntPP")) fpsIntPP = PlayerPrefs.GetInt("fpsIntPP");
        else fpsIntPP = 0;

        if (shaderIntPP == 0) shaderToggle.isOn = false;
        else shaderToggle.isOn = true;

        if (shadowsIntPP == 1) shadowsToggle.isOn = true;
        else shadowsToggle.isOn = false;

        if (fpsIntPP == 1) fpsToggle.isOn = true;
        else fpsToggle.isOn = false;

        SettingsSet();

        highScore.text = "HI " + PlayerPrefsSafe.GetInt("maxScore");
        redCoins.text = PlayerPrefsSafe.GetInt("allRedCoins").ToString();
        orangeCoins.text = PlayerPrefsSafe.GetInt("allOrangeCoins").ToString();
        GameManager.score = 0;
        InitValues();
        canBeep = true;
        GameManager.speedAdderIterations = 0;
    }
    public void FixedUpdate()
    {
        SettingsSet();
    }
    public void VolumeSetup()
    {
        SingletonManager.soundVolume = volumeSlider.value;
        menuMusicSource.volume = 0.15f * SingletonManager.soundVolume;
        selectSource.volume = 0.5f * SingletonManager.soundVolume;
        volumeText.text = Mathf.Round(SingletonManager.soundVolume * 100) + "%";
    }
    public void SelectSoundPlay()
    {
        selectSource.Play();
    }
    public void DisableObject(GameObject obj)
    {
        obj.SetActive(false);
    }
    public void EnableObject(GameObject obj)
    {
        obj.SetActive(true);
    }
    public void TwoDButton()
    {
        InitValues();
        selectSource.Play();
        QualitySettings.shadowDistance = 50;
        SceneChanger.levelToLoad = 1;
        sceneChanger.FadeToLevel();
    }
    public void ThreeDButton()
    {
        InitValues();
        selectSource.Play();
        QualitySettings.shadowDistance = 70;
        SceneChanger.levelToLoad = 2;
        sceneChanger.FadeToLevel();
    }
    public void InitValues()
    {
        GameManager.lifesCount = 1;
        GameManager.isMagnet = false;
        GameManager.isX2 = false;
        GameManager.isShield = false;
        GameManager.isX2Coins = false;

        PlayerMovementNonControlable.speed = 15;
        PlayerMovementNonControlable2D.speed = -10;
        Time.timeScale = 1;

        PlayerPrefs.SetFloat("itemSpace2D", 17);
        PlayerPrefs.SetFloat("itemSpace", 30);

        if (PlayerPrefs.HasKey("GraphicsQuality"))
            roadDropdown.value = PlayerPrefs.GetInt("GraphicsQuality");
        else
        {
            roadDropdown.value = 2;
            PlayerPrefs.SetInt("GraphicsQuality", 2);
        }
    }
    void SettingsSet()
    {
        if (shaderToggle.isOn)
        {
            godMaterial.shader = standart;
            backgroundMaterial.shader = standart;
            shaderIntPP = 1;
        }
        else
        {
            godMaterial.shader = diffuse;
            backgroundMaterial.shader = diffuse;
            shaderIntPP = 0;
        }

        if (shadowsToggle.isOn)
        {
            shadowsIntPP = 1;
            IsShadows = true;
        }
        else
        {
            IsShadows = false;
            shadowsIntPP = 0;
        }

        if (fpsToggle.isOn)
            fpsIntPP = 1;
        else
            fpsIntPP = 0;

        PlayerPrefs.SetFloat("volume", volumeSlider.value);
        PlayerPrefs.SetInt("shaderIntPP", shaderIntPP);
        PlayerPrefs.SetInt("shadowsIntPP", shadowsIntPP);
        PlayerPrefs.SetInt("fpsIntPP", fpsIntPP);
    }
    public void RiddleButton()
    {
        if (Social.localUser.authenticated)
            Social.ReportProgress(GPS.achievement_how_did_you_find_it, 101f, (bool success) => { });

        riddleObject.SetActive(true);
        riddleObject.transform.parent.GetComponent<Button>().interactable = false;

        PlayerPrefsSafe.SetInt("isUnlocked3DRiddle", 1);
        PlayerPrefsSafe.SetInt("isUnlocked2DRiddle", 1);

        riddleSound.volume = SingletonManager.soundVolume;
        riddleObject.GetComponent<Animator>().SetTrigger("Go");
        riddleSound.Play();
    }
    public void SetDropdown(int value)
    {
        if (canBeep)
            selectSource.Play();

        PlayerPrefs.SetInt("GraphicsQuality", value);
    }
    public void LanguageChanged(int index)
    {
        selectSource.Play();

        if (languageDropdown.value == 0)
            localization.CurrentLanguage = "Russian";
        else
            localization.CurrentLanguage = "English";

        PlayerPrefs.SetString("Language", localization.CurrentLanguage);
    }
    public void ShowLeaderBoard()
    {
        Social.ShowLeaderboardUI();
    }
    public void ShowAchievementsUI()
    {
        Social.ShowAchievementsUI();
    }
}

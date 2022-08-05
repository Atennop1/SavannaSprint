using System.Collections.Generic;
using Lean.Localization;
using UnityEngine;
using UnityEngine.UI;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;

public class MenuManager : MonoCache
{
    [Header("Localization")]
    [SerializeField] private LeanLocalization _localization;

    [Header("Shaders")]
    [SerializeField] private Shader _standart;
    [SerializeField] private Shader _diffuse;

    [SerializeField] private Material _godMaterial;
    [SerializeField] private Material _backgroundMaterial;

    [Header("UI")]
    [SerializeField] private Toggle _shaderToggle;
    [SerializeField] private Toggle _shadowsToggle;
    [SerializeField] private Toggle _fpsToggle;

    [Space]
    [SerializeField] private Slider _volumeSlider;

    [Space]
    [SerializeField] private Dropdown _roadDropdown;
    [SerializeField] private Dropdown _languageDropdown;

    [Header("Source")]
    [SerializeField] private AudioSource _menuMusicSource;
    [SerializeField] private AudioSource _selectSource;
    [SerializeField] private AudioSource _riddleSound;

    [Header("Objects")]
    [SerializeField] private Text _volumeText;
    [SerializeField] private SceneChanger _sceneChanger;
    [SerializeField] private GameManager _gameManager;

    public bool IsShadows { get; private set; }
    [SerializeField] private Text _highScore;
    [SerializeField] private Text _redCoins;
    [SerializeField] private Text _orangeCoins;

    [Space]
    [SerializeField] private GameObject _ageScreen;
    [SerializeField] private GameObject _riddleObject;

    [Space]
    [SerializeField] private SkinSystem _riddleSkin;
    [SerializeField] private SkinSystem _riddleSkin2d;

    [Space]
    [SerializeField] private List<GameObject> _pieces;

    private bool _isSaving;
    private Text _logText;
    private Text _outputText;

    private List<int> _toSave;
    private List<string> _stringsToSave;

    private int _shaderSettingValue;
    private int _shadowsSettingValue;
    private int _fpsSettingValue;

    private bool _canBeep;

    public void InitializeGPS()
    {
        PlayGamesPlatform.Activate();
        Social.localUser.Authenticate((success) => { });
    }
    #region SaveIntoCloud
    public void OpenSave(bool saving)
    {
        if (_logText)
            _logText.text = "";
        if(Social.localUser.authenticated)
        {
            if (_logText)
                _logText.text += "User is aunthenticated\n";
            _isSaving = saving;
            ((PlayGamesPlatform)Social.Active).SavedGame.OpenWithAutomaticConflictResolution("SaveFile",
                DataSource.ReadCacheOrNetwork,
                ConflictResolutionStrategy.UseLongestPlaytime,
                SaveGameOpen);
        }
        else
            if (_logText)
                _logText.text += "User isn't aunthenticated\n";
    }
    private void SaveGameOpen(SavedGameRequestStatus status, ISavedGameMetadata data)
    {
        
        if (status == SavedGameRequestStatus.Success)
        {
            if (_isSaving)
            {
                if (_logText)
                    _logText.text += "Status: successfull. Attempting to save...\n";
                byte[] myData = System.Text.ASCIIEncoding.ASCII.GetBytes(GetSaveString());
                SavedGameMetadataUpdate updateForMetadata = new SavedGameMetadataUpdate.Builder().Build();
                ((PlayGamesPlatform)Social.Active).SavedGame.CommitUpdate(data, updateForMetadata, myData, SaveCallback);
            }
            else
            {
                if (_logText)
                    _logText.text += "Status: successfull. Attempting to load...\n";
                ((PlayGamesPlatform)Social.Active).SavedGame.ReadBinaryData(data, LoadCallback);
            }
        }
        else
            if (_logText)
                _logText.text += $"Status: {status.ToString()}.\n";
    }
    public string GetSaveString()
    {
        string dataToSave = "";

        foreach(int data in _toSave)
            dataToSave += data.ToString() + "|";
        dataToSave = dataToSave.Remove(dataToSave.Length - 1);

        return dataToSave;
    }
    public void LoadSaveString(string cloudData)
    {
        string[] cloudArray = cloudData.Split('|');
        for (int i = 0; i < _toSave.Count; i++)
        {
            PlayerPrefsSafe.SetInt(_stringsToSave[i], int.Parse(cloudArray[i]));
            Debug.Log(_stringsToSave[i] + ": " + cloudArray[i]);
            if (cloudArray[i] == "0" && (_stringsToSave[i] == "magnet" || _stringsToSave[i] == "x2" || _stringsToSave[i] == "x2Coins"))
                PlayerPrefsSafe.SetInt(_stringsToSave[i], 1);
        }

        _gameManager.maxScore = PlayerPrefsSafe.GetInt("maxScore");
        _gameManager.allOrangeCoins = PlayerPrefsSafe.GetInt("allOrangeCoins");
        _gameManager.allRedCoins = PlayerPrefsSafe.GetInt("allRedCoins");

        _highScore.text = "HI " + PlayerPrefsSafe.GetInt("maxScore");
        _redCoins.text = PlayerPrefsSafe.GetInt("allRedCoins").ToString();
        _orangeCoins.text = PlayerPrefsSafe.GetInt("allOrangeCoins").ToString();

        if (_outputText)
        {
            _outputText.text = "";
            _outputText.text += "High Score: " + PlayerPrefsSafe.GetInt("maxScore") + "\n";
            _outputText.text += "Orange Coins: " + PlayerPrefsSafe.GetInt("allOrangeCoins") + "\n";
            _outputText.text += "Red Coins: " + PlayerPrefsSafe.GetInt("allRedCoins") + "\n";
            _outputText.text += "Riddle Skin: " + PlayerPrefsSafe.GetInt("isUnlocked3DRiddle") + "\n";
            _outputText.text += "Magnet Level: " + PlayerPrefsSafe.GetInt("magnet") + "\n";
        }
    }
    private void SaveCallback(SavedGameRequestStatus status, ISavedGameMetadata data)
    {
        if (status == SavedGameRequestStatus.Success)
            if (_logText)
                _logText.text += "File successfully saved\n";
        else
            if (_logText)
                _logText.text += "File failed to saved\n";
    }
    private void LoadCallback(SavedGameRequestStatus status, byte[] data)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            if (_logText)
                _logText.text += "Load successful. Attempting to read data...\n";
            string loadedData = System.Text.ASCIIEncoding.ASCII.GetString(data);
            LoadSaveString(loadedData);
        }
    }
    #endregion
    public void Awake()
    {
        _toSave = new List<int> {
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

        _stringsToSave = new List<string>
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

        _highScore = GameObject.Find("ScoreText").GetComponent<Text>();
        _orangeCoins = GameObject.Find("MenuOrangeCoins").GetComponent<Text>();
        _redCoins = GameObject.Find("MenuRedCoins").GetComponent<Text>();

        if (!PlayerPrefsSafe.HasKey("FirstTime"))
        {
            _ageScreen.SetActive(true);
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
            Social.ReportScore(_gameManager.maxScore, GPS.leaderboard_best_runners, (bool success) => { });
            if (_gameManager.maxScore > 0)
                Social.ReportProgress(GPS.achievement_foundation_of_the_foundations, 101f, (bool success) => { });

            if (_gameManager.maxScore >= 5000)
                Social.ReportProgress(GPS.achievement_real_knight, 101f, (bool success) => { });

            if (_gameManager.maxScore >= 6666)
                Social.ReportProgress(GPS.achievement_demonic_runner, 101f, (bool success) => { });

            if (PlayerPrefsSafe.HasKey("isUnlocked3DRiddle") && PlayerPrefsSafe.GetInt("isUnlocked3DRiddle") == 1)
                Social.ReportProgress(GPS.achievement_how_did_you_find_it, 101f, (bool success) => { });

            if ((PlayerPrefsSafe.HasKey("isUnlocked3DGolden") || PlayerPrefsSafe.HasKey("isUnlocked2DGolden")) && (PlayerPrefsSafe.GetInt("isUnlocked3DGolden") == 1 || PlayerPrefsSafe.GetInt("isUnlocked2DGolden") == 1))
                Social.ReportProgress(GPS.achievement_richer_than_midas, 101f, (bool success) => { });
        }

        if (PlayerPrefs.GetString("Language") == "Russian")
        {
            _languageDropdown.value = 0;
            _localization.CurrentLanguage = "Russian";
            PlayerPrefs.SetString("Language", "Russian");
        }
        else
        {
            _languageDropdown.value = 1;
            _localization.CurrentLanguage = "English";
            PlayerPrefs.SetString("Language", "English");
        }

        if (PlayerPrefsSafe.GetInt("isUnlocked3DRiddle") == 1)
        {
            _riddleObject.transform.parent.GetComponent<Button>().interactable = false;
            _riddleObject.SetActive(true);

            foreach (GameObject piece in _pieces)
                Destroy(piece);
        }

        if (PlayerPrefs.HasKey("volume")) _volumeSlider.value = PlayerPrefs.GetFloat("volume");
        else _volumeSlider.value = 1f;

        if (PlayerPrefs.HasKey("shaderIntPP")) _shaderSettingValue = PlayerPrefs.GetInt("shaderIntPP");
        else _shaderSettingValue = 0;

        if (PlayerPrefs.HasKey("shadowsIntPP")) _shadowsSettingValue = PlayerPrefs.GetInt("shadowsIntPP");
        else _shadowsSettingValue = 0;

        if (PlayerPrefs.HasKey("fpsIntPP")) _fpsSettingValue = PlayerPrefs.GetInt("fpsIntPP");
        else _fpsSettingValue = 0;

        if (_shaderSettingValue == 0) _shaderToggle.isOn = false;
        else _shaderToggle.isOn = true;

        if (_shadowsSettingValue == 1) _shadowsToggle.isOn = true;
        else _shadowsToggle.isOn = false;

        if (_fpsSettingValue == 1) _fpsToggle.isOn = true;
        else _fpsToggle.isOn = false;

        SettingsSet();

        UpdateText();
        InitValues();
        _canBeep = true;
    }

    public void UpdateText()
    {
        _highScore.text = "HI " + PlayerPrefsSafe.GetInt("maxScore");
        _redCoins.text = PlayerPrefsSafe.GetInt("allRedCoins").ToString();
        _orangeCoins.text = PlayerPrefsSafe.GetInt("allOrangeCoins").ToString();
    }
    public void FixedUpdate()
    {
        SettingsSet();
    }
    public void VolumeSetup()
    {
        SingletonManager.instance.soundVolume = _volumeSlider.value;
        _menuMusicSource.volume = 0.15f * SingletonManager.instance.soundVolume;
        _selectSource.volume = 0.5f * SingletonManager.instance.soundVolume;
        _volumeText.text = Mathf.Round(SingletonManager.instance.soundVolume * 100) + "%";
    }
    public void SelectSoundPlay()
    {
        _selectSource.Play();
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
        _selectSource.Play();
        QualitySettings.shadowDistance = 50;
        _sceneChanger.levelToLoad = 1;
        _sceneChanger.FadeToLevel();
    }
    public void ThreeDButton()
    {
        InitValues();
        _selectSource.Play();
        QualitySettings.shadowDistance = 70;
        _sceneChanger.levelToLoad = 2;
        _sceneChanger.FadeToLevel();
    }
    public void InitValues()
    {
        _gameManager.lifesCount = 1;
        _gameManager.isMagnet = false;
        _gameManager.isX2 = false;
        _gameManager.isShield = false;
        _gameManager.isX2Coins = false;

        Time.timeScale = 1;

        PlayerPrefs.SetFloat("itemSpace2D", 17);
        PlayerPrefs.SetFloat("itemSpace", 30);

        if (PlayerPrefs.HasKey("GraphicsQuality"))
            _roadDropdown.value = PlayerPrefs.GetInt("GraphicsQuality");
        else
        {
            _roadDropdown.value = 2;
            PlayerPrefs.SetInt("GraphicsQuality", 2);
        }
    }
    void SettingsSet()
    {
        if (_shaderToggle.isOn)
        {
            _godMaterial.shader = _standart;
            _backgroundMaterial.shader = _standart;
            _shaderSettingValue = 1;
        }
        else
        {
            _godMaterial.shader = _diffuse;
            _backgroundMaterial.shader = _diffuse;
            _shaderSettingValue = 0;
        }

        if (_shadowsToggle.isOn)
        {
            _shadowsSettingValue = 1;
            IsShadows = true;
        }
        else
        {
            IsShadows = false;
            _shadowsSettingValue = 0;
        }

        if (_fpsToggle.isOn)
            _fpsSettingValue = 1;
        else
            _fpsSettingValue = 0;

        PlayerPrefs.SetFloat("volume", _volumeSlider.value);
        PlayerPrefs.SetInt("shaderIntPP", _shaderSettingValue);
        PlayerPrefs.SetInt("shadowsIntPP", _shadowsSettingValue);
        PlayerPrefs.SetInt("fpsIntPP", _fpsSettingValue);
    }
    public void RiddleButton()
    {
        if (Social.localUser.authenticated)
            Social.ReportProgress(GPS.achievement_how_did_you_find_it, 101f, (bool success) => { });

        _riddleObject.SetActive(true);
        _riddleObject.transform.parent.GetComponent<Button>().interactable = false;

        PlayerPrefsSafe.SetInt("isUnlocked3DRiddle", 1);
        PlayerPrefsSafe.SetInt("isUnlocked2DRiddle", 1);

        _riddleSound.volume = SingletonManager.instance.soundVolume;
        _riddleObject.GetComponent<Animator>().SetTrigger("Go");
        _riddleSound.Play();
    }
    public void SetDropdown(int value)
    {
        if (_canBeep)
            _selectSource.Play();

        PlayerPrefs.SetInt("GraphicsQuality", value);
    }
    public void LanguageChanged(int index)
    {
        _selectSource.Play();

        if (_languageDropdown.value == 0)
            _localization.CurrentLanguage = "Russian";
        else
            _localization.CurrentLanguage = "English";

        PlayerPrefs.SetString("Language", _localization.CurrentLanguage);
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

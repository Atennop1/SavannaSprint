using Lean.Localization;
using UnityEngine;
using UnityEngine.UI;

public class SettingsSetter : MonoBehaviour
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
    [SerializeField] private Text _volumeText;

    [Space]
    [SerializeField] private Dropdown _graphicsDropdown;
    [SerializeField] private Dropdown _languageDropdown;

    [Header("Source")]
    [SerializeField] private AudioSource _menuMusicSource;
    [SerializeField] private AudioSource _selectSource;
    private bool _canBeep;

    public void SetupVolume()
    {
        if (MusicPlayer.Instance)
            MusicPlayer.Instance.SetVolume(_volumeSlider.value);

        _menuMusicSource.volume = 0.15f * _volumeSlider.value;
        _selectSource.volume = 0.5f * _volumeSlider.value;
        _volumeText.text = Mathf.Round(_volumeSlider.value * 100) + "%";
    }

    public void SetGraphicsDropdown(int value)
    {
        if (_canBeep) _selectSource.Play();
        PlayerPrefs.SetInt("GraphicsQuality", _graphicsDropdown.value);
    }

    public void LanguageChanged(int index)
    {
        if (_canBeep) _selectSource.Play();
        _localization.CurrentLanguage = _languageDropdown.value == 0 ? "Russian" : "English";
        PlayerPrefs.SetString("Language", _localization.CurrentLanguage);
    }

    private void SaveSettings()
    {
        _backgroundMaterial.shader = _godMaterial.shader = _shaderToggle.isOn ? _standart : _diffuse;
        QualitySettings.shadows = _shadowsToggle.isOn ? ShadowQuality.HardOnly : ShadowQuality.Disable;

        PlayerPrefs.SetFloat("volume", _volumeSlider.value);
        PlayerPrefs.SetInt("shaderIntPP", _shaderToggle.isOn ? 1 : 0);
        PlayerPrefs.SetInt("shadowsIntPP", _shadowsToggle.isOn ? 1 : 0);
        PlayerPrefs.SetInt("fpsIntPP", _fpsToggle.isOn ? 1 : 0);
    }

    private void Start() => SetupVolume();

    private void Awake()
    {
        Application.targetFrameRate = 60;
        _languageDropdown.value = PlayerPrefs.GetString("Language") == "Russian" ? 0 : 1;
        _localization.CurrentLanguage = PlayerPrefs.GetString("Language");
        
        _volumeSlider.value = PlayerPrefs.HasKey("volume") ? PlayerPrefs.GetFloat("volume") : 1;
        _graphicsDropdown.value = SetupSetting("GraphicsQuality", 2);

        _shaderToggle.isOn = SetupSetting("shaderIntPP", 0) > 0;
        _shadowsToggle.isOn = SetupSetting("shadowsIntPP", 0) > 0;
        _fpsToggle.isOn = SetupSetting("fpsIntPP", 0) > 0;

        SetupSettingIfItNoExist("ActiveSkin2D", "Default");
        SetupSettingIfItNoExist("ActiveSkin3D", "Default");
        
        SetupBonusIfItNoExist("magnet", 1);
        SetupBonusIfItNoExist("x2Coins", 1);
        SetupBonusIfItNoExist("x2", 1);
        
        _canBeep = true;
    }

    private void OnDisable() => SaveSettings();

    private int SetupSetting(string key, int defaultValue) =>
        PlayerPrefs.HasKey(key) ? PlayerPrefs.GetInt(key) : defaultValue;

    private void SetupBonusIfItNoExist(string key, int value)
    {
        if (!PlayerPrefsSafe.HasKey(key))
            PlayerPrefsSafe.SetInt(key, value);
    }

    private void SetupSettingIfItNoExist(string key, string value)
    {
        if (!PlayerPrefs.HasKey(key))
            PlayerPrefs.SetString(key, value);
    }
}

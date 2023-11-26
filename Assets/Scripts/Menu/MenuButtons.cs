using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MenuButtons : MonoCache
{
    [Header("Source")]
    [SerializeField] private AudioSource _selectSource;
    [SerializeField] private AudioSource _riddleSound;

    [Header("Objects")]
    [SerializeField] private SceneChanger _sceneChanger;
    [FormerlySerializedAs("_settingsSet")] [SerializeField] private SettingsSetter _settingsSetter;

    [Space]
    [SerializeField] private GameObject _riddleObject;
    [SerializeField] private List<GameObject> _pieces;

    private void Awake()
    {
        Time.timeScale = 1;
        if (PlayerPrefsSafe.GetInt("isUnlocked3DRiddle") != 1) 
            return;
        
        _riddleObject.transform.parent.GetComponent<Button>().interactable = false;
        _riddleObject.SetActive(true);

        foreach (var piece in _pieces)
            Destroy(piece);
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
        Destroy(MusicPlayer.Instance.gameObject);
        _selectSource.Play();

        QualitySettings.shadowDistance = 50;
        _sceneChanger.LoadScene(1);
    }

    public void ThreeDButton()
    {
        Destroy(MusicPlayer.Instance.gameObject);
        _selectSource.Play();
        
        QualitySettings.shadowDistance = 70;
        _sceneChanger.LoadScene(2);
    }

    public void RiddleButton()
    {
        if (Social.localUser.authenticated)
            Social.ReportProgress(GPS.achievement_how_did_you_find_it, 101f, (success) => { });

        _riddleObject.SetActive(true);
        _riddleObject.transform.parent.GetComponent<Button>().interactable = false;

        PlayerPrefsSafe.SetInt("isUnlocked3DRiddle", 1);
        PlayerPrefsSafe.SetInt("isUnlocked2DRiddle", 1);

        _riddleSound.volume = MusicPlayer.Instance.Volume;
        _riddleObject.GetComponent<Animator>().SetTrigger("Go");
        _riddleSound.Play();
    }
}

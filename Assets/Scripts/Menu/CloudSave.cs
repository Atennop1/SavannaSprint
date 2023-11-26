using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;

public class CloudSave : MonoBehaviour
{
    [SerializeField] private GameObject _ageScreen;
    [SerializeField] private PlayerStatisticsView _statisticsView;

    private bool _isSaving;
    private Text _logText;
    private Text _outputText;

    private List<int> _toSave;
    private List<string> _stringsToSave;

    private void OpenSave(bool saving)
    {
        if (_logText)
            _logText.text = "";
        if(Social.localUser.authenticated)
        {
            if (_logText)
                _logText.text += "User is authenticated\n";
            _isSaving = saving;
            ((PlayGamesPlatform)Social.Active).SavedGame.OpenWithAutomaticConflictResolution("SaveFile",
                DataSource.ReadCacheOrNetwork,
                ConflictResolutionStrategy.UseLongestPlaytime,
                SaveGameOpen);
        }
        else
            if (_logText)
                _logText.text += "User isn't authenticated\n";
    }

    private void SaveGameOpen(SavedGameRequestStatus status, ISavedGameMetadata data)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            if (_isSaving)
            {
                if (_logText)
                    _logText.text += "Status: successful. Attempting to save...\n";
                var myData = System.Text.Encoding.ASCII.GetBytes(GetSaveString());
                var updateForMetadata = new SavedGameMetadataUpdate.Builder().Build();
                ((PlayGamesPlatform)Social.Active).SavedGame.CommitUpdate(data, updateForMetadata, myData, SaveCallback);
            }
            else
            {
                if (_logText)
                    _logText.text += "Status: successful. Attempting to load...\n";
                ((PlayGamesPlatform)Social.Active).SavedGame.ReadBinaryData(data, LoadCallback);
            }
        }
        else if (_logText)
        {
            _logText.text += $"Status: {status.ToString()}.\n";
        }
    }

    private string GetSaveString()
    {
        var dataToSave = _toSave.Aggregate("", (current, data) => current + (data.ToString() + "|"));
        dataToSave = dataToSave.Remove(dataToSave.Length - 1);
        return dataToSave;
    }

    private void LoadSaveString(string cloudData)
    {
        string[] cloudArray = cloudData.Split('|');
        for (int i = 0; i < _toSave.Count; i++)
        {
            PlayerPrefsSafe.SetInt(_stringsToSave[i], int.Parse(cloudArray[i]));
            Debug.Log(_stringsToSave[i] + ": " + cloudArray[i]);
            if (cloudArray[i] == "0" && (_stringsToSave[i] == "magnet" || _stringsToSave[i] == "x2" || _stringsToSave[i] == "x2Coins"))
                PlayerPrefsSafe.SetInt(_stringsToSave[i], 1);
        }

        _statisticsView.UpdateText();

        if (!_outputText) 
            return;
        
        _outputText.text = "";
        _outputText.text += "High Score: " + PlayerPrefsSafe.GetInt("maxScore") + "\n";
        _outputText.text += "Orange Coins: " + PlayerPrefsSafe.GetInt("allOrangeCoins") + "\n";
        _outputText.text += "Red Coins: " + PlayerPrefsSafe.GetInt("allRedCoins") + "\n";
        _outputText.text += "Riddle Skin: " + PlayerPrefsSafe.GetInt("isUnlocked3DRiddle") + "\n";
        _outputText.text += "Magnet Level: " + PlayerPrefsSafe.GetInt("magnet") + "\n";
    }

    private void SaveCallback(SavedGameRequestStatus status, ISavedGameMetadata data)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            if (_logText)
                _logText.text += "File successfully saved\n";
        }
        else if (_logText)
        {
            _logText.text += "File failed to saved\n";
        }
    }

    private void LoadCallback(SavedGameRequestStatus status, byte[] data)
    {
        if (status != SavedGameRequestStatus.Success) 
            return;
        
        if (_logText)
            _logText.text += "Load successful. Attempting to read data...\n";
        
        var loadedData = System.Text.Encoding.ASCII.GetString(data);
        LoadSaveString(loadedData);
    }

    private void Awake()
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

        if (PlayerPrefsSafe.HasKey("FirstTime")) 
            return;
        
        _ageScreen.SetActive(true);
        PlayerPrefsSafe.SetInt("FirstTime", 1);
        PlayerPrefs.SetString("ActiveSkin2D", "Default");
        PlayerPrefs.SetString("ActiveSkin3D", "Default");

        PlayerPrefsSafe.SetInt("x2Coins", 1);
        PlayerPrefsSafe.SetInt("x2", 1);
        PlayerPrefsSafe.SetInt("magnet", 1);

        PlayerPrefs.SetString("Language", "English");
        PlayerPrefs.SetInt("BonusesCurrency", 1);

        //outputText = GameObject.Find("Output").GetComponent<Text>();
        //logText = GameObject.Find("Log").GetComponent<Text>();
    }
}

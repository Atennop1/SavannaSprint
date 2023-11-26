using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine;

public class GPSSetup : MonoBehaviour
{
    public void ShowLeaderBoard()
    {
        Social.ShowLeaderboardUI();
    }

    public void ShowAchievements()
    {
        Social.ShowAchievementsUI();
    }

    private void InitializeGPS()
    {
        PlayGamesPlatform.Activate();
        PlayGamesPlatform.Instance.Authenticate(success => { });
        Social.localUser.Authenticate(success => { });
    }

    private void Awake()
    {
        if (!PlayGamesPlatform.Instance.IsAuthenticated())
            InitializeGPS();

        if (!Social.localUser.authenticated) 
            return;
        
        if (PlayerPrefsSafe.HasKey("isUnlocked3DRiddle") && PlayerPrefsSafe.GetInt("isUnlocked3DRiddle") == 1)
            Social.ReportProgress(GPS.achievement_how_did_you_find_it, 101f, success => { });

        if ((PlayerPrefsSafe.HasKey("isUnlocked3DGolden") || PlayerPrefsSafe.HasKey("isUnlocked2DGolden")) && (PlayerPrefsSafe.GetInt("isUnlocked3DGolden") == 1 || PlayerPrefsSafe.GetInt("isUnlocked2DGolden") == 1))
            Social.ReportProgress(GPS.achievement_richer_than_midas, 101f, success => { });
    }
}
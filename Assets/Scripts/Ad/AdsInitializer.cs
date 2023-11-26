using UnityEngine;
using Yodo1.MAS;

public sealed class AdsInitializer : MonoBehaviour
{
    [SerializeField] private Player _player;

    public void ShowRewarded()
    {
        if (Yodo1U3dMas.IsRewardedAdLoaded())
            Yodo1U3dRewardAd.GetInstance().ShowAd();
    }

    private void Start()
    {
        InitializeAge();
        
        if (!Yodo1U3dMasCallback.isInitialized())
            Yodo1U3dMas.InitializeMasSdk();
        
        InitializeRewardedAds();
    }

    private void InitializeAge()
    {
        if (!PlayerPrefs.HasKey("age")) 
            return;
        
        var age = PlayerPrefs.GetInt("age");

        Yodo1U3dMas.SetCCPA(false);
        Yodo1U3dMas.SetGDPR(age <= 15);
        Yodo1U3dMas.SetCOPPA(age <= 12);
    }

    private void InitializeRewardedAds()
    {
        Yodo1U3dRewardAd.GetInstance().OnAdOpenedEvent += OnRewardedAdOpenedEvent;
        Yodo1U3dRewardAd.GetInstance().OnAdClosedEvent += OnRewardedAdClosedEvent;
        Yodo1U3dRewardAd.GetInstance().OnAdOpenFailedEvent += OnRewardedAdErrorEvent;
        Yodo1U3dRewardAd.GetInstance().OnAdEarnedEvent += OnAdReceivedRewardEvent;
    }
    
    private void OnAdReceivedRewardEvent(Yodo1U3dRewardAd ad)
    {
        Debug.Log("[Yodo1 Mas] Rewarded ad received reward");
        if (_player != null && _player.GameOver.IsGameOver)
            _player.RebornMethod(true);
    }

    private void OnRewardedAdOpenedEvent(Yodo1U3dRewardAd ad) { Debug.Log("[Yodo1 Mas] Rewarded ad opened"); }
    private void OnRewardedAdClosedEvent(Yodo1U3dRewardAd ad) { Debug.Log("[Yodo1 Mas] Rewarded ad closed"); }
    private void OnRewardedAdErrorEvent(Yodo1U3dRewardAd ad, Yodo1U3dAdError error) { Debug.Log("[Yodo1 Mas] Rewarded ad error - " + error); }
}
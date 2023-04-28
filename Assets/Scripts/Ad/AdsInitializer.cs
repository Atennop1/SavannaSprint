using UnityEngine;
using UnityEngine.SceneManagement;
using Yodo1.MAS;

public class AdsInitializer : MonoBehaviour
{
    [SerializeField] private PlayerController _player;
    [SerializeField] private PlayerController2D _player2d;

    public void Start()
    {
        InitializeAge();
        Yodo1U3dMas.InitializeSdk();
        InitializeRewardedAds();
    }

    public void InitializeAge()
    {
        int age = PlayerPrefs.GetInt("age");
        Yodo1U3dMas.SetCCPA(false);

        if (age <= 15)
            Yodo1U3dMas.SetGDPR(false);
        else
            Yodo1U3dMas.SetGDPR(true);

        if (age <= 12)
            Yodo1U3dMas.SetCOPPA(true);
        else
            Yodo1U3dMas.SetCOPPA(false);
    }

    public void ShowRewarded()
    {
        if (Yodo1U3dMas.IsRewardedAdLoaded()) Yodo1U3dMas.ShowRewardedAd();
    }

    private void InitializeRewardedAds()
    {
        Yodo1U3dMasCallback.Rewarded.OnAdOpenedEvent -= OnRewardedAdOpenedEvent;
        Yodo1U3dMasCallback.Rewarded.OnAdClosedEvent -= OnRewardedAdClosedEvent;
        Yodo1U3dMasCallback.Rewarded.OnAdErrorEvent -= OnRewardedAdErorEvent;
        Yodo1U3dMasCallback.Rewarded.OnAdReceivedRewardEvent -= OnAdReceivedRewardEvent;

        Yodo1U3dMasCallback.Rewarded.OnAdOpenedEvent += OnRewardedAdOpenedEvent;
        Yodo1U3dMasCallback.Rewarded.OnAdClosedEvent += OnRewardedAdClosedEvent;
        Yodo1U3dMasCallback.Rewarded.OnAdErrorEvent += OnRewardedAdErorEvent;
        Yodo1U3dMasCallback.Rewarded.OnAdReceivedRewardEvent += OnAdReceivedRewardEvent;
    }

    private void OnRewardedAdOpenedEvent() { Debug.Log("[Yodo1 Mas] Rewarded ad opened"); }
    private void OnRewardedAdClosedEvent() { Debug.Log("[Yodo1 Mas] Rewarded ad closed"); }
    private void OnRewardedAdErorEvent(Yodo1U3dAdError adError) { Debug.Log("[Yodo1 Mas] Rewarded ad error - " + adError.ToString()); }
    private void OnAdReceivedRewardEvent()
    {
        Debug.Log("[Yodo1 Mas] Rewarded ad received reward");
        if (SceneManager.GetActiveScene().name == "3d World" && _player != null && _player.GameOver.isGameOver)
            _player.RebornMethod(true);
        if (SceneManager.GetActiveScene().name == "2d World" && _player2d != null && _player2d.GameOver.isGameOver)
            _player2d.RebornMethod(true);
    }
}
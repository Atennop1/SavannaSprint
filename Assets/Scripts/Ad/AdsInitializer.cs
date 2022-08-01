using System.ComponentModel;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yodo1.MAS;

public class AdsInitializer : MonoBehaviour
{
    [HideInInspector] public static PlayerController player;
    [HideInInspector] public static PlayerController2D player2d;

    public void Start()
    {
        player = PlayerController.instance;
        player2d = PlayerController2D.instance;

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
    private static void OnRewardedAdOpenedEvent() { Debug.Log("[Yodo1 Mas] Rewarded ad opened"); }
    private static void OnRewardedAdClosedEvent() { Debug.Log("[Yodo1 Mas] Rewarded ad closed"); }
    private static void OnRewardedAdErorEvent(Yodo1U3dAdError adError) { Debug.Log("[Yodo1 Mas] Rewarded ad error - " + adError.ToString()); }
    private static void OnAdReceivedRewardEvent()
    {
        Debug.Log("[Yodo1 Mas] Rewarded ad received reward");
        if (SceneManager.GetActiveScene().name == "3d World" && player != null && GameOverScript.isGameOver)
            player.RebornMethod(true);
        if (SceneManager.GetActiveScene().name == "2d World" && player2d != null && GameOverScript.isGameOver)
            player2d.RebornMethod(true);
    }
}
using UnityEngine;
using Yodo1.MAS;

public class Yodo1AdsTest : MonoBehaviour
{
    bool enableBannerApiV2 = true;
    
    void Start()
    {
        Yodo1U3dMasCallback.OnSdkInitializedEvent += (success, error) =>
        {

            Debug.Log(Yodo1U3dMas.TAG + "OnSdkInitializedEvent, success:" + success + ", error: " + error.ToString());
            Debug.Log(Yodo1U3dMas.TAG + "OnSdkInitializedEvent, age:" + Yodo1U3dMas.GetUserAge());
            if (success)
            {
                InitializeBannerAds();
                InitializeInterstitialAds();
                InitializeRewardedAds();
                InitializeNativeAds();
            }

        };

        Yodo1MasUserPrivacyConfig userPrivacyConfig = new Yodo1MasUserPrivacyConfig()
            .titleBackgroundColor(Color.green)
            .titleTextColor(Color.blue)
            .contentBackgroundColor(Color.black)
            .contentTextColor(Color.white)
            .buttonBackgroundColor(Color.red)
            .buttonTextColor(Color.green);

        Yodo1AdBuildConfig config = new Yodo1AdBuildConfig()
            .enableAdaptiveBanner(true)
            .enableUserPrivacyDialog(true)
            .userPrivacyConfig(userPrivacyConfig);
        Yodo1U3dMas.SetAdBuildConfig(config);

        Yodo1U3dMas.InitializeSdk();
    }

    #region Banner Ad Methods
    private void InitializeBannerAds()
    {
        if (enableBannerApiV2 == false)
        {
            InitializeBannerAdsV1();
        }
        else
        {
            InitializeBannerAdsV2();
        }
    }

    private void ShowBannerAds()
    {
        if (enableBannerApiV2 == false)
        {
            ShowBannerAdsV1();
        }
        else
        {
            ShowBannerAdsV2();
        }
    }

    private void HideBannerAds()
    {
        if (enableBannerApiV2 == false)
        {
            HideBannerAdsV1();
        }
        else
        {
            HideBannerAdsV2();
        }
    }
    #endregion

    #region Banner Ad Methods - V1
    private void InitializeBannerAdsV1()
    {
        Yodo1U3dMasCallback.Banner.OnAdOpenedEvent += OnBannerAdOpenedEvent;
        Yodo1U3dMasCallback.Banner.OnAdClosedEvent += OnBannerAdClosedEvent;
        Yodo1U3dMasCallback.Banner.OnAdErrorEvent += OnBannerAdErrorEvent;

        ShowBannerAdsV1();
    }

    private void OnBannerAdErrorEvent(Yodo1U3dAdError adError)
    {
        Debug.Log(Yodo1U3dMas.TAG + "Banner ad error - " + adError.ToString());
    }

    private void OnBannerAdOpenedEvent()
    {
        Debug.Log(Yodo1U3dMas.TAG + "Banner ad opened");
    }

    private void OnBannerAdClosedEvent()
    {
        Debug.Log(Yodo1U3dMas.TAG + "Banner ad closed");
    }

    private void ShowBannerAdsV1()
    {
        int align = Yodo1U3dBannerAlign.BannerTop | Yodo1U3dBannerAlign.BannerHorizontalCenter;
        Yodo1U3dMas.ShowBannerAd(align);
    }

    private void HideBannerAdsV1()
    {
        Yodo1U3dMas.DismissBannerAd();
    }
    #endregion

    #region Banner Ad Methods - V2
    Yodo1U3dBannerAdView bannerAdView = null;
    //Yodo1U3dBannerAdView bannerAdView2 = null;

    /// <summary>
    /// The banner is displayed automatically after loaded
    /// </summary>
    private void InitializeBannerAdsV2()
    {
        // Clean up banner before reusing
        if (bannerAdView != null)
        {
            bannerAdView.Destroy();
            bannerAdView = null;
        }

        // Create a 320x50 banner at bottom of the screen
        bannerAdView = new Yodo1U3dBannerAdView(Yodo1U3dBannerAdSize.Banner, Yodo1U3dBannerAdPosition.BannerBottom | Yodo1U3dBannerAdPosition.BannerHorizontalCenter);

        // Create a 320x50 banner ad at coordinate (0,50) on screen.
        //bannerAdView = new Yodo1U3dBannerAdView(Yodo1U3dBannerAdSize.Banner, 0, 50);

        // Add Events
        bannerAdView.OnAdLoadedEvent += OnBannerAdLoadedEvent;
        bannerAdView.OnAdFailedToLoadEvent += OnBannerAdFailedToLoadEvent;
        bannerAdView.OnAdOpenedEvent += OnBannerAdOpenedEvent;
        bannerAdView.OnAdFailedToOpenEvent += OnBannerAdFailedToOpenEvent;
        bannerAdView.OnAdClosedEvent += OnBannerAdClosedEvent;

        // Load banner ads, the banner ad will be displayed automatically after loaded
        bannerAdView.LoadAd();


    }

    private void OnBannerAdLoadedEvent(Yodo1U3dBannerAdView adView)
    {
        // Banner ad is ready to be shown.
        Debug.Log(Yodo1U3dMas.TAG + "BannerV2 ad loaded");
    }

    private void OnBannerAdFailedToLoadEvent(Yodo1U3dBannerAdView adView, Yodo1U3dAdError adError)
    {
        Debug.Log(Yodo1U3dMas.TAG + "BannerV2 ad failed to load with error code: " + adError.ToString());
    }

    private void OnBannerAdOpenedEvent(Yodo1U3dBannerAdView adView)
    {
        Debug.Log(Yodo1U3dMas.TAG + "BannerV2 ad opened");
    }

    private void OnBannerAdFailedToOpenEvent(Yodo1U3dBannerAdView adView, Yodo1U3dAdError adError)
    {
        Debug.Log(Yodo1U3dMas.TAG + "BannerV2 ad failed to load with error code: " + adError.ToString());
    }

    private void OnBannerAdClosedEvent(Yodo1U3dBannerAdView adView)
    {
        Debug.Log(Yodo1U3dMas.TAG + "BannerV2 ad closed");
    }

    /// <summary>
    /// (Optional) Show banner ads
    /// </summary>
    public void ShowBannerAdsV2()
    {
        if (bannerAdView != null)
        {
            bannerAdView.Show();
        }
    }

    /// <summary>
    /// (Optional) Hide banner ads
    /// </summary>
    public void HideBannerAdsV2()
    {
        if (bannerAdView != null)
        {
            bannerAdView.Hide();
        }
    }
    #endregion

    #region Interstitial Ad Methods
    private void InitializeInterstitialAds()
    {
        Yodo1U3dMasCallback.Interstitial.OnAdOpenedEvent += OnInterstitialAdOpenedEvent;
        Yodo1U3dMasCallback.Interstitial.OnAdClosedEvent += OnInterstitialAdClosedEvent;
        Yodo1U3dMasCallback.Interstitial.OnAdErrorEvent += OnInterstitialAdErorEvent;
    }

    private void OnInterstitialAdOpenedEvent()
    {
        Debug.Log(Yodo1U3dMas.TAG + "Interstitial ad opened");
    }

    private void OnInterstitialAdClosedEvent()
    {
        Debug.Log(Yodo1U3dMas.TAG + "Interstitial ad closed");
    }

    private void OnInterstitialAdErorEvent(Yodo1U3dAdError adError)
    {
        Debug.Log(Yodo1U3dMas.TAG + "Interstitial ad error - " + adError.ToString());
    }

    public void ShowInterstitialAds()
    {
        if (Yodo1U3dMas.IsInterstitialAdLoaded())
        {
            Yodo1U3dMas.ShowInterstitialAd();
        }
        else
        {
            Debug.Log(Yodo1U3dMas.TAG + "Interstitial ad has not been cached.");
        }
    }
    #endregion

    #region Reward video Ad Methods
    private void InitializeRewardedAds()
    {
        Yodo1U3dMasCallback.Rewarded.OnAdOpenedEvent += OnRewardedAdOpenedEvent;
        Yodo1U3dMasCallback.Rewarded.OnAdClosedEvent += OnRewardedAdClosedEvent;
        Yodo1U3dMasCallback.Rewarded.OnAdReceivedRewardEvent += OnAdReceivedRewardEvent;
        Yodo1U3dMasCallback.Rewarded.OnAdErrorEvent += OnRewardedAdErorEvent;
    }

    private void OnRewardedAdOpenedEvent()
    {
        Debug.Log(Yodo1U3dMas.TAG + "Rewarded ad opened");
    }

    private void OnRewardedAdClosedEvent()
    {
        Debug.Log(Yodo1U3dMas.TAG + "Rewarded ad closed");
    }

    private void OnAdReceivedRewardEvent()
    {
        Debug.Log(Yodo1U3dMas.TAG + "Rewarded ad received reward");
    }

    private void OnRewardedAdErorEvent(Yodo1U3dAdError adError)
    {
        Debug.Log(Yodo1U3dMas.TAG + "Rewarded ad error - " + adError.ToString());
    }

    public void ShowRewardedAds()
    {
        if (Yodo1U3dMas.IsRewardedAdLoaded())
        {
            Yodo1U3dMas.ShowRewardedAd();
        }
        else
        {
            Debug.Log(Yodo1U3dMas.TAG + "Reward video ad has not been cached.");
        }
    }
    #endregion

    #region Yodo1U3dNativeAdView
    Yodo1U3dNativeAdView nativeAdView = null;


    /// <summary>
    /// The banner is displayed automatically after loaded
    /// </summary>
    private void InitializeNativeAds()
    {
        // Clean up native before reusing
        if (nativeAdView != null)
        {
            nativeAdView.Destroy();
            nativeAdView = null;
        }

        nativeAdView = new Yodo1U3dNativeAdView(Yodo1U3dNativeAdPosition.NativeTop | Yodo1U3dNativeAdPosition.NativeHorizontalCenter, 0, 0, 360, 300);

        nativeAdView.SetBackgroundColor(Color.grey);
        // Add Events
        nativeAdView.OnAdLoadedEvent += OnNativeAdLoadedEvent;
        nativeAdView.OnAdFailedToLoadEvent += OnNativeAdFailedToLoadEvent;
       
        // Load native ads, the native ad will be displayed automatically after loaded
        nativeAdView.LoadAd();
    }

    private void OnNativeAdLoadedEvent(Yodo1U3dNativeAdView adView)
    {
        // Banner ad is ready to be shown.
        Debug.Log(Yodo1U3dMas.TAG + "Native ad loaded");
    }

    private void OnNativeAdFailedToLoadEvent(Yodo1U3dNativeAdView adView, Yodo1U3dAdError adError)
    {
        Debug.Log(Yodo1U3dMas.TAG + "Native ad failed to load with error code: " + adError.ToString());
    }
    #endregion
}

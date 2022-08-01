using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Yodo1.MAS;
using UnityEngine.UI;

public class InterstitialAtTimer : MonoBehaviour
{
    [Header("PlacementID (optional) ")]
    public string placementID;
    [Space(10)]
    [Header("Interstitial Ad Timer (Ad will show after specified minutes) ")]
    [Tooltip("Please add time in mins, Interstitial ad will keep showing after the time interval specified.")]
    public float ShowInterstitialAfterMins = 1;
    GameObject adbreak, panel;
    Button btn;

    [Space(10)]
    [Header("Interstitial AD Events (optional)")]
    [SerializeField] UnityEvent OnInterstitialAdLoaded;
    [SerializeField] UnityEvent OnInterstitialAdLoadFailed;
    [SerializeField] UnityEvent OnInterstitialAdOpened;
    [SerializeField] UnityEvent OnInterstitialAdOpenFailed;
    [SerializeField] UnityEvent OnInterstitialAdClosed;

    //[System.Obsolete("Please use `OnAdLoadFailedEvent` and `OnAdOpenFailedEvent` instead.", false)]
    //[SerializeField] UnityEvent OnInterstitialAdError;

    private void Start()
    {
        adbreak = (GameObject)Resources.Load("adbreakpanel");
        InvokeRepeating("ShowInterstitialAd", ShowInterstitialAfterMins * 60f, ShowInterstitialAfterMins * 60);
    }

    private void OnEnable()
    {
        Yodo1U3dInterstitialAd.GetInstance().OnAdLoadedEvent += OnInterstitialAdLoadedEvent;
        Yodo1U3dInterstitialAd.GetInstance().OnAdLoadFailedEvent += OnInterstitialAdLoadFailedEvent;
        Yodo1U3dInterstitialAd.GetInstance().OnAdOpenedEvent += OnInterstitialAdOpenedEvent;
        Yodo1U3dInterstitialAd.GetInstance().OnAdOpenFailedEvent += OnInterstitialAdOpenFailedEvent;
        Yodo1U3dInterstitialAd.GetInstance().OnAdClosedEvent += OnInterstitialAdClosedEvent;

        Yodo1U3dInterstitialAd.GetInstance().LoadAd();
    }

    private void OnDisable()
    {
        Yodo1U3dInterstitialAd.GetInstance().OnAdLoadedEvent -= OnInterstitialAdLoadedEvent;
        Yodo1U3dInterstitialAd.GetInstance().OnAdLoadFailedEvent -= OnInterstitialAdLoadFailedEvent;
        Yodo1U3dInterstitialAd.GetInstance().OnAdOpenedEvent -= OnInterstitialAdOpenedEvent;
        Yodo1U3dInterstitialAd.GetInstance().OnAdOpenFailedEvent -= OnInterstitialAdOpenFailedEvent;
        Yodo1U3dInterstitialAd.GetInstance().OnAdClosedEvent -= OnInterstitialAdClosedEvent;
    }

    private void FixedUpdate()
    {
        if (panel == null || btn == null)
        {
            return;
        }

        if (Screen.width > Screen.height)
        {
            float x = (float)Screen.height / (float)Screen.width;
            btn.transform.localScale = new Vector3(x, 1.0f, 1.0f);
        }
        else
        {
            float y = (float)Screen.width / (float)Screen.height;
            btn.transform.localScale = new Vector3(1.0f, y, 1.0f);
        }
    }

    void ShowInterstitialAd()
    {
        if (panel != null)
        {
            return;
        }
        if (Yodo1U3dInterstitialAd.GetInstance().IsLoaded())
        {
            panel = Instantiate(adbreak, GameObject.FindObjectOfType<Canvas>().transform);
            btn = panel.GetComponentInChildren<Button>();
            btn.interactable = false;
            btn.onClick.AddListener(OnButtonClick);
            panel.SetActive(true);
            StartCoroutine(Invokee(MakeButtonActive, 1f));
        }
        else
        {
            Debug.Log(Yodo1U3dMas.TAG + "NoCode Interstitial ad has not been cached.");
        }
    }

    void MakeButtonActive()
    {
        btn.interactable = true;
    }

    void OnButtonClick()
    {
        ShowIntertitial();
    }

    void ShowIntertitial()
    {
        Destroy(panel);
        adbreak.SetActive(false);

        if (string.IsNullOrEmpty(placementID))
        {
            Yodo1U3dInterstitialAd.GetInstance().ShowAd();
        }
        else
        {
            Yodo1U3dInterstitialAd.GetInstance().ShowAd(placementID);
        }
    }

    private void OnInterstitialAdLoadedEvent(Yodo1U3dInterstitialAd ad)
    {
        Debug.Log(Yodo1U3dMas.TAG + "NoCode Interstitial ad loaded");
        OnInterstitialAdLoaded.Invoke();
    }

    private void OnInterstitialAdLoadFailedEvent(Yodo1U3dInterstitialAd ad, Yodo1U3dAdError adError)
    {
        Debug.Log(Yodo1U3dMas.TAG + "NoCode Interstitial ad load failed, error - " + adError.ToString());
        OnInterstitialAdLoadFailed.Invoke();
        //OnInterstitialAdError.Invoke();
    }

    private void OnInterstitialAdOpenedEvent(Yodo1U3dInterstitialAd ad)
    {
        Debug.Log(Yodo1U3dMas.TAG + "NoCode Interstitial ad opened");
        OnInterstitialAdOpened.Invoke();
    }

    private void OnInterstitialAdOpenFailedEvent(Yodo1U3dInterstitialAd ad, Yodo1U3dAdError adError)
    {
        Debug.Log(Yodo1U3dMas.TAG + "NoCode Interstitial ad open failed, error - " + adError.ToString());
        OnInterstitialAdOpenFailed.Invoke();
        //OnInterstitialAdError.Invoke();
    }

    private void OnInterstitialAdClosedEvent(Yodo1U3dInterstitialAd ad)
    {
        Debug.Log(Yodo1U3dMas.TAG + "NoCode Interstitial ad closed - AdTimer");
        OnInterstitialAdClosed.Invoke();

        Yodo1U3dInterstitialAd.GetInstance().LoadAd();
    }

    public IEnumerator Invokee(System.Action action, float Delay)
    {
        yield return new WaitForSecondsRealtime(Delay);
        if (action != null)
        {
            action();
        }
    }
}

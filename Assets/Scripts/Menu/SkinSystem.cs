using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Skin
{
    Default,
    Demon,
    Riddle,
    Gentle,
    Golden,
    Master,
    Sleepy,
    Zombie,
    Cyberpunk,
    Knight,
    Hacker
}

public class SkinSystem : MonoBehaviour
{
    [SerializeField] private Skin skin;
    [SerializeField] private int cost;

    [SerializeField] private bool is3d;
    [HideInInspector] public bool isSelected;
    private bool isUnlocked;

    [Header("Source")]
    [SerializeField] private AudioSource money;
    [SerializeField] private AudioSource buttonSound;
    [SerializeField] private AudioSource unlockSound;

    [Header("UI")]
    [SerializeField] private Button selectButton;
    [SerializeField] private Button buyButton;

    [Space]
    [SerializeField] private Sprite selected;
    [SerializeField] private Sprite select;

    [Space]
    [SerializeField] private Text selectText;
    [SerializeField] private Text balanceText;

    [Header("Objects")]
    [SerializeField] private GameObject lockObject;
    [SerializeField] private GameObject buy_screen;
    [SerializeField] private GameObject moneyObject;
    [SerializeField] private Lean.Localization.LeanLocalization localization;

    [SerializeField] private List<SkinSystem> otherSkins;
    public Coroutine checkCoroutine;

    public void OnEnable()
    {
        if (!is3d)
        {
            balanceText.text = GameManager.allRedCoins.ToString();
            if (PlayerPrefs.GetString("ActiveSkin2D") == skin.ToString())
                Select(true);
        }
        else
        {
            balanceText.text = GameManager.allOrangeCoins.ToString();
            if (PlayerPrefs.GetString("ActiveSkin3D") == skin.ToString())
                Select(true);
        }
        if (skin != Skin.Default)
        {
            if (is3d)
            {
                if (PlayerPrefsSafe.GetInt("isUnlocked3D" + skin.ToString()) == 1 ? true : false)
                    buy_screen.SetActive(false);
            }
            else
            {
                if (PlayerPrefsSafe.GetInt("isUnlocked2D" + skin.ToString()) == 1 ? true : false)
                    buy_screen.SetActive(false);
            }
        }
        if(isActiveAndEnabled)
            checkCoroutine = StartCoroutine(CheckCoroutine());
    }
    public void Select(bool isStart)
    {
        isSelected = true;
        if (!isStart)
        {
            buttonSound.volume = SingletonManager.soundVolume;
            buttonSound.Play();
        }
        foreach (SkinSystem theSkin in otherSkins)
        {
            theSkin.isSelected = false;
            theSkin.StopCheckCoroutine(checkCoroutine);
        }
        if (is3d)
            PlayerPrefs.SetString("ActiveSkin3D", skin.ToString());
        else
            PlayerPrefs.SetString("ActiveSkin2D", skin.ToString());

        selectButton.enabled = false;
        selectButton.gameObject.GetComponent<Image>().sprite = selected;
        if (localization.CurrentLanguage == "Russian")
            selectText.text = "Выбран";
        else
            selectText.text = "Selected";
    }
    public void StopCheckCoroutine(Coroutine checkCoroutine)
    {
        if (checkCoroutine != null)
            StopCoroutine(this.checkCoroutine);
        this.checkCoroutine = StartCoroutine(CheckCoroutine());
    }
    public void Unlock()
    {
        if (is3d)
        {
            if (GameManager.allOrangeCoins >= cost)
            {
                if (Social.localUser.authenticated && skin == Skin.Golden)
                    Social.ReportProgress(GPS.achievement_richer_than_midas, 101f, (bool success) => { });
                if (buyButton != null)
                    buyButton.enabled = false;
                unlockSound.volume = SingletonManager.soundVolume;
                unlockSound.Play();
                GameManager.allOrangeCoins -= cost;
                balanceText.text = GameManager.allOrangeCoins.ToString();
                MenuManager.orangeCoins.text = GameManager.allOrangeCoins.ToString();
                PlayerPrefsSafe.SetInt("allOrangeCoins", GameManager.allOrangeCoins);
                isUnlocked = true;
                if (is3d)
                    PlayerPrefsSafe.SetInt("isUnlocked3D" + skin.ToString(), isUnlocked ? 1 : 0);
                else
                    PlayerPrefsSafe.SetInt("isUnlocked2D" + skin.ToString(), isUnlocked ? 1 : 0);
                lockObject.gameObject.GetComponent<Animator>().enabled = true;
                StartCoroutine(UnlockCoroutine());
            }
            else
            {
                money.volume = SingletonManager.soundVolume;
                money.Play();
                moneyObject.SetActive(true);
                moneyObject.GetComponent<Animator>().Play("Money!");
            }
        }
        else
        {
            if (GameManager.allRedCoins >= cost)
            {
                if (Social.localUser.authenticated && skin == Skin.Golden)
                    Social.ReportProgress(GPS.achievement_richer_than_midas, 101f, (bool success) => { });

                unlockSound.volume = SingletonManager.soundVolume;
                unlockSound.Play();

                GameManager.allRedCoins -= cost;
                balanceText.text = GameManager.allRedCoins.ToString();
                MenuManager.redCoins.text = GameManager.allRedCoins.ToString();

                PlayerPrefsSafe.SetInt("allRedCoins", GameManager.allRedCoins);
                isUnlocked = true;

                if (is3d)
                    PlayerPrefsSafe.SetInt("isUnlocked3D" + skin.ToString(), isUnlocked ? 1 : 0);
                else
                    PlayerPrefsSafe.SetInt("isUnlocked2D" + skin.ToString(), isUnlocked ? 1 : 0);

                lockObject.gameObject.GetComponent<Animator>().enabled = true;
                StartCoroutine(UnlockCoroutine());
            }
            else
            {
                money.volume = SingletonManager.soundVolume;
                money.Play();
                moneyObject.SetActive(true);
                moneyObject.GetComponent<Animator>().Play("Money!");
            }
        }
    }
    private IEnumerator UnlockCoroutine()
    {
        yield return new WaitForSeconds(0.85f);
        buy_screen.SetActive(false);
        lockObject.SetActive(false);
    }
    public IEnumerator CheckCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.02f);
            if (!isSelected)
            {
                selectButton.enabled = true;
                selectButton.gameObject.GetComponent<Image>().sprite = select;

                if (localization.CurrentLanguage == "Russian")
                    selectText.text = "Выбрать";
                else
                    selectText.text = "Select";
            }
        }
    }
    public void OKButton()
    {
        moneyObject.SetActive(false);
    }
}

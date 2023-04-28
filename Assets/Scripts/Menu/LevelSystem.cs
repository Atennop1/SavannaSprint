using UnityEngine;
using UnityEngine.UI;


public class LevelSystem : MonoBehaviour
{
    [SerializeField] private Lean.Localization.LeanLocalization localizator;
    [SerializeField] private BonusType type;

    [Header("UI")]
    [SerializeField] private Text levelTextNumber;
    [SerializeField] private Text maxLevelText;

    [Space]
    [SerializeField] private Text levelText;
    [SerializeField] private Text costText;

    [Space]
    [SerializeField] private Text moneyText;
    [SerializeField] private Text moneyText2;

    [Space]
    [SerializeField] private Text coins;
    [SerializeField] private Text coins2;

    [Space]
    [SerializeField] private Image moneySprite;
    [SerializeField] private Sprite redCoin;
    [SerializeField] private Sprite orangeCoin;

    [Header("Source")]
    [SerializeField] private AudioSource money;
    [SerializeField] private AudioSource level_up;
    [SerializeField] private AudioSource max_level;

    [Header("Objects")]
    [SerializeField] private GameObject moneyObject;
    [SerializeField] private MenuManager menu;
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private GameObject[] levels;

    private int curLevel;
    private bool is3d = true;
    private bool isRussian;

    public void OnEnable()
    {
        if (PlayerPrefs.GetInt("BonusesCurrency") == 1)
            is3d = true;
        else
            is3d = false;

        if (localizator.CurrentLanguage == "Russian")
            isRussian = true;
        else
            isRussian = false;

        if (!isRussian)
        {
            levelText.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(58, 87.79236f, 0);
            levelText.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(783.4067f, 199.8443f);
            levelText.gameObject.GetComponent<RectTransform>().localScale = new Vector3(0.26765f, 0.32841f, 0.26765f);
        }
        else
        {
            levelText.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(55.5f, 87.79236f, 0);
            levelText.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(1061.334f, 199.8443f);
            levelText.gameObject.GetComponent<RectTransform>().localScale = new Vector3(0.2215324f, 0.2718231f, 0.2215324f);
        }

        switch (type)
        {
            case BonusType.X2:
                if (PlayerPrefsSafe.HasKey("x2"))
                    curLevel = PlayerPrefsSafe.GetInt("x2");
                PlayerPrefsSafe.SetInt("x2", curLevel);
                break;
            case BonusType.X2Coins:
                if (PlayerPrefsSafe.HasKey("x2Coins"))
                    curLevel = PlayerPrefsSafe.GetInt("x2Coins");
                PlayerPrefsSafe.SetInt("x2Coins", curLevel);
                break;
            case BonusType.Magnet:
                if (PlayerPrefsSafe.HasKey("magnet"))
                    curLevel = PlayerPrefsSafe.GetInt("magnet");
                PlayerPrefsSafe.SetInt("magnet", curLevel);
                break;
        }

        if (PlayerPrefs.GetInt("BonusesCurrency") == 1)
            is3d = true;
        else
            is3d = false;

        ChangeCurrency(is3d);

        coins.text = _gameManager.allOrangeCoins.ToString();
        coins2.text = _gameManager.allRedCoins.ToString();

        UpdateButton();
    }
    public void Click()
    {
        moneyText.text = _gameManager.allOrangeCoins.ToString();
        moneyText2.text = _gameManager.allRedCoins.ToString();

        if (curLevel < 8)
        {
            if (_gameManager.allOrangeCoins >= curLevel * 1000 && is3d)
                _gameManager.allOrangeCoins -= curLevel * 1000;
            else if (_gameManager.allRedCoins >= curLevel * 500 && !is3d)
                _gameManager.allRedCoins -= curLevel * 500;
            else
                return;

            if (curLevel < 7)
            {
                level_up.volume = SingletonManager.instance.soundVolume;
                level_up.Play();
            }
            else
            {
                max_level.volume = SingletonManager.instance.soundVolume;
                max_level.Play();
            }

            menu.UpdateText();
            _gameManager.UpdateText();

            curLevel++;
            UpdateButton();

            switch (type)
            {
                case BonusType.X2:
                    PlayerPrefsSafe.SetInt("x2", curLevel);
                    break;
                case BonusType.X2Coins:
                    PlayerPrefsSafe.SetInt("x2Coins", curLevel);
                    break;
                case BonusType.Magnet:
                    PlayerPrefsSafe.SetInt("magnet", curLevel);
                    break;
            }
        }
        else if (curLevel < 8)
        {
            GetComponent<Button>().enabled = false;
            money.volume = SingletonManager.instance.soundVolume;

            money.Play();
            moneyObject.SetActive(true);
            moneyObject.GetComponent<Animator>().Play("Money!");
        }

        coins.text = _gameManager.allOrangeCoins.ToString();
        coins2.text = _gameManager.allRedCoins.ToString();
    }
    void UpdateButton()
    {
        if (curLevel < 8)
        {
            levelTextNumber.text = $"{curLevel}";

            if (is3d)
                costText.text = $"{curLevel * 1000}";
            else
                costText.text = $"{curLevel * 500}";

            foreach (GameObject level in levels)
            {
                if (level == levels[curLevel])
                    break;
                level.SetActive(true);
            }
        }
        else
        {
            GetComponent<Button>().enabled = false;
            foreach (GameObject level in levels)
            {
                if (level == levels[curLevel])
                    break;
                level.SetActive(true);
            }

            levelTextNumber.gameObject.SetActive(false);
            maxLevelText.gameObject.SetActive(true);

            if (isRussian)
                levelText.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(111, 26f, 0);
            else
                levelText.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(119, 26f, 0);

            levelText.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(1064.964f, 432.9235f);
            levelText.gameObject.GetComponent<RectTransform>().localScale = new Vector3(0.2770976f, 0.3399989f, 0.2770976f);
            levelText.alignment = TextAnchor.MiddleCenter;

            levels[0].transform.parent.GetComponent<RectTransform>().anchoredPosition = new Vector3(96.7f, -67f, 0);
            costText.gameObject.SetActive(false);
        }
    }
    public void OKButton()
    {
        GetComponent<Button>().enabled = true;
        moneyObject.SetActive(false);
    }
    public void ChangeCurrency(bool is3d)
    {
        if (is3d)
        {
            this.is3d = true;
            costText.text = $"{curLevel * 1000}";
            moneySprite.sprite = orangeCoin;
        }
        else
        {
            this.is3d = false;
            costText.text = $"{curLevel * 500}";
            moneySprite.sprite = redCoin;
        }
    }
}

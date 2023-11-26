using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public enum SkinType
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

public class SkinView : MonoBehaviour
{
    [SerializeField] private bool _is3DMode;
    [SerializeField] private Skin _skin;

    [Header("UI")]
    [SerializeField] private Image _selectButton;
    [SerializeField] private Text _selectText;

    [Header("Objects")]
    [SerializeField] private Animator _lockAnimator;
    [SerializeField] private GameObject _buyScreen;
    [SerializeField] private SkinsHandler _handler;

    public void Select()
    {
        _handler.UnSelectAll();

        _skin.Select(_is3DMode);
        _selectButton.sprite = _handler.SelectedSprite;
        _selectText.text = _handler.Localization.CurrentLanguage == "Russian" ? "Выбран" : "Selected";
    }

    public void UnSelect()
    {
        _skin.UnSelect();
        _selectButton.sprite = _handler.NotSelectedSprite;
        _selectText.text = _handler.Localization.CurrentLanguage == "Russian" ? "Выбрать" : "Select";
    }

    public void Unlock()
    {
        if (_skin.TryBuy(_is3DMode, _handler.StatisticsView.StatisticsModel))
        {
            _lockAnimator.enabled = true;
            PlaySound(_handler.UnlockSource);
            StartCoroutine(UnlockCoroutine());
            _handler.StatisticsView.UpdateText();
        }
        else
        {
            PlaySound(_handler.NotEnoughMoneySource);
            _handler.ShowNotEnoughMoneyPanel();
        }
    }

    public void PlaySound(AudioSource source)
    {
        source.volume = MusicPlayer.Instance.Volume;
        source.Play();
    }

    private IEnumerator UnlockCoroutine()
    {
        yield return new WaitForSeconds(0.85f);
        _buyScreen.SetActive(false);
    }

    private void OnEnable()
    {
        InitSkin();
    }

    private void InitSkin()
    {
        _skin.Init(_is3DMode);

        if (_skin.IsSelected)
            Select();

        if (_skin.IsUnlocked)
            _buyScreen?.SetActive(false);
    }
}

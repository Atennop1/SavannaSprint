using UnityEngine;
using System.Collections.Generic;
using Lean.Localization;
using UnityEngine.UI;

public class SkinsHandler : MonoBehaviour
{
    [field: SerializeField, Header("Objects")] public PlayerStatisticsView StatisticsView { get; private set; }
    [field: SerializeField] public LeanLocalization Localization { get; private set; }
    [SerializeField] private Animator _notEnoughMoneyAnimator;
            
    [field: SerializeField, Header("UI")] public Sprite SelectedSprite { get; private set; }
    [field: SerializeField] public Sprite NotSelectedSprite { get; private set; }

    [field: SerializeField, Header("Source")] public AudioSource NotEnoughMoneySource { get; private set; }
    [field: SerializeField] public AudioSource UnlockSource { get; private set; }

    [SerializeField] private List<SkinView> _views;

    public void UnSelectAll()
    {
        foreach (var view in _views)
            view.UnSelect();
    }
    
    public void CloseNotEnoughMoneyPanel()
    {
        _notEnoughMoneyAnimator.gameObject.SetActive(false);
    }

    public void ShowNotEnoughMoneyPanel()
    {
        _notEnoughMoneyAnimator.gameObject.SetActive(true);
        _notEnoughMoneyAnimator.Play("Money!");
    }
}
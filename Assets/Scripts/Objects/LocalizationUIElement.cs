using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class LocalizationUIElement : MonoCache
{
    [SerializeField] private Lean.Localization.LeanLocalization localization;

    [Space]
    [SerializeField] private Vector3 positionEN;
    [SerializeField] private Vector3 positionRU;

    [Space]
    [SerializeField] private Vector3 sizeEN;
    [SerializeField] private Vector3 sizeRU;

    [Space]
    [SerializeField] private Vector3 scaleEN;
    [SerializeField] private Vector3 scaleRU;

    private RectTransform thisRect;

    public void OnEnable()
    {
        thisRect = GetComponent<RectTransform>();
        localization.CurrentLanguage = PlayerPrefs.GetString("Language");

        if (localization.CurrentLanguage == "Russian")
        {
            thisRect.anchoredPosition = positionRU;
            thisRect.sizeDelta = sizeRU;
            thisRect.localScale = scaleRU;
        }
        else
        {
            thisRect.anchoredPosition = positionEN;
            thisRect.sizeDelta = sizeEN;
            thisRect.localScale = scaleEN;
        }
    }
}

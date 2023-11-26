using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(RectTransform))]
public class LocalizedUIElement : MonoCache
{
    [SerializeField] private Lean.Localization.LeanLocalization _localization;
    
    [Space]
    [FormerlySerializedAs("positionEN")] [SerializeField] private Vector3 _positionEN;
    [FormerlySerializedAs("positionRU")] [SerializeField] private Vector3 _positionRU;
    
    [Space]
    [FormerlySerializedAs("sizeEN")] [SerializeField] private Vector3 _sizeEN;
    [FormerlySerializedAs("sizeRU")] [SerializeField] private Vector3 _sizeRU;
    
    [Space]
    [FormerlySerializedAs("scaleEN")] [SerializeField] private Vector3 _scaleEN;
    [FormerlySerializedAs("scaleRU")] [SerializeField] private Vector3 _scaleRU;

    private RectTransform _thisRect;

    public override void OnEnable()
    {
        base.OnEnable();
        _thisRect = GetComponent<RectTransform>();
        _localization.CurrentLanguage = PlayerPrefs.GetString("Language");

        if (_localization.CurrentLanguage == "Russian")
        {
            _thisRect.anchoredPosition = _positionRU;
            _thisRect.sizeDelta = _sizeRU;
            _thisRect.localScale = _scaleRU;
        }
        else
        {
            _thisRect.anchoredPosition = _positionEN;
            _thisRect.sizeDelta = _sizeEN;
            _thisRect.localScale = _scaleEN;
        }
    }
}

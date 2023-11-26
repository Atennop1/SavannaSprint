using UnityEngine;
using UnityEngine.UI;

public class TemporaryBonusHandler : BonusHandler
{
    [Space]
    [SerializeField] private Slider _bonusSlider;
    
    private float _sliderValue;

    public void SelfInit(float sliderValue)
    {
        _sliderValue = sliderValue;
    }

    protected override void ConcreteBonusInit()
    {
        (Bonus as TemporaryBonus)?.SelfInit(_bonusSlider, _sliderValue);
    }
}

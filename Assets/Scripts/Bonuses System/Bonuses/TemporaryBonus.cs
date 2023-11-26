using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TemporaryBonus : Bonus
{
    private Slider _slider; 
    private string _name;

    public float SliderValue { get; private set; }

    public TemporaryBonus(BonusType type) : base(type) { }

    public void SelfInit(Slider bonusSlider, float sliderValue)
    {
        _slider = bonusSlider;
        SliderValue = sliderValue;

        _name = Type.ToString();
        _name = _name[0].ToString().ToLower() + _name.Substring(1);
    }

    protected override void TemplateSave()
    {
        PlayerPrefsSafe.SetFloat("bonus" + Type + "Value", _slider.value);
    }

    protected override void TemplateLoad()
    {
        SliderValue = PlayerPrefsSafe.GetFloat("bonus" + Type + "Value");
    }

    protected override void TemplateClear()
    {
        PlayerPrefsSafe.SetFloat("bonus" + Type + "Value", 0);
    }

    protected override IEnumerator TemplateActivate()
    {
        _slider.transform.parent.gameObject.SetActive(true);
        _slider.value = SliderValue;

        var bonusTime = PlayerPrefsSafe.GetInt(_name);
        if (PlayerPrefsSafe.GetInt(_name) % 2 != 0)
            bonusTime++;

        var wait = new WaitForFixedUpdate();
        while (_slider.value > 0 && CanContinue())
        {   
            _slider.value -= 0.001f * (3f / bonusTime);
            yield return wait;
        }

        StopRunningCoroutine();
    }

    protected override void TerminationCallback()
    {
        _slider.transform.parent.gameObject.SetActive(false);
    }
}

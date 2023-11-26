using UnityEngine.UI;
using UnityEngine;

public class FPSDisplay : MonoCache
{
    [SerializeField] private Text _fpsText;

    private readonly float _pollingTime = 0.5f;
    private float _time;
    private int _framesCount;

    private void Awake()
        => _fpsText.gameObject.SetActive(PlayerPrefs.GetInt("fpsIntPP") == 1);
    
    protected override void OnTick()
    {
        if (PlayerPrefs.GetInt("fpsIntPP") != 1)
            return;
        
        _time += Time.deltaTime;
        _framesCount++;

        if (!(_time >= _pollingTime)) 
            return;
        
        var frameRate = Mathf.RoundToInt(_framesCount / _time);
        _fpsText.text = frameRate.ToString();

        _time -= _pollingTime;
        _framesCount = 0;
    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    [SerializeField] private Slider _loadingSlider;
    [SerializeField] private GameObject _loadingScreen;

    private Animator _fadeAnimator;
    private int _levelToLoad;

    public void LoadScene(int levelToLoad)
    {
        _levelToLoad = levelToLoad;
        _fadeAnimator.SetTrigger("fade");
    }

    public void OnFadeComplete()
    {
        StartCoroutine(LoadSceneAsync());
    }

    private IEnumerator LoadSceneAsync()
    {
        var operation = SceneManager.LoadSceneAsync(_levelToLoad);
        _loadingScreen.SetActive(true);
        
        while (!operation.isDone)
        {
            var progress = Mathf.Clamp01(operation.progress / .9f);
            _loadingSlider.value = progress;
            yield return null;
        }
    }

    private void Start()
    {
        _fadeAnimator = GetComponent<Animator>();
    }
}

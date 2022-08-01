using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    [SerializeField] private Slider loadingSlider;
    [SerializeField] private GameObject loadingScreen;

    private bool is2d;
    public static int levelToLoad;
    private static Animator anim;
    [HideInInspector] public PlayerController player;
    [HideInInspector] public PlayerController2D player2d;
    private void Start()
    {
        is2d = SceneManager.GetActiveScene().name == "2d World";
        player = PlayerController.instance;
        player2d = PlayerController2D.instance;
        anim = GetComponent<Animator>();
    }
    public void FadeToLevel()
    {
        anim.SetTrigger("fade");
    }
    public void OnFadeComplete()
    {
        StartCoroutine(LoadingScreenOnFade());
    }
    public void StartAnimation()
    {
        if (SceneManager.GetActiveScene().name != "Menu")
        {
            if (is2d)
                StartCoroutine(player2d.StartMethod());
            else
                StartCoroutine(player.StartMethod());
        }
    }
    IEnumerator LoadingScreenOnFade()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(levelToLoad);
        loadingScreen.SetActive(true);
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / .9f);
            loadingSlider.value = progress;
            yield return null;
        }
    }
}

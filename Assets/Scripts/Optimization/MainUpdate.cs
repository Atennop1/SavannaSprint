using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainUpdate : MonoBehaviour
{
    private Scene _currentScene;

    private void Update()
    {
        foreach (var monoCache in MonoCache.AllUpdates.Where(monoCache => monoCache.gameObject.scene == _currentScene).ToList())
            monoCache.Tick();
    }

    private void Awake()
    {
        _currentScene = SceneManager.GetActiveScene();
    }
}

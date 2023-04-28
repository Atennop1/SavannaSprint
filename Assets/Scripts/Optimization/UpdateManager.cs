using UnityEngine;
using UnityEngine.SceneManagement;

public class UpdateManager : MonoBehaviour
{
    public static UpdateManager instance;
    public void Start()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(instance);
    }
    
    public void Update()
    {
        for (int i = 0; i < MonoCache.AllUpdates.Count; i++)
            if (MonoCache.AllUpdates[i].gameObject.scene == SceneManager.GetActiveScene())
                MonoCache.AllUpdates[i].Tick();
    }
}

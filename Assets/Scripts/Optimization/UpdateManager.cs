using System.Collections;
using System.Collections.Generic;
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
        for (int i = 0; i < MonoCache.allUpdates.Count; i++)
            if (MonoCache.allUpdates[i].gameObject.scene == SceneManager.GetActiveScene())
                MonoCache.allUpdates[i].Tick();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolMonoBehaviour : MonoBehaviour
{
    [SerializeField] protected int _count;
    [SerializeField] private List<Component> LODS;

    public Component Prefab { get; private set; }
    public Pool<Component> Pool { get; set; }

    protected void Awake()
    {
        if (PlayerPrefs.GetInt("GraphicsQuality") == 0)
            Prefab = LODS[2];
        else if (PlayerPrefs.GetInt("GraphicsQuality") == 1)
            Prefab = LODS[1];
        else
            Prefab = LODS[0];
        
        Pool = new Pool<Component>(Prefab, _count, this.transform);
    }
}

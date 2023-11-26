using System.Collections.Generic;
using UnityEngine;

public abstract class PoolSetup : MonoBehaviour
{
    [SerializeField] protected int _count;
    [SerializeField] private List<Component> _LODS;

    public Component Prefab { get; private set; }
    public Pool<Component> Pool => _pool;

    private Pool<Component> _pool;

    protected void Awake()
    {
        if (PlayerPrefs.GetInt("GraphicsQuality") == 0)
            Prefab = _LODS[2];
        else if (PlayerPrefs.GetInt("GraphicsQuality") == 1)
            Prefab = _LODS[1];
        else
            Prefab = _LODS[0];
        
        CreatePool(_count, out _pool);
    }

    protected virtual void CreatePool(int count, out Pool<Component> pool) { pool = null; }
}

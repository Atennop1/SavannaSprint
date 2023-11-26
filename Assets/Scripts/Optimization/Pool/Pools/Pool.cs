using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Pool<T> where T: Component
{
    private readonly T _prefab;
    private List<T> _pool;
    private readonly Transform _container;

    public Pool(T prefab, int count, Transform container = null)
    {
        _prefab = prefab;
        _container = container;
    }

    public T GetFreeElement(Vector3 position, Transform parent)
    {
        foreach (var t in _pool.Where(t => !t.gameObject.activeInHierarchy))
        {
            t.gameObject.SetActive(true);

            t.transform.position = position;
            t.transform.SetParent(parent);
                
            return t;
        }

        return CreateObject(true);
    }

    protected void CreatePool(int count)
    {
        _pool = new List<T>();

        for (int i = 0; i< count; i++)
            CreateObject();
    }

    private T CreateObject(bool isActiveByDefault = false)
    {
        var createdObject = Object.Instantiate(_prefab, _container);
        createdObject.gameObject.SetActive(isActiveByDefault);
        InitializeObject(createdObject);

        _pool.Add(createdObject);
        return createdObject;
    }

    protected virtual void InitializeObject(T obj) { }
}

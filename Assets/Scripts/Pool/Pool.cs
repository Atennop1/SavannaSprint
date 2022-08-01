using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pool<T> where T: Component
{
    private T _prefab;
    private List<T> _pool;
    private Transform _container;

    public Pool(T prefab, int count, Transform container = null)
    {
        _prefab = prefab;
        _container = container;

        CreatePool(count);
    }

    public void CreatePool(int count)
    {
        _pool = new List<T>();

        for (int i = 0; i< count; i++)
            CreateObject();
    }

    private T CreateObject(bool isActiveByDefault = false)
    {
        T createdObject = Object.Instantiate(_prefab, _container);
        createdObject.gameObject.SetActive(isActiveByDefault);
        InitializeObject(createdObject);

        _pool.Add(createdObject);
        return createdObject;
    }

    public T GetFreeElement(Vector3 position, Transform parent)
    {
        for (int i = 0; i < _pool.Count; i++)
        {
            if (!_pool[i].gameObject.activeInHierarchy)
            {
                _pool[i].gameObject.SetActive(true);

                _pool[i].transform.position = position;
                _pool[i].transform.SetParent(parent);
                
                return _pool[i];
            }
        }

        return CreateObject(true);
    }

    public virtual void InitializeObject(T obj) { }
}

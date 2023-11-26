using System.Collections.Generic;
using UnityEngine;

public class MonoCache : MonoBehaviour
{
    public static readonly List<MonoCache> AllUpdates = new List<MonoCache>(1000);

    public virtual void OnEnable() => AllUpdates.Add(this);
    public virtual void OnDisable() => AllUpdates.Remove(this);
    public virtual void OnDestroy() => AllUpdates.Remove(this);

    public void Tick() => OnTick();
    protected virtual void OnTick() { }
}

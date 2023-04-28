using System.Collections.Generic;
using UnityEngine;

public class MonoCache : MonoBehaviour
{
    public static List<MonoCache> AllUpdates = new List<MonoCache>(100);
    private void OnEnable() => AllUpdates.Add(this);
    private void OnDisable() => AllUpdates.Remove(this);
    private void OnDestroy() => AllUpdates.Remove(this);
    public void Tick() => OnTick();
    public virtual void OnTick() { }
}

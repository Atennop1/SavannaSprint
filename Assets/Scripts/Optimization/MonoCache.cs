using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoCache : MonoBehaviour
{
    public static List<MonoCache> allUpdates = new List<MonoCache>(100);
    private void OnEnable() => allUpdates.Add(this);
    private void OnDisable() => allUpdates.Remove(this);
    private void OnDestroy() => allUpdates.Remove(this);
    public void Tick() => OnTick();
    public virtual void OnTick() { }
}

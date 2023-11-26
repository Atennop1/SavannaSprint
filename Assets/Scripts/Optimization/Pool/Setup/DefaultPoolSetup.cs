using UnityEngine;

public class DefaultPoolSetup : PoolSetup
{
    protected override void CreatePool(int count, out Pool<Component> pool)
    { 
        pool = new DefaultPool(Prefab, _count, transform);
    }
}

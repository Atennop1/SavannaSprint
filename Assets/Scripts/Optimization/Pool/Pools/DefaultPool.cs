using UnityEngine;

public class DefaultPool : Pool<Component>
{
    public DefaultPool(Component prefab, int count, Transform container = null) 
        : base(prefab, count, container) 
    { 
        CreatePool(count);
    }

    protected override void InitializeObject(Component obj) { }
}

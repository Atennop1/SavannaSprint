using UnityEngine;

public class SphereRockPoolSetup : PoolSetup
{
    [SerializeField] private PlayerForwardMovement _forwardMovement;

    protected override void CreatePool(int count, out Pool<Component> pool)
    { 
        pool = new SphereRockPool(_forwardMovement, Prefab.GetComponent<SphereRockSetup>(), _count, transform);
    }
}

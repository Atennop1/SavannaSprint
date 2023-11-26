using UnityEngine;

public class SphereRockPool : Pool<Component>
{
    private readonly PlayerForwardMovement _forwardMovement;

    public SphereRockPool(PlayerForwardMovement forwardMovement, SphereRockSetup prefab, int count, Transform container = null) 
        : base(prefab, count, container) 
    { 
        _forwardMovement = forwardMovement;
        CreatePool(count);
    }

    protected override void InitializeObject(Component obj)
    { 
        obj.GetComponent<SphereRockSetup>().Init(_forwardMovement);
    }
}

using UnityEngine;

public class CoinPool : Pool<Component>
{
    private readonly Player _player;

    public CoinPool(Player player, CoinMove prefab, int count, Transform container = null) 
        : base(prefab, count, container) 
    { 
        _player = player;
        CreatePool(count);
    }

    protected override void InitializeObject(Component obj)
    { 
        obj.gameObject.GetComponent<CoinMove>().Init(_player);
    }
}

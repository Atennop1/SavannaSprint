using UnityEngine;

public class CoinPoolSetup : PoolSetup
{
    [SerializeField] private Player _player;

    protected override void CreatePool(int count, out Pool<Component> pool)
    { 
        pool = new CoinPool(_player, Prefab.GetComponent<CoinMove>(), _count, transform);
    }
}

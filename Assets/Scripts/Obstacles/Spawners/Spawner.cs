using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

public abstract class Spawner : MonoCache
{
    [field: SerializeField] public Player Player { get; private set; }
    [field: SerializeField] protected PlayerStatisticsView StatisticsView { get; private set; }
    [field: SerializeField] protected ObstaclesData ObstaclesData { get; private set; }
    [SerializeField] private List<PoolSetup> _pools;

    protected PlayerForwardMovement PlayerForwardMovement { get; private set; }
    protected bool Is3DMode { get; private set; }

    protected PoolSetup GetPool(Component component)
    {
        return _pools.FirstOrDefault(t => component == t.Prefab);
    }

    private void Awake()
    {
        ObstaclesData.Setup();
        PlayerForwardMovement = Player.GetPlayerPart<PlayerForwardMovement>();
        Is3DMode = SceneManager.GetActiveScene().name == "3d World";
    }
}
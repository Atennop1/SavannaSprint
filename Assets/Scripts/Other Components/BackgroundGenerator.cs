using System.Collections.Generic;
using UnityEngine;

public class BackgroundGenerator : MonoCache
{
    [Header("Transforms")]
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private Transform _leftBackgroundParentTransform;
    [SerializeField] private Transform _rightBackgroundParentTransform;

    [Header("Values")]
    [SerializeField] private float _zSize;
    [SerializeField] private int _startCount;
    [SerializeField] private Vector3 _spawnPosition;

    [Header("Obstacles Data")]
    [SerializeField] private ObstaclesData _obstaclesData;

    private readonly List<GameObject> _activeBackgroundsLeft = new List<GameObject>();
    private readonly List<GameObject> _activeBackgroundsRight = new List<GameObject>();

    protected override void OnTick()
    {
        if (!(_playerTransform.position.z > _spawnPosition.z - _zSize * _startCount)) 
            return;
        
        _spawnPosition.z += _zSize;
        DeleteBackground(true);
        DeleteBackground(false);
    }

    private void Start()
    {
        _obstaclesData.Setup();

        for (var i = 0; i < _startCount; i++)
        {
            SpawnBackground(true);
            SpawnBackground(false);
            _spawnPosition.z += _zSize;
        }
    }

    private void SpawnBackground(bool right)
    {
        var obj = right ? _obstaclesData.CanyonRight : _obstaclesData.CanyonLeft;
        var position = new Vector3(right ? _spawnPosition.x : -_spawnPosition.x, _spawnPosition.y, _spawnPosition.z - _zSize * 2);
        var rotation = right ? _obstaclesData.CanyonRight.transform.rotation : _obstaclesData.CanyonLeft.transform.rotation;
        var parent = right ? _rightBackgroundParentTransform : _leftBackgroundParentTransform;

        var nextBackground = Instantiate(obj, position, rotation, parent);
        if (right) _activeBackgroundsRight.Add(nextBackground);
        else _activeBackgroundsLeft.Add(nextBackground);
    }

    private void DeleteBackground(bool right)
    {
        var nearestPlatform = right ? _activeBackgroundsRight[0] : _activeBackgroundsLeft[0];
        nearestPlatform.transform.position = new Vector3(right ? _spawnPosition.x : -_spawnPosition.x, _spawnPosition.y, _spawnPosition.z - _zSize * 3);

        if (right)
        {
            _activeBackgroundsRight.Remove(nearestPlatform);
            _activeBackgroundsRight.Add(nearestPlatform);
        }
        else
        {
            _activeBackgroundsLeft.Remove(nearestPlatform);
            _activeBackgroundsLeft.Add(nearestPlatform);
        }
    }
}

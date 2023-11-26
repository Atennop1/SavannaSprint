using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlatformsGenerator : MonoCache
{
    [Header("Transforms")]
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private Transform _parentTransform;

    [Header("Values")]
    [SerializeField] private float _zSize;
    [SerializeField] private int _startCount;

    [Header("Obstacles Data")]
    [SerializeField] private ObstaclesData _obstaclesData;

    private bool _is3DMode;
    private float _spawnZPosition;

    private readonly List<MeshRenderer> _activePlatforms = new List<MeshRenderer>();

    protected override void OnTick()
    {
        if (!(_playerTransform.position.z > _spawnZPosition - _zSize * _startCount)) 
            return;
        
        _spawnZPosition += _zSize;
        DeletePlatform();
    }

    private void Start()
    {
        _obstaclesData.Setup();
        _is3DMode = SceneManager.GetActiveScene().name == "3d World";

        for (int i = 0; i < _startCount; i++)
            SpawnPlatform();
    }

    private void SpawnPlatform()
    {
        var nextTile = Instantiate(_obstaclesData.Platform, transform.forward * (_spawnZPosition - (_is3DMode ? 36 : 19.6f)), _obstaclesData.Platform.transform.rotation, _parentTransform);
        _activePlatforms.Add(nextTile);
        _spawnZPosition += _zSize;
    }

    private void DeletePlatform()
    {
        var lastPlatform = _activePlatforms[0];
        _activePlatforms[0].transform.position = transform.forward * (_spawnZPosition - (_is3DMode ? 36 : 19.6f) - _zSize);

        _activePlatforms.Remove(lastPlatform);
        _activePlatforms.Add(lastPlatform);
    }
}

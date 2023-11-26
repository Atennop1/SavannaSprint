using System.Collections.Generic;
using UnityEngine;

public enum LinePosition 
{ 
    Left = -1, 
    Center = 0, 
    Right = 1 
}

public class ObstacleSpawner : Spawner
{
    [Space]
    [SerializeField] private CoinsSpawner _coinsSpawner;
    [SerializeField] private int _startMaps;
    [SerializeField] private float _obstaclesDisableDistance;

    private float _currentSpaceBetweenObstacles;
    private List<GameObject> _activeMaps;
    private GameObject lastObstacle;

    public void InitValues3D()
    {
        if (!StatisticsView.TempStatisticsModel.IsSpaceValid)
        {
            _currentSpaceBetweenObstacles = 30;
            StatisticsView.TempStatisticsModel.IncreaseSpaceBetweenObstacles(30);
        }
        else
        {
            _currentSpaceBetweenObstacles = StatisticsView.TempStatisticsModel.SpaceBetweenObstacles * (30f / 17f);
        }
    }

    public void UpdateValues3D()
    {
        _coinsSpawner.UpdateValues();

        _currentSpaceBetweenObstacles += 0.03f;
        StatisticsView.TempStatisticsModel.IncreaseSpaceBetweenObstacles(0.03f);
    }

    public void InitValues2D()
    {
        _coinsSpawner.UpdateValues();

        if (!StatisticsView.TempStatisticsModel.IsSpaceValid)
        {
            _currentSpaceBetweenObstacles = 17;
            StatisticsView.TempStatisticsModel.IncreaseSpaceBetweenObstacles(17);
        }
        else
        {
            _currentSpaceBetweenObstacles = StatisticsView.TempStatisticsModel.SpaceBetweenObstacles * (17f / 30f);
        }
    }

    public void UpdateValues2D()
    {
        _coinsSpawner.UpdateValues();

        _currentSpaceBetweenObstacles += 0.03f;
        StatisticsView.TempStatisticsModel.IncreaseSpaceBetweenObstacles(0.03f);
    }

    protected override void OnTick()
    {
        if (!(_activeMaps[0].transform.position.z < Player.transform.position.z - _obstaclesDisableDistance)) 
            return;
        
        RemoveFirstActiveMap();
        AddActiveMap(MakeMap());
    }

    private void Start()
    {
        _activeMaps = new List<GameObject>();
        for (int i = 0; i < _startMaps; i++)
            AddActiveMap(MakeMap());
    }

    private void RemoveFirstActiveMap()
    {  
        foreach(Transform child in _activeMaps[0].transform)
            child.gameObject.SetActive(false);
    
        _activeMaps.RemoveAt(0);
    }

    private void AddActiveMap(GameObject map)
    {
        map.transform.position = _activeMaps.Count > 0 ? (_activeMaps[_activeMaps.Count - 1].transform.position + Vector3.forward * _currentSpaceBetweenObstacles) : (Is3DMode ? new Vector3(0, 0, 30) : new Vector3(0, 0, 15));
        _activeMaps.Add(map);
    }

    private GameObject MakeMap()
    {
        var result = new GameObject();
        result.transform.SetParent(transform);

        var firstUsedLine = (LinePosition)(-2);
        var secondUsedLine = (LinePosition)(-2);

        for (var numberOfObstacle = 0; numberOfObstacle < (Is3DMode ? Random.Range(1, 4) : 1); numberOfObstacle++)
        {
            float chance = 0;
            float currentChance = Random.Range(0, 101);
            
            foreach (var data in ObstaclesData.Obstacles)
            {
                data.SetupLOD();
                var obstacleObject = data.SelectedLOD;

                chance += data.Chance;
                if (!(currentChance <= chance) || obstacleObject == lastObstacle) 
                    continue;
                
                if (!data.IsForOneLine)
                {
                    if (numberOfObstacle != 0)
                        continue;

                    numberOfObstacle = 4;
                }

                var randomLine = LinePosition.Center;
                if (Is3DMode)
                    while (randomLine == firstUsedLine || randomLine == secondUsedLine)
                        randomLine = (LinePosition)Random.Range(-1, 2);

                data.Setup(randomLine, _currentSpaceBetweenObstacles);
                lastObstacle = obstacleObject;

                if (Random.Range(1, 5) != 1)
                    _coinsSpawner.CreateCoins(data.CoinsStyle, data.CoinsPosition, result);

                GetPool(obstacleObject.transform)?.Pool.GetFreeElement(new Vector3(data.Position.x, data.Position.y, data.Position.z - 2), result.transform);
                    
                if (numberOfObstacle == 0) firstUsedLine = randomLine;
                else secondUsedLine = randomLine;

                break;
            }
        }

        return result;
    }
}

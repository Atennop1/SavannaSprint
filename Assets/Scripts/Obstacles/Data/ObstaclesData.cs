using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ObstaclesData", fileName = "New ObstaclesData")]
public class ObstaclesData : ScriptableObject
{
    [field: SerializeField] public List<ObstacleData> Obstacles { get; private set; }

    [Header("Platforms")]
    [SerializeField] private MeshRenderer _platformLOD0;
    [SerializeField] private MeshRenderer _platformLOD1;
    [SerializeField] private MeshRenderer _platformLOD2;

    [Header("Canyons")]
    [SerializeField] private GameObject _canyonLeftLOD0;
    [SerializeField] private GameObject _canyonLeftLOD1;
    [SerializeField] private GameObject _canyonLeftLOD2;

    [Space]
    [SerializeField] private GameObject _canyonRightLOD0;
    [SerializeField] private GameObject _canyonRightLOD1;
    [SerializeField] private GameObject _canyonRightLOD2;

    [Header("Coins")]
    [SerializeField] private GameObject _coinLOD0;
    [SerializeField] private GameObject _coinLOD1;
    [SerializeField] private GameObject _coinLOD2;

    public MeshRenderer Platform { get; private set; }
    public GameObject Coin { get; private set; }
    public GameObject CanyonRight { get; private set; }
    public GameObject CanyonLeft { get; private set; }

    public void Setup()
    {
        if (PlayerPrefs.GetInt("GraphicsQuality") == 0)
        {
            CanyonRight = _canyonRightLOD2;
            CanyonLeft = _canyonLeftLOD2;
            Platform = _platformLOD2;
            Coin = _coinLOD2;
        }
        else if (PlayerPrefs.GetInt("GraphicsQuality") == 1)
        {
            CanyonRight = _canyonRightLOD1;
            CanyonLeft = _canyonLeftLOD1;
            Platform = _platformLOD1;
            Coin = _coinLOD1;
        }
        else
        {
            CanyonRight = _canyonRightLOD0;
            CanyonLeft = _canyonLeftLOD0;
            Platform = _platformLOD0;
            Coin = _coinLOD0;
        }
    }
}

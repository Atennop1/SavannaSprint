using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObstacleData
{
    public float chance;

    [Header("LODs")]
    public GameObject obstacleLOD0;
    public GameObject obstacleLOD1;
    public GameObject obstacleLOD2;

    [Header("Parametrs")]
    public bool isDependsPlayerSpeed;
    public bool isForOneLine;
    public bool isSphereRock;

    [Header("Info")]
    public float positionY;

    [Space]
    public Vector3 coinsOffset;
    public Vector3 sphereRockPosition;

    [Space]
    public List<CoinsStyle> coinsStyles = new List<CoinsStyle>();

    [HideInInspector] public CoinsStyle coinsStyle;
    [HideInInspector] public Vector3 obstaclePosition;
    [HideInInspector] public Vector3 coinsPosition;

    public void Setup(LinePosition linePosition, float z, bool is2d)
    {
        coinsStyle = coinsStyles[Random.Range(0, coinsStyles.Count)];

        if (!isForOneLine)
            obstaclePosition = new Vector3(0, positionY, z);
        else
            obstaclePosition = new Vector3((float)linePosition * 3.3f, positionY, z);

        if (isSphereRock)
            obstaclePosition = sphereRockPosition + new Vector3(0, 0, z);

        if (!isDependsPlayerSpeed)
            coinsPosition = obstaclePosition + coinsOffset;
        else
        {
            if (!is2d)
                coinsPosition = new Vector3(isForOneLine ? obstaclePosition.x + coinsOffset.x : 3.3f * Random.Range(1, -2), obstaclePosition.y + coinsOffset.y, obstaclePosition.z + coinsOffset.z * PlayerMovementNonControlable.speed / 15);
            else
                coinsPosition = new Vector3(isForOneLine ? obstaclePosition.x + coinsOffset.x : 3.3f * Random.Range(1, -2), obstaclePosition.y + coinsOffset.y, obstaclePosition.z + coinsOffset.z * PlayerMovementNonControlable2D.speed / -10);
        }
    }
}

[CreateAssetMenu(menuName = "ObstaclesData", fileName = "New ObstaclesData")]
public class ObstaclesData : ScriptableObject
{
    public List<ObstacleData> data;

    [Header("Platforms")]
    [SerializeField] private MeshRenderer platformLOD0;
    [SerializeField] private MeshRenderer platformLOD1;
    [SerializeField] private MeshRenderer platformLOD2;

    [Header("Canyons")]
    [SerializeField] private GameObject canyonLeftLOD0;
    [SerializeField] private GameObject canyonLeftLOD1;
    [SerializeField] private GameObject canyonLeftLOD2;

    [Space]
    [SerializeField] private GameObject canyonRightLOD0;
    [SerializeField] private GameObject canyonRightLOD1;
    [SerializeField] private GameObject canyonRightLOD2;

    [Header("Coins")]
    [SerializeField] private GameObject coinLOD0;
    [SerializeField] private GameObject coinLOD1;
    [SerializeField] private GameObject coinLOD2;

    [HideInInspector] public MeshRenderer platform;
    [HideInInspector] public GameObject coin;
    [HideInInspector] public GameObject canyonRight;
    [HideInInspector] public GameObject canyonLeft;

    public void Setup()
    {
        if (PlayerPrefs.GetInt("GraphicsQuality") == 0)
        {
            platform = platformLOD2;
            coin = coinLOD2;
            canyonLeft = canyonLeftLOD2;
            canyonRight = canyonRightLOD2;
        }
        else if (PlayerPrefs.GetInt("GraphicsQuality") == 1)
        {
            platform = platformLOD1;
            coin = coinLOD1;
            canyonLeft = canyonLeftLOD1;
            canyonRight = canyonRightLOD1;
        }
        else
        {
            platform = platformLOD0;
            coin = coinLOD0;
            canyonLeft = canyonLeftLOD0;
            canyonRight = canyonRightLOD0;
        }
    }
}

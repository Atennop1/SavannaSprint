using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LinePosition 
{ 
    Left = -1, 
    Center = 0, 
    Right = 1 
};

public enum CoinsStyle 
{ 
    Line, 
    Jump, 
    Ramp, 
    None 
};

public class SpawnManager : MonoCache
{
    private bool is2d;

    [HideInInspector] public PlayerController player;
    [HideInInspector] public PlayerController2D player2D;

    [Header("Map Stuff")]
    public float lineDistance = 3.3f;
    public int itemCountInMap = 1;

    [Header("Coins Stuff")]
    public float coinsInLine = 13;
    public float coinsItemSpace = 15;
    public int coinsCountInItem = 13;
    public int coinsWidth = 1;
    public float coinsHeight = 0.45f;

    [Header("Obstacles Data")]
    [SerializeField] private ObstaclesData obstaclesData;
    [SerializeField] private List<PoolMonoBehaviour> pools;

    private LinePosition lastLine = LinePosition.Center;
    private LinePosition lastLine2 = LinePosition.Center;
    private LinePosition randomLine = LinePosition.Left;

    private GameObject lastObstacle;
    private GameObject result;

    private float currentCoinsInItem;
    private float currentCoinsItemSpace;
    private float currentItemSpace;
    private List<GameObject> activeMaps = new List<GameObject>();

    private void Awake()
    {
        is2d = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "2d World";
        obstaclesData.Setup();

        if (!is2d)
        {
            currentCoinsInItem = coinsCountInItem * (int)(PlayerMovementNonControlable.speed / 15);
            currentCoinsItemSpace = coinsItemSpace * (PlayerMovementNonControlable.speed / 15);
        }
        else
        {
            currentCoinsInItem = -coinsCountInItem * (int)(PlayerMovementNonControlable2D.speed / 10);
            currentCoinsItemSpace = -coinsItemSpace * (PlayerMovementNonControlable2D.speed / 10);
        }
    }

    public void Start()
    {
        player = PlayerController.instance;
        player2D = PlayerController2D.instance;

        if (!is2d)
            for (int i = 0; i < 15; i++)
                AddActiveMap(MakeMap());

        else if (is2d)
            for (int i = 0; i < 9; i++)
                AddActiveMap(MakeMap());
    }

    public override void OnTick()
    {
        if (!is2d && activeMaps[0].transform.position.z < player.transform.position.z - 120)
        {
            RemoveFirstActiveMap();
            AddActiveMap(MakeMap());
        }
        else if (is2d && activeMaps[0].transform.position.z < player2D.transform.position.z - 48)
        {
            RemoveFirstActiveMap();
            AddActiveMap(MakeMap());
        }
    }

    private void RemoveFirstActiveMap()
    {  
        foreach(Transform child in activeMaps[0].transform)
            child.gameObject.SetActive(false);
    
        activeMaps.RemoveAt(0);
    }

    private void AddActiveMap(GameObject map)
    {
        map.transform.position = activeMaps.Count > 0 ? activeMaps[activeMaps.Count - 1].transform.position + Vector3.forward * currentItemSpace : (is2d ? new Vector3(0, 0, 15) : new Vector3(0, 0, 30));
        activeMaps.Add(map);
    }

    private GameObject MakeMap()
    {
        result = new GameObject();
        result.transform.SetParent(transform);

        int obstaclesCount;
        if (!is2d)
            obstaclesCount = UnityEngine.Random.Range(1, 4);
        else
            obstaclesCount = 1;

        for (int i = 0; i < obstaclesCount; i++)
        {
            float chance = 0;
            float currentChance = UnityEngine.Random.Range(0, 101);
            for (int j = 0; j < obstaclesData.data.Count; j++)
            {
                ObstacleData obstacle = obstaclesData.data[j];
                GameObject obstacleObject;

                if (PlayerPrefs.GetInt("GraphicsQuality") == 0)
                    obstacleObject = obstacle.obstacleLOD2;
                else if (PlayerPrefs.GetInt("GraphicsQuality") == 1)
                    obstacleObject = obstacle.obstacleLOD1;
                else
                    obstacleObject = obstacle.obstacleLOD0;

                chance += obstacle.chance;
                if (currentChance <= chance && obstacleObject != lastObstacle)
                {
                    if (i != 0 && !obstacle.isForOneLine)
                        continue;

                    if (!obstacle.isForOneLine)
                        i = 4;

                    if (!is2d)
                        while (randomLine == lastLine || randomLine == lastLine2)
                            randomLine = (LinePosition)UnityEngine.Random.Range(-1, 2);
                    else
                        randomLine = LinePosition.Center;

                    obstaclesData.data[j].Setup(randomLine, currentItemSpace, false);
                    lastObstacle = obstacleObject;

                    int canCoins = UnityEngine.Random.Range(1, 5);
                    if (canCoins != 1)
                        CreateCoins(obstacle.coinsStyle, obstacle.coinsPosition, result);

                    GetPool(obstacleObject.transform)?.Pool.GetFreeElement(new Vector3(obstacle.obstaclePosition.x, obstacle.obstaclePosition.y, obstacle.obstaclePosition.z - 2), result.transform);

                    if (i == 0)
                        lastLine = randomLine;
                    else if (i == 1)
                        lastLine2 = randomLine;

                    break;
                }
            }
        }
        return result;
    }

    private void CreateCoins(CoinsStyle style, Vector3 pos, GameObject parentObject)
    {
        Vector3 coinPos = Vector3.zero;
        if (style == CoinsStyle.Line)
        {
            for (int i = (int)-coinsInLine / 2; i < (int)coinsInLine / 2; i++)
            {
                coinPos.y = coinsHeight;
                coinPos.z = i * (coinsItemSpace / coinsCountInItem);
                
                GetPool(obstaclesData.coin.transform)?.Pool.GetFreeElement(coinPos + pos, parentObject.transform);
            }
        }
        if (style == CoinsStyle.Jump)
        {
            for (int i = (int)-currentCoinsInItem / 2; i < currentCoinsInItem / 2; i++)
            {
                if (is2d)
                    coinPos.y = Mathf.Max((-0.4f / coinsWidth) * Mathf.Pow(i, 2) + 4, 1.5f) - 0.5f;
                else
                    coinPos.y = Mathf.Max((-0.4f / coinsWidth) * Mathf.Pow(i, 2) + 4, coinsHeight);
                coinPos.z = i * (currentCoinsItemSpace / currentCoinsInItem);

                GetPool(obstaclesData.coin.transform)?.Pool.GetFreeElement(coinPos + pos, parentObject.transform);
            }
        }
        if (style == CoinsStyle.Ramp)
        {
            for (int i = -coinsCountInItem / 2; i < coinsCountInItem / 2; i++)
            {
                coinPos.y = Mathf.Min(Mathf.Max(1.6f * (i + 2), coinsHeight), 5);
                coinPos.z = i * ((float)coinsItemSpace / coinsCountInItem);

                GetPool(obstaclesData.coin.transform)?.Pool.GetFreeElement(coinPos + pos, parentObject.transform);
            }
        }
    }

    private PoolMonoBehaviour GetPool(Component component)
    {
        for (int k = 0; k < pools.Count; k++)
            if (component == pools[k].Prefab)             
                return pools[k];
        return null;            
    }
    
    public void InitValues3D()
    {
        if (PlayerPrefs.HasKey("itemSpace"))
            currentItemSpace = PlayerPrefs.GetFloat("itemSpace");
        else
            currentItemSpace = 30;

        PlayerMovementNonControlable.speed = -PlayerMovementNonControlable2D.speed / 10 * 15;
        coinsInLine += 0.005f * GameManager.speedAdderIterations;
        currentItemSpace += 0.03f * GameManager.speedAdderIterations;
        currentCoinsInItem += 0.005f * GameManager.speedAdderIterations;
        currentCoinsItemSpace += 0.05f * GameManager.speedAdderIterations;
        CoinsUpdate3D();
    }

    public void UpdateValues3D()
    {
        PlayerMovementNonControlable.speed += 0.06f;
        coinsInLine += 0.005f;
        currentItemSpace += 0.03f;
        PlayerPrefs.SetFloat("itemSpace", currentItemSpace);

        currentCoinsInItem += 0.005f;
        currentCoinsItemSpace += 0.03f;
        CoinsUpdate3D();
    }

    private void CoinsUpdate3D()
    {
        coinsWidth = 1 + (int)((PlayerMovementNonControlable.speed - 10) / 3f);
    }

    public void InitValues2D()
    {
        if (PlayerPrefs.HasKey("itemSpace2D"))
            currentItemSpace = PlayerPrefs.GetFloat("itemSpace2D");
        else
            currentItemSpace = 17;

        PlayerMovementNonControlable2D.speed = -PlayerMovementNonControlable.speed / 15 * 10;
        coinsInLine += 0.01f * GameManager.speedAdderIterations;
        currentItemSpace += 0.018f * GameManager.speedAdderIterations;
        CoinsUpdate2D(PlayerMovementNonControlable2D.speed);
    }

    public void UpdateValues2D()
    {
        PlayerMovementNonControlable2D.speed -= 0.04f;
        coinsInLine += 0.01f;
        currentItemSpace += 0.018f;
        PlayerPrefs.SetFloat("itemSpace2D", currentItemSpace);
        CoinsUpdate2D(PlayerMovementNonControlable2D.speed);
    }

    private void CoinsUpdate2D(float speed)
    {
        currentCoinsInItem = -(int)(coinsCountInItem * (speed / 10));
        currentCoinsItemSpace = -coinsItemSpace * (speed / 13);
        coinsWidth = 1 + -(int)((speed - 10) / 2.5f);
    }
}


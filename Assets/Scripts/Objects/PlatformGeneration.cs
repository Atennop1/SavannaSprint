using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlatformGeneration : MonoCache
{
    [Header("Transforms")]
    [SerializeField] private Transform _player;
    [SerializeField] private Transform left;
    [SerializeField] private Transform right;
    [SerializeField] private Transform road;


    [Header("Length")]
    public float tileLenghtCenter;
    public float tileLenghtBackground;

    [Header("Start Values")]
    [SerializeField] private int startPlatforms;
    [SerializeField] private int startBackgrounds;

    [Header("Positions")]
    [SerializeField] private float backgroundPosY;
    [SerializeField] private float backgroundsXOffset;
    [SerializeField] private float spawnPosBackground;

    [Header("Obstacles Data")]
    [SerializeField] private ObstaclesData data;

    private bool is2d;
    private float spawnPos;
    private List<MeshRenderer> activePlatforms = new List<MeshRenderer>();

    private List<GameObject> activeBackgroundsLeft = new List<GameObject>();
    private List<GameObject> activeBackgroundsRight = new List<GameObject>();

    private void Start()
    {
        data.Setup();
        is2d = SceneManager.GetActiveScene().name == "2d World";

        SpawnPlatform();
        for (int i = 0; i < startPlatforms; i++)
            SpawnPlatform();

        for (int i = 0; i < startBackgrounds; i++)
        {
            SpawnBackgroundLeft();
            SpawnBackgroundRight();
            spawnPosBackground += tileLenghtBackground;
        }
    }
    public override void OnTick()
    {
        if (_player.position.z > spawnPos - tileLenghtCenter * startPlatforms)
        {
            spawnPos += tileLenghtCenter;
            DeletePlatform();
        }
        if (_player.position.z > spawnPosBackground - tileLenghtBackground * startBackgrounds)
        {
            spawnPosBackground += tileLenghtBackground;
            DeleteBackgroundLeft();
            DeleteBackgroundRight();
        }
    }
    private void SpawnPlatform()
    {
        MeshRenderer nextTile = Instantiate(data.platform, transform.forward * spawnPos, data.platform.transform.rotation, road);
        activePlatforms.Add(nextTile);
        spawnPos += tileLenghtCenter;
    }
    private void DeletePlatform()
    {
        var lastPlatform = activePlatforms[0];
        if (!is2d)
            activePlatforms[0].transform.position = transform.forward * (spawnPos - 36);
        else
            activePlatforms[0].transform.position = transform.forward * (spawnPos - 19.6f);

        activePlatforms.Remove(lastPlatform);
        activePlatforms.Add(lastPlatform);
    }
    private void SpawnBackgroundLeft()
    {
        GameObject nextBackgroundLeft = Instantiate(data.canyonLeft, new Vector3(-backgroundsXOffset, backgroundPosY, spawnPosBackground - tileLenghtBackground), data.canyonLeft.transform.rotation, left);
        activeBackgroundsLeft.Add(nextBackgroundLeft);
    }
    private void DeleteBackgroundLeft()
    {
        var lastPlatform = activeBackgroundsLeft[0];
        activeBackgroundsLeft[0].transform.position = new Vector3(-backgroundsXOffset, backgroundPosY, spawnPosBackground - tileLenghtBackground * 2);
        activeBackgroundsLeft.Remove(lastPlatform);
        activeBackgroundsLeft.Add(lastPlatform);
    }
    private void SpawnBackgroundRight()
    {
        GameObject nextBackgroundRight = Instantiate(data.canyonRight, new Vector3(backgroundsXOffset, backgroundPosY, spawnPosBackground - tileLenghtBackground), data.canyonRight.transform.rotation, right);
        activeBackgroundsRight.Add(nextBackgroundRight);
    }
    private void DeleteBackgroundRight()
    {
        var lastPlatform = activeBackgroundsRight[0];
        activeBackgroundsRight[0].transform.position = new Vector3(backgroundsXOffset, backgroundPosY, spawnPosBackground - tileLenghtBackground * 2);
        activeBackgroundsRight.Remove(lastPlatform);
        activeBackgroundsRight.Add(lastPlatform);
    }
}

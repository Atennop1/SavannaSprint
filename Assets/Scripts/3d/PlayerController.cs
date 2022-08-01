using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PlayerState
{ 
    Run,
    Jump,
    Ctrl,
    Ramp,
    Changing,
    Death,
    None
}

[RequireComponent(typeof(PlayerLose))]
[RequireComponent(typeof(PlayerBonuses))]
[RequireComponent(typeof(PlayerAnimations))]
[RequireComponent(typeof(PlayerMovementControlable))]
[RequireComponent(typeof(PlayerMovementNonControlable))]
public class PlayerController : MonoCache
{
    public static PlayerState playerState;
    public static PlayerController instance;

    [HideInInspector] public PlayerMovementNonControlable playerMovementNonControlable;
    [HideInInspector] public PlayerMovementControlable playerMovementControlable;
    [HideInInspector] public PlayerAnimations playerAnimations;
    [HideInInspector] public PlayerBonuses playerBonuses;
    [HideInInspector] public PlayerLose playerLose;

    [Header("Objects")]
    public Image guideImage;

    [Header("Source")]
    [SerializeField] private AudioSource coinSource;
    [SerializeField] private AudioSource teleportSource;

    [Header("Particles")]
    public ParticleSystem teleportParticles;

    [Header("Components")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private SceneChanger sceneChanger;

    private void Awake()
    {
        instance = this;
        SingletonManager.canPlay = false;
        playerState = PlayerState.None;

        playerMovementNonControlable = GetComponent<PlayerMovementNonControlable>();
        playerMovementControlable = GetComponent<PlayerMovementControlable>();
        playerAnimations = GetComponent<PlayerAnimations>();
        playerBonuses = GetComponent<PlayerBonuses>();
        playerLose = GetComponent<PlayerLose>();

        SkinInfo thisSkinInfo;
        if (PlayerPrefs.GetString("ActiveSkin3D") != "Cyberpunk")
            thisSkinInfo = Resources.Load<SkinInfo>("Skins/" + PlayerPrefs.GetString("ActiveSkin3D"));
        else
            thisSkinInfo = Resources.Load<SkinInfo>("Skins/" + PlayerPrefs.GetString("ActiveSkin3D") + "3D");

        thisSkinInfo.Init();
        playerAnimations.playerAnimator.runtimeAnimatorController = thisSkinInfo.skinAnimator;

        Material material = thisSkinInfo.skinMaterial;
        material.shader = thisSkinInfo.currentShader;
        GetComponent<VoxelImporter.VoxelFrameAnimationObject>().playMaterial0 = material;

        playerMovementControlable.runDust.GetComponent<ParticleSystemRenderer>().material = playerMovementControlable.jumpDust.GetComponent<ParticleSystemRenderer>().material = thisSkinInfo.dustMaterial;
        playerLose.gameOverParticles.GetComponent<ParticleSystemRenderer>().SetMeshes(thisSkinInfo.gameOverParticlesMeshes, thisSkinInfo.gameOverParticlesMeshes.Length);
    }
    
    public IEnumerator StartMethod()
    {
        StartCoroutine(playerBonuses.StartMethod());
        StartCoroutine(playerAnimations.StartMethod());
        StartCoroutine(playerMovementControlable.StartMethod());
        StartCoroutine(playerMovementNonControlable.StartMethod());

        playerLose.pauseButton.gameObject.SetActive(false);
        gameManager.UpdateText();
        PlayerController2D.playerState = PlayerState.Run;

        yield return new WaitForSeconds(1.3f);

        SingletonManager.canPlay = true;
        if (SingletonManager.instance.musicSource.isPlaying)
            SingletonManager.instance.musicSource.UnPause();
        else
            SingletonManager.instance.musicSource.Play();

        yield return new WaitForSeconds(0.2f);

        playerLose.pauseButton.gameObject.SetActive(true);
        teleportSource.volume = 0.5f * SingletonManager.soundVolume;
        coinSource.volume = 0.27f * SingletonManager.soundVolume;
        playerLose.gameOverSource.volume = SingletonManager.soundVolume;
    }

    public override void OnTick()
    {
        if (playerState != PlayerState.Death)
        {
            if (transform.position.y < -1)
                transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Changer")
        {
            collision.gameObject.GetComponentInParent<CoinsRotate>().speed = 0;
            playerLose.pauseButton.gameObject.SetActive(false);
            StartCoroutine(Change());
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Coin")
        {
            coinSource.Play();
            SingletonManager.instance.coins += 1 + (GameManager.isX2Coins ? 1 : 0);
            GameManager.allOrangeCoins += 1 + (GameManager.isX2Coins ? 1 : 0);
            gameManager.UpdateText();
            other.transform.parent.gameObject.SetActive(false);
        }

        if (other.tag == "Lose" && playerState == PlayerState.Run && !other.transform.parent.GetComponent<Obstacle>().isGuided)
            other.transform.parent.GetComponent<Obstacle>().SlowMotionStart();

        if (other.tag == "GuideCantMove" && !other.transform.parent.GetComponent<Obstacle>().isGuided)
        {
            Obstacle.cantMoveCollider = other.GetComponentsInChildren<Collider>()[0];
            Obstacle.CantMove();
        }

        if (other.GetComponent<SphereRockController>() != null)
            other.GetComponent<SphereRockController>().MoveRock();
    }
    public void RebornMethod(bool ad)
    {
        StartCoroutine(Reborn(ad));
    }
    public IEnumerator Reborn(bool ad)
    {
        StartCoroutine(playerBonuses.Reborn());
        StartCoroutine(playerAnimations.Reborn());
        StartCoroutine(playerMovementControlable.Reborn());
        StartCoroutine(playerMovementNonControlable.Reborn());

        if (!ad)
        {
            GameManager.allOrangeCoins -= 500 * GameManager.lifesCount;
            if (GameManager.lifesCount != 20)
                GameManager.lifesCount++;
        }

        foreach (Transform child in playerLose.obstacleKiller)
            child.gameObject.SetActive(false);

        playerLose.pauseButton.gameObject.SetActive(true);
        playerLose.gameOverPanel.gameObject.SetActive(false);

        yield return new WaitForSeconds(1.6f);

        SingletonManager.instance.musicSource.UnPause();
        SingletonManager.canPlay = true;
    }
    public IEnumerator Change()
    {
        StartCoroutine(playerAnimations.Change());
        StartCoroutine(playerMovementControlable.Change());
        StartCoroutine(playerMovementNonControlable.Change());

        SingletonManager.canPlay = false;
        SingletonManager.instance.musicSource.Pause();
        playerBonuses.magnetParticles.Stop();

        yield return new WaitForSeconds(1.6f);

        teleportSource.Play();
        teleportParticles.Play();

        yield return new WaitForSeconds(0.1f);

        SceneChanger.levelToLoad = 1;
        sceneChanger.FadeToLevel();
    }
}
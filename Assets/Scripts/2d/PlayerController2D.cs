using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerLose2D))]
[RequireComponent(typeof(PlayerBonuses))]
[RequireComponent(typeof(PlayerAnimations2D))]
[RequireComponent(typeof(PlayerMovementNonControlable2D))]
public class PlayerController2D : MonoCache
{
    public static PlayerState playerState;
    public static PlayerController2D instance;

    [HideInInspector] public PlayerMovementNonControlable2D playerMovementNonControlable;
    [HideInInspector] public PlayerAnimations2D playerAnimations;
    [HideInInspector] public PlayerBonuses playerBonuses;
    [HideInInspector] public PlayerLose2D playerLose;

    [Header("Objects")]
    public Image guideImage;

    [Header("Source")]
    [SerializeField] private AudioSource teleportSource;
    [SerializeField] private AudioSource coinSource;

    [Header("Particles")]
    public ParticleSystem teleportParticle;

    [Header("Components")]
    [SerializeField] private SceneChanger sceneChanger;
    public Jump jump;
    public Ctrl ctrl;

    [Header("Move")]
    [SerializeField] private GameObject ctrlCol;
    [SerializeField] private GameObject runCol;

    [HideInInspector] public bool canCtrl;
    [HideInInspector] public bool canJump;

    private void Awake()
    {
        SingletonManager.canPlay = false;
        instance = this;
        canCtrl = canJump = true;
        playerState = PlayerState.None;

        playerMovementNonControlable = GetComponent<PlayerMovementNonControlable2D>();
        playerAnimations = GetComponent<PlayerAnimations2D>();
        playerBonuses = GetComponent<PlayerBonuses>();
        playerLose = GetComponent<PlayerLose2D>();
        jump.playerRb = GetComponent<Rigidbody>();

        SkinInfo thisSkinInfo;
        if (PlayerPrefs.GetString("ActiveSkin2D") != "Cyberpunk")
            thisSkinInfo = Resources.Load<SkinInfo>("Skins/" + PlayerPrefs.GetString("ActiveSkin2D"));
        else
            thisSkinInfo = Resources.Load<SkinInfo>("Skins/" + PlayerPrefs.GetString("ActiveSkin2D") + "2D");

        thisSkinInfo.Init();
        playerAnimations.playerAnimator.runtimeAnimatorController = thisSkinInfo.skinAnimator;

        Material material = thisSkinInfo.skinMaterial;
        material.shader = thisSkinInfo.currentShader;
        GetComponent<VoxelImporter.VoxelFrameAnimationObject>().playMaterial0 = material;

        playerMovementNonControlable.runDust.GetComponent<ParticleSystemRenderer>().material = playerMovementNonControlable.jumpDust.GetComponent<ParticleSystemRenderer>().material = thisSkinInfo.dustMaterial;
        playerLose.gameOverParticles.GetComponent<ParticleSystemRenderer>().SetMeshes(thisSkinInfo.gameOverParticlesMeshes, thisSkinInfo.gameOverParticlesMeshes.Length);

    }
    public override void OnTick()
    {
        transform.position = new Vector3(0, transform.position.y, transform.position.z);
    }
    public void OnCollisionEnter(Collision collision)
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
            SingletonManager.instance.redCoins += 1 + (GameManager.isX2Coins ? 1 : 0);
            GameManager.allRedCoins += 1 + (GameManager.isX2Coins ? 1 : 0);
            other.transform.parent.gameObject.SetActive(false);
            coinSource.Play();
        }

        if (other.tag == "Lose" && playerState == PlayerState.Run && !other.transform.parent.GetComponent<Obstacle>().isGuided)
            other.transform.parent.GetComponent<Obstacle>().SlowMotionStart();

        if (other.tag == "GuideCantMove" && !other.transform.parent.GetComponent<Obstacle>().isGuided)
        {
            Obstacle.cantMoveCollider = other.GetComponentsInChildren<Collider>()[0];
            Obstacle.CantMove();
        }
    }
    public IEnumerator StartMethod()
    {
        StartCoroutine(playerMovementNonControlable.StartMethod());
        StartCoroutine(playerBonuses.StartMethod());
        StartCoroutine(playerLose.StartMethod());
        StartCoroutine(playerAnimations.StartMethod());

        playerLose.pauseButton.gameObject.SetActive(false);
        Jump.canDust = false;
        PlayerController.playerState = PlayerState.Run;

        yield return new WaitForSeconds(1.5f);

        playerState = PlayerState.Run;

        SingletonManager.canPlay = true;
        if (SingletonManager.instance.musicSource.isPlaying)
            SingletonManager.instance.musicSource.UnPause();
        else
            SingletonManager.instance.musicSource.Play();

        teleportSource.volume = 0.5f * SingletonManager.soundVolume;
        coinSource.volume = 0.27f * SingletonManager.soundVolume;
    }
    public void RebornMethod(bool ad)
    {
        StartCoroutine(Reborn(ad));
    }
    public IEnumerator Reborn(bool ad)
    {
        StartCoroutine(playerMovementNonControlable.Reborn());
        StartCoroutine(playerBonuses.Reborn());
        StartCoroutine(playerLose.Reborn());
        StartCoroutine(playerAnimations.Reborn());

        if (!ad)
        {
            GameManager.allRedCoins -= 100 * GameManager.lifesCount;
            if (GameManager.lifesCount != 20)
                GameManager.lifesCount++;
        }

        transform.position = new Vector3(transform.position.x, 0.0593245f, transform.position.z);
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;

        ctrlCol.SetActive(false);
        runCol.SetActive(true);
        Jump.canDust = false;

        yield return new WaitForSeconds(1.6f);

        SingletonManager.canPlay = true;
        GameOverScript.isGameOver = false;

        playerState = PlayerState.Run;
        SingletonManager.instance.musicSource.UnPause();
    }
    public IEnumerator Change()
    {
        StartCoroutine(playerMovementNonControlable.Change());
        StartCoroutine(playerAnimations.Change());

        SingletonManager.canPlay = false;
        ctrlCol.SetActive(false);
        runCol.SetActive(true);

        playerState = PlayerState.Changing;
        playerBonuses.magnetParticles.Stop();
        SingletonManager.instance.musicSource.Pause();
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;

        yield return new WaitForSeconds(1.6f);

        teleportSource.Play();
        teleportParticle.Play();

        yield return new WaitForSeconds(0.1f);

        SceneChanger.levelToLoad = 2;
        sceneChanger.FadeToLevel();
    }
}

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

    public PlayerMovementNonControlable PlayerMovementNonControlable { get; private set; }
    public PlayerMovementControlable PlayerMovementControlable { get; private set; }
    public PlayerAnimations PlayerAnimations { get; private set; }
    public PlayerBonuses PlayerBonuses { get; private set; }
    public PlayerLose PlayerLose { get; private set; }

    [field: SerializeField, Header("Objects")] public Image GuideImage { get; private set; }

    [Header("Source")]
    [SerializeField] private AudioSource _coinSource;
    [SerializeField] private AudioSource _teleportSource;

    [Header("Particles")]
    [SerializeField] private ParticleSystem _teleportParticles;

    [Header("Components")]
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private SceneChanger _sceneChanger;

    private void Awake()
    {
        instance = this;
        SingletonManager.canPlay = false;
        playerState = PlayerState.None;

        PlayerMovementNonControlable = GetComponent<PlayerMovementNonControlable>();
        PlayerMovementControlable = GetComponent<PlayerMovementControlable>();
        PlayerAnimations = GetComponent<PlayerAnimations>();
        PlayerBonuses = GetComponent<PlayerBonuses>();
        PlayerLose = GetComponent<PlayerLose>();

        SkinInfo thisSkinInfo;
        if (PlayerPrefs.GetString("ActiveSkin3D") != "Cyberpunk")
            thisSkinInfo = Resources.Load<SkinInfo>("Skins/" + PlayerPrefs.GetString("ActiveSkin3D"));
        else
            thisSkinInfo = Resources.Load<SkinInfo>("Skins/" + PlayerPrefs.GetString("ActiveSkin3D") + "3D");

        thisSkinInfo.Init();
        PlayerAnimations.PlayerAnimator.runtimeAnimatorController = thisSkinInfo.skinAnimator;

        Material material = thisSkinInfo.skinMaterial;
        material.shader = thisSkinInfo.currentShader;
        GetComponent<VoxelImporter.VoxelFrameAnimationObject>().playMaterial0 = material;

        //PlayerMovementControlable._runDust.GetComponent<ParticleSystemRenderer>().material = PlayerMovementControlable._jumpDust.GetComponent<ParticleSystemRenderer>().material = thisSkinInfo.dustMaterial;
        //PlayerLose._gameOverParticles.GetComponent<ParticleSystemRenderer>().SetMeshes(thisSkinInfo.gameOverParticlesMeshes, thisSkinInfo.gameOverParticlesMeshes.Length);
    }
    
    public IEnumerator StartMethod()
    {
        StartCoroutine(PlayerBonuses.StartMethod());
        StartCoroutine(PlayerAnimations.StartMethod());
        StartCoroutine(PlayerMovementControlable.StartMethod());
        StartCoroutine(PlayerMovementNonControlable.StartMethod());

        //PlayerLose.pauseButton.gameObject.SetActive(false);
        _gameManager.UpdateText();
        PlayerController2D.playerState = PlayerState.Run;

        yield return new WaitForSeconds(1.3f);

        SingletonManager.canPlay = true;
        if (SingletonManager.instance.musicSource.isPlaying)
            SingletonManager.instance.musicSource.UnPause();
        else
            SingletonManager.instance.musicSource.Play();

        yield return new WaitForSeconds(0.2f);

        //PlayerLose.pauseButton.gameObject.SetActive(true);
        _teleportSource.volume = 0.5f * SingletonManager.soundVolume;
        _coinSource.volume = 0.27f * SingletonManager.soundVolume;
        //PlayerLose.gameOverSource.volume = SingletonManager.soundVolume;
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
            //PlayerLose.pauseButton.gameObject.SetActive(false);
            StartCoroutine(Change());
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Coin")
        {
            _coinSource.Play();
            SingletonManager.instance.coins += 1 + (GameManager.isX2Coins ? 1 : 0);
            GameManager.allOrangeCoins += 1 + (GameManager.isX2Coins ? 1 : 0);
            _gameManager.UpdateText();
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
        StartCoroutine(PlayerBonuses.Reborn());
        StartCoroutine(PlayerAnimations.Reborn());
        StartCoroutine(PlayerMovementControlable.Reborn());
        StartCoroutine(PlayerMovementNonControlable.Reborn());

        if (!ad)
        {
            GameManager.allOrangeCoins -= 500 * GameManager.lifesCount;
            if (GameManager.lifesCount != 20)
                GameManager.lifesCount++;
        }

        //foreach (Transform child in PlayerLose._obstacleKiller)
        //    child.gameObject.SetActive(false);

        //PlayerLose.pauseButton.gameObject.SetActive(true);
        //PlayerLose._gameOverPanel.gameObject.SetActive(false);

        yield return new WaitForSeconds(1.6f);

        SingletonManager.instance.musicSource.UnPause();
        SingletonManager.canPlay = true;
    }
    public IEnumerator Change()
    {
        StartCoroutine(PlayerAnimations.Change());
        StartCoroutine(PlayerMovementControlable.Change());
        StartCoroutine(PlayerMovementNonControlable.Change());

        SingletonManager.canPlay = false;
        SingletonManager.instance.musicSource.Pause();
        //PlayerBonuses._magnetParticles.Stop();

        yield return new WaitForSeconds(1.6f);

        _teleportSource.Play();
        _teleportParticles.Play();

        yield return new WaitForSeconds(0.1f);

        SceneChanger.levelToLoad = 1;
        _sceneChanger.FadeToLevel();
    }
}
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerLose2D))]
[RequireComponent(typeof(PlayerBonuses))]
[RequireComponent(typeof(PlayerAnimations2D))]
[RequireComponent(typeof(PlayerMovementNonControlable2D))]
public class PlayerController2D : MonoCache
{
    public PlayerState PlayerState;

    public PlayerMovementNonControlable2D PlayerMovementNonControlable { get; private set; }
    public PlayerAnimations2D PlayerAnimations  {get; private set; }
    public PlayerBonuses PlayerBonuses { get; private set; }
    public PlayerLose2D PlayerLose { get; private set; }

    [field: SerializeField, Header("Objects")] public Image GuideImage { get; private set; }
    [field: SerializeField] public GameManager GameManager { get; private set; }
    [field: SerializeField] public GameOverScript GameOver { get; private set; }

    [Header("Source")]
    [SerializeField] private AudioSource _teleportSource;
    [SerializeField] private AudioSource _coinSource;

    [Header("Particles")]
    [SerializeField] private ParticleSystem _teleportParticle;

    [Header("Components")]
    [SerializeField] private SceneChanger _sceneChanger;
    [SerializeField] private Jump _jump;
    [SerializeField] private Ctrl _ctrl;

    [Header("Move")]
    [SerializeField] private GameObject ctrlCol;
    [SerializeField] private GameObject runCol;

    [HideInInspector] public bool canCtrl;
    [HideInInspector] public bool canJump;

    private void Awake()
    {
        SingletonManager.instance.canPlay = false;
        canCtrl = canJump = true;
        PlayerState = PlayerState.None;

        PlayerMovementNonControlable = GetComponent<PlayerMovementNonControlable2D>();
        PlayerAnimations = GetComponent<PlayerAnimations2D>();
        PlayerBonuses = GetComponent<PlayerBonuses>();
        PlayerLose = GetComponent<PlayerLose2D>();

        SkinInfo thisSkinInfo;
        if (PlayerPrefs.GetString("ActiveSkin2D") != "Cyberpunk")
            thisSkinInfo = Resources.Load<SkinInfo>("Skins/" + PlayerPrefs.GetString("ActiveSkin2D"));
        else
            thisSkinInfo = Resources.Load<SkinInfo>("Skins/" + PlayerPrefs.GetString("ActiveSkin2D") + "2D");

        thisSkinInfo.Init();
        PlayerAnimations.PlayerAnimator.runtimeAnimatorController = thisSkinInfo.SkinAnimator;

        Material material = thisSkinInfo.SkinMaterial;
        material.shader = thisSkinInfo.CurrentShader;
        GetComponent<VoxelImporter.VoxelFrameAnimationObject>().playMaterial0 = material;

        PlayerMovementNonControlable.RunDust.GetComponent<ParticleSystemRenderer>().material = PlayerMovementNonControlable.JumpDust.GetComponent<ParticleSystemRenderer>().material = thisSkinInfo.DustMaterial;
        //PlayerLose._gameOverParticles.GetComponent<ParticleSystemRenderer>().SetMeshes(thisSkinInfo.gameOverParticlesMeshes, thisSkinInfo.gameOverParticlesMeshes.Length);
    }
    
    public override void OnTick()
    {
        transform.position = new Vector3(0, transform.position.y, transform.position.z);
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Changer")
        {
            collision.gameObject.GetComponentInParent<CoinsRotate>().StopRotation();
            //PlayerLose._pauseButton.gameObject.SetActive(false);
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
            _coinSource.Play();
        }

        if (other.tag == "Lose" && PlayerState == PlayerState.Run && !other.transform.parent.GetComponent<Obstacle>().isGuided)
            other.transform.parent.GetComponent<Obstacle>().SlowMotionStart();

        if (other.tag == "GuideCantMove" && !other.transform.parent.GetComponent<Obstacle>().isGuided)
        {
            Obstacle.cantMoveCollider = other.GetComponentsInChildren<Collider>()[0];
            Obstacle.CantMove();
        }
    }

    public IEnumerator StartMethod()
    {
        StartCoroutine(PlayerMovementNonControlable.StartMethod());
        StartCoroutine(PlayerBonuses.StartMethod());
        StartCoroutine(PlayerLose.StartMethod());
        StartCoroutine(PlayerAnimations.StartMethod());

        //PlayerLose._pauseButton.gameObject.SetActive(false);
        //Jump.CanDust = false;
        ////PlayerController.playerState = PlayerState.Run;

        yield return new WaitForSeconds(1.5f);

        PlayerState = PlayerState.Run;

        SingletonManager.instance.canPlay = true;
        if (SingletonManager.instance.musicSource.isPlaying)
            SingletonManager.instance.musicSource.UnPause();
        else
            SingletonManager.instance.musicSource.Play();

        _teleportSource.volume = 0.5f * SingletonManager.instance.soundVolume;
        
        _coinSource.volume = 0.27f * SingletonManager.instance.soundVolume;
    }
    public void RebornMethod(bool ad)
    {
        StartCoroutine(Reborn(ad));
    }

    public IEnumerator Reborn(bool ad)
    {
        StartCoroutine(PlayerMovementNonControlable.Reborn());
        StartCoroutine(PlayerBonuses.Reborn());
        StartCoroutine(PlayerLose.Reborn());
        StartCoroutine(PlayerAnimations.Reborn());

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
        //Jump.CanDust = false;

        yield return new WaitForSeconds(1.6f);

        SingletonManager.instance.canPlay = true;
        GameOver.isGameOver = false;

        PlayerState = PlayerState.Run;
        SingletonManager.instance.musicSource.UnPause();
    }

    public IEnumerator Change()
    {
        StartCoroutine(PlayerMovementNonControlable.Change());
        StartCoroutine(PlayerAnimations.Change());

        SingletonManager.instance.canPlay = false;
        ctrlCol.SetActive(false);
        runCol.SetActive(true);

        PlayerState = PlayerState.Changing;
        //PlayerBonuses._magnetParticles.Stop();
        SingletonManager.instance.musicSource.Pause();
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;

        yield return new WaitForSeconds(1.6f);

        _teleportSource.Play();
        _teleportParticle.Play();

        yield return new WaitForSeconds(0.1f);

        _sceneChanger.levelToLoad = 2;
        _sceneChanger.FadeToLevel();
    }
}

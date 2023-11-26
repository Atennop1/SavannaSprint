using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VoxelImporter;
using UnityEngine.SceneManagement;

public enum PlayerState
{ 
    Run,
    Jump,
    Ctrl,
    CtrlJump,
    Ramp,
    Changing,
    Death,
    None
}

public class Player : MonoCache
{
    [HideInInspector] public PlayerState CurrentState;

    [field: SerializeField] public GameOver GameOver { get; private set; }
    [field: SerializeField] public Game.Input.Input Input { get; private set; }

    [field: SerializeField, Space] public SceneChanger SceneChanger { get; private set; }
    [field: SerializeField] public BonusHandlersDatabase BonusHandlersDatabase { get; private set; }

    [Space]
    [SerializeField] private List<MonoBehaviour> _partsMonobehaviours;
    private List<IPlayerPart> _parts;
    private bool _is3DMode;
    
    public void RebornMethod(bool ad)
    {
        StartCoroutine(Reborn(ad));
    }

    public T GetPlayerPart<T>() where T: class, IPlayerPart
    {
        if (_parts == null)
            InitParts();

        if (_parts != null) 
            return _parts.Where(part => part.GetType() == typeof(T)).Cast<T>().FirstOrDefault();

        throw new ArgumentException("No player parts");
    }

    public IEnumerator Lose()
    {
        CurrentState = PlayerState.Death;
        GameOver.SetGameOver(true);

        foreach (var part in _parts)
            StartCoroutine(part.Lose());

        MusicPlayer.Instance.PauseMusic();
        yield return new WaitForSeconds(1.5f);
        BonusHandlersDatabase.Disable();
    }

    protected override void OnTick()
    {
        if (CurrentState != PlayerState.Death && transform.position.y < -1)
            transform.position = new Vector3(transform.position.x, 0, transform.position.z);
    }

    private IEnumerator Reborn(bool ad)
    {
        foreach (var part in _parts)
            StartCoroutine(part.Reborn(ad));

        yield return new WaitForSeconds(1.55f);

        CurrentState = PlayerState.Run;
        GameOver.SetGameOver(false);
        MusicPlayer.Instance.ContinueMusic();
        BonusHandlersDatabase.Activate();
    }
        
    private IEnumerator Main()
    {
        foreach (var part in _parts)
            StartCoroutine(part.Main());
        
        yield return new WaitForSeconds(1.5f);

        CurrentState = PlayerState.Run;
        MusicPlayer.Instance.ContinueMusic();
        BonusHandlersDatabase.Activate();
    }

    private IEnumerator Change()
    {
        foreach (var part in _parts)
            StartCoroutine(part.Change());

        MusicPlayer.Instance.PauseMusic();
        CurrentState = PlayerState.Changing;

        yield return new WaitForSeconds(1.7f);
        SceneChanger.LoadScene(SceneManager.GetActiveScene().buildIndex == 1 ? 2 : 1);
    }

    private void Awake()
    {
        InitParts();
        _is3DMode = SceneManager.GetActiveScene().name == "3d World";
        MusicPlayer.Instance.PauseMusic();
        CurrentState = PlayerState.None;

        var skin = Resources.Load<Skin>("Skins/" + PlayerPrefs.GetString("ActiveSkin" + (_is3DMode ? "3D" : "2D")) + 
                                        (PlayerPrefs.GetString("ActiveSkin" + (_is3DMode ? "3D" : "2D")) != "Cyberpunk" ? "" : (_is3DMode ? "3D" : "2D")));

        skin.Init(_is3DMode);
        skin.SetupSkinMaterial(GetComponent<VoxelFrameAnimationObject>());

        foreach (var part in _parts)
            part.SetupSkin(skin);
        
        StartCoroutine(Main());
    }

    private void InitParts()
    {
        _parts = new List<IPlayerPart>();
        foreach(var behaviour in _partsMonobehaviours)
            if (behaviour is IPlayerPart part)
                _parts.Add(part);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out SphereRockSetup setup))
            setup.MoveRock();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.TryGetComponent(out ChangerTrigger _)) 
            return;
        
        collision.gameObject.GetComponentInParent<CoinRotate>().StopRotation();
        StartCoroutine(Change());
    }
}
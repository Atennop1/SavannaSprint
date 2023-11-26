using UnityEngine;

public class MusicPlayer : MonoCache
{
    public static MusicPlayer Instance { get; private set; }

    [SerializeField] private GameOver _gameOver;
    [SerializeField] private AudioClip[] _music;

    public AudioSource MusicSource { get; private set; }
    public float Volume { get; private set; } = 1;
    private AudioClip _playingClip;

    public void PauseMusic()
    {
        MusicSource.Pause();
        MusicSource.mute = true;
    }

    public void ContinueMusic()
    {
        if (MusicSource.isPlaying) MusicSource.UnPause();
        else MusicSource.Play();

        if (Time.timeScale != 0)
            MusicSource.mute = false;
    }
    
    public void SetVolume(float value)
    {
        if (value > 0 && value < 1)
            Volume = value;
    }

    private void FixedUpdate()
    {
        if (!MusicSource.isPlaying && !_gameOver.IsGameOver && !MusicSource.mute)
            ChangeMusic();
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        MusicSource = GetComponent<AudioSource>();
        Volume = PlayerPrefs.GetFloat("volume");
        ChangeMusic();
    }
    
    private void ChangeMusic()
    {
        int musicNumber = Random.Range(0, 4);
        while (_music[musicNumber] == _playingClip)
            musicNumber = Random.Range(0, 4);

        MusicSource.volume = musicNumber switch
        {
            0 => 0.2f * Volume,
            1 => 0.4f * Volume,
            2 => 0.4f * Volume,
            3 => 0.35f * Volume,
            _ => MusicSource.volume
        };

        _playingClip = _music[musicNumber];
        MusicSource.clip = _music[musicNumber];
        
        if (MusicSource.enabled)
            MusicSource.Play();
    }
}

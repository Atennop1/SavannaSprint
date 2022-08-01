using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SingletonManager : MonoCache
{
    public AudioSource musicSource;
    [SerializeField] private AudioClip[] music;

    [HideInInspector] public int coins;
    [HideInInspector] public int redCoins;

    public static SingletonManager instance;
    public static float soundVolume = 1;
    public static bool canPlay = false;
    private AudioClip playingNow;

    void Start()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
        DontDestroyOnLoad(instance);

        musicSource = GetComponent<AudioSource>();

        int musicNumber = Random.Range(0, 4);
        switch (musicNumber)
        {
            case 0:
                musicSource.volume = 0.2f * soundVolume;
                break;
            case 1:
                musicSource.volume = 0.4f * soundVolume;
                break;
            case 2:
                musicSource.volume = 0.4f * soundVolume;
                break;
            case 3:
                musicSource.volume = 0.35f * soundVolume;
                break;
        }

        musicSource.clip = music[musicNumber];
        playingNow = music[musicNumber];
    }
    public void Update()
    {
        if (Time.timeScale == 0)
        {
            musicSource.mute = true;
            return;
        }
        else
            musicSource.mute = false;

        if (!musicSource.isPlaying && !GameOverScript.isGameOver && canPlay)
            UpdateMusic(playingNow);
    }
    private void UpdateMusic(AudioClip nowPlay)
    {
        int musicNumber = Random.Range(0, 4);
        while (music[musicNumber] == nowPlay)
            musicNumber = Random.Range(0, 4);

        switch (musicNumber)
        {
            case 0:
                musicSource.volume = 0.2f * soundVolume;
                break;
            case 1:
                musicSource.volume = 0.4f * soundVolume;
                break;
            case 2:
                musicSource.volume = 0.4f * soundVolume;
                break;
            case 3:
                musicSource.volume = 0.35f * soundVolume;
                break;
        }

        playingNow = music[musicNumber];
        musicSource.clip = music[musicNumber];
        musicSource.Play();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLose2D : MonoCache
{
    [SerializeField] private AudioSource _gameOverSource;
    [SerializeField] private AudioSource _shieldSource;

    [Space]
    [SerializeField] private ParticleSystem _gameOverParticles;
    [SerializeField] private ParticleSystem _magnetParticles;

    [Space]
    [SerializeField] private Button _respawnCoins;
    [SerializeField] private Button _rebornButton;
    [SerializeField] private Button _pauseButton;

    [Space]
    [SerializeField] private GameObject _gameOverPanel;
    [SerializeField] private Text _coinsRebornText;

    private PlayerController2D _player;
    private Transform _obstacleKiller;

    public void Start()
    {
        _player = PlayerController2D.instance;
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Lose" && PlayerController2D.playerState != PlayerState.Death && PlayerController2D.playerState != PlayerState.None)
        {
            if (!GameManager.isShield)
            {
                _obstacleKiller = collision.gameObject.transform.parent;
                StartCoroutine(Lose());
            }
            else
            {
                _obstacleKiller = collision.gameObject.transform.parent;
                StartCoroutine(ShieldLose());
            }
        }
    }

    public IEnumerator ShieldLose()
    {
        _shieldSource.Play();
        if (_player.PlayerAnimations.ShieldAnimator.gameObject.activeInHierarchy)
            _player.PlayerAnimations.ShieldAnimator.SetTrigger("crush");

        foreach (Transform child in _obstacleKiller)
            child.gameObject.SetActive(false);

        yield return new WaitForSeconds(0.7f);

        _player.PlayerAnimations.ShieldAnimator.gameObject.SetActive(false);
        GameManager.isShield = false;
    }

    public IEnumerator Lose()
    {
        Obstacle.StopSlowMotion();
        StartCoroutine(_player.PlayerMovementNonControlable.Lose());

        SingletonManager.instance.musicSource.Pause();
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        PlayerController2D.playerState = PlayerState.Death;
        _player.PlayerAnimations.PlayerAnimator.Play("Lose");

        SingletonManager.canPlay = false;
        GameOverScript.isGameOver = true;

        _pauseButton.gameObject.SetActive(false);
        _magnetParticles.Stop();

        yield return new WaitForSeconds(1.7f);

        _gameOverSource.Play();
        _gameOverParticles.Play();

        yield return new WaitForSeconds(0.2f);

        _respawnCoins.interactable = GameManager.allRedCoins >= 200 * GameManager.lifesCount;
        _rebornButton.interactable = GameManager.allRedCoins >= 200 * GameManager.lifesCount;

        _coinsRebornText.text = (200 * GameManager.lifesCount).ToString();
        _gameOverPanel.SetActive(true);
    }

    public IEnumerator StartMethod()
    {
        yield return new WaitForSeconds(1.5f);
        _gameOverSource.volume = SingletonManager.soundVolume;
        _pauseButton.gameObject.SetActive(true);
    }

    public IEnumerator Reborn()
    {
        foreach (Transform child in _obstacleKiller)
            child.gameObject.SetActive(false);

        _gameOverPanel.SetActive(false);
        _pauseButton.gameObject.SetActive(true);

        yield return null;
    }
}

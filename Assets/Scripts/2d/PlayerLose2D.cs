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

    [SerializeField] private PlayerController2D _player;
    private Transform _obstacleKiller;

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Lose" && _player.PlayerState != PlayerState.Death && _player.PlayerState != PlayerState.None)
        {
            if (!_player.GameManager.isShield)
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
        _player.GameManager.isShield = false;
    }

    public IEnumerator Lose()
    {
        Obstacle.StopSlowMotion();
        StartCoroutine(_player.PlayerMovementNonControlable.Lose());

        SingletonManager.instance.musicSource.Pause();
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        _player.PlayerState = PlayerState.Death;
        _player.PlayerAnimations.PlayerAnimator.Play("Lose");

        SingletonManager.instance.canPlay = false;
        _player.GameOver.isGameOver = true;

        _pauseButton.gameObject.SetActive(false);
        _magnetParticles.Stop();

        yield return new WaitForSeconds(1.7f);

        _gameOverSource.Play();
        _gameOverParticles.Play();

        yield return new WaitForSeconds(0.2f);

        _respawnCoins.interactable = _player.GameManager.allRedCoins >= 200 * _player.GameManager.lifesCount;
        _rebornButton.interactable = _player.GameManager.allRedCoins >= 200 * _player.GameManager.lifesCount;

        _coinsRebornText.text = (200 * _player.GameManager.lifesCount).ToString();
        _gameOverPanel.SetActive(true);
    }

    public IEnumerator StartMethod()
    {
        yield return new WaitForSeconds(1.5f);
        _gameOverSource.volume = SingletonManager.instance.soundVolume;
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

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLose : MonoCache
{
    [SerializeField] private ParticleSystem _gameOverParticles;
    [SerializeField] private PlayerController _player;

    [Space]
    [SerializeField] private AudioSource _shieldSource;
    [SerializeField] private AudioSource _gameOverSource;

    [Space]
    [SerializeField] private GameObject _gameOverPanel;
    [SerializeField] private Button _respawnButton;
    [SerializeField] private Button _pauseButton;

    [Space]
    [SerializeField] private Text _coinsRebornText;
    [SerializeField] private Button _respawnCoinsButton;

    private Transform _obstacleKiller;

    public void Start()
    {
        _respawnButton.onClick.AddListener(delegate { _player.RebornMethod(false); });
        _respawnCoinsButton.onClick.AddListener(delegate { _player.RebornMethod(false); });
    }
    public void OnCollisionEnter(Collision collision)
    {
        if ((collision.gameObject.CompareTag("Lose") || collision.gameObject.CompareTag("RampLose")) && !_player.GameOver.isGameOver)
        {
            _obstacleKiller = collision.gameObject.transform.parent;
            if (collision.gameObject.CompareTag("RampLose"))
                _obstacleKiller = collision.gameObject.transform.parent.gameObject.transform.parent;

            if (!_player.GameManager.isShield)
            {
                _player.PlayerState = PlayerState.Death;
                _player.GameOver.isGameOver = true;
                StartCoroutine(Lose());
            }
            else
                StartCoroutine(ShieldLose());
        }
    }
    public IEnumerator ShieldLose()
    {
        foreach (Transform child in _obstacleKiller)
            child.gameObject.SetActive(false);

        if (_player.PlayerAnimations.ShieldAnimator.gameObject.activeInHierarchy)
            _player.PlayerAnimations.ShieldAnimator.SetTrigger("crush");

        _shieldSource.Play();

        yield return new WaitForSeconds(0.7f);

        _player.PlayerAnimations.ShieldAnimator.gameObject.SetActive(false);
        _player.GameManager.isShield = false;
    }
    public IEnumerator Lose()
    {
        Obstacle.StopSlowMotion();
        StartCoroutine(_player.PlayerMovementControlable.Lose());
        StartCoroutine(_player.PlayerAnimations.Lose());
        StartCoroutine(_player.PlayerMovementNonControlable.Lose());

        _player.PlayerMovementControlable.StopMoveHorizontalCoroutine();

        SingletonManager.instance.musicSource.Pause();
        SingletonManager.instance.canPlay = false;
        //_player.PlayerBonuses._magnetParticles.Stop();
        _pauseButton.gameObject.SetActive(false);

        yield return new WaitForSeconds(1.5f);

        _gameOverParticles.Play();
        _gameOverSource.Play();
        _coinsRebornText.text = (500 * _player.GameManager.lifesCount).ToString();

        yield return new WaitForSeconds(0.4f);

        _respawnCoinsButton.interactable = _player.GameManager.allOrangeCoins >= 500 * _player.GameManager.lifesCount;
        _respawnButton.interactable = _player.GameManager.allOrangeCoins >= 500 * _player.GameManager.lifesCount;

        _gameOverPanel.SetActive(true);
    }
}

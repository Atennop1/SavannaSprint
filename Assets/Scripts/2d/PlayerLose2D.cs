using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLose2D : MonoCache
{
    [SerializeField] private AudioSource gameOverSource;
    [SerializeField] private AudioSource shieldSource;

    [Space]
    public ParticleSystem gameOverParticles;
    public ParticleSystem magnetParticles;

    [Space]
    [SerializeField] private Button respawnCoins;
    [SerializeField] private Button rebornButton;
    public Button pauseButton;

    [Space]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Text coinsRebornText;

    private PlayerController2D player;
    private Transform obstacleKiller;

    public void Start()
    {
        player = PlayerController2D.instance;
    }
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Lose" && PlayerController2D.playerState != PlayerState.Death && PlayerController2D.playerState != PlayerState.None)
        {
            if (!GameManager.isShield)
            {
                obstacleKiller = collision.gameObject.transform.parent;
                StartCoroutine(Lose());
            }
            else
            {
                obstacleKiller = collision.gameObject.transform.parent;
                StartCoroutine(ShieldLose());
            }
        }
    }
    public IEnumerator ShieldLose()
    {
        shieldSource.Play();
        if (player.playerAnimations.shieldAnimator.gameObject.activeInHierarchy)
            player.playerAnimations.shieldAnimator.SetTrigger("crush");

        foreach (Transform child in obstacleKiller)
            child.gameObject.SetActive(false);

        yield return new WaitForSeconds(0.7f);

        player.playerAnimations.shieldAnimator.gameObject.SetActive(false);
        GameManager.isShield = false;
    }
    public IEnumerator Lose()
    {
        Obstacle.StopSlowMotion();
        StartCoroutine(player.playerMovementNonControlable.Lose());

        SingletonManager.instance.musicSource.Pause();
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        PlayerController2D.playerState = PlayerState.Death;
        player.playerAnimations.playerAnimator.Play("Lose");

        SingletonManager.canPlay = false;
        GameOverScript.isGameOver = true;

        pauseButton.gameObject.SetActive(false);
        magnetParticles.Stop();

        yield return new WaitForSeconds(1.7f);

        gameOverSource.Play();
        gameOverParticles.Play();

        yield return new WaitForSeconds(0.2f);

        respawnCoins.interactable = GameManager.allRedCoins >= 200 * GameManager.lifesCount;
        rebornButton.interactable = GameManager.allRedCoins >= 200 * GameManager.lifesCount;

        coinsRebornText.text = (200 * GameManager.lifesCount).ToString();
        gameOverPanel.SetActive(true);
    }
    public IEnumerator StartMethod()
    {
        yield return new WaitForSeconds(1.5f);
        gameOverSource.volume = SingletonManager.soundVolume;
        pauseButton.gameObject.SetActive(true);
    }
    public IEnumerator Reborn()
    {
        foreach (Transform child in obstacleKiller)
            child.gameObject.SetActive(false);

        gameOverPanel.SetActive(false);
        pauseButton.gameObject.SetActive(true);

        yield return null;
    }
}

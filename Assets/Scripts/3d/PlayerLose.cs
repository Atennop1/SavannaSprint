using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLose : MonoCache
{
    public ParticleSystem gameOverParticles;

    [Space]
    [SerializeField] private AudioSource shieldSource;
    public AudioSource gameOverSource;

    [Space]
    public GameObject gameOverPanel;
    public Button respawnButton;
    public Button pauseButton;

    [Space]
    [SerializeField] private Text coinsRebornText;
    public Button respawnCoinsButton;

    [HideInInspector] public Transform obstacleKiller;
    private PlayerController player;

    public void Start()
    {
        player = PlayerController.instance;
        respawnButton.onClick.AddListener(delegate { player.RebornMethod(false); });
        respawnCoinsButton.onClick.AddListener(delegate { player.RebornMethod(false); });
    }
    public void OnCollisionEnter(Collision collision)
    {
        if ((collision.gameObject.CompareTag("Lose") || collision.gameObject.CompareTag("RampLose")) && !GameOverScript.isGameOver)
        {
            obstacleKiller = collision.gameObject.transform.parent;
            if (collision.gameObject.CompareTag("RampLose"))
                obstacleKiller = collision.gameObject.transform.parent.gameObject.transform.parent;

            if (!GameManager.isShield)
            {
                PlayerController.playerState = PlayerState.Death;
                GameOverScript.isGameOver = true;
                StartCoroutine(Lose());
            }
            else
                StartCoroutine(ShieldLose());
        }
    }
    public IEnumerator ShieldLose()
    {
        foreach (Transform child in obstacleKiller)
            child.gameObject.SetActive(false);

        if (player.playerAnimations.shieldAnimator.gameObject.activeInHierarchy)
            player.playerAnimations.shieldAnimator.SetTrigger("crush");

        shieldSource.Play();

        yield return new WaitForSeconds(0.7f);

        player.playerAnimations.shieldAnimator.gameObject.SetActive(false);
        GameManager.isShield = false;
    }
    public IEnumerator Lose()
    {
        Obstacle.StopSlowMotion();
        StartCoroutine(player.playerMovementControlable.Lose());
        StartCoroutine(player.playerAnimations.Lose());
        StartCoroutine(player.playerMovementNonControlable.Lose());

        if (player.playerMovementControlable.moveHorizontalCoroutine != null)
            StopCoroutine(player.playerMovementControlable.moveHorizontalCoroutine);

        SingletonManager.instance.musicSource.Pause();
        SingletonManager.canPlay = false;
        player.playerBonuses.magnetParticles.Stop();
        pauseButton.gameObject.SetActive(false);

        yield return new WaitForSeconds(1.5f);

        gameOverParticles.Play();
        gameOverSource.Play();
        coinsRebornText.text = (500 * GameManager.lifesCount).ToString();

        yield return new WaitForSeconds(0.4f);

        respawnCoinsButton.interactable = GameManager.allOrangeCoins >= 500 * GameManager.lifesCount;
        respawnButton.interactable = GameManager.allOrangeCoins >= 500 * GameManager.lifesCount;

        gameOverPanel.SetActive(true);
    }
}

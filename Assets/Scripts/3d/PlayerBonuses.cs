using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBonuses : MonoBehaviour
{
    [SerializeField] private AudioSource powerUpSource;

    [Space]
    [SerializeField] private Bonus magnetBonus;
    [SerializeField] private Bonus x2CoinsBonus;
    [SerializeField] private Bonus x2Bonus;

    [Space]
    [SerializeField] private GameObject magnetObject;
    [SerializeField] private GameObject x2CoinsObject;
    [SerializeField] private GameObject x2Object;

    [Space]
    public ParticleSystem magnetParticles;

    private Coroutine magnetCoroutine;
    private Coroutine x2Coroutine;
    private Coroutine x2CoinsCoroutine;

    private PlayerController player;
    private PlayerController2D player2d;

    public void Start()
    {
        player = PlayerController.instance;
        player2d = PlayerController2D.instance;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Bonus>() != null)
        {
            powerUpSource.Play();
            Bonus bonus = other.GetComponent<Bonus>();
            switch (bonus.bonusType)
            {
                case BonusType.Magnet:
                    if (magnetCoroutine != null)
                        StopCoroutine(magnetCoroutine);
                    magnetCoroutine = CreateBonusCoroutine(magnetBonus, magnetObject, magnetParticles);
                    break;

                case BonusType.X2Coins:
                    if (x2CoinsCoroutine != null)
                        StopCoroutine(x2CoinsCoroutine);
                    x2CoinsCoroutine = CreateBonusCoroutine(x2CoinsBonus, x2CoinsObject, null);
                    break;

                case BonusType.X2:
                    if (x2Coroutine != null)
                        StopCoroutine(x2Coroutine);
                    x2Coroutine = CreateBonusCoroutine(x2Bonus, x2Object, null);
                    break;
            }
            other.GetComponent<MeshRenderer>().enabled = false;
        }
        if (other.gameObject.tag == "Shield")
        {
            GameManager.isShield = true;

            if (player)
                player.playerAnimations.shieldAnimator.gameObject.SetActive(true);
            else
                player2d.playerAnimations.shieldAnimator.gameObject.SetActive(true);

            other.gameObject.SetActive(false);
        }
    }
    public IEnumerator StartMethod()
    {
        yield return new WaitForSeconds(1.5f);

        powerUpSource.volume = 1 * SingletonManager.soundVolume;
        if (GameManager.isX2)
            x2Coroutine = CreateBonusCoroutine(x2Bonus, x2Object, null);
        if (GameManager.isMagnet)
            magnetCoroutine = CreateBonusCoroutine(magnetBonus, magnetObject, magnetParticles);
        if (GameManager.isX2Coins)
            x2CoinsCoroutine = x2CoinsCoroutine = CreateBonusCoroutine(x2CoinsBonus, x2CoinsObject, null);
    }
    public IEnumerator Reborn()
    {
        yield return new WaitForSeconds(1.6f);

        if (GameManager.isX2)
            x2Coroutine = CreateBonusCoroutine(x2Bonus, x2Object, null);
        if (GameManager.isMagnet)
            magnetCoroutine = CreateBonusCoroutine(magnetBonus, magnetObject, magnetParticles);
        if (GameManager.isX2Coins)
            x2CoinsCoroutine = CreateBonusCoroutine(x2CoinsBonus, x2CoinsObject, null);
    }
    public Coroutine CreateBonusCoroutine(Bonus bonus,  GameObject bonusObject, ParticleSystem particles)
    {
        return StartCoroutine(bonus.Activate(bonusObject, particles));
    }
}

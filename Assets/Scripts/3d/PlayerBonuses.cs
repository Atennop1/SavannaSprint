using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBonuses : MonoBehaviour
{
    [SerializeField] private AudioSource _powerUpSource;

    [Space]
    [SerializeField] private Bonus _magnetBonus;
    [SerializeField] private Bonus _x2CoinsBonus;
    [SerializeField] private Bonus _x2Bonus;

    [Space]
    [SerializeField] private GameObject _magnetObject;
    [SerializeField] private GameObject _x2CoinsObject;
    [SerializeField] private GameObject _x2Object;

    [Space]
    [SerializeField] private ParticleSystem _magnetParticles;

    private Coroutine _magnetCoroutine;
    private Coroutine _x2Coroutine;
    private Coroutine _x2CoinsCoroutine;

    private PlayerController _player;
    private PlayerController2D _player2d;

    public void Start()
    {
        _player = PlayerController.instance;
        _player2d = PlayerController2D.instance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Bonus>() != null)
        {
            _powerUpSource.Play();
            Bonus bonus = other.GetComponent<Bonus>();
            switch (bonus.bonusType)
            {
                case BonusType.Magnet:
                    if (_magnetCoroutine != null)
                        StopCoroutine(_magnetCoroutine);
                    _magnetCoroutine = CreateBonusCoroutine(_magnetBonus, _magnetObject, _magnetParticles);
                    break;

                case BonusType.X2Coins:
                    if (_x2CoinsCoroutine != null)
                        StopCoroutine(_x2CoinsCoroutine);
                    _x2CoinsCoroutine = CreateBonusCoroutine(_x2CoinsBonus, _x2CoinsObject, null);
                    break;

                case BonusType.X2:
                    if (_x2Coroutine != null)
                        StopCoroutine(_x2Coroutine);
                    _x2Coroutine = CreateBonusCoroutine(_x2Bonus, _x2Object, null);
                    break;
            }
            other.GetComponent<MeshRenderer>().enabled = false;
        }
        if (other.gameObject.tag == "Shield")
        {
            GameManager.isShield = true;

            if (_player)
                _player.PlayerAnimations.ShieldAnimator.gameObject.SetActive(true);
            else
                _player2d.PlayerAnimations.ShieldAnimator.gameObject.SetActive(true);

            other.gameObject.SetActive(false);
        }
    }

    public IEnumerator StartMethod()
    {
        yield return new WaitForSeconds(1.5f);

        _powerUpSource.volume = 1 * SingletonManager.soundVolume;
        if (GameManager.isX2)
            _x2Coroutine = CreateBonusCoroutine(_x2Bonus, _x2Object, null);
        if (GameManager.isMagnet)
            _magnetCoroutine = CreateBonusCoroutine(_magnetBonus, _magnetObject, _magnetParticles);
        if (GameManager.isX2Coins)
            _x2CoinsCoroutine = _x2CoinsCoroutine = CreateBonusCoroutine(_x2CoinsBonus, _x2CoinsObject, null);
    }
    
    public IEnumerator Reborn()
    {
        yield return new WaitForSeconds(1.6f);

        if (GameManager.isX2)
            _x2Coroutine = CreateBonusCoroutine(_x2Bonus, _x2Object, null);
        if (GameManager.isMagnet)
            _magnetCoroutine = CreateBonusCoroutine(_magnetBonus, _magnetObject, _magnetParticles);
        if (GameManager.isX2Coins)
            _x2CoinsCoroutine = CreateBonusCoroutine(_x2CoinsBonus, _x2CoinsObject, null);
    }

    public Coroutine CreateBonusCoroutine(Bonus bonus,  GameObject bonusObject, ParticleSystem particles)
    {
        return StartCoroutine(bonus.Activate(bonusObject, particles));
    }
}

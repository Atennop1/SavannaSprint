using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public enum BonusType
{
    Magnet,
    X2Coins,
    X2
}

public class Bonus : MonoCache
{
    public BonusType bonusType;
    [SerializeField] private string bonusName;

    private PlayerController _player;
    private GameManager _gameManager;

    public void Init(PlayerController player, GameManager gameManager)
    {
        _player = player;
        _gameManager = gameManager;
    }

    public IEnumerator Activate(GameObject bonusObject, ParticleSystem particles)
    {
        if (particles)
            particles.Play();

        Slider bonusSlider = bonusObject.GetComponentInChildren<Slider>();
        bonusSlider.value = 1;
        ChangeBonusActive(true);
        bonusObject.SetActive(true);

        while (bonusSlider.value != 0 && !_player.GameOver.isGameOver && _player.PlayerState != PlayerState.Changing)
        {
            int bonusTime = PlayerPrefsSafe.GetInt(bonusName);
            if (PlayerPrefsSafe.GetInt(bonusName) % 2 != 0)
                bonusTime++;

            bonusSlider.value -= 0.001f * (3f / bonusTime);
            yield return new WaitForFixedUpdate();
        }

        if (!_player.GameOver.isGameOver && _player.PlayerState != PlayerState.Changing)
        {
            ChangeBonusActive(false);
            bonusObject.SetActive(false);
            bonusSlider.value = 1;

            if (particles)
                particles.Stop();
        }
    }

    public void ChangeBonusActive(bool active)
    {
        switch (bonusType)
        {
            case BonusType.Magnet:
                _gameManager.isMagnet = active;
                break;
            case BonusType.X2Coins:
                _gameManager.isX2Coins = active;
                break;
            case BonusType.X2:
                _gameManager.isX2 = active;
                break;

        }
    }
}

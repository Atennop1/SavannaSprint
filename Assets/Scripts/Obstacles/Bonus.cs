using System.Collections;
using System.Collections.Generic;
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

    public IEnumerator Activate(GameObject bonusObject, ParticleSystem particles)
    {
        if (particles)
            particles.Play();

        Slider bonusSlider = bonusObject.GetComponentInChildren<Slider>();
        bonusSlider.value = 1;
        ChangeBonusActive(true);
        bonusObject.SetActive(true);
        while (bonusSlider.value != 0 && !GameOverScript.isGameOver && !(PlayerController.playerState == PlayerState.Changing))
        {
            int bonusTime = PlayerPrefsSafe.GetInt(bonusName);
            if (PlayerPrefsSafe.GetInt(bonusName) % 2 != 0)
                bonusTime++;

            bonusSlider.value -= 0.001f * (3f / bonusTime);
            yield return new WaitForFixedUpdate();
        }
        if (!GameOverScript.isGameOver && !(PlayerController.playerState == PlayerState.Changing))
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
                GameManager.isMagnet = active;
                break;
            case BonusType.X2Coins:
                GameManager.isX2Coins = active;
                break;
            case BonusType.X2:
                GameManager.isX2 = active;
                break;

        }
    }
}

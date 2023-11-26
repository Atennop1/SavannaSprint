using UnityEngine;

public class ShieldBonus : PermanentBonus
{
    private GameObject _shield;

    public ShieldBonus(BonusType type) : base(type) { }

    public void SelfInit(GameObject shield)
    {
        _shield = shield;
    }

    protected override void TerminationCallback()
    {
        PlayerLose playerLose = Player.GetPlayerPart<PlayerLose3D>();
        if (playerLose == null)
            playerLose = Player.GetPlayerPart<PlayerLose2D>();
            
        Player.StartCoroutine(playerLose.ShieldLose());
    }

    protected override void StartCallback()
    { 
        _shield?.gameObject.SetActive(true);
    }
}
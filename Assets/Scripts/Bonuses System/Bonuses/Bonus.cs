using System.Collections;
using UnityEngine;

public enum BonusType
{
    Magnet,
    X2Coins,
    Shield,
    X2
}

public abstract class Bonus
{
    public bool Active { get; private set; }
    public BonusType Type { get; }

    private Coroutine RunningCoroutine { get; set; }
    protected Player Player { get; private set; }

    protected Bonus(BonusType type)
    {
        Type = type;
    }

    public void Init(Player player)
    {
        Player = player;
    }

    public void StopRunningCoroutine()
    {
        if (RunningCoroutine == null) 
            return;
        
        Player.StopCoroutine(RunningCoroutine);
        RunningCoroutine = null;
    }

    public IEnumerator Activate()
    {
        Active = true;
        StartCallback();
        StopRunningCoroutine();

        RunningCoroutine = Player.StartCoroutine(TemplateActivate());
        yield return new WaitUntil(() => RunningCoroutine == null);

        if (CanFinish())
        {
            Active = false;
            TerminationCallback();
        }
        else IncorrectTerminationCallback();
    }

    public void Save()
    {
        PlayerPrefsSafe.SetInt("bonus" + Type.ToString() + "Active", Active ? 1 : 0);
        TemplateSave();
    }

    public void Load()
    {
        Active = PlayerPrefsSafe.GetInt("bonus" + Type.ToString() + "Active") == 1;
        TemplateLoad();
    }

    public void ClearSave()
    {
        PlayerPrefsSafe.SetInt("bonus" + Type.ToString() + "Active", 0);
        Active = false;
        TemplateClear();
    }
    
    protected virtual bool CanContinue()
    {
        return !Player.GameOver.IsGameOver && Player.CurrentState != PlayerState.Changing;
    }

    protected virtual void StartCallback() { }
    protected virtual IEnumerator TemplateActivate() { yield break; }

    protected virtual void TerminationCallback() { }
    protected virtual void IncorrectTerminationCallback() { }

    protected virtual void TemplateSave() { }
    protected virtual void TemplateLoad() { }
    protected virtual void TemplateClear() { }

    private bool CanFinish() { return CanContinue(); }
}

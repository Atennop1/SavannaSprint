using UnityEngine;
using System.Collections;

public abstract class PermanentBonus : Bonus
{
    public PermanentBonus(BonusType type) : base(type) { }

    protected override bool CanContinue() => true;
    
    protected override IEnumerator TemplateActivate() 
    { 
        var wait = new WaitForFixedUpdate();

        while (true)
            yield return wait;
    }
}
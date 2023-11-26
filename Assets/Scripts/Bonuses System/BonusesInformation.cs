using System.Collections.Generic;

public class BonusesInformation
{
    private readonly IEnumerable<BonusHandler> _handlers;

    public BonusesInformation(IEnumerable<BonusHandler> handlers)
    {
        _handlers = handlers;
    }

    public void Activate()
    {
        foreach (var handler in _handlers)
        {
            Bonus newBonus = handler.Type switch
            {
                BonusType.Shield => new ShieldBonus(handler.Type),
                BonusType.Magnet => new MagnetBonus(handler.Type),
                _ => new TemporaryBonus(handler.Type)
            };

            newBonus.Load();
            if (!newBonus.Active) continue;
            
            handler.Init(newBonus);
            (handler as TemporaryBonusHandler)?.SelfInit(((TemporaryBonus)newBonus).SliderValue);
            handler.Activate();
        }
    }

    public void Disable()
    {
        foreach (var handler in _handlers)
           handler.Bonus?.Save();
    }

    public void Clear()
    {
        foreach (var handler in _handlers)
            handler.Bonus?.ClearSave();
    }

    public bool IsBonusActive(BonusType type)
    {
        foreach (var handler in _handlers)
        {
            if (handler.Type == type && handler.Bonus != null)
                return handler.Bonus.Active;
        }

        return false;
    }
}

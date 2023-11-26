using System.Collections.Generic;
using UnityEngine;

public class BonusHandlersDatabase : MonoBehaviour
{
    [field: SerializeField] public List<BonusHandler> Handlers { get; private set; }
    private BonusesInformation _bonusesInfo;

    public bool IsHandlerActive(BonusType type) => _bonusesInfo.IsBonusActive(type);
    public BonusHandler GetHandler(BonusType type) => Handlers.Find(x => x.Type == type);
    public void Clear() => _bonusesInfo.Clear();
    public void Activate() => _bonusesInfo.Activate();
    public void Disable() => _bonusesInfo.Disable();
    private void Awake() => _bonusesInfo = new BonusesInformation(Handlers);

    private void OnDisable()
    {
        Disable();

        #if UNITY_EDITOR
        _bonusesInfo.Clear();
        #endif
    }
}

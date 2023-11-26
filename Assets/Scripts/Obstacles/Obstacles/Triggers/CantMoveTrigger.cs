using UnityEngine;
public class CantMoveTrigger : MonoBehaviour
{ 
    public bool Used { get; private set; }

    public void Use() => Used = true;
}

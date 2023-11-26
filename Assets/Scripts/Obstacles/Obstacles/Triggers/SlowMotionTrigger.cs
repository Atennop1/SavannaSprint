using UnityEngine;
public class SlowMotionTrigger : MonoBehaviour 
{ 
    public bool Used { get; private set; }

    public void Use() => Used = true;
}
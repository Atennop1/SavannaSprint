using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Input
{
    public class CtrlButtonInput : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private ButtonsInput _input;

        public void OnPointerDown(PointerEventData data)
        {
            _input.SetInput(InputType.Down, true);
        }
        
        public void OnPointerUp(PointerEventData data)
        {
            _input.SetInput(InputType.Down, false);
        }
    }
}

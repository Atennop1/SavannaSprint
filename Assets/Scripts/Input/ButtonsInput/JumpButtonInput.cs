using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Input
{
    public class JumpButtonInput : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private ButtonsInput _input;

        public void OnPointerDown(PointerEventData data)
        {
            _input.SetInput(InputType.Up, true);
        }
        
        public void OnPointerUp(PointerEventData data)
        {
            _input.SetInput(InputType.Up, false);
        }
    }
}
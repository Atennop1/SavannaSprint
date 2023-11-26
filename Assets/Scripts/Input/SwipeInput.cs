using UnityEngine;

namespace Game.Input
{
    public class SwipeInput : Input
    {
        private Vector2 _startTouch;
        private Vector2 _swipeDelta;
        private bool _isDragging;

        private void Update()
        {
            ActiveDown = ActiveUp = ActiveLeft = ActiveRight = false;
            if (UnityEngine.Input.GetMouseButtonDown(0))
            {
                _isDragging = true;
                _startTouch = UnityEngine.Input.mousePosition;
            }
            else if (UnityEngine.Input.GetMouseButtonUp(0))
            {
                _isDragging = false;
                Reset();
            }

            if (UnityEngine.Input.touches.Length > 0)
            {
                if (UnityEngine.Input.touches[0].phase == TouchPhase.Began)
                {
                    _isDragging = true;
                    _startTouch = UnityEngine.Input.touches[0].position;
                }
                else if (UnityEngine.Input.touches[0].phase == TouchPhase.Ended || UnityEngine.Input.touches[0].phase == TouchPhase.Canceled)
                {
                    _isDragging = false;
                    Reset();
                }
            }

            _swipeDelta = Vector2.zero;
            if (_isDragging)
            {
                if (UnityEngine.Input.touches.Length < 0)
                    _swipeDelta = UnityEngine.Input.touches[0].position - _startTouch;
                else if (UnityEngine.Input.GetMouseButton(0))
                    _swipeDelta = (Vector2)UnityEngine.Input.mousePosition - _startTouch;
            }

            if (!(_swipeDelta.magnitude > 50)) 
                return;
            
            var x = _swipeDelta.x;
            var y = _swipeDelta.y;
            
            if (Mathf.Abs(x) > Mathf.Abs(y))
            {
                ActiveLeft = x < 0;
                ActiveRight = x > 0;
            }
            else
            {
                ActiveDown = y < 0;
                ActiveUp = y > 0;
            }
            
            Reset();
        }

        private void Reset()
        {
            _startTouch = _swipeDelta = Vector2.zero;
            _isDragging = false;
        }
    }
}
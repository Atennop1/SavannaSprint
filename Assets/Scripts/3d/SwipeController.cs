using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeController : MonoBehaviour
{
    public static bool Tap, SwipeLeft, SwipeRight, SwipeUp, SwipeDown;

    private Vector2 _startTouch, _swipeDelta;
    private bool _isDraging;

    private void Update()
    {
        Tap = SwipeDown = SwipeUp = SwipeLeft = SwipeRight = false;
        #region ��-������
        if (Input.GetMouseButtonDown(0))
        {
            Tap = true;
            _isDraging = true;
            _startTouch = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            _isDraging = false;
            Reset();
        }
        #endregion

        #region ��������� ������
        if (Input.touches.Length > 0)
        {
            if (Input.touches[0].phase == TouchPhase.Began)
            {
                Tap = true;
                _isDraging = true;
                _startTouch = Input.touches[0].position;
            }
            else if (Input.touches[0].phase == TouchPhase.Ended || Input.touches[0].phase == TouchPhase.Canceled)
            {
                _isDraging = false;
                Reset();
            }
        }
        #endregion

        //���������� ���������
        _swipeDelta = Vector2.zero;
        if (_isDraging)
        {
            if (Input.touches.Length < 0)
                _swipeDelta = Input.touches[0].position - _startTouch;
            else if (Input.GetMouseButton(0))
                _swipeDelta = (Vector2)Input.mousePosition - _startTouch;
        }
        //�������� �� ������������ ����������
        if (_swipeDelta.magnitude > 50)
        {
            //����������� �����������
            float x = _swipeDelta.x;
            float y = _swipeDelta.y;
            if (Mathf.Abs(x) > Mathf.Abs(y))
            {
                if (x < 0)
                    SwipeLeft = true;
                else
                    SwipeRight = true;
            }
            else
            {
                if (y < 0)
                    SwipeDown = true;
                else
                    SwipeUp = true;
            }
            Reset();
        }
    }

    private void Reset()
    {
        _startTouch = _swipeDelta = Vector2.zero;
        _isDraging = false;
    }
}
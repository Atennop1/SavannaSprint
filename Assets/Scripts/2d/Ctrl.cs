using UnityEngine;
using UnityEngine.EventSystems;

public class Ctrl : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [field: SerializeField] public PlayerController2D Player { get; private set; }

    [SerializeField] private GameObject _runCollider;
    [SerializeField] private GameObject _ctrlCollider;

    public bool IsClicked { get; private set; }

    public void OnPointerDown(PointerEventData a)
    {
        IsClicked = true;
        if (Player.PlayerState != PlayerState.Jump && !Player.GameOver.isGameOver && Player.PlayerState != PlayerState.Changing)
            Player.PlayerState = PlayerState.Ctrl;
    }
    
    public void OnPointerUp(PointerEventData a)
    {
        IsClicked = false;
        if (Player.PlayerState == PlayerState.Ctrl && !Player.GameOver.isGameOver && Player.PlayerState != PlayerState.Changing)
            Player.PlayerState = PlayerState.Run;
    }

    private void Update()
    {
        if (!Player.GameOver.isGameOver && Player.PlayerState != PlayerState.Changing && (Player.PlayerState == PlayerState.Run || Player.PlayerState == PlayerState.Ctrl) && Time.timeScale != 0)
        {
            if (IsClicked && Player.canCtrl && Player.PlayerState == PlayerState.Ctrl)
            {
                if (Obstacle.isShowing)
                    Obstacle.StopSlowMotion();

                _ctrlCollider.SetActive(true);
                _runCollider.SetActive(false);
            }
            else if (Player.PlayerState == PlayerState.Run)
            {
                _ctrlCollider.SetActive(false);
                _runCollider.SetActive(true);
            }
        }
    }
}

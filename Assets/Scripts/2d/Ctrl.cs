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
        if (PlayerController2D.playerState != PlayerState.Jump && !GameOverScript.isGameOver && PlayerController2D.playerState != PlayerState.Changing)
            PlayerController2D.playerState = PlayerState.Ctrl;
    }
    
    public void OnPointerUp(PointerEventData a)
    {
        IsClicked = false;
        if (PlayerController2D.playerState == PlayerState.Ctrl && !GameOverScript.isGameOver && PlayerController2D.playerState != PlayerState.Changing)
            PlayerController2D.playerState = PlayerState.Run;
    }

    private void Update()
    {
        if (!GameOverScript.isGameOver && PlayerController2D.playerState != PlayerState.Changing && (PlayerController2D.playerState == PlayerState.Run || PlayerController2D.playerState == PlayerState.Ctrl) && Time.timeScale != 0)
        {
            if (IsClicked && Player.canCtrl && PlayerController2D.playerState == PlayerState.Ctrl)
            {
                if (Obstacle.isShowing)
                    Obstacle.StopSlowMotion();

                _ctrlCollider.SetActive(true);
                _runCollider.SetActive(false);
            }
            else if (PlayerController2D.playerState == PlayerState.Run)
            {
                _ctrlCollider.SetActive(false);
                _runCollider.SetActive(true);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Ctrl : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public PlayerController2D player;

    public GameObject runCol;
    public GameObject ctrlCol;

    [HideInInspector] public bool isClicked;

    public void OnPointerDown(PointerEventData a)
    {
        isClicked = true;
        if (PlayerController2D.playerState != PlayerState.Jump && !GameOverScript.isGameOver && PlayerController2D.playerState != PlayerState.Changing)
            PlayerController2D.playerState = PlayerState.Ctrl;
    }
    
    public void OnPointerUp(PointerEventData a)
    {
        isClicked = false;
        if (PlayerController2D.playerState == PlayerState.Ctrl && !GameOverScript.isGameOver && PlayerController2D.playerState != PlayerState.Changing)
            PlayerController2D.playerState = PlayerState.Run;
    }

    private void Update()
    {
        if (!GameOverScript.isGameOver && PlayerController2D.playerState != PlayerState.Changing && (PlayerController2D.playerState == PlayerState.Run || PlayerController2D.playerState == PlayerState.Ctrl) && Time.timeScale != 0)
        {
            if (isClicked && player.canCtrl && PlayerController2D.playerState == PlayerState.Ctrl)
            {
                if (Obstacle.isShowing)
                    Obstacle.StopSlowMotion();

                ctrlCol.SetActive(true);
                runCol.SetActive(false);
            }
            else if (PlayerController2D.playerState == PlayerState.Run)
            {
                ctrlCol.SetActive(false);
                runCol.SetActive(true);
            }
        }
    }
}

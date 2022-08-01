using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinMove : MonoBehaviour
{
    private bool canMove = false;

    void Update()
    {
        if (canMove && GameManager.isMagnet && !GameOverScript.isGameOver && PlayerController.playerState != PlayerState.Changing && PlayerController2D.playerState != PlayerState.Changing)
            transform.position = Vector3.MoveTowards(transform.position, Coin.playerTransform.position, Coin.moveSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("CoinDetector"))
            canMove = true;
        else
            canMove = false;
    }

    public void OnDisable()
    {
        canMove = false;
    }
}
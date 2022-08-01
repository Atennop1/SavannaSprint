using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Coin : MonoBehaviour
{
    public static Transform playerTransform;
    public static float moveSpeed = 40f;

    void Start()
    {
        if (SceneManager.GetActiveScene().name == "3d World")
            playerTransform = PlayerController.instance.transform;
        else
            playerTransform = PlayerController2D.instance.transform;
    }
}

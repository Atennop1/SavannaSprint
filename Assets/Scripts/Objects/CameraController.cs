using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraController : MonoCache
{
    public Transform player;

    private bool is2d;
    private Vector3 offset;

    void Start()
    {
        if (SceneManager.GetActiveScene().name == "3d World")
        {
            player = PlayerController.instance.transform;
            is2d = false;
        }
        else
        {
            player = PlayerController2D.instance.transform;
            is2d = true;
        }

        offset = transform.position - player.transform.position;
    }
    public override void OnTick()
    {
        Vector3 newPosition;

        if (!is2d)
            newPosition = new Vector3(offset.x + player.position.x, offset.y + player.position.y, offset.z + player.position.z);
        else
            newPosition = new Vector3(offset.x + player.position.x, offset.y, offset.z + player.position.z);
            
        transform.position = newPosition;
    }
}

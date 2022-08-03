using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraController : MonoBehaviour
{
    public Transform player;

    private bool is2d;
    [SerializeField] private Vector3 offset;

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

    public void FixedUpdate()
    {
        Vector3 newPosition;

        if (!is2d)
            newPosition = Vector3.Lerp(transform.position, player.position + offset, 0.2f);
        else
            newPosition = new Vector3(offset.x + player.position.x, offset.y, offset.z + player.position.z);
            
        transform.position = newPosition;
    }
}

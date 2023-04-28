using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform _player;
    private Vector3 offset;

    private bool _is2d;

    private void Start()
    {

        _is2d = SceneManager.GetActiveScene().name == "32d World";
        offset = transform.position - _player.transform.position;
    }

    public void FixedUpdate()
    {
        Vector3 newPosition;

        if (!_is2d)
            newPosition = Vector3.Lerp(transform.position, _player.position + offset, 0.2f);
        else
            newPosition = new Vector3(offset.x + _player.position.x, offset.y, offset.z + _player.position.z);
            
        transform.position = newPosition;
    }
}

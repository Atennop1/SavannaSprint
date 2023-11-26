using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform _player;
    private Vector3 _offset;

    private bool _is3DMode;

    private void Start()
    {
        _is3DMode = SceneManager.GetActiveScene().name == "3d World";
        _offset = transform.position - _player.transform.position;
    }

    private void FixedUpdate()
    {
        var newPosition = _is3DMode ? Vector3.Lerp(transform.position, _player.position + _offset, 0.2f)
            : new Vector3(_offset.x + _player.position.x, _offset.y, _offset.z + _player.position.z);
        transform.position = newPosition;
    }
}

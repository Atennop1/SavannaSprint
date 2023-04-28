using UnityEngine;

public class RepulsiveWall : MonoBehaviour
{
    private bool _collided;

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.TryGetComponent<PlayerMovementControlable>(out PlayerMovementControlable movement) && !_collided)
        {
            Debug.Log("Collide");
            movement.CancelMoveHorizontal();
            _collided = true;
        }
    }
}

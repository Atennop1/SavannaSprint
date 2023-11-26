using UnityEngine;
using System.Collections;

public class RepulsiveWallTrigger : MonoBehaviour
{
    private bool _collided;

    private void OnCollisionEnter(Collision other)
    {
        if (!other.gameObject.TryGetComponent(out PlayerHorizontalMovement movement) || _collided) 
            return;
        
        movement.CancelMoveHorizontal();
        StartCoroutine(CoolDownCoroutine());
        _collided = true;
    }

    private IEnumerator CoolDownCoroutine()
    {
        yield return new WaitForSeconds(0.1f);
        _collided = false;
    }
}

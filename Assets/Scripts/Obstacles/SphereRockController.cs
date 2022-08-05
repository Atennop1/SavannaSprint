using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereRockController : MonoBehaviour
{
    [SerializeField] private bool isLeft;

    [Space]
    [SerializeField] private BoxCollider triggerCollider;
    [SerializeField] private Rigidbody rockRigidbody;

    [Space]
    [SerializeField] private Vector3 forceVector;

    private PlayerMovementNonControlable _playerMovement;

    public void Init(PlayerMovementNonControlable playerMovement)
    {
        _playerMovement = playerMovement;
    }

    public void OnEnable()
    {
        triggerCollider.center = new Vector3(triggerCollider.center.x, triggerCollider.center.y, (isLeft ? -36 : 36) * _playerMovement.Speed / 15);
        rockRigidbody.transform.localPosition = Vector3.zero;
    }

    public void MoveRock()
    {
        rockRigidbody.AddForce(forceVector);
    }
}
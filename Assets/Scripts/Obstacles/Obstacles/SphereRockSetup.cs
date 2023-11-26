using UnityEngine;

public class SphereRockSetup : MonoBehaviour
{
    [SerializeField] private float _zCoefficient;

    [Space]
    [SerializeField] private BoxCollider _triggerCollider;
    [SerializeField] private Rigidbody _rockRigidbody;

    [Space]
    [SerializeField] private Vector3 _forceVector;

    private PlayerForwardMovement _playerForwardMovement;

    public void Init(PlayerForwardMovement playerForwardMovement)
    {
        _playerForwardMovement = playerForwardMovement;
    }

    public void MoveRock()
    {
        _rockRigidbody.AddForce(_forceVector);
    }

    private void OnEnable()
    {
        if (_playerForwardMovement == null) 
            return;
        
        _triggerCollider.center = new Vector3(_triggerCollider.center.x, _triggerCollider.center.y, 36 * _zCoefficient * _playerForwardMovement.Speed / 15);
        _rockRigidbody.transform.localPosition = Vector3.zero;
    }
}
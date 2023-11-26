using UnityEngine;

public class SphereRock : MonoBehaviour
{
    private AudioSource _source;
    private Rigidbody _rockRigidbody;

    private void Start()
    {
        _rockRigidbody = GetComponent<Rigidbody>();
        _source = GetComponent<AudioSource>();
        _source.volume = MusicPlayer.Instance.Volume;
        OnEnable();
    }

    private void OnEnable()
    {
        if (_rockRigidbody != null)
            _rockRigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezePositionZ;

        transform.rotation = Quaternion.identity;
    }

    private void OnDisable()
    {
        if (_rockRigidbody != null)
            _rockRigidbody.constraints = RigidbodyConstraints.FreezeAll;
            
        transform.rotation = Quaternion.identity;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.TryGetComponent(out GroundTrigger _) &&
            (!collision.gameObject.TryGetComponent(out SphereRockFinishTrigger _) || _rockRigidbody.constraints == 
            (RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ))) return;
        
        _source.Play();
        if (collision.gameObject.TryGetComponent(out SphereRockFinishTrigger _))
            _rockRigidbody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereRock : MonoBehaviour
{
    private AudioSource source;
    private Rigidbody rockRigidbody;

    public void Start()
    {
        rockRigidbody = GetComponent<Rigidbody>();
        source = GetComponent<AudioSource>();
        source.volume = SingletonManager.soundVolume;
        OnEnable();
    }

    public void OnEnable()
    {
        if (rockRigidbody != null)
            rockRigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezePositionZ;

        transform.rotation = Quaternion.identity;
    }

    public void OnDisable()
    {
        if (rockRigidbody != null)
            rockRigidbody.constraints = RigidbodyConstraints.FreezeAll;
            
        transform.rotation = Quaternion.identity;
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Road") || collision.gameObject.CompareTag("Finish") && rockRigidbody.constraints != (RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ))
        {
            source.Play();

            if (collision.gameObject.CompareTag("Finish"))
                rockRigidbody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
        }
    }
}
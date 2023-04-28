using System.Collections;
using UnityEngine;

public class CoinsRotate : MonoBehaviour
{
    [SerializeField] private float _speed;
    private Coroutine _rotateCoroutine;

    public void StopRotation()
    {
        if (_rotateCoroutine != null)
            StopCoroutine(_rotateCoroutine);
    }

    private void OnEnable()
    {
        transform.rotation = Quaternion.identity;
        StopRotation();
        _rotateCoroutine = StartCoroutine(Rotate());
    }

    private IEnumerator Rotate()
    {
        while (true)
        {
            transform.Rotate(0, _speed * Time.deltaTime, 0);
            yield return new WaitForFixedUpdate();
        }
    }
}

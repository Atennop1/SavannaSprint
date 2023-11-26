using System.Collections;
using UnityEngine;

public class CoinRotate : MonoBehaviour
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
        var wait = new WaitForFixedUpdate();
        while (true)
        {
            transform.Rotate(0, _speed * Time.deltaTime, 0);
            yield return wait;
        }
    }
}

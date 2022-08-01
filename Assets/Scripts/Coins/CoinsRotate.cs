using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinsRotate : MonoBehaviour
{
    public float speed;

    public void OnEnable()
    {
        transform.rotation = Quaternion.identity;;
        if (gameObject.activeInHierarchy) 
            StartCoroutine(Rotate());
    }

    private IEnumerator Rotate()
    {
        while (true)
        {
            transform.Rotate(0, speed * Time.deltaTime, 0);
            yield return new WaitForFixedUpdate();
        }
    }
}

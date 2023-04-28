using UnityEngine;

public class Coin : MonoBehaviour
{
    [field: SerializeField] public Transform Player { get; private set; }
    [field: SerializeField] public float MoveSpeed { get; private set; } = 40f;

    private void Init(Transform player)
    {
        Player = player;
    }
}

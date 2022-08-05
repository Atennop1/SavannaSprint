using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinMove : MonoBehaviour
{
    [SerializeField] private Transform _playerPosition;
    [SerializeField] private float _moveSpeed = 40f;

    private bool _canMove;
    private PlayerController _player;
    private PlayerController2D _player2d;

    public void Init(PlayerController player, PlayerController2D player2d)
    {
        _player = player;
        _player2d = player2d;
    }

    private void Update()
    {
        if (_canMove && 
            ((_player != null && _player.GameManager.isMagnet && !_player.GameOver.isGameOver && _player.PlayerState != PlayerState.Changing) ||
            (_player2d != null && _player2d.GameManager.isMagnet && !_player2d.GameOver.isGameOver && _player2d.PlayerState != PlayerState.Changing)))
        {
            transform.position = Vector3.MoveTowards(transform.position, _playerPosition.position, _moveSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("CoinDetector"))
            _canMove = true;
        else
            _canMove = false;
    }

    private void OnDisable()
    {
        _canMove = false;
    }
}
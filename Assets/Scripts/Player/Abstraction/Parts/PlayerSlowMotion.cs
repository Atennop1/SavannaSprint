using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSlowMotion : MonoBehaviour, IPlayerPart
{
    [SerializeField] private Player _player;
    [SerializeField] private Image _guideImage;

    private float _timer;
    private Obstacle _currentObstacle;
    private PlayerForwardMovement _playerForwardMovement;

    public bool IsActive { get; private set; }

    public IEnumerator StartSlowMotion()
    {
        _currentObstacle.StartSlowMotion();
        IsActive = true;
        _timer = 0;
            
        float slowMotionTime = 0;
        slowMotionTime = 0.25f / (_playerForwardMovement.Speed / 15);

        while (_timer < slowMotionTime)
        {
            _timer += Time.deltaTime;
            Time.timeScale = Mathf.Lerp(1, 0.001f, _timer * (1 / slowMotionTime));
            yield return new WaitForFixedUpdate();
        }
    }

    public void StopSlowMotion()
    {
        if (!IsActive) 
            return;
        
        _timer = 1;
        Time.timeScale = 1;

        _guideImage.enabled = false;
        IsActive = false;

        _player.Input.SetFreezeAll(_currentObstacle, false);
        _currentObstacle.StopSlowMotion();
    }

    public void CantMove(Obstacle obstacle)
    {
        if (_player.CurrentState == PlayerState.Ctrl)
            _player.CurrentState = PlayerState.Run;
        
        obstacle.Init(_player, _guideImage);
        obstacle.CantMove();
        _currentObstacle = obstacle;
    }

    public IEnumerator Lose()
    {
        StopSlowMotion();
        yield return null;
    }

    public IEnumerator Change()
    { 
        StopSlowMotion();
        yield return null; 
    }

    public IEnumerator Main() { yield return null; }
    public IEnumerator Reborn(bool ad) {  yield return null; }
    public void SetupSkin(Skin info) { }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out CantMoveTrigger cantMoveTrigger) && !cantMoveTrigger.Used)
        {
            CantMove(other.transform.parent.GetComponent<Obstacle>());
            cantMoveTrigger.Use();
        }

        if (!other.TryGetComponent(out SlowMotionTrigger slowMotionTrigger) || slowMotionTrigger.Used ||
            _player.CurrentState != PlayerState.Run) return;
        
        StartCoroutine(StartSlowMotion());
        slowMotionTrigger.Use();
    }

    private void Awake()
    {
        _playerForwardMovement = _player.GetPlayerPart<PlayerForwardMovement>();
    }
}

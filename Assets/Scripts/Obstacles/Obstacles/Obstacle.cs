using UnityEngine;
using UnityEngine.UI;
using Game.Input;

public class Obstacle : MonoBehaviour
{
    [SerializeField] private string _guideAnimationName;
    [SerializeField] private Collider _slowMotionCollider;

    [Space]
    [SerializeField] private bool _canMoveRight;
    [SerializeField] private bool _canMoveLeft;
    [SerializeField] private bool _canMoveUp;
    [SerializeField] private bool _canMoveDown;

    private Image _guideImage;
    private Player _player;

    public bool IsGuided { get; private set; }

    public void Init(Player player, Image guideImage)
    {
        _player = player;
        _guideImage = guideImage;
    }

    public void StartSlowMotion()
    {
        if (IsGuided)
            return;

        var imageAnimator = _guideImage.GetComponent<Animator>();
        _guideImage.enabled = true;

        if (_guideAnimationName == "Wall")
        {
            _canMoveLeft = transform.position.x == 3.3f || transform.position.x == 0;
            _canMoveRight = transform.position.x == -3.3f || transform.position.x == 0;

            if (_canMoveRight && !_canMoveLeft)
                _guideAnimationName = "Right";

            if (_canMoveLeft && !_canMoveRight)
                _guideAnimationName = "Left";
        }
        imageAnimator.Play(_guideAnimationName);

        _player.Input.SetFreeze(InputType.Down, this, !_canMoveDown);
        _player.Input.SetFreeze(InputType.Left, this, !_canMoveLeft);
        _player.Input.SetFreeze(InputType.Right, this, !_canMoveRight);
        _player.Input.SetFreeze(InputType.Up, this, !_canMoveUp);
    }

    public void StopSlowMotion()
    {
        PlayerPrefs.SetInt("isGuided" + _guideAnimationName + (_player == null ? "2D" : "3D"), 1);
        IsGuided = true;
    }

    public void CantMove()
    {
        IsGuided = PlayerPrefs.HasKey("isGuided" + _guideAnimationName + (_player == null ? "2D" : "3D"));
        _slowMotionCollider.enabled = !IsGuided;

        if (!IsGuided)
            _player.Input.SetFreezeAll(this, true);
    }
}

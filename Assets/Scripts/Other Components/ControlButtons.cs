using UnityEngine;
using UnityEngine.SceneManagement;

public class ControlButtons : MonoBehaviour
{
    [SerializeField] private Player _player;
    [SerializeField] private SceneChanger _sceneChanger;
    [SerializeField] private PlayerStatisticsView _statisticsView;
    [SerializeField] private BonusHandlersDatabase _bonusesDatabase;

    [Space]
    [SerializeField] private AudioSource _selectSource;
    [SerializeField] private GameObject _pauseMenu;

    private bool _isFocused = true;
    private PlayerSlowMotion _playerSlowMotion;

    public void QuitToMenu()
    {
        _selectSource.Play();
        _statisticsView.Clear();
        _bonusesDatabase.Clear();
        _sceneChanger.LoadScene(0);
        Destroy(MusicPlayer.Instance);
    }
    
    public void Restart()
    {
        _selectSource.Play();
        _statisticsView.Clear();
        _bonusesDatabase.Clear();
        _sceneChanger.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Pause()
    {
        if (_isFocused)
            _selectSource.Play();

        if (!_playerSlowMotion.IsActive)
            Time.timeScale = 0;

        _pauseMenu.SetActive(true);
        MusicPlayer.Instance.MusicSource.Pause();
    }

    public void Resume()
    {
        _selectSource.Play();

        if (!_playerSlowMotion.IsActive)
            Time.timeScale = 1;

        _pauseMenu.SetActive(false);
        MusicPlayer.Instance.MusicSource.UnPause();
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().name != "Menu")
            _selectSource.volume = 0.5f * MusicPlayer.Instance.Volume;

        _playerSlowMotion = _player.GetPlayerPart<PlayerSlowMotion>();
    }

    private void OnApplicationPause(bool pause)
    {
        if (!pause || SceneManager.GetActiveScene().name == "Menu" || _player.GameOver.IsGameOver ||
            _player.CurrentState == PlayerState.Changing || _player.CurrentState == PlayerState.Death ||
            (_playerSlowMotion != null && !_playerSlowMotion.IsActive)) return;
        
        _isFocused = false;
        Pause();
    }

    private void OnApplicationFocus(bool focus)
    {
        OnApplicationPause(!focus);
    }
}
using UnityEngine;

public enum GameState
{
    MainMenu,
    Gameplay,
    Death
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private GameState _currentGameState;
    private GameState _lastGameState;

    public GameState State => _currentGameState;
    
    [SerializeField] private GameObject _deathScreen;
    private PlayerHealth _playerHealth;
    
    private void Awake()
    {
        if (Instance && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    void Start()
    {
        _currentGameState = GameState.MainMenu;
        _playerHealth = FindFirstObjectByType<PlayerHealth>();
        StartGame();
    }
    public void SwitchState(GameState newState)
    {
        if (newState == _currentGameState)
        {
            Debug.LogWarning($"Already in {_currentGameState} state, cannot change");
            return;
        }

        _currentGameState = newState;
        Debug.Log($"Game State: {_currentGameState}");
    }
    
    public void StartGame()
    {
        SwitchState(GameState.Gameplay);
        
        ScoreManager.Instance.ResetScore();
        
        _playerHealth.ResetHealth();
        _playerHealth.gameObject.SetActive(true);
        
        WaveManager.Instance.ResetAllWaves();
        
        // Hide death screen
        _deathScreen.SetActive(false);
    }

    public void EndGame()
    {
        SwitchState(GameState.MainMenu);
    }

    public void PlayerDied()
    {
        SwitchState(GameState.Death);
        _deathScreen.gameObject.SetActive(true);
    }
}
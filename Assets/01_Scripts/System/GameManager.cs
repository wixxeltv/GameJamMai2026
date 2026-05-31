using UnityEngine;

public enum GameState
{
    MainMenu,
    Gameplay,
    Death,
    Paused,
    Conversation
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private GameState _currentGameState;
    private GameState _lastGameState;

    public GameState State => _currentGameState;
    
    [SerializeField] private GameObject _deathScreen;
    [SerializeField] private GameObject _pauseMenu; 
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

        _lastGameState = _currentGameState; // Always store last state
        _currentGameState = newState;
        
        HandleStateChange(newState);
        
        Debug.Log($"Game State: {_currentGameState}");
    }
    
    private void HandleStateChange(GameState newState)
    {
        switch (newState)
        {
            case GameState.Gameplay:
                Time.timeScale = 1f;
                break;
                
            case GameState.Paused:
                Time.timeScale = 0f;
                if (_pauseMenu != null) _pauseMenu.SetActive(true);
                break;
                
            case GameState.Conversation:
                Time.timeScale = 1f;
                break;
                
            case GameState.Death:
                Time.timeScale = 1f;
                _deathScreen.SetActive(true);
                break;
                
            case GameState.MainMenu:
                Time.timeScale = 1f;
                break;
        }
    }
    
    public void StartGame()
    {
        SwitchState(GameState.Gameplay);
        
        ScoreManager.Instance.ResetScore();
        
        _playerHealth.ResetHealth();
        _playerHealth.gameObject.SetActive(true);
        
        WaveManager.Instance.ResetAllWaves();
        
        _deathScreen.SetActive(false);
    }

    public void EndGame()
    {
        SwitchState(GameState.MainMenu);
    }

    public void PlayerDied()
    {
        SwitchState(GameState.Death);
    }
    
    public void Pause()
    {
        if (_currentGameState == GameState.Paused) return; // Don't pause if already paused
        SwitchState(GameState.Paused);
    }

    public void UnPause()
    {
        if (_currentGameState != GameState.Paused) return;
        
        // Return to the state before pause
        if (_lastGameState == GameState.Paused)
            _lastGameState = GameState.Gameplay; // Safety fallback
            
        SwitchState(_lastGameState);
    }
    
    // For conversations
    public void StartConversation()
    {
        if (_currentGameState == GameState.Conversation) return;
        SwitchState(GameState.Conversation);
    }
    
    public void EndConversation()
    {
        if (_currentGameState != GameState.Conversation) return;
        
        // Return to the state before conversation
        if (_lastGameState == GameState.Conversation)
            _lastGameState = GameState.Gameplay; // Safety fallback
            
        SwitchState(_lastGameState);
    }
    
    // Check if player can perform actions
    public bool CanPlayerAct()
    {
        return _currentGameState == GameState.Gameplay;
    }
}
using UnityEngine;

public enum GameState
{
    MainMenu,
    Gameplay,
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private GameState _currentGameState;
    private GameState _lastGameState;

    public GameState State => _currentGameState;
    
    private void Awake()
    {
        if (Instance && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    void Start()
    {
        _currentGameState = GameState.MainMenu;
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
    }

    public void EndGame()
    {
        SwitchState(GameState.MainMenu);
    }
}
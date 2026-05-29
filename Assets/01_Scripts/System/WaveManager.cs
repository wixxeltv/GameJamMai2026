using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [SerializeField] private Wave[] waves;
    private int _currentWave;
    private int _enemiesKilled;
    
    private WaveInfoUI _waveInfoUI;
    private EnemyManager _enemyManager;

    public static WaveManager Instance;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        _waveInfoUI = FindFirstObjectByType<WaveInfoUI>();
        _enemyManager = FindFirstObjectByType<EnemyManager>();
        StartWave();
    }
    
    private void StartWave()
    {
        _waveInfoUI.SetWaveCount(_currentWave);
        _enemyManager.SpawnInterval = (waves[_currentWave].spawnInterval);
        _enemyManager.SetEnemies(waves[_currentWave].enemies);
    }

    public void EnemyKilled() => _enemiesKilled++;
    
    private void Update()
    {
        if (_enemiesKilled >= waves[_currentWave].enemeisToKill)
        {
            EndWave();
        }
    }

    public void EndWave()
    {
        Debug.Log("Moving to new round");
        _currentWave++;
        StartWave();
    }
    
}

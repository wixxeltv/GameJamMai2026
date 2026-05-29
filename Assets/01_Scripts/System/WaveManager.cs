using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [SerializeField] private Wave[] waves;
    private int _currentWave;
    
    private WaveInfoUI _waveInfoUI;
    private EnemyManager _enemyManager;

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

    public void EndWave()
    {
        _currentWave++;
        StartWave();
    }
    
}

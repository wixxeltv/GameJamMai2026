using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class WaveManager : MonoBehaviour
{
    [SerializeField] private Wave[] waves;
    [SerializeField] private float _timeBetweenWaves = 5f;
    [SerializeField] private AudioClip _waveSound;
    [SerializeField] private AudioClip _babyCelebration;

    public UnityEvent<int> OnWaveStarted;
    public UnityEvent OnWaveEnd;
    public UnityEvent<float> OnWaveTimerTick;
    public UnityEvent OnAllWavesCompleted;

    public static WaveManager Instance;

    private int _currentWave;
    private int _enemiesKilled;
    private bool _isTransitioning;

    private WaveInfoUI _waveInfoUI;
    private EnemyManager _enemyManager;
    private ComboSystem _comboSystem;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        _waveInfoUI = FindFirstObjectByType<WaveInfoUI>();
        _enemyManager = FindFirstObjectByType<EnemyManager>();
        _comboSystem = FindFirstObjectByType<ComboSystem>();
        AudioManager.Instance.PlayMusic(AudioManager.Instance.attackBGM, 80f);
        StartWave();
    }

    private void Update()
    {
        if (_isTransitioning) return;
        if (_currentWave >= waves.Length) return;

        if (_enemiesKilled >= waves[_currentWave].enemeisToKill)
            StartCoroutine(EndWaveRoutine());
    }

    private void StartWave()
    {
        _enemiesKilled = 0;
        _isTransitioning = false;

        Wave wave = waves[_currentWave];
        _waveInfoUI?.SetWaveCount(_currentWave);

        if (wave.isBoss)
        {
            _waveInfoUI?.SetEnemiesLeft(1);
            if (AudioManager.Instance) AudioManager.Instance.PlayMusic(AudioManager.Instance.bossBGM, 80);
            _enemyManager.SetSpawning(false);
            _enemyManager.SpawnBoss(wave.enemies[0]);
        }
        else
        {
            _waveInfoUI?.SetEnemiesLeft(wave.enemeisToKill);
            _enemyManager.SetEnemies(wave.enemies);
            _enemyManager.SpawnInterval = wave.spawnInterval;
            _enemyManager.SetMaxEnemies(wave.maxEnemies > 0 ? wave.maxEnemies : 999);
            _enemyManager.SetSpawning(true);
        }

        _comboSystem?.GenerateCombos(_currentWave + 1);
        OnWaveStarted?.Invoke(_currentWave);
    }
    
    public void ResetAllWaves()
    {
        StopAllCoroutines();
        _currentWave = 0;
        _isTransitioning = false;
        _enemiesKilled = 0;
        _enemyManager.SetSpawning(false);
        _enemyManager.KillAllEnemies();
        
        // Reset audio
        AudioManager.Instance.PlayMusic(AudioManager.Instance.attackBGM, 70f);
        
        StartWave();
    }
    
    public void ResetWaveCount()
    {
        _currentWave = -1;
        _enemyManager.SetSpawning(false);
        _enemyManager.KillAllEnemies();
        StartWave();
    }

    private IEnumerator EndWaveRoutine()
    {
        _waveInfoUI?.SetEnemiesLeft(0);
        _isTransitioning = true;
        _enemyManager.SetSpawning(false);
        _enemyManager.KillAllEnemies();
        AudioManager.Instance.PlaySfx(_waveSound, 100f);
        AudioManager.Instance.PlaySfx(_babyCelebration, 100f);
        float timer = _timeBetweenWaves;
        while (timer > 0f)
        {
            OnWaveTimerTick?.Invoke(timer);
            timer -= Time.deltaTime;
            yield return null;
        }

        _currentWave++;

        if (_currentWave >= waves.Length)
        {
            OnAllWavesCompleted?.Invoke();
            yield break;
        }

        StartWave();
        OnWaveEnd?.Invoke();
    }

    public void EnemyKilled()
    { 
        _enemiesKilled++;
        _waveInfoUI?.SetEnemiesLeft(waves[_currentWave].enemeisToKill-_enemiesKilled);
    }
}

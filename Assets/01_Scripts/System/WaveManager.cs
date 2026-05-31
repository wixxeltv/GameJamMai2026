using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class WaveManager : MonoBehaviour
{
    [SerializeField] private Wave[] waves;
    [SerializeField] private float _timeBetweenWaves = 5f;
    [SerializeField] private AudioClip _waveSound;
    [SerializeField] private AudioClip _babyCelebration;
    [SerializeField] private GameObject _bossHealthUI;

    public UnityEvent<int> OnWaveStarted;
    public UnityEvent OnWaveEnd;
    public UnityEvent<float> OnWaveTimerTick;
    public UnityEvent OnAllWavesCompleted;

    public static WaveManager Instance;

    private int _currentWave;
    private int _enemiesKilled;
    private bool _isTransitioning;
    private bool _tutorialCompletedThisSession;
    public bool _waitingForDialogue;

    private TutorialUI _tutorialUI;
    private WaveInfoUI _waveInfoUI;
    private EnemyManager _enemyManager;
    private ComboSystem _comboSystem;
    private PrebossFightUI _prebossFightUI;

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
        _tutorialUI = FindFirstObjectByType<TutorialUI>();
        _prebossFightUI = FindFirstObjectByType<PrebossFightUI>();
        
        // Subscribe to dialogue complete event
        if (_prebossFightUI != null)
            _prebossFightUI.OnDialogueComplete.AddListener(OnBossDialogueComplete);
        
        StartWave();
    }

    private void Update()
    {
        if (_isTransitioning || _waitingForDialogue) return; // Check both flags
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

        if (wave.isTutorial)
        {
            if (_tutorialCompletedThisSession)
            {
                Debug.Log("Tutorial already completed this session, skipping...");
                _tutorialUI?.gameObject.SetActive(false);
                _enemiesKilled = wave.enemeisToKill;
            }
            else
            {
                _tutorialUI.StartTutorial();
                _enemyManager.SetSpawning(false);
            }
        }
        else if(!wave.isTutorial && _currentWave == 1)
        {
            _tutorialUI.gameObject.SetActive(false);
            AudioManager.Instance.ChangeBGM(AudioManager.Instance.attackBGM);
        }
        
        if (wave.isBoss)
        {
            _waveInfoUI?.SetEnemiesLeft(1);
            
            // Check if there's a pre-boss dialogue
            if (_prebossFightUI != null) // Add this field to Wave class
            {
                _waitingForDialogue = true;
                _prebossFightUI.StartDialogue();
                // Don't start boss fight yet - wait for dialogue to complete
                return;
            }
            
            StartBossFight(wave);
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
    
    private void StartBossFight(Wave wave)
    {
        if (AudioManager.Instance) AudioManager.Instance.ChangeBGM(AudioManager.Instance.bossBGM);
        _bossHealthUI?.SetActive(true);
        _enemyManager.SetSpawning(false);
        _enemyManager.SpawnBoss(wave.enemies[0]);
    }
    
    private void OnBossDialogueComplete()
    {
        _waitingForDialogue = false;
        
        // Now actually start the boss fight
        Wave wave = waves[_currentWave];
        StartBossFight(wave);
        
        _comboSystem?.GenerateCombos(_currentWave + 1);
        OnWaveStarted?.Invoke(_currentWave);
    }
    
    public void ResetAllWaves()
    {
        StopAllCoroutines();
        _currentWave = 0;
        _isTransitioning = false;
        _waitingForDialogue = false;
        _enemiesKilled = 0;
        _bossHealthUI?.SetActive(false);
        _enemyManager.SetSpawning(false);
        _enemyManager.KillAllEnemies();
        
        AudioManager.Instance.PlayMusic(AudioManager.Instance.attackBGM, 70f);
        
        StartWave();
    }
    
    public void ResetWaveCount()
    {
        _currentWave = 0;
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
        
        if (waves[_currentWave].isTutorial)
        {
            _tutorialCompletedThisSession = true;
        }
        
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
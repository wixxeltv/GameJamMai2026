using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class WaveManager : MonoBehaviour
{
    [SerializeField] private Light _light;
    [SerializeField] private Color _bossLight;
    [SerializeField] private Color _normalLight;
    
    [Header("Waves")]
    [SerializeField] private Wave[] waves;
    [SerializeField] private float _timeBetweenWaves = 5f;
    
    [Header("Audio")]
    [SerializeField] private AudioClip _waveSound;
    [SerializeField] private AudioClip _babyCelebration;
    
    [Header("UI References")]
    [SerializeField] private GameObject _bossHealthUI;
    [SerializeField] private GameObject _creditsCanvas;

    [Header("Events")]
    public UnityEvent<int> OnWaveStarted;
    public UnityEvent OnWaveEnd;
    public UnityEvent<float> OnWaveTimerTick;
    public UnityEvent OnAllWavesCompleted;

    public static WaveManager Instance;
    public bool waitingForDialogue;

    private int _currentWave;
    private int _enemiesKilled;
    private bool _isTransitioning;
    private bool _tutorialCompletedThisSession;

    private TutorialUI _tutorialUI;
    private WaveInfoUI _waveInfoUI;
    private EnemyManager _enemyManager;
    private ComboSystem _comboSystem;
    private PrebossFightUI _prebossFightUI;
    private PostBossConvoUI _postBossConvoUI;
    private SceneTransitionUI _sceneTransitionUI;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        CacheReferences();
        SubscribeToEvents();
        StartWave();
    }

    private void CacheReferences()
    {
        _waveInfoUI = FindFirstObjectByType<WaveInfoUI>();
        _enemyManager = FindFirstObjectByType<EnemyManager>();
        _comboSystem = FindFirstObjectByType<ComboSystem>();
        _tutorialUI = FindFirstObjectByType<TutorialUI>();
        _prebossFightUI = FindFirstObjectByType<PrebossFightUI>();
        _postBossConvoUI = FindFirstObjectByType<PostBossConvoUI>();
        _sceneTransitionUI = FindFirstObjectByType<SceneTransitionUI>();
    }

    private void SubscribeToEvents()
    {
        if (_prebossFightUI != null)
            _prebossFightUI.OnDialogueComplete.AddListener(OnPreBossDialogueComplete);
        
        if (_postBossConvoUI != null)
            _postBossConvoUI.OnDialogueComplete.AddListener(OnPostBossDialogueComplete);
    }

    private void Update()
    {
        if (GameManager.Instance.State != GameState.Gameplay) return;
        if (_isTransitioning || waitingForDialogue) return;
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
            HandleTutorialWave(wave);
        else if (!wave.isTutorial && _currentWave == 1)
            HandleFirstRealWave();
        
        if (wave.isBoss)
            HandleBossWave(wave);
        else
            HandleNormalWave(wave);

        _comboSystem?.GenerateCombos(_currentWave + 1);
        OnWaveStarted?.Invoke(_currentWave);
    }

    private void HandleTutorialWave(Wave wave)
    {
        if (_tutorialCompletedThisSession)
        {
            _tutorialUI?.gameObject.SetActive(false);
            _enemiesKilled = wave.enemeisToKill;
        }
        else
        {
            _tutorialUI.StartTutorial();
            _enemyManager.SetSpawning(false);
        }
    }

    private void HandleFirstRealWave()
    {
        _tutorialUI.gameObject.SetActive(false);
        AudioManager.Instance.ChangeBGM(AudioManager.Instance.attackBGM);
    }

    private void HandleBossWave(Wave wave)
    {
        _waveInfoUI?.SetEnemiesLeft(1);

        if (_prebossFightUI != null)
        {
            waitingForDialogue = true;
            _prebossFightUI.StartDialogue();
            return;
        }

        StartBossFight(wave);
    }

    private void HandleNormalWave(Wave wave)
    {
        _waveInfoUI?.SetEnemiesLeft(wave.enemeisToKill);
        _enemyManager.SetEnemies(wave.enemies);
        _enemyManager.SpawnInterval = wave.spawnInterval;
        _enemyManager.SetMaxEnemies(wave.maxEnemies > 0 ? wave.maxEnemies : 999);
        _enemyManager.SetSpawning(true);
    }

    private void StartBossFight(Wave wave)
    {
        AudioManager.Instance.ChangeBGM(AudioManager.Instance.bossBGM);
        _light.color = _bossLight;
        
        _bossHealthUI?.SetActive(true);
        _enemyManager.SetSpawning(false);
        _enemyManager.SpawnBoss(wave.enemies[0]);
    }
    
    private IEnumerator EndWaveRoutine()
    {
        _waveInfoUI?.SetEnemiesLeft(0);
        _isTransitioning = true;
        _enemyManager.SetSpawning(false);
        _enemyManager.KillAllEnemies();
        
        PlayWaveEndSounds();
        
        if (waves[_currentWave].isTutorial)
            _tutorialCompletedThisSession = true;

        yield return RunCountdownTimer();

        _currentWave++;

        if (_currentWave >= waves.Length)
        {
            OnAllWavesCompleted?.Invoke();
            yield break;
        }

        StartWave();
        OnWaveEnd?.Invoke();
    }

    private void PlayWaveEndSounds()
    {
        AudioManager.Instance.PlaySfx(_waveSound, 100f);
        AudioManager.Instance.PlaySfx(_babyCelebration, 100f);
    }

    private IEnumerator RunCountdownTimer()
    {
        float timer = _timeBetweenWaves;
        while (timer > 0f)
        {
            OnWaveTimerTick?.Invoke(timer);
            timer -= Time.deltaTime;
            yield return null;
        }
    }
    
    private void OnPreBossDialogueComplete()
    {
        waitingForDialogue = false;
        
        Wave wave = waves[_currentWave];
        StartBossFight(wave);
        
        _comboSystem?.GenerateCombos(_currentWave + 1);
        OnWaveStarted?.Invoke(_currentWave);
    }

    public void BossKilled()
    {
        Wave wave = waves[_currentWave];
        
        if (wave.isBoss && _postBossConvoUI != null)
        {
            waitingForDialogue = true;
            StartCoroutine(PostBossDialogueRoutine());
        }
        else
        {
            StartCoroutine(EndWaveRoutine());
        }
    }

    private IEnumerator PostBossDialogueRoutine()
    {
        yield return new WaitForSeconds(1f);
        
        _isTransitioning = true;
        _enemyManager.SetSpawning(false);
        
        _postBossConvoUI.StartDialogue();
    }

    private void OnPostBossDialogueComplete()
    {
        bool isLastWave = _currentWave >= waves.Length - 1;
        
        if (isLastWave)
            StartCoroutine(ShowCreditsRoutine());
        else
            StartCoroutine(EndWaveRoutine());
    }
    
    private IEnumerator ShowCreditsRoutine()
    {
        _sceneTransitionUI.DarkenScreen();
        AudioManager.Instance.ChangeBGM(AudioManager.Instance.creditsBGM);
        
        yield return new WaitForSeconds(1f);
        
        _bossHealthUI?.SetActive(false);
        
        if (_creditsCanvas != null)
            _creditsCanvas.SetActive(true);
        
        OnAllWavesCompleted?.Invoke();
        _sceneTransitionUI.LightenScreen();
    }
    
    public void EnemyKilled()
    {
        _enemiesKilled++;
        _waveInfoUI?.SetEnemiesLeft(waves[_currentWave].enemeisToKill - _enemiesKilled);
    }

    public void ResetAllWaves()
    {
        StopAllCoroutines();
        _currentWave = 0;
        _isTransitioning = false;
        waitingForDialogue = false;
        _enemiesKilled = 0;
        _light.color = _normalLight;
        
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
}
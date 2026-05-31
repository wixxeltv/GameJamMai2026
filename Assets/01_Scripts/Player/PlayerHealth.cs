using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float _maxHp = 100f;

    [SerializeField] private float _invincibilityDuration = 1f;
    
    public float CurrentHp => _currentHp;
    public float MaxHp => _maxHp;
    public bool IsAlive => _currentHp > 0f;

    public UnityEvent OnDeath;
    public UnityEvent<float> OnHealthChanged;

    private float _currentHp;
    private float _invincibilityTimer;
    
    [SerializeField] private AudioClip happyvoicelineSFX;
    [SerializeField] private AudioClip healSFX;
    [SerializeField] private Transform respawnPosition;
    
    private ProgressBar _progressBar;
    private PlayerFeedback _playerFeedback;

    private void Start()
    {
        _progressBar = FindFirstObjectByType<ProgressBar>();
        _playerFeedback = GetComponent<PlayerFeedback>();
        _progressBar.maximum = (int)_maxHp;
        _progressBar.current = (int)CurrentHp;
        _currentHp = _maxHp;
    }

    private void Update()
    {
        _progressBar.current = (int)CurrentHp;
        if (_invincibilityTimer > 0f)
            _invincibilityTimer -= Time.deltaTime;
    }

    public void TakeDamage(float damage)
    {
        if (_invincibilityTimer > 0f) return;

        _currentHp = Mathf.Max(0f, _currentHp - damage);
        _invincibilityTimer = _invincibilityDuration;

        OnHealthChanged?.Invoke(_currentHp);
        CameraShake.Instance?.Shake(0.5f, 0.2f);
        _playerFeedback.StartBlinking(_invincibilityDuration);

        if (_currentHp <= 0f) Die();
    }

    public void Heal(float amount)
    {
        _currentHp = Mathf.Min(_currentHp + amount, _maxHp);
        AudioManager.Instance.PlaySfx(healSFX);
        AudioManager.Instance.PlaySfx(happyvoicelineSFX);
        OnHealthChanged?.Invoke(_currentHp);
    }

    public void ResetHealth()
    {
        StopAllCoroutines();
        _currentHp = _maxHp;
        _invincibilityTimer = 0f;
        gameObject.transform.position = respawnPosition.position;
        OnHealthChanged?.Invoke(_currentHp);
    }
    private void Die()
    {
        _progressBar.current = 0;
        _playerFeedback.DeathEffect();
        OnDeath?.Invoke();
        gameObject.SetActive(false);
        GameManager.Instance.PlayerDied();
    }
}

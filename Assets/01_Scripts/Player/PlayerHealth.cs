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
        
        _playerFeedback.StartBlinking(_invincibilityDuration);

        if (_currentHp <= 0f) Die();
    }

    public void ResetHealth()
    {
        gameObject.SetActive(true);
        _currentHp = _maxHp;
    }
    private void Die()
    {
        _progressBar.current = 0;
        OnDeath?.Invoke();
        gameObject.SetActive(false);
        GameManager.Instance.PlayerDied();
    }
}

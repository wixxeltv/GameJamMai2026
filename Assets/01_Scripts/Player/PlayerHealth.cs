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

    private void Start()
    {
        _currentHp = _maxHp;
    }

    private void Update()
    {
        if (_invincibilityTimer > 0f)
            _invincibilityTimer -= Time.deltaTime;
    }

    public void TakeDamage(float damage)
    {
        if (_invincibilityTimer > 0f) return;

        _currentHp = Mathf.Max(0f, _currentHp - damage);
        _invincibilityTimer = _invincibilityDuration;

        OnHealthChanged?.Invoke(_currentHp);

        if (_currentHp <= 0f) Die();
    }

    private void Die()
    {
        OnDeath?.Invoke();
        gameObject.SetActive(false);
    }
}

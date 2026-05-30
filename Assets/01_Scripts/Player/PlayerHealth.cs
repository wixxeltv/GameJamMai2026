using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float _maxHp = 100f;

    [SerializeField] private float _invincibilityDuration = 1f;
    
    [Header("Blink Settings")]
    [SerializeField] private float _blinkInterval = 0.1f;
    [SerializeField] private Color _blinkColor = Color.white;
    
    public float CurrentHp => _currentHp;
    public float MaxHp => _maxHp;
    public bool IsAlive => _currentHp > 0f;

    public UnityEvent OnDeath;
    public UnityEvent<float> OnHealthChanged;

    private float _currentHp;
    private float _invincibilityTimer;
    private bool _isBlinking;
    
    private ProgressBar _progressBar;
    private Renderer _renderer;
    private Material _material;
    private Color _originalColor;
    private Coroutine _blinkCoroutine;

    private void Start()
    {
        _progressBar = FindFirstObjectByType<ProgressBar>();
        _progressBar.maximum = (int)_maxHp;
        _progressBar.current = (int)CurrentHp;
        _currentHp = _maxHp;
        
        _renderer = GetComponent<Renderer>();
        if (_renderer != null)
        {
            // Get the material instance to avoid modifying the shared material
            _material = _renderer.material;
            _originalColor = _material.color;
        }
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
        
        if (!_isBlinking && _material != null)
        {
            StartBlinking();
        }

        if (_currentHp <= 0f) Die();
    }

    private void Die()
    {
        _progressBar.current = 0;
        OnDeath?.Invoke();
        gameObject.SetActive(false);
    }
    
    
    private void StartBlinking()
    {
        if (_blinkCoroutine != null)
        {
            StopCoroutine(_blinkCoroutine);
        }
        _blinkCoroutine = StartCoroutine(BlinkCoroutine());
    }
    
    private IEnumerator BlinkCoroutine()
    {
        _isBlinking = true;
        float elapsedTime = 0f;
        bool isBlinkOn = false;

        while (elapsedTime < _invincibilityDuration)
        {
            // Toggle between blink color and original color
            _material.color = isBlinkOn ? _blinkColor : _originalColor;

            isBlinkOn = !isBlinkOn;
            
            yield return new WaitForSeconds(_blinkInterval);
            elapsedTime += _blinkInterval;
        }

        // Ensure we return to original color
        _material.color = _originalColor;
        _isBlinking = false;
    }
}

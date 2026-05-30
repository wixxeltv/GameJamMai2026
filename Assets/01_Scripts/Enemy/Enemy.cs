using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class Enemy : MonoBehaviour
{
    [Header("Wobble")] [SerializeField] private Transform objectToWobble;
    [SerializeField] private float wobbleSpeed = 10f;
    [SerializeField] private float wobbleAmount = 5f;
    private Quaternion _initialWobbleRotation;

    [Header("Stats")] [SerializeField] protected float _maxHp = 30f;
    [SerializeField] protected float _moveSpeed = 3f;
    [SerializeField] protected int _score = 0;
    [SerializeField] private float _wrongColorDamageMultiplier = 0.1f;
    [SerializeField] private float _wrongColorKnockbackMultiplier = 0.3f;

    [Header("Couleur")] [SerializeField] private bool _isColorless = false;
    [SerializeField] private ColorType _enemyColor = ColorType.Red;

    public bool IsColorless => _isColorless;
    public ColorType EnemyColor => _enemyColor;

    protected void SetEnemyColor(ColorType color) => _enemyColor = color;
    public bool IsAlive => _currentHp > 0f && !_isDying;
    public float CurrentHp => _currentHp;
    public float MaxHp => _maxHp;

    protected float _currentHp;
    protected Transform _player;
    private Renderer _renderer;
    private ProgressBar _healthBar;
    private Collider _collider;
    private float _startY;
    protected Vector3 _movement;

    private bool _isDying;
    private EnemyFeedbackBase _enemyFeedback;

    protected virtual void Awake()
    {
        _renderer = GetComponentInChildren<Renderer>();
        _healthBar = GetComponent<ProgressBar>();
        _collider = GetComponent<Collider>();
        _startY = transform.position.y;
    }

    protected virtual void Update()
    {
        if (!_isDying && _player != null)
        {
            // Store the movement direction for wobble
            Vector3 targetPosition = _player.position;
            Vector3 direction = (targetPosition - transform.position).normalized;
            _movement = direction * _moveSpeed;
        }
    }

    private void LateUpdate()
    {
        if (!_isDying)
        {
            transform.position = new Vector3(transform.position.x, _startY, transform.position.z);
            HandleWobble();
        }
    }

    protected virtual void Start()
    {
        _currentHp = _maxHp;
        _player = GameObject.FindGameObjectWithTag("Player")?.transform;
        _enemyFeedback = GetComponent<EnemyFeedbackBase>();

        if (objectToWobble)
            _initialWobbleRotation = objectToWobble.localRotation;
        if (_healthBar)
        {
            _healthBar.minimum = 0;
            _healthBar.maximum = _maxHp;
            _healthBar.current = _maxHp;
        }
    }

    public void TakeDamage(float damage, ColorType bulletColor, Vector3 fromPosition = default)
    {
        if (_isDying) return;
        bool wrongColor = !_isColorless && bulletColor != _enemyColor;
        if (_enemyFeedback) _enemyFeedback.Blink(wrongColor);
        if (wrongColor) damage *= _wrongColorDamageMultiplier;
        _currentHp -= damage;
        if (_healthBar) _healthBar.current = _currentHp;
        if (_player != null)
            _enemyFeedback?.Knockback(_player.position, wrongColor ? _wrongColorKnockbackMultiplier : 1f);
        if (_currentHp <= 0f) Die();
    }

    protected virtual void Die()
    {
        if (_isDying) return;
        _isDying = true;
        if (_collider) _collider.enabled = false;
        _enemyFeedback?.DeathEffect();
        WaveManager.Instance.EnemyKilled();
        ScoreManager.Instance.IncreaseScore(_score);
    }

    public void Kill()
    {
        if (_isDying) return;
        _currentHp = 0;
        _isDying = true;
        if (_collider) _collider.enabled = false;
        _enemyFeedback?.DeathEffect();
    }

    private void HandleWobble()
    {
        if (!objectToWobble) return;

        if (_movement.magnitude > 0.1f)
        {
            float wobbleAngle = Mathf.Sin(Time.time * wobbleSpeed) * wobbleAmount;

            objectToWobble.localRotation = _initialWobbleRotation * Quaternion.Euler(0f, 0f, wobbleAngle);
        }
        else
        {
            objectToWobble.localRotation = Quaternion.Lerp(objectToWobble.localRotation, _initialWobbleRotation,
                Time.deltaTime * wobbleSpeed);
        }
    }

    protected Vector3 GetSeparationForce(float radius = 2f, float strength = 3f)
    {
        Vector3 force = Vector3.zero;
        Collider[] neighbours = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider col in neighbours)
        {
            if (col.gameObject == gameObject) continue;
            if (!col.TryGetComponent<Enemy>(out _)) continue;
            Vector3 away = transform.position - col.transform.position;
            away.y = 0f;
            if (away.sqrMagnitude > 0f)
                force += away.normalized / away.magnitude;
        }

        return force * strength;
    }
}
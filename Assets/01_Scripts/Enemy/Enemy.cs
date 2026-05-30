using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class Enemy : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] protected float _maxHp = 30f;
    [SerializeField] protected float _moveSpeed = 3f;
    [SerializeField] protected int _score = 0;

    [Header("Couleur")]
    [SerializeField] private bool _isColorless = false;
    [SerializeField] private ColorType _enemyColor = ColorType.Red;

    [Header("Couleurs visuelles")]
    [SerializeField] private Color _redColor = Color.red;
    [SerializeField] private Color _yellowColor = Color.yellow;
    [SerializeField] private Color _blueColor = Color.cyan;
    [SerializeField] private Color _colorlessColor = Color.white;

    public bool IsColorless => _isColorless;
    public ColorType EnemyColor => _enemyColor;
    public bool IsAlive => _currentHp > 0f && !_isDying;
    public float CurrentHp => _currentHp;
    public float MaxHp => _maxHp;

    protected float _currentHp;
    protected Transform _player;
    private Renderer _renderer;

    private bool _isDying;
    private EnemyFeedback _enemyFeedback;
    private ProgressBar _healthBar;

    protected virtual void Awake()
    {
        _renderer = GetComponentInChildren<Renderer>();
    }

    protected virtual void Start()
    {
        _currentHp = _maxHp;
        _player = GameObject.FindGameObjectWithTag("Player")?.transform;
        _healthBar = GetComponent<ProgressBar>();
        _enemyFeedback = GetComponent<EnemyFeedback>();
        _healthBar.maximum = _maxHp;
        _healthBar.current = _maxHp;
      //  _healthBar?.SetValues(_maxHp, _maxHp);
        ApplyVisualColor();
    }

    public void TakeDamage(float damage, ColorType bulletColor)
    {
        _currentHp -= damage;
        if (_isDying) return;
        if (_enemyFeedback) _enemyFeedback.Blink();
        if (_healthBar) _healthBar.current=_currentHp;
        if (!_isColorless && bulletColor != _enemyColor) return;
        _currentHp -= damage;
        //_healthBar?.SetValues(_currentHp, _maxHp);
        if (_currentHp <= 0f) Die();
    }

    protected virtual void Die()
    {
        if (_isDying) return;
        _healthBar.current = 0;
        _isDying = true;
        _enemyFeedback?.DeathEffect();
        WaveManager.Instance.EnemyKilled();
        ScoreManager.Instance.IncreaseScore(_score);
    }
    
    public void Kill()
    {
        if (_isDying) return;
        _isDying = true;
        _enemyFeedback?.DeathEffect();
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
            if (away.sqrMagnitude > 0f)
                force += away.normalized / away.magnitude;
        }
        return force * strength;
    }

    private void ApplyVisualColor()
    {
        if (_renderer == null) return;
        _renderer.material.color = _isColorless ? _colorlessColor : _enemyColor switch
        {
            ColorType.Red    => _redColor,
            ColorType.Yellow => _yellowColor,
            ColorType.Blue   => _blueColor,
            _                => _colorlessColor
        };
    }
}

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
    public bool IsAlive => _currentHp > 0f;

    protected float _currentHp;
    protected Transform _player;
    private Renderer _renderer;

    protected virtual void Awake()
    {
        _renderer = GetComponentInChildren<Renderer>();
    }

    protected virtual void Start()
    {
        _currentHp = _maxHp;
        _player = GameObject.FindGameObjectWithTag("Player")?.transform;
        ApplyVisualColor();
    }

    public void TakeDamage(float damage, ColorType bulletColor)
    {
        if (!_isColorless && bulletColor != _enemyColor) return;
        _currentHp -= damage;
        if (_currentHp <= 0f) Die();
    }

    protected virtual void Die()
    {
        WaveManager.Instance.EnemyKilled();
        ScoreManager.Instance.IncreaseScore(_score);
        Destroy(gameObject);
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

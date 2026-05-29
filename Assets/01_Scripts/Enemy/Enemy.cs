using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float _maxHp = 30f;
    [SerializeField] private float _moveSpeed = 3f;

    [Header("Couleur aléatoire")]
    [SerializeField] private float _colorlessChance = 0.25f; // 0 à 1, les 3 couleurs se partagent le reste

    [Header("Couleurs visuelles")]
    [SerializeField] private Color _redColor = Color.red;
    [SerializeField] private Color _yellowColor = Color.yellow;
    [SerializeField] private Color _blueColor = Color.cyan;
    [SerializeField] private Color _colorlessColor = Color.white;

    private float _currentHp;
    private Transform _player;
    private Renderer _renderer;
    private bool _isColorless;
    private ColorType _color;

    private void Awake()
    {
        _renderer = GetComponentInChildren<Renderer>();
    }

    private void Start()
    {
        _currentHp = _maxHp;
        _player = GameObject.FindGameObjectWithTag("Player")?.transform;
        RollColor();
        ApplyColor();
    }

    private void Update()
    {
        if (_player == null) return;
        Vector3 dir = (_player.position - transform.position).normalized;
        transform.position += dir * _moveSpeed * Time.deltaTime;
    }

    public void TakeDamage(float damage, ColorType bulletColor)
    {
        if (!_isColorless && bulletColor != _color) return;

        _currentHp -= damage;
        if (_currentHp <= 0f) Die();
    }

    private void RollColor()
    {
        float roll = Random.value;

        if (roll < _colorlessChance)
        {
            _isColorless = true;
            return;
        }

        _isColorless = false;
        float colorRoll = Random.value;

        if (colorRoll < 0.333f)       _color = ColorType.Red;
        else if (colorRoll < 0.666f)  _color = ColorType.Yellow;
        else                           _color = ColorType.Blue;
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    private void ApplyColor()
    {
        if (_renderer == null) return;
        _renderer.material.color = _isColorless ? _colorlessColor : _color switch
        {
            ColorType.Red    => _redColor,
            ColorType.Yellow => _yellowColor,
            ColorType.Blue   => _blueColor,
            _ => _colorlessColor
        };
    }
}

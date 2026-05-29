using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerColorController : MonoBehaviour
{
    [Header("Bullet Prefabs")]
    [SerializeField] private GameObject _redBulletPrefab;
    [SerializeField] private GameObject _yellowBulletPrefab;
    [SerializeField] private GameObject _blueBulletPrefab;

    [Header("Blue Spread")]
    [SerializeField] private float _spreadAngle = 20f;

    [Header("Stats par couleur")]
    [SerializeField] private float _redDamageMultiplier = 1.5f;
    [SerializeField] private float _yellowSpeedMultiplier = 1.3f;
    [SerializeField] private float _blueDamageMultiplier = 0.6f;
    [SerializeField] private float _blueScale = 1.8f;

    [Header("Couleurs visuelles")]
    [SerializeField] private Color _redColor = Color.red;
    [SerializeField] private Color _yellowColor = Color.yellow;
    [SerializeField] private Color _blueColor = Color.cyan;

    public ColorType CurrentColor { get; private set; } = ColorType.Red;
    public float SpeedMultiplier { get; private set; } = 1f;

    private Renderer _playerRenderer;

    private void Awake()
    {
        _playerRenderer = GetComponentInChildren<Renderer>();
    }

    private void Start()
    {
        ApplyStats();
    }

    // Appelé via Unity Event (performed)
    public void OnColorSwitch(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            CurrentColor = (ColorType)(((int)CurrentColor + 1) % 3);
            ApplyStats();
        }
    }

    public void Shoot(Transform firePoint)
    {
        switch (CurrentColor)
        {
            case ColorType.Red:
                ShootRed(firePoint);
                break;
            case ColorType.Yellow:
                ShootYellow(firePoint);
                break;
            case ColorType.Blue:
                ShootBlue(firePoint);
                break;
        }
    }

    private void ShootRed(Transform firePoint)
    {
        if (_redBulletPrefab == null) return;
        var go = Instantiate(_redBulletPrefab, firePoint.position, firePoint.rotation);
        if (go.TryGetComponent<Bullet>(out var b)) b.Damage = 10f * _redDamageMultiplier;
        ApplyBulletColor(go);
    }

    private void ShootYellow(Transform firePoint)
    {
        if (_yellowBulletPrefab == null) return;
        var go = Instantiate(_yellowBulletPrefab, firePoint.position, firePoint.rotation);
        if (go.TryGetComponent<HomingBullet>(out var b)) b.Damage = 10f;
        ApplyBulletColor(go);
    }

    private void ShootBlue(Transform firePoint)
    {
        if (_blueBulletPrefab == null) return;

        float[] angles = { -_spreadAngle, 0f, _spreadAngle };
        foreach (float angle in angles)
        {
            Quaternion rot = firePoint.rotation * Quaternion.Euler(0f, angle, 0f);
            var go = Instantiate(_blueBulletPrefab, firePoint.position, rot);
            go.transform.localScale *= _blueScale;
            if (go.TryGetComponent<Bullet>(out var b)) b.Damage = 10f * _blueDamageMultiplier;
            ApplyBulletColor(go);
        }
    }

    private Color GetCurrentColor() => CurrentColor switch
    {
        ColorType.Red => _redColor,
        ColorType.Yellow => _yellowColor,
        ColorType.Blue => _blueColor,
        _ => Color.white
    };

    private void ApplyStats()
    {
        SpeedMultiplier = CurrentColor == ColorType.Yellow ? _yellowSpeedMultiplier : 1f;

        if (_playerRenderer != null)
            _playerRenderer.material.color = GetCurrentColor();
    }

    private void ApplyBulletColor(GameObject bullet)
    {
        if (bullet.TryGetComponent<Renderer>(out var r))
            r.material.color = GetCurrentColor();
    }
}

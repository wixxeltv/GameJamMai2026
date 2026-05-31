using UnityEngine;

public class EnemySpiral : Enemy
{
    [Header("Spiral")]
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private int _bulletsPerVolley = 4;
    [SerializeField] private float _fireInterval = 1.5f;

    [Header("Orbite")]
    [SerializeField] private float _orbitRadius = 8f;
    [SerializeField] private float _orbitSpeed = 45f;
    [SerializeField] private float _orbitSpeedCharge = 120f;
    [SerializeField] private float _chargeUpDuration = 0.6f;
    [SerializeField] private float _minDistanceToPlayer = 4f;

    [SerializeField] private float _movementSmoothing = 4f;

    private float _fireTimer;
    private float _volleyAngle;
    private float _orbitAngle;
    private float _currentOrbitSpeed;
    private bool _isCharging;
    private float _chargeTimer;
    private Vector3 _currentVelocity;

    protected override void Start()
    {
        base.Start();
        _fireTimer = _fireInterval;
        _orbitAngle = Random.Range(0f, 360f);
        _currentOrbitSpeed = _orbitSpeed;
    }

    private void Update()
    {
        if (_player == null || !IsAlive) return;

        Orbit();

        _fireTimer -= Time.deltaTime;

        if (!_isCharging && _fireTimer <= _chargeUpDuration)
        {
            _isCharging = true;
            _currentOrbitSpeed = _orbitSpeedCharge;
        }

        if (_fireTimer <= 0f)
        {
            FireVolley();
            _fireTimer = _fireInterval;
            _isCharging = false;
            _currentOrbitSpeed = _orbitSpeed;
        }
    }

    private void Orbit()
    {
        _orbitAngle += _currentOrbitSpeed * Time.deltaTime;

        float rad = _orbitAngle * Mathf.Deg2Rad;
        Vector3 targetPos = _player.position + new Vector3(Mathf.Cos(rad), 0f, Mathf.Sin(rad)) * _orbitRadius;

        Vector3 dir;
        float distToPlayer = Vector3.Distance(transform.position, _player.position);
        if (distToPlayer < _minDistanceToPlayer)
            dir = (transform.position - _player.position).normalized;
        else
            dir = (targetPos - transform.position).normalized;

        Vector3 desired = dir * _moveSpeed + GetSeparationForce() + GetWallAvoidanceForce();
        _currentVelocity = Vector3.Lerp(_currentVelocity, desired, _movementSmoothing * Time.deltaTime);
        transform.position += _currentVelocity * Time.deltaTime;
    }

    private void FireVolley()
    {
        if (_bulletPrefab == null) return;

        float angleStep = 360f / _bulletsPerVolley;
        for (int i = 0; i < _bulletsPerVolley; i++)
        {
            float angle = _volleyAngle + angleStep * i;
            Quaternion rot = Quaternion.Euler(0f, angle, 0f);
            TrackBullet(Instantiate(_bulletPrefab, transform.position, rot));
        }

        _volleyAngle += 15f;
    }
}

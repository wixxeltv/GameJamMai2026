using UnityEngine;

public class EnemyShooter : Enemy
{
    [Header("Tir")]
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private float _fireInterval = 2f;
    [SerializeField] private float _preferredDistance = 8f;
    [SerializeField] private float _distanceThreshold = 2f;

    [Header("Strafe")]
    [SerializeField] private float _strafeChangeInterval = 2f;

    [Header("Fluidité")]
    [SerializeField] private float _movementSmoothing = 4f;

    private float _fireTimer;
    private float _strafeDirection = 1f;
    private float _strafeTimer;
    private Vector3 _currentVelocity;

    protected override void Start()
    {
        base.Start();
        _fireTimer = _fireInterval;
        _strafeTimer = _strafeChangeInterval;
        _strafeDirection = Random.value > 0.5f ? 1f : -1f;
    }

    private void Update()
    {
        if (_player == null || !IsAlive) return;

        Move();

        _fireTimer -= Time.deltaTime;
        if (_fireTimer <= 0f)
        {
            Shoot();
            _fireTimer = _fireInterval;
        }
    }

    private void Move()
    {
        Vector3 toPlayer = (_player.position - transform.position).normalized;
        float dist = Vector3.Distance(transform.position, _player.position);

        Vector3 dir;
        if (dist > _preferredDistance + _distanceThreshold)
        {
            dir = toPlayer;
        }
        else if (dist < _preferredDistance - _distanceThreshold)
        {
            dir = -toPlayer;
        }
        else
        {
            // Strafe latéral
            dir = Vector3.Cross(toPlayer, Vector3.up) * _strafeDirection;

            _strafeTimer -= Time.deltaTime;
            if (_strafeTimer <= 0f)
            {
                _strafeDirection *= -1f;
                _strafeTimer = _strafeChangeInterval + Random.Range(-0.5f, 0.5f);
            }
        }

        Vector3 desiredVelocity = dir * _moveSpeed + GetSeparationForce();
        _currentVelocity = Vector3.Lerp(_currentVelocity, desiredVelocity, _movementSmoothing * Time.deltaTime);
        transform.position += _currentVelocity * Time.deltaTime;
    }

    private void Shoot()
    {
        if (_bulletPrefab == null) return;
        Vector3 dir = (_player.position - transform.position).normalized;
        Instantiate(_bulletPrefab, transform.position, Quaternion.LookRotation(dir), transform);
    }
}

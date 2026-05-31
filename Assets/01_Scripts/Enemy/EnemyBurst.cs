using UnityEngine;
using System.Collections;

public class EnemyBurst : Enemy
{
    [Header("Burst")]
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private int _bulletsPerBurst = 4;
    [SerializeField] private float _timeBetweenBullets = 0.1f;
    [SerializeField] private float _burstInterval = 3f;
    [SerializeField] private float _spreadAngle = 15f;

    [Header("Telegraph & Dash")]
    [SerializeField] private float _telegraphDuration = 0.5f;
    [SerializeField] private float _dashSpeed = 18f;
    [SerializeField] private float _dashDuration = 0.25f;
    [SerializeField] private float _dashMinDistance = 1.5f;

    [Header("Retreat")]
    [SerializeField] private float _retreatSpeed = 5f;
    [SerializeField] private float _retreatDuration = 1f;
    [SerializeField] private float _preferredDistance = 6f;
    [SerializeField] private float _movementSmoothing = 5f;

    private float _burstTimer;
    private bool _isActing;
    private Vector3 _currentVelocity;

    protected override void Start()
    {
        base.Start();
        _burstTimer = _burstInterval;
    }

    private void Update()
    {
        if (_player == null || !IsAlive) return;

        if (!_isActing)
        {
            float dist = Vector3.Distance(transform.position, _player.position);
            Vector3 dir = dist > _preferredDistance
                ? (_player.position - transform.position).normalized
                : Vector3.zero;
            Vector3 desired = dir * _moveSpeed + GetSeparationForce() + GetWallAvoidanceForce();
            _currentVelocity = Vector3.Lerp(_currentVelocity, desired, _movementSmoothing * Time.deltaTime);
            transform.position += _currentVelocity * Time.deltaTime;

            _burstTimer -= Time.deltaTime;
            if (_burstTimer <= 0f)
            {
                StartCoroutine(BurstSequence());
                _burstTimer = _burstInterval;
            }
        }
    }

    private IEnumerator BurstSequence()
    {
        _isActing = true;

        // Telegraph
        float t = 0f;
        Vector3 origin = transform.position;
        while (t < _telegraphDuration)
        {
            if (!IsAlive) { _isActing = false; yield break; }
            t += Time.deltaTime;
            transform.position = origin + Random.insideUnitSphere * 0.15f;
            transform.position = new Vector3(transform.position.x, origin.y, transform.position.z);
            yield return null;
        }
        transform.position = origin;

        // Dash
        float dashTimer = 0f;
        while (dashTimer < _dashDuration)
        {
            if (!IsAlive) { _isActing = false; yield break; }
            dashTimer += Time.deltaTime;
            if (_player != null)
            {
                if (Vector3.Distance(transform.position, _player.position) > _dashMinDistance)
                    transform.position += (_player.position - transform.position).normalized * _dashSpeed * Time.deltaTime;
            }
            yield return null;
        }

        // Burst
        if (_bulletPrefab != null && IsAlive)
        {
            Vector3 dir = _player != null ? (_player.position - transform.position).normalized : transform.forward;
            Quaternion baseRot = Quaternion.LookRotation(dir);
            float halfSpread = _spreadAngle * (_bulletsPerBurst - 1) / 2f;

            for (int i = 0; i < _bulletsPerBurst; i++)
            {
                if (!IsAlive) break;
                float angle = -halfSpread + _spreadAngle * i;
                TrackBullet(Instantiate(_bulletPrefab, transform.position, baseRot * Quaternion.Euler(0f, angle, 0f)));
                yield return new WaitForSeconds(_timeBetweenBullets);
            }
        }

        // Retreat
        float retreatTimer = 0f;
        Vector3 retreatDir = _player != null
            ? (transform.position - _player.position).normalized
            : -transform.forward;
        retreatDir.y = 0f;

        while (retreatTimer < _retreatDuration)
        {
            if (!IsAlive) { _isActing = false; yield break; }
            retreatTimer += Time.deltaTime;
            transform.position += retreatDir * _retreatSpeed * Time.deltaTime;
            yield return null;
        }

        _isActing = false;
    }
}

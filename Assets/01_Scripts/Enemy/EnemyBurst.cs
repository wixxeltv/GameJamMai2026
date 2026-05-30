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

    private float _burstTimer;
    private bool _isActing;

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
            Vector3 dir = (_player.position - transform.position).normalized;
            transform.position += (dir * _moveSpeed + GetSeparationForce()) * Time.deltaTime;

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

        // Telegraph : s'arrête et tremble légèrement
        float t = 0f;
        Vector3 origin = transform.position;
        while (t < _telegraphDuration)
        {
            t += Time.deltaTime;
            transform.position = origin + Random.insideUnitSphere * 0.15f;
            transform.position = new Vector3(transform.position.x, origin.y, transform.position.z);
            yield return null;
        }
        transform.position = origin;

        // Dash vers le joueur
        float dashTimer = 0f;
        while (dashTimer < _dashDuration)
        {
            dashTimer += Time.deltaTime;
            if (_player != null)
            {
                Vector3 dashDir = (_player.position - transform.position).normalized;
                transform.position += dashDir * _dashSpeed * Time.deltaTime;
            }
            yield return null;
        }
        
        // Burst
        if (_bulletPrefab != null)
        {
            Vector3 dir = (_player.position - transform.position).normalized;
            Quaternion baseRot = Quaternion.LookRotation(dir);
            float halfSpread = _spreadAngle * (_bulletsPerBurst - 1) / 2f;

            for (int i = 0; i < _bulletsPerBurst; i++)
            {
                float angle = -halfSpread + _spreadAngle * i;
                Instantiate(_bulletPrefab, transform.position, baseRot * Quaternion.Euler(0f, angle, 0f));
                yield return new WaitForSeconds(_timeBetweenBullets);
            }
        }

        _isActing = false;
    }
}

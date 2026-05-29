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

    private float _burstTimer;

    protected override void Start()
    {
        base.Start();
        _burstTimer = _burstInterval;
    }

    private void Update()
    {
        if (_player == null) return;

        MoveTowardPlayer();

        _burstTimer -= Time.deltaTime;
        if (_burstTimer <= 0f)
        {
            StartCoroutine(FireBurst());
            _burstTimer = _burstInterval;
        }
    }

    private void MoveTowardPlayer()
    {
        Vector3 dir = (_player.position - transform.position).normalized;
        transform.position += dir * _moveSpeed * Time.deltaTime;
    }

    private IEnumerator FireBurst()
    {
        if (_bulletPrefab == null) yield break;

        Vector3 dir = (_player.position - transform.position).normalized;
        Quaternion baseRot = Quaternion.LookRotation(dir);
        float halfSpread = _spreadAngle * (_bulletsPerBurst - 1) / 2f;

        for (int i = 0; i < _bulletsPerBurst; i++)
        {
            float angle = -halfSpread + _spreadAngle * i;
            Quaternion rot = baseRot * Quaternion.Euler(0f, angle, 0f);
            Instantiate(_bulletPrefab, transform.position, rot);
            yield return new WaitForSeconds(_timeBetweenBullets);
        }
    }
}

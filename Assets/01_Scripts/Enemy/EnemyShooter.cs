using UnityEngine;

public class EnemyShooter : Enemy
{
    [Header("Tir")]
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private float _fireInterval = 2f;
    [SerializeField] private float _shootRange = 12f;

    private float _fireTimer;

    protected override void Start()
    {
        base.Start();
        _fireTimer = _fireInterval;
    }

    private void Update()
    {
        if (_player == null) return;

        MoveTowardPlayer();

        _fireTimer -= Time.deltaTime;
        if (_fireTimer <= 0f && Vector3.Distance(transform.position, _player.position) <= _shootRange)
        {
            Shoot();
            _fireTimer = _fireInterval;
        }
    }

    private void MoveTowardPlayer()
    {
        Vector3 dir = (_player.position - transform.position).normalized;
        transform.position += dir * _moveSpeed * Time.deltaTime;
    }

    private void Shoot()
    {
        if (_bulletPrefab == null) return;
        Vector3 dir = (_player.position - transform.position).normalized;
        Quaternion rot = Quaternion.LookRotation(dir);
        Instantiate(_bulletPrefab, transform.position, rot);
    }
}

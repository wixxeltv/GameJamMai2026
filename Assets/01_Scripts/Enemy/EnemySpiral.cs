using UnityEngine;

public class EnemySpiral : Enemy
{
    [Header("Spiral")]
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private int _bulletsPerVolley = 4;
    [SerializeField] private float _fireInterval = 1.5f;

    private float _fireTimer;
    private float _volleyAngle;

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
        if (_fireTimer <= 0f)
        {
            FireVolley();
            _fireTimer = _fireInterval;
        }
    }

    private void MoveTowardPlayer()
    {
        Vector3 dir = (_player.position - transform.position).normalized;
        transform.position += dir * _moveSpeed * Time.deltaTime;
    }

    private void FireVolley()
    {
        if (_bulletPrefab == null) return;

        float angleStep = 360f / _bulletsPerVolley;

        for (int i = 0; i < _bulletsPerVolley; i++)
        {
            float angle = _volleyAngle + angleStep * i;
            Quaternion rot = Quaternion.Euler(0f, angle, 0f);
            Instantiate(_bulletPrefab, transform.position, rot);
        }

        _volleyAngle += 15f; // décale le pattern à chaque volée → effet spirale
    }
}

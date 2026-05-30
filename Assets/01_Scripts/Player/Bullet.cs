using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _speed = 20f;
    [SerializeField] private float _lifetime = 3f;
    [SerializeField] private ParticleSystem _hitEffect;
    public float Damage { get; set; } = 10f;
    public ColorType BulletColor { get; set; } = ColorType.Red;

    private void Start()
    {
        Destroy(gameObject, _lifetime);
    }

    private void Update()
    {
        transform.Translate(Vector3.forward * _speed * Time.deltaTime, Space.Self);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Enemy>(out var enemy))
        {
            enemy.TakeDamage(Damage, BulletColor, transform.position);
            SpawnHitEffect();
            Destroy(gameObject);
        }
    }

    private void SpawnHitEffect()
    {
        if (_hitEffect == null) return;
        var effect = Instantiate(_hitEffect, transform.position, Quaternion.identity);
        Destroy(effect.gameObject, effect.main.duration + effect.main.startLifetime.constantMax);
    }
}

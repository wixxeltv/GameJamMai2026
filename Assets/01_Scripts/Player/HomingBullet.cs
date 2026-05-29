using UnityEngine;

public class HomingBullet : MonoBehaviour
{
    [SerializeField] private float _speed = 18f;
    [SerializeField] private float _turnSpeed = 200f;
    [SerializeField] private float _lifetime = 4f;

    public float Damage { get; set; } = 10f;
    public ColorType BulletColor { get; set; } = ColorType.Yellow;

    private Transform _target;

    private void Start()
    {
        Destroy(gameObject, _lifetime);
        FindTarget();
    }

    private void Update()
    {
        if (_target == null) FindTarget();

        if (_target != null)
        {
            Vector3 dir = (_target.position - transform.position).normalized;
            Vector3 newDir = Vector3.RotateTowards(transform.forward, dir, _turnSpeed * Mathf.Deg2Rad * Time.deltaTime, 0f);
            transform.rotation = Quaternion.LookRotation(newDir);
        }

        transform.Translate(Vector3.forward * _speed * Time.deltaTime, Space.Self);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Enemy>(out var enemy))
        {
            enemy.TakeDamage(Damage, BulletColor);
            Destroy(gameObject);
        }
    }

    private void FindTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float closest = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            float dist = Vector3.Distance(transform.position, enemy.transform.position);
            if (dist < closest)
            {
                closest = dist;
                _target = enemy.transform;
            }
        }
    }
}

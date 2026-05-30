using System.Collections;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [SerializeField] private float _speed = 10f;
    [SerializeField] private float _lifetime = 5f;
    [SerializeField] private float _angularSpeed = 0f;
    [SerializeField] private float _damage = 10f;
    [SerializeField] private float _shrinkDuration = 0.15f;

    private void Start()
    {
        StartCoroutine(DestroyAfterLifetime());
    }

    private void Update()
    {
        if (_angularSpeed != 0f)
            transform.Rotate(0f, _angularSpeed * Time.deltaTime, 0f);

        transform.Translate(Vector3.forward * _speed * Time.deltaTime, Space.Self);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerHealth>()?.TakeDamage(_damage);
            StartCoroutine(ShrinkAndDestroy());
        }
    }

    private IEnumerator DestroyAfterLifetime()
    {
        yield return new WaitForSeconds(_lifetime);
        StartCoroutine(ShrinkAndDestroy());
    }

    private IEnumerator ShrinkAndDestroy()
    {
        Vector3 initialScale = transform.localScale;
        float timer = 0f;

        while (timer < _shrinkDuration)
        {
            timer += Time.deltaTime;
            transform.localScale = Vector3.Lerp(initialScale, Vector3.zero, timer / _shrinkDuration);
            yield return null;
        }

        Destroy(gameObject);
    }
}

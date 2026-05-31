using System.Collections;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [SerializeField] private float _healAmount = 20f;
    [SerializeField] private float _lifetime = 10f;
    [SerializeField] private ParticleSystem _pickupEffect;
    [SerializeField] private ParticleSystem _despawnEffect;
    [SerializeField] private AudioClip _pickupSFX;

    private void Start()
    {
        StartCoroutine(LifetimeRoutine());
    }

    private IEnumerator LifetimeRoutine()
    {
        yield return new WaitForSeconds(_lifetime);
        SpawnEffect(_despawnEffect);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        other.GetComponent<PlayerHealth>()?.Heal(_healAmount);
        SpawnEffect(_pickupEffect);
        if (_pickupSFX && AudioManager.Instance) AudioManager.Instance.PlaySfx(_pickupSFX);
        Destroy(gameObject);
    }

    private void SpawnEffect(ParticleSystem effect)
    {
        if (effect == null) return;
        var vfx = Instantiate(effect, transform.position, Quaternion.identity);
        Destroy(vfx.gameObject, vfx.main.duration + vfx.main.startLifetime.constantMax);
    }
}

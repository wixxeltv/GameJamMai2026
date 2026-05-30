using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class EnemyFeedback : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject shootPoint;
    [SerializeField] private GameObject apperance;
    
    [Header("FX")]
    [SerializeField] private AudioClip deathSfx;
    [SerializeField] private ParticleSystem deathParticles;
    
    private Coroutine _deathCoroutine;
    
    public void DeathEffect()
    {
        if (_deathCoroutine != null) return;
        
        apperance.SetActive(false);
        _deathCoroutine = StartCoroutine(DeathSequence());
    }

    private IEnumerator DeathSequence()
    {
        var effect= Instantiate(deathParticles, shootPoint.transform.position, Quaternion.identity, shootPoint.transform);
        effect.Play();
        if (AudioManager.Instance) AudioManager.Instance.PlaySfx(deathSfx, 100f);
        
        while (effect.isPlaying) yield return null;
        
        Destroy(gameObject);
    }
}

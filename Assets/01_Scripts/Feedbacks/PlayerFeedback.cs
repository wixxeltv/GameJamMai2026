using UnityEngine;
using UnityEngine.Serialization;

public class PlayerFeedback : MonoBehaviour
{
    [SerializeField] private GameObject shootPoint;
    
    [SerializeField] private AudioClip shootSFX;
    [SerializeField] private ParticleSystem shootParticles;
    
    
    [SerializeField] private AudioClip deathSFX;
    [SerializeField] private ParticleSystem deathParticles;
    
    public void ShootEffect()
    {
        var effect= Instantiate(shootParticles, shootPoint.transform.position, Quaternion.identity, shootPoint.transform);
        effect.Play();
        if (AudioManager.Instance) AudioManager.Instance.PlaySfx(shootSFX, 100f);
    }
    
    public void DeathEffect()
    {
        var effect= Instantiate(shootParticles, shootPoint.transform.position, Quaternion.identity, shootPoint.transform);
        effect.Play();
        if (AudioManager.Instance) AudioManager.Instance.PlaySfx(deathSFX, 100f);
    }
}
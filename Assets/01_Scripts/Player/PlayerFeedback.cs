using UnityEngine;

public class PlayerFeedback : MonoBehaviour
{
    [SerializeField] private GameObject shootPoint;
    
    [SerializeField] private AudioClip shootEffect;
    [SerializeField] private ParticleSystem shootParticles;
    
    public void ShootEffect()
    {
        var effect= Instantiate(shootParticles, shootPoint.transform.position, Quaternion.identity, shootPoint.transform);
        effect.Play();
        if (AudioManager.Instance) AudioManager.Instance.PlaySfx(shootEffect, 100f);
    }
}
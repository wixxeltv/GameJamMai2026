using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerFeedback : MonoBehaviour
{
    [SerializeField] private GameObject shootPoint;
    
    [SerializeField] private AudioClip playerHitSFX;
    
    [SerializeField] private AudioClip shootSFX;
    [SerializeField] private ParticleSystem shootParticles;
    
    [SerializeField] private AudioClip deathSFX;
    [SerializeField] private ParticleSystem deathParticles;
    
    [Header("Blink Settings")]
    [SerializeField] private float _blinkInterval = 0.1f;
    [SerializeField] private Color _blinkColor = Color.white;
    
    private bool _isBlinking;
    private Renderer _renderer;
    private Material _material;
    private Color _originalColor;
    private Coroutine _blinkCoroutine;

    void Start()
    {
        _renderer = GetComponent<Renderer>();
        if (_renderer != null)
        {
            // Get the material instance to avoid modifying the shared material
            _material = _renderer.material;
            _originalColor = _material.color;
        }
    }
    
    public void ShootEffect()
    {
        var effect= Instantiate(shootParticles, shootPoint.transform.position, Quaternion.identity, shootPoint.transform);
        effect.Play();
        if (AudioManager.Instance) AudioManager.Instance.PlaySfx(shootSFX, 100f);
    }
    
    public void DeathEffect()
    {
        // var effect= Instantiate(shootParticles, shootPoint.transform.position, Quaternion.identity, shootPoint.transform);
        // effect.Play();
        if (AudioManager.Instance) AudioManager.Instance.PlaySfx(deathSFX, 100f);
    }
    
    
    public void StartBlinking(float duration)
    {
        if (_isBlinking && _material == null) return;
        
        if (_blinkCoroutine != null)
        {
            StopCoroutine(_blinkCoroutine);
        }
        _blinkCoroutine = StartCoroutine(BlinkCoroutine(duration));
    }
    
    private IEnumerator BlinkCoroutine(float duration)
    {
        _isBlinking = true;
        float elapsedTime = 0f;
        bool isBlinkOn = false;
        AudioManager.Instance.PlaySfx(playerHitSFX, 100f);

        while (elapsedTime < duration)
        {
            // Toggle between blink color and original color
            _material.color = isBlinkOn ? _blinkColor : _originalColor;

            isBlinkOn = !isBlinkOn;
            
            yield return new WaitForSeconds(_blinkInterval);
            elapsedTime += _blinkInterval;
        }

        // Ensure we return to original color
        _material.color = _originalColor;
        _isBlinking = false;
    }
}
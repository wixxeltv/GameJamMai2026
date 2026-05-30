using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class EnemyFeedback : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject shootPoint;
    [SerializeField] private GameObject appearance;
    
    [Header("FX")]
    [SerializeField] private AudioClip deathSfx;
    [SerializeField] private ParticleSystem deathParticles;
    
    private Coroutine _deathCoroutine;
    
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
        _renderer = GetComponentInChildren<Renderer>();
        if (_renderer != null)
        {
            // Get the material instance to avoid modifying the shared material
            _material = _renderer.material;
            _originalColor = _material.color;
        }
    }
    
    public void DeathEffect()
    {
        if (_deathCoroutine != null) return;
        
        appearance.SetActive(false);
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
    
    public void Blink()
    {
        if (_isBlinking && _material == null) return;
        
        if (_blinkCoroutine != null)
        {
            StopCoroutine(_blinkCoroutine);
        }
        _blinkCoroutine = StartCoroutine(BlinkCoroutine(0.2f));
    }
    
    private IEnumerator BlinkCoroutine(float duration)
    {
        _isBlinking = true;
        float elapsedTime = 0f;
        bool isBlinkOn = false;

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

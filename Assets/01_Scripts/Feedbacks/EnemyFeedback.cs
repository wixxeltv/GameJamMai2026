using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class EnemyFeedback : EnemyFeedbackBase
{
    [Header("References")]
    [SerializeField] private GameObject shootPoint;
    [SerializeField] private GameObject appearance;
    [SerializeField] private Slider healthUI;
    
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
    
    public override void DeathEffect()
    {
        if (_deathCoroutine != null) return;

        if (appearance != null) appearance.SetActive(false);
        _deathCoroutine = StartCoroutine(DeathSequence());
    }

    private IEnumerator DeathSequence()
    {
        var effect= Instantiate(deathParticles, shootPoint.transform.position, Quaternion.identity, shootPoint.transform);
        effect.Play();
        if (AudioManager.Instance) AudioManager.Instance.PlaySfx(deathSfx, 100f);

        while (effect.isPlaying)
        {
            yield return null;
        }

        if (healthUI != null) Destroy(healthUI.gameObject);
        Destroy(gameObject);
    }
    
    public override void Blink()
    {
        _renderer = GetComponentInChildren<Renderer>();
        if (_renderer == null) return;
        _material = _renderer.material;
        _originalColor = _material.color;

        if (_blinkCoroutine != null)
            StopCoroutine(_blinkCoroutine);
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

using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerFeedback : MonoBehaviour
{
    [SerializeField] private GameObject shootPoint;
    
    [SerializeField] private AudioClip[] playerHitSFX;
    
    [SerializeField] private AudioClip shootSFX;
    [SerializeField] private ParticleSystem shootParticles;
    
    [SerializeField] private AudioClip deathSFX;
    [SerializeField] private ParticleSystem deathParticles;
    
    [Header("Blink Settings")]
    [SerializeField] private float _blinkInterval = 0.1f;
    [SerializeField] private Color _blinkColor = Color.white;
    
    [Header("Blink Renderer")]
    [SerializeField] private Renderer _renderer;

    private bool _isBlinking;
    private Material _material;
    private Color _originalColor;
    private Coroutine _blinkCoroutine;

    void Start()
    {
        if (_renderer != null)
            _material = _renderer.material;
    }
    
    public void ShootEffect()
    {
        var effect= Instantiate(shootParticles, shootPoint.transform.position, Quaternion.identity, shootPoint.transform);
        effect.Play();
        if (AudioManager.Instance) AudioManager.Instance.PlaySfx(shootSFX, 100f);
    }
    
    public void DeathEffect()
    {
        if (AudioManager.Instance) AudioManager.Instance.PlaySfx(deathSFX, 100f);
    }
    
    
    public void StartBlinking(float duration)
    {
        if (_material == null) return;

        if (_blinkCoroutine != null)
            StopCoroutine(_blinkCoroutine);
        _blinkCoroutine = StartCoroutine(BlinkCoroutine(duration));
    }

    private IEnumerator BlinkCoroutine(float duration)
    {
        _isBlinking = true;
        _originalColor = _material.HasProperty("_BaseColor")
            ? _material.GetColor("_BaseColor")
            : _material.color;

        float elapsedTime = 0f;
        bool isBlinkOn = false;

        if (playerHitSFX != null && playerHitSFX.Length > 0 && AudioManager.Instance)
        {
            int random = Random.Range(0, playerHitSFX.Length);
            AudioManager.Instance.PlaySfx(playerHitSFX[random], 100f);
        }

        while (elapsedTime < duration)
        {
            Color c = isBlinkOn ? _blinkColor : _originalColor;
            if (_material.HasProperty("_BaseColor")) _material.SetColor("_BaseColor", c);
            _material.color = c;
            isBlinkOn = !isBlinkOn;

            yield return new WaitForSeconds(_blinkInterval);
            elapsedTime += _blinkInterval;
        }

        if (_material.HasProperty("_BaseColor")) _material.SetColor("_BaseColor", _originalColor);
        _material.color = _originalColor;
        _isBlinking = false;
    }
}
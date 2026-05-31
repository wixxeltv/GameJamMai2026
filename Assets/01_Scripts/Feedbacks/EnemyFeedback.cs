using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnemyFeedback : EnemyFeedbackBase
{
    [Header("References")]
    [SerializeField] private GameObject shootPoint;
    [SerializeField] private GameObject appearance;
    [SerializeField] private Slider healthUI;
    [SerializeField] private Renderer _renderer;

    [Header("FX")]
    [SerializeField] private AudioClip deathSfx;
    [SerializeField] private ParticleSystem deathParticles;

    [Header("Blink Settings")]
    [SerializeField] private float _blinkInterval = 0.05f;
    [SerializeField] private int _blinkCount = 3;
    [SerializeField] private Color _blinkColor = Color.white;
    [SerializeField] private Color _wrongColorBlinkColor = Color.grey;

    private static readonly int ColorProp = Shader.PropertyToID("_BaseColor");

    [Header("Knockback")]
    [SerializeField] private float _knockbackForce = 3f;
    [SerializeField] private float _knockbackDuration = 0.15f;

    private Coroutine _deathCoroutine;
    private Coroutine _blinkCoroutine;
    private Coroutine _knockbackCoroutine;
    private MaterialPropertyBlock _propBlock;

    void Start()
    {
        if (_renderer == null) _renderer = GetComponentInChildren<Renderer>();
        _propBlock = new MaterialPropertyBlock();
    }

    public override void DeathEffect()
    {
        if (_deathCoroutine != null) return;
        if (appearance != null) appearance.SetActive(false);
        if (healthUI != null) Destroy(healthUI.gameObject);
        _deathCoroutine = StartCoroutine(DeathSequence());
    }

    private IEnumerator DeathSequence()
    {
        if (deathParticles != null && shootPoint != null)
        {
            var effect = Instantiate(deathParticles, shootPoint.transform.position, Quaternion.identity);
            effect.Play();
            if (AudioManager.Instance) AudioManager.Instance.PlaySfx(deathSfx, 100f);
            while (effect != null && effect.isPlaying)
                yield return null;
        }
        Destroy(gameObject);
    }

    public override void Knockback(Vector3 from, float forceMultiplier = 1f)
    {
        if (_knockbackCoroutine != null) StopCoroutine(_knockbackCoroutine);
        _knockbackCoroutine = StartCoroutine(KnockbackRoutine(from, forceMultiplier));
    }

    private IEnumerator KnockbackRoutine(Vector3 from, float forceMultiplier)
    {
        Vector3 dir = (transform.position - from).normalized;
        dir.y = 0f;
        float elapsed = 0f;
        while (elapsed < _knockbackDuration)
        {
            float t = 1f - (elapsed / _knockbackDuration);
            transform.position += dir * _knockbackForce * forceMultiplier * t * Time.deltaTime;
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    public override void Blink(bool wrongColor = false)
    {
        if (_renderer == null) return;
        if (_blinkCoroutine != null) StopCoroutine(_blinkCoroutine);
        _renderer.SetPropertyBlock(null);
        _blinkCoroutine = StartCoroutine(BlinkCoroutine(wrongColor ? _wrongColorBlinkColor : _blinkColor));
    }

    public override void StartBlinking(float duration)
    {
        if (_renderer == null) return;
        if (_blinkCoroutine != null) StopCoroutine(_blinkCoroutine);
        _renderer.SetPropertyBlock(null);
        _blinkCoroutine = StartCoroutine(DurationBlinkCoroutine(duration));
    }

    private IEnumerator BlinkCoroutine(Color blinkColor)
    {
        for (int i = 0; i < _blinkCount; i++)
        {
            _propBlock.SetColor(ColorProp, blinkColor);
            _renderer.SetPropertyBlock(_propBlock);
            yield return new WaitForSeconds(_blinkInterval);
            _renderer.SetPropertyBlock(null);
            yield return new WaitForSeconds(_blinkInterval);
        }
    }

    private IEnumerator DurationBlinkCoroutine(float duration)
    {
        float elapsed = 0f;
        bool blinkOn = false;
        while (elapsed < duration)
        {
            if (blinkOn)
            {
                _propBlock.SetColor(ColorProp, _blinkColor);
                _renderer.SetPropertyBlock(_propBlock);
            }
            else
            {
                _renderer.SetPropertyBlock(null);
            }
            blinkOn = !blinkOn;
            yield return new WaitForSeconds(_blinkInterval);
            elapsed += _blinkInterval;
        }
        _renderer.SetPropertyBlock(null);
    }
}

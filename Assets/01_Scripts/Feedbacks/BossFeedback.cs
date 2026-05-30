using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BossFeedback : EnemyFeedbackBase
{
    [Header("Death FX")]
    [SerializeField] private ParticleSystem _deathParticles;
    [SerializeField] private Transform _particleSpawnPoint;
    [SerializeField] private AudioClip _deathSfx;
    [SerializeField] private Slider _healthUI;

    [Header("Blink")]
    [SerializeField] private float _blinkDuration = 0.2f;
    [SerializeField] private float _blinkInterval = 0.05f;
    [SerializeField] private Color _blinkColor = Color.white;

    private Coroutine _blinkCoroutine;
    private Coroutine _deathCoroutine;
    private Color _originalColor;
    private bool _isBlinking;

    public override void Blink()
    {
        Renderer r = GetActiveRenderer();
        if (r == null) return;

        if (!_isBlinking)
            _originalColor = r.material.color;

        if (_blinkCoroutine != null)
            StopCoroutine(_blinkCoroutine);

        _blinkCoroutine = StartCoroutine(BlinkRoutine(r));
    }

    public override void DeathEffect()
    {
        if (_deathCoroutine != null) return;
        _deathCoroutine = StartCoroutine(DeathRoutine());
    }

    private IEnumerator BlinkRoutine(Renderer r)
    {
        _isBlinking = true;
        Material mat = r.material;
        float elapsed = 0f;
        bool on = false;

        while (elapsed < _blinkDuration)
        {
            if (r == null || !r.gameObject.activeInHierarchy) break;
            mat.color = on ? _blinkColor : _originalColor;
            on = !on;
            yield return new WaitForSeconds(_blinkInterval);
            elapsed += _blinkInterval;
        }

        if (r != null && r.gameObject.activeInHierarchy)
            mat.color = _originalColor;

        _isBlinking = false;
        _blinkCoroutine = null;
    }

    private IEnumerator DeathRoutine()
    {
        foreach (var r in GetComponentsInChildren<Renderer>())
            r.gameObject.SetActive(false);

        if (_deathParticles != null && _particleSpawnPoint != null)
        {
            var effect = Instantiate(_deathParticles, _particleSpawnPoint.position, Quaternion.identity);
            effect.Play();
            if (AudioManager.Instance) AudioManager.Instance.PlaySfx(_deathSfx, 100f);
            while (effect.isPlaying) yield return null;
        }

        if (_healthUI != null) Destroy(_healthUI.gameObject);
        Destroy(gameObject);
    }

    private Renderer GetActiveRenderer()
    {
        foreach (var r in GetComponentsInChildren<Renderer>())
            if (r.gameObject.activeInHierarchy) return r;
        return null;
    }
}

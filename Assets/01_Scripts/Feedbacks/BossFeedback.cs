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
    [SerializeField] private float _blinkInterval = 0.05f;
    [SerializeField] private int _blinkCount = 3;

    private static readonly int ColorProp = Shader.PropertyToID("_BaseColor");

    private Coroutine _blinkCoroutine;
    private Coroutine _deathCoroutine;
    private MaterialPropertyBlock _propBlock;

    void Start()
    {
        _propBlock = new MaterialPropertyBlock();
    }

    public override void Blink()
    {
        Renderer r = GetActiveRenderer();
        if (r == null) return;
        if (_blinkCoroutine != null) StopCoroutine(_blinkCoroutine);
        r.SetPropertyBlock(null);
        _blinkCoroutine = StartCoroutine(BlinkCoroutine(r));
    }

    public override void DeathEffect()
    {
        if (_deathCoroutine != null) return;
        if (_healthUI != null) Destroy(_healthUI.gameObject);
        _deathCoroutine = StartCoroutine(DeathRoutine());
    }

    private IEnumerator BlinkCoroutine(Renderer r)
    {
        for (int i = 0; i < _blinkCount; i++)
        {
            if (r == null || !r.gameObject.activeInHierarchy)
            {
                ResetAllPropertyBlocks();
                yield break;
            }
            _propBlock.SetColor(ColorProp, Color.white);
            r.SetPropertyBlock(_propBlock);
            yield return new WaitForSeconds(_blinkInterval);
            r.SetPropertyBlock(null);
            yield return new WaitForSeconds(_blinkInterval);
        }
    }

    private IEnumerator DeathRoutine()
    {
        ResetAllPropertyBlocks();
        foreach (var r in GetComponentsInChildren<Renderer>())
            r.gameObject.SetActive(false);

        if (_deathParticles != null && _particleSpawnPoint != null)
        {
            var effect = Instantiate(_deathParticles, _particleSpawnPoint.position, Quaternion.identity);
            effect.Play();
            if (AudioManager.Instance) AudioManager.Instance.PlaySfx(_deathSfx, 100f);
            while (effect.isPlaying) yield return null;
        }

        Destroy(gameObject);
    }

    private void ResetAllPropertyBlocks()
    {
        foreach (var r in GetComponentsInChildren<Renderer>(true))
            r.SetPropertyBlock(null);
    }

    private Renderer GetActiveRenderer()
    {
        foreach (var r in GetComponentsInChildren<Renderer>())
            if (r.gameObject.activeInHierarchy) return r;
        return null;
    }
}

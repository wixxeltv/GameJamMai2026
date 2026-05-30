using System.Collections;
using UnityEngine;
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

    [Header("Blink Settings")]
    [SerializeField] private float _blinkInterval = 0.05f;
    [SerializeField] private int _blinkCount = 3;

    private static readonly int ColorProp = Shader.PropertyToID("_BaseColor");

    private Coroutine _deathCoroutine;
    private Coroutine _blinkCoroutine;
    private Renderer _renderer;
    private MaterialPropertyBlock _propBlock;

    void Start()
    {
        _renderer = GetComponentInChildren<Renderer>();
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
            var effect = Instantiate(deathParticles, shootPoint.transform.position, Quaternion.identity, shootPoint.transform);
            effect.Play();
            if (AudioManager.Instance) AudioManager.Instance.PlaySfx(deathSfx, 100f);
            while (effect.isPlaying)
                yield return null;
        }
        Destroy(gameObject);
    }

    public override void Blink()
    {
        if (_renderer == null) return;
        if (_blinkCoroutine != null) StopCoroutine(_blinkCoroutine);
        _renderer.SetPropertyBlock(null);
        _blinkCoroutine = StartCoroutine(BlinkCoroutine());
    }

    private IEnumerator BlinkCoroutine()
    {
        for (int i = 0; i < _blinkCount; i++)
        {
            _propBlock.SetColor(ColorProp, Color.white);
            _renderer.SetPropertyBlock(_propBlock);
            yield return new WaitForSeconds(_blinkInterval);
            _renderer.SetPropertyBlock(null);
            yield return new WaitForSeconds(_blinkInterval);
        }
    }
}

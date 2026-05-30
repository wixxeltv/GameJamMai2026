using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Camera))]
public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    private Vector3 _offset;
    private Camera _cam;
    private Coroutine _shakeCoroutine;

    private void Awake()
    {
        Instance = this;
        _cam = GetComponent<Camera>();
    }

    private void OnEnable()
    {
        RenderPipelineManager.beginCameraRendering += OnBeginRender;
        RenderPipelineManager.endCameraRendering += OnEndRender;
    }

    private void OnDisable()
    {
        RenderPipelineManager.beginCameraRendering -= OnBeginRender;
        RenderPipelineManager.endCameraRendering -= OnEndRender;
    }

    private void OnBeginRender(ScriptableRenderContext ctx, Camera cam)
    {
        if (cam != _cam || _offset == Vector3.zero) return;
        cam.transform.position += _offset;
    }

    private void OnEndRender(ScriptableRenderContext ctx, Camera cam)
    {
        if (cam != _cam || _offset == Vector3.zero) return;
        cam.transform.position -= _offset;
    }

    public void Shake(float duration, float magnitude)
    {
        if (_shakeCoroutine != null) StopCoroutine(_shakeCoroutine);
        _shakeCoroutine = StartCoroutine(ShakeRoutine(duration, magnitude));
    }

    private IEnumerator ShakeRoutine(float duration, float magnitude)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = 1f - (elapsed / duration);
            float m = magnitude * t;
            _offset = new Vector3(Random.Range(-1f, 1f) * m, 0f, Random.Range(-1f, 1f) * m);
            elapsed += Time.deltaTime;
            yield return null;
        }
        _offset = Vector3.zero;
        _shakeCoroutine = null;
    }
}

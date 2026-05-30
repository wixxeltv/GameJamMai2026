using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    public Vector3 Offset { get; private set; }

    private Coroutine _shakeCoroutine;

    private void Awake()
    {
        Instance = this;
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
            float x = Random.Range(-1f, 1f) * magnitude;
            float z = Random.Range(-1f, 1f) * magnitude;
            Offset = new Vector3(x, 0f, z);
            elapsed += Time.deltaTime;
            yield return null;
        }
        Offset = Vector3.zero;
        _shakeCoroutine = null;
    }
}

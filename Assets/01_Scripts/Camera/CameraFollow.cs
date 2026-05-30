using System.Collections;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public static CameraFollow Instance { get; private set; }

    [SerializeField] private Transform _target;
    [SerializeField] private float _height = 15f;

    private Vector3 _shakeOffset;
    private Coroutine _shakeCoroutine;

    private void Awake()
    {
        Instance = this;
        Debug.Log($"[CameraFollow] Awake sur '{gameObject.name}'");
    }

    private void Start()
    {
        transform.rotation = Quaternion.Euler(90f, 0f, 0f);
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
            float currentMagnitude = magnitude * t;
            float x = Random.Range(-1f, 1f) * currentMagnitude;
            float z = Random.Range(-1f, 1f) * currentMagnitude;
            _shakeOffset = new Vector3(x, 0f, z);
            elapsed += Time.deltaTime;
            yield return null;
        }
        _shakeOffset = Vector3.zero;
        _shakeCoroutine = null;
    }

    private void LateUpdate()
    {
        if (_target == null) return;
        transform.position = new Vector3(
            _target.position.x + _shakeOffset.x,
            _height,
            _target.position.z + _shakeOffset.z
        );
    }
}

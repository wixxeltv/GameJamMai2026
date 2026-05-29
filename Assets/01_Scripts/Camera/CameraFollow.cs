using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private float _height = 15f;

    private void Start()
    {
        transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }

    private void LateUpdate()
    {
        if (_target == null) return;
        transform.position = new Vector3(_target.position.x, _height, _target.position.z);
    }
}

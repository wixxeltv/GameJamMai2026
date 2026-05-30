using UnityEngine;

public class UIFollowAndFaceCamera : MonoBehaviour
{
    [SerializeField] private Transform _target; 
    
    [SerializeField] private Vector3 _offset = new Vector3(0, 2f, 0);

    private Transform _cameraTransform;

    private void Awake() 
    {
        _cameraTransform = Camera.main.transform;
    }

    private void LateUpdate()
    {
        if (_target == null) return;

        transform.rotation = _cameraTransform.rotation;
    }
}
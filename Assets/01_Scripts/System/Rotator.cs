using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField] private Vector3 _rotationSpeed = new Vector3(0f, 180f, 0f);

    private void Update()
    {
        transform.Rotate(_rotationSpeed * Time.deltaTime, Space.Self);
    }
}

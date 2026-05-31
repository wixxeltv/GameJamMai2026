using UnityEngine;

public class ItemHoverAnimation : MonoBehaviour
{
    [Header("Hover Settings")]
    [SerializeField] private float hoverHeight = 0.5f;
    [SerializeField] private float hoverSpeed = 2f;
    [SerializeField] private float threshold = 0.25f;
    
    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 50f;
    [SerializeField] private bool rotateClockwise = true;
    
    private Vector3 upPosition;
    private Vector3 downPosition;
    private bool IsUp = false;

    private void Start()
    {
        Vector3 startPosition = transform.position;
        upPosition = new Vector3(startPosition.x, startPosition.y + hoverHeight, startPosition.z);
        downPosition = new Vector3(startPosition.x, startPosition.y - hoverHeight, startPosition.z);
    }

    private void Update()
    {
        // Hover movement
        float targetY = IsUp ? upPosition.y : downPosition.y;
        Vector3 targetPosition = new Vector3(transform.position.x, targetY, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPosition, hoverSpeed * Time.deltaTime);

        // Direction change
        if (transform.position.y >= upPosition.y - threshold || transform.position.y <= downPosition.y + threshold)
        {
            IsUp = !IsUp;
        }

        // Self rotation
        float direction = rotateClockwise ? 1f : -1f;
        transform.Rotate(Vector3.up, rotationSpeed * direction * Time.deltaTime);
    }
}
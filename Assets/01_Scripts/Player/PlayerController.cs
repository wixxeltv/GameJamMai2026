using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float _moveSpeed = 8f;

    [Header("Shooting")]
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private Transform _firePoint;
    [SerializeField] private float _fireRate = 0.2f;

    private Vector2 _moveInput;
    private bool _isFiring;
    private float _nextFireTime;

    public void OnMove(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed) _isFiring = true;
        if (context.canceled) _isFiring = false;
    }

    private void Update()
    {
        HandleMovement();
        HandleShooting();
    }

    private void HandleMovement()
    {
        Vector3 move = new Vector3(_moveInput.x, 0f, _moveInput.y).normalized * _moveSpeed * Time.deltaTime;
        transform.position += move;
    }

    private void HandleShooting()
    {
        if (_isFiring && Time.time >= _nextFireTime)
        {
            _nextFireTime = Time.time + _fireRate;
            Shoot();
        }
    }

    private void Shoot()
    {
        if (_bulletPrefab == null || _firePoint == null) return;
        Instantiate(_bulletPrefab, _firePoint.position, _firePoint.rotation);
    }
}

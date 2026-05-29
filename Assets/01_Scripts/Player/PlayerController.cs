using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float _moveSpeed = 8f;

    [Header("Shooting")]
    [SerializeField] private Transform _firePoint;
    [SerializeField] private float _fireRate = 0.2f;

    private PlayerColorController _colorController;
    private PlayerFeedback _playerFeedback;
    
    private Vector2 _moveInput;
    private bool _isFiring;
    private float _nextFireTime;

    private void Awake()
    {
        _colorController = GetComponent<PlayerColorController>();
        _playerFeedback = GetComponent<PlayerFeedback>();
    }

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
        float speed = _moveSpeed * (_colorController != null ? _colorController.SpeedMultiplier : 1f);
        Vector3 move = new Vector3(_moveInput.x, 0f, _moveInput.y).normalized * speed * Time.deltaTime;
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

    public void Shoot()
    {
        if (_firePoint == null || _colorController == null || _playerFeedback == null) return;
        _colorController.Shoot(_firePoint);
        _playerFeedback.ShootEffect();
    }
}

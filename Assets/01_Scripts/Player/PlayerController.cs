using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float _moveSpeed = 8f;

    [Header("Shooting")]
    [SerializeField] private Transform _firePoint;
    [SerializeField] private float _fireRate = 0.2f;
    
    [Header("Wobble")]
    [SerializeField] private Transform objectToWobble;
    [SerializeField] private float wobbleSpeed = 10f;
    [SerializeField] private float wobbleAmount = 5f;

    private PlayerColorController _colorController;
    private PlayerFeedback _playerFeedback;
    private Rigidbody _rb;

    private Vector2 _moveInput;
    private bool _isFiring;
    private float _nextFireTime;

    private Quaternion _initialWobbleRotation;
    private Vector3 _movement;
    
    private void Awake()
    {
        _colorController = GetComponent<PlayerColorController>();
        _playerFeedback = GetComponent<PlayerFeedback>();
        _rb = GetComponent<Rigidbody>();
        
        if (objectToWobble != null)
            _initialWobbleRotation = objectToWobble.localRotation;
    }

    private void OnEnable()
    {
        _isFiring = false;
        _moveInput = Vector2.zero;
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
        HandleShooting();
        
        _movement = new Vector3(_moveInput.x, 0f, _moveInput.y).normalized * _moveSpeed;
    }

    private void FixedUpdate()
    {
        HandleMovement();
        if (_rb != null)
        {
            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
        }
    }

    private void LateUpdate()
    {
        HandleWobble();
    }

    private void HandleWobble()
    {
        if (objectToWobble == null) return;

        if (_movement.magnitude > 0.1f)
        {
            float wobbleAngle = Mathf.Sin(Time.time * wobbleSpeed) * wobbleAmount;
            objectToWobble.localRotation = _initialWobbleRotation * Quaternion.Euler(0f, 0f, wobbleAngle);
        }
        else
        {
            objectToWobble.localRotation = Quaternion.Lerp(
                objectToWobble.localRotation,
                _initialWobbleRotation,
                Time.deltaTime * wobbleSpeed
            );
        }
    }
    
    private void HandleMovement()
    {
        float speed = _moveSpeed * (_colorController != null ? _colorController.SpeedMultiplier : 1f);
        Vector3 move = new Vector3(_moveInput.x, 0f, _moveInput.y).normalized * speed * Time.fixedDeltaTime;
        if (_rb != null)
            _rb.MovePosition(_rb.position + move);
        else
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

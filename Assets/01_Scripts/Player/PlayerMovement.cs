using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController), typeof(PlayerInputHandler), typeof(PlayerInput))]
public class PlayerMovement : MonoBehaviour
{
    
    /* property */
    [field: SerializeField] public bool OnGrounded { get; private set; } = true;
    [field: SerializeField] public float GroundedRadius { get; private set; } = 0.26f;
    [field: SerializeField] public LayerMask GroundLayers { get; private set; }
    [field: SerializeField] public float RotationSmoothTime { get; private set; } = 0.05f;

    [Tooltip("코요테 타임")] public float FallTimeout = 0.15f;
    /* Movement */
    private float _curSpeed = 0f;
    private float _verticalVelocity = 0f;
    [SerializeField] private bool bIsAnalogMovement = false;
    [SerializeField] private float SpeedChangeRate = 10;
    
    /* Velocity */
    private float _fallTimeoutDelta = 0f;

    /* Camera */
    private const float _threshold = 0.01f;
    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;
    [SerializeField] private float _cameraAngleOverride;
    private float _targetRotation;
    private float _rotationVelocity;
    private Camera _mainCamera;
    [SerializeField] private float _bottomClamp = -30;
    [SerializeField] private float _topClamp = 70;
    
    /* Class */
    private CharacterController _characterController;
    private PlayerInputHandler _inputHandler;
    private PlayerInput _playerInput;
    
    /* Inspector Obj or Class or SO */
    [SerializeField] private Transform _cameraTargetTransform;
    [SerializeField] private Transform _groundCheckTransform;
    [SerializeField] private PlayerStat _playerStat;

    /* Animation */
    [SerializeField] private Animator _animator;
    private static readonly int OnGround = Animator.StringToHash("OnGround");
    private static readonly int Speed = Animator.StringToHash("speed");
    private static readonly int Fall = Animator.StringToHash("fall");

    private bool IsCurrentDeviceMouse
    {
        get
        {
            return _playerInput.currentControlScheme == "KeyboardMouse";
        }
    }
    
    private void Awake()
    {
        TryGetComponent(out _characterController);
        TryGetComponent(out _inputHandler);
        TryGetComponent(out _playerInput);
        _mainCamera = Camera.main;
        Debug.Assert(_animator != null, "플레이어 Animator가 null입니다.");
    }

    private void Start()
    {
        _fallTimeoutDelta = FallTimeout;
    }

    private void Update()
    {
        Move();
        ProcessGravity();
        ProcessCharacterControllerMove();
    }

    private void FixedUpdate()
    {
        CheckGrounded();
    }

    private void LateUpdate()
    {
        CameraRotation();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_groundCheckTransform.position, GroundedRadius);
    }
    private void CheckGrounded()
    {
        OnGrounded = Physics.CheckSphere(_groundCheckTransform.position, GroundedRadius, GroundLayers,
            QueryTriggerInteraction.Ignore);

        _animator.SetBool(OnGround, OnGrounded);
    }
    private void CameraRotation()
    {
        // if there is an input and camera position is not fixed
        if (_inputHandler.lookVec.sqrMagnitude >= _threshold)
        {
            //Don't multiply mouse input by Time.deltaTime;
            float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

            _cinemachineTargetYaw += _inputHandler.lookVec.x * deltaTimeMultiplier;
            _cinemachineTargetPitch += -_inputHandler.lookVec.y * deltaTimeMultiplier;
        }

        // clamp our rotations so our values are limited 360 degrees
        _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, _bottomClamp, _topClamp);

        // Cinemachine will follow this target
        _cameraTargetTransform.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + _cameraAngleOverride,
            _cinemachineTargetYaw, 0.0f);
    }
    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

    private void Move()
    {
        float targetSpeed = _inputHandler.sprint ? _playerStat.RunSpeed : _playerStat.WalkSpeed;

        if (_inputHandler.moveVec == Vector2.zero)
        {
            targetSpeed = 0f;
        }
        
        float currentHorizontalSpeed = new Vector3(_characterController.velocity.x, 0.0f, _characterController.velocity.z).magnitude;

        float speedOffset = 0.1f;
        float inputMagnitude = bIsAnalogMovement ? _inputHandler.moveVec.magnitude : 1f;

        // accelerate or decelerate to target speed
        if (currentHorizontalSpeed < targetSpeed - speedOffset ||
            currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            // creates curved result rather than a linear one giving a more organic speed change
            // note T in Lerp is clamped, so we don't need to clamp our speed
            _curSpeed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                Time.deltaTime * SpeedChangeRate);

            // round speed to 3 decimal places
            _curSpeed = Mathf.Round(_curSpeed * 1000f) / 1000f;
        }
        else
        {
            _curSpeed = targetSpeed;
        }
        
        Vector3 inputDirection = new Vector3(_inputHandler.moveVec.x, 0.0f, _inputHandler.moveVec.y).normalized;
        if (_inputHandler.moveVec != Vector2.zero)
        {
            _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                              _mainCamera.transform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                RotationSmoothTime);

            // rotate to face input direction relative to camera position
            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }
        

        _animator.SetFloat(Speed, _curSpeed / _playerStat.RunSpeed);
    }

    private void ProcessCharacterControllerMove()
    {
        Vector3 targetDir = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;
        
        _characterController.Move(targetDir.normalized * (_curSpeed * Time.deltaTime) +
                                  new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
    }
    private void ProcessGravity()
    {
        if (OnGrounded)
        {
            _fallTimeoutDelta = FallTimeout;

            _animator.SetBool(Fall, false);
            
            if (_verticalVelocity < 0.0f)
            {
                _verticalVelocity = -2f;
            }
        }
        else
        {

            if (_fallTimeoutDelta >= 0.0f)
            {
                _fallTimeoutDelta -= Time.deltaTime;
            }
            else
            {
                _animator.SetBool(Fall, true);
            }

        }
        if (_verticalVelocity < _playerStat.TerminalVelocity)
        {
            _verticalVelocity += _playerStat.Gravity * Time.deltaTime;
        }
    }
}

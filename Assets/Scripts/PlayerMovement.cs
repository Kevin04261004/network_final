using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

[RequireComponent(typeof(CharacterController), typeof(PlayerInputHandler), typeof(PlayerInput))]
public class PlayerMovement : MonoBehaviour
{
    
    /* property */
    [field: SerializeField] public float RunSpeed { get; private set; } = 6f;
    [field: SerializeField] public float WalkSpeed { get; private set; } = 3f;
    [field: SerializeField] public float JumpPower { get; private set; } = 6f;
    [field: SerializeField] public float Gravity { get; private set; } = -9.81f;
    [field: SerializeField] public bool OnGrounded { get; private set; } = true;
    [field: SerializeField] public float GroundedRadius { get; private set; } = 0.26f;
    [field: SerializeField] public LayerMask GroundLayers { get; private set; }
    [field: SerializeField] public float RotationSmoothTime { get; private set; } = 0.05f;
    [Tooltip("점프 다시 가능한 시간")] public float JumpTimeout = 0.50f;

    [Tooltip("코요테 타임")] public float FallTimeout = 0.15f;
    /* Movement */
    private float _curSpeed = 0f;
    private float _verticalVelocity = 0f;
    
    /* Jump && Velocity */
    private float _fallTimeoutDelta = 0f;
    private float _jumpTimeoutDelta = 0f;
    
    /* Camera */
    private const float _threshold = 0.01f;
    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;
    [SerializeField] private float _cameraAngleOverride;
    private float _targetRotation;
    private float _rotationVelocity;
    private Camera _mainCamera;
    [Tooltip("최대 떨어지는 속력")][SerializeField] private float _terminalVelocity = 63f;
    [SerializeField] private float _bottomClamp = -30;
    [SerializeField] private float _topClamp = 70;
    
    /* Class */
    private CharacterController _characterController;
    private PlayerInputHandler _inputHandler;
    private PlayerInput _playerInput;
    
    /* Inspector Obj or Class or SO */
    [SerializeField] private Transform _cameraTargetTransform;
    [SerializeField] private Transform _groundCheckTransform;

    /* Animation */
    [SerializeField] private Animator _animator;
    private static readonly int OnGround = Animator.StringToHash("OnGround");
    private static readonly int Speed = Animator.StringToHash("speed");
    private static readonly int Jump = Animator.StringToHash("jump");
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
        _jumpTimeoutDelta = JumpTimeout;
        _fallTimeoutDelta = FallTimeout;
    }

    private void Update()
    {
        Move();
        ProcessJump();
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
        _curSpeed = _inputHandler.sprint ? RunSpeed : WalkSpeed;

        if (_inputHandler.moveVec == Vector2.zero)
        {
            _curSpeed = 0f;
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
        

        if (Math.Abs(_curSpeed - RunSpeed) < float.Epsilon)
        {
            _animator.SetFloat(Speed, RunSpeed);
        }
        else if (Math.Abs(_curSpeed - WalkSpeed) < float.Epsilon)
        {
            _animator.SetFloat(Speed, WalkSpeed);
        }
        else
        {
            _animator.SetFloat(Speed, _curSpeed / RunSpeed);
        }
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
            
            _animator.SetBool(Jump, false);
            _animator.SetBool(Fall, false);
            
            if (_verticalVelocity < 0.0f)
            {
                _verticalVelocity = -2f;
            }

            if (_jumpTimeoutDelta >= 0.0f)
            {
                _jumpTimeoutDelta -= Time.deltaTime;
            }
        }
        else
        {
            _jumpTimeoutDelta = JumpTimeout;

            if (_fallTimeoutDelta >= 0.0f)
            {
                _fallTimeoutDelta -= Time.deltaTime;
            }
            else
            {
                _animator.SetBool(Fall, true);
            }

            // if we are not grounded, do not jump
            _inputHandler.jump = false;
        }
        if (_verticalVelocity < _terminalVelocity)
        {
            _verticalVelocity += Gravity * Time.deltaTime;
        }
    }

    private void ProcessJump()
    {
        if (OnGrounded && _inputHandler.jump && _jumpTimeoutDelta <= 0.0f)
        {
            // the square root of H * -2 * G = how much velocity needed to reach desired height
            _verticalVelocity = Mathf.Sqrt(JumpPower * -2f * Gravity);

            _animator.SetBool(Jump, true);
        }
    }
}

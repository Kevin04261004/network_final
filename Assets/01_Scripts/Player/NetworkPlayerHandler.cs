using System;
using System.Net;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

[RequireComponent(typeof(PlayerInputHandler),typeof(PlayerMovement))]
public class NetworkPlayerHandler : MonoBehaviour
{
    /* Host */
    private PlayerInputHandler _playerInputHandler;
    private PlayerMovement _playerMovement;
    private PlayerInput _playerInput;
    /* Others */
    private NetworkTransformSync _transformSync;
    private NetworkAnimationSync _animationSync;

    [SerializeField] private GameObject _aimCamera;
    [SerializeField] private Transform _panty;
    public NetworkPlayer NetworkPlayerData
    {
        set
        {
            IsMine = value.IsMine;
            _transformSync.NetworkPlayerData = value;
            _animationSync.NetworkPlayerData = value;
            // _panty.GetComponent<SkinnedMeshRenderer>().material.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        }
    }
    public bool IsMine
    {
        set
        {
            if (value == true)
            {
                _playerInputHandler.enabled = true;
                _playerMovement.enabled = true;
                _playerInput.enabled = true;
            }
            else
            {
                _playerInputHandler.enabled = false;
                _playerMovement.enabled = false;
                _playerInput.enabled = false;
            }
            _transformSync.enabled = true;
            _animationSync.enabled = true;
            _aimCamera.SetActive(value);
        }
    }
    private void Awake()
    {
        TryGetComponent(out _playerInputHandler);
        TryGetComponent(out _playerInput);
        TryGetComponent(out _playerMovement);
        TryGetComponent(out _transformSync);
        TryGetComponent(out _animationSync);
    }
    public void SetPosAndRot(Vector3 pos, Quaternion rot)
    {
        // TODO: NetworkTransformSync를 사용하여 자연스러운 보간 구현하기.
        MainThreadWorker.Instance.EnqueueJob(() =>
        {
            _transformSync.targetPosition = pos;
            _transformSync.targetRotation = rot;
            _transformSync.interpolationStartTime = Time.time;
        });
    }

    public void SetAnimation(bool moveKey, bool shiftKey)
    {
        MainThreadWorker.Instance.EnqueueJob(() =>
        {
            _animationSync.MoveKeyDown = moveKey;
            _animationSync.ShiftKeyDown = shiftKey;
        });
    }
}

using System;
using System.Net;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInputHandler),typeof(PlayerMovement))]
public class NetworkPlayerHandler : MonoBehaviour
{
    /* Host */
    private PlayerInputHandler _playerInputHandler;
    private PlayerMovement _playerMovement;
    private PlayerInput _playerInput;
    /* Others */
    private NetworkTransformSync _transformSync;
    [SerializeField] private GameObject _aimCamera;
    public NetworkPlayer NetworkPlayerData
    {
        set
        {
            IsMine = value.IsMine;
            _transformSync.NetworkPlayerData = value;
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
            _aimCamera.SetActive(value);
        }
    }
    private void Awake()
    {
        TryGetComponent(out _playerInputHandler);
        TryGetComponent(out _playerInput);
        TryGetComponent(out _playerMovement);
        TryGetComponent(out _transformSync);
    }
    public void SetPosAndRot(Vector3 pos, Quaternion rot)
    {
        // TODO: NetworkTransformmSync를 사용하여 자연스러운 보간 구현하기.
        MainThreadWorker.Instance.EnqueueJob(() =>
        {
            transform.position = pos;
            transform.rotation = rot; 
        });
    }
    
}

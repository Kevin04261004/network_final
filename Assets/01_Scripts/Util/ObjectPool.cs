using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [SerializeField] private List<ObjectPoolSO> _objectPoolList;

    private Transform _cachedTransform;

    private void Awake()
    {
        _cachedTransform = transform;
    }

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        foreach (var pool in _objectPoolList)
        {
            pool.InitPool(_cachedTransform);
        }
    }
}

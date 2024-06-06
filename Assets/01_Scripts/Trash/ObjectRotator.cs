using System;
using UnityEngine;

public class ObjectRotator : MonoBehaviour
{
    [SerializeField] private Vector3 dir;
    [SerializeField] private float speed = 4; 
    private void Update()
    {
        transform.Rotate(dir * speed * Time.deltaTime);
    }
}

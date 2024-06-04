using UnityEngine;

public class MoveToVector : MonoBehaviour
{
    [SerializeField] private Vector3 moveDir = Vector3.right;
    [field: SerializeField] public float _moveSpeed { get; set; } = 4f;
    private void Update()
    {
        transform.position += moveDir * Time.deltaTime * _moveSpeed;
    }
}

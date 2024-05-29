using UnityEngine;

public class TrashObject : NetworkObject
{
    [field: SerializeField] public TrashData Data { get; private set; }

}

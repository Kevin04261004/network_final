using GameLogicServer;
using UnityEngine;
using Util;

public class TrashObject : NetworkObject
{
    [field: SerializeField] public TrashData TrashData { get; private set; }
}

using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/PlayerStat", order = 1)]
public class PlayerStat : BaseScriptableObject
{
    [field: SerializeField] public float WalkSpeed { get; private set; } = 3;
    [field: SerializeField] public float RunSpeed { get; private set; } = 6;
    [field: SerializeField] public float Gravity { get; private set; } = -9.81f;
    [field: SerializeField] public float TerminalVelocity { get; private set; } = 63f;
    
}

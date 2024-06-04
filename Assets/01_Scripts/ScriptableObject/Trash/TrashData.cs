using UnityEngine;
public enum ETrashType
{
    None,
    Plastic,
    Can,
    Glass,
}

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/TrashData", order = 1)]
public class TrashData : ScriptableObject
{
    [field: SerializeField] public ETrashType trashType { get; private set; }
    [field: SerializeField] public int point { get; private set; }
}

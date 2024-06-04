using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/MapData/MapEdgeData", order = 1)]
public class MapEdgeData : BaseScriptableObject
{
    [field: SerializeField] public Vector2 rightTop;
    [field: SerializeField] public Vector2 leftBottom;
}

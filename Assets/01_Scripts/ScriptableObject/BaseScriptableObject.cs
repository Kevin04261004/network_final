using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/BaseScriptableObj", order = 1)]
public abstract class BaseScriptableObject : ScriptableObject
{
     [TextArea] public string information;
}

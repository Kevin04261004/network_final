using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Object Pool", order = 1)]
public class ObjectPoolSO : BaseScriptableObject
{
    [field: SerializeField] public GameObject _objectPrefab { get; private set; }
    [SerializeField] private int _defaultCount;
    [SerializeField] private int _addCount;

    private Queue<GameObject> _objectPoolQueue = new Queue<GameObject>();
    private Transform _parent;
    public void InitPool(Transform parent)
    {
        _parent = parent;
        
        ExtendPool(_defaultCount);
    }

    private void ExtendPool(int count)
    {
        for (int i = 0; i < count; ++i)
        {
            Return(Instantiate(_objectPrefab, _parent));
        }
    }

    public GameObject Get(Vector3 pos = default, Quaternion rotation = default, Transform parent = null)
    {
        if (_objectPoolQueue.Count <= 0)
        {
            ExtendPool(_addCount);
        }

        GameObject go = _objectPoolQueue.Dequeue();

        go.transform.parent = parent;
        if (parent == null)
        {
            go.transform.position = pos;
            go.transform.rotation = rotation;
        }
        else
        {
            go.transform.localPosition = pos;
            go.transform.localRotation = rotation;
        }
        
        go.SetActive(true);

        return go;
    }

    public void Return(GameObject go)
    {
        if (_objectPoolQueue.Contains(go))
        {
            return;
        }
        
        go.SetActive(false);
        go.transform.SetParent(_parent);
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;
        
        _objectPoolQueue.Enqueue(go);
    }
}

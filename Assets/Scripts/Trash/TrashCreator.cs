using Unity.VisualScripting;
using UnityEngine;

public class TrashCreator : MonoBehaviour
{
    [SerializeField] private TrashObject[] _trashObjects;
    [SerializeField] private MapEdgeData _mapEdgeData;
    
    public void SpawnTrash()
    {
        TrashObject spawnObj = _trashObjects[Random.Range(0, _trashObjects.Length)];
        Vector3 spawnPos = new Vector3(_mapEdgeData.leftBottom.y, 0, Random.Range(_mapEdgeData.leftBottom.x, _mapEdgeData.rightTop.x));
        TrashObject temp = GameObject.Instantiate(spawnObj, spawnPos, Quaternion.identity, transform);
        temp.AddComponent<MoveToVector>()._moveSpeed = Random.Range(4f, 6f);
    }

    private float curTime;
    public void Update()
    {
        curTime += Time.deltaTime;
        if (curTime > 0.1f)
        {
            SpawnTrash();
            curTime = 0f;
        }
    }
}

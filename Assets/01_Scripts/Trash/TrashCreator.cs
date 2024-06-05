using Unity.VisualScripting;
using UnityEngine;

public class TrashCreator : MonoBehaviour
{
    [SerializeField] private ObjectPool[] _trashObjectPools;
    [SerializeField] private MapEdgeData _mapEdgeData;
    
    public void SpawnTrash()
    {
        Vector3 spawnPos = new Vector3(_mapEdgeData.leftBottom.y, 0, Random.Range(_mapEdgeData.leftBottom.x, _mapEdgeData.rightTop.x));
        //GameObject spawnObj = _trashObjectPools[Random.Range(0, _trashObjectPools.Length)].Get(spawnPos, Quaternion.identity);
        //spawnObj.GetComponent<MoveToVector>()._moveSpeed = Random.Range(3f, 4f);
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

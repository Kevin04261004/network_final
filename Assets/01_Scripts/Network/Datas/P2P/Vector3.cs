using UnityEngine;

public struct MyVector3
{
    public float x;
    public float y;
    public float z;

    public MyVector3(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }
    public MyVector3(UnityEngine.Vector3 vec)
    {
        this.x = vec.x;
        this.y = vec.y;
        this.z = vec.z;
    }

    public MyVector3(Quaternion quaternion)
    {
        this.x = quaternion.eulerAngles.x;
        this.y = quaternion.eulerAngles.y;
        this.z = quaternion.eulerAngles.z;
    }
}
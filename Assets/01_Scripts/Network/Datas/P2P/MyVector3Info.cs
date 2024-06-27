using System;
using UnityEngine;

public static class MyVector3Info
{
    public static readonly int X_SIZE = sizeof(float);
    public static readonly int Y_SIZE = sizeof(float);
    public static readonly int Z_SIZE = sizeof(float);
    
    public static int GetByteSize()
    {
        return X_SIZE + Y_SIZE + Z_SIZE;
    }

    public static byte[] Serialize(Vector3 vec)
    {
        byte[] data = new byte[GetByteSize()];
        byte[] xBytes = BitConverter.GetBytes(vec.x);
        byte[] yBytes = BitConverter.GetBytes(vec.y);
        byte[] zBytes = BitConverter.GetBytes(vec.z);

        int offset = 0;
        Array.Copy(xBytes, 0, data, offset, xBytes.Length);
        offset += xBytes.Length;
        Array.Copy(yBytes, 0, data, offset, yBytes.Length);
        offset += yBytes.Length;
        Array.Copy(zBytes, 0, data, offset, zBytes.Length);
        offset += zBytes.Length;

        return data;
    }
    public static byte[] Serialize(Quaternion quaternion)
    {
        byte[] data = new byte[GetByteSize()];
        byte[] xBytes = BitConverter.GetBytes(quaternion.eulerAngles.x);
        byte[] yBytes = BitConverter.GetBytes(quaternion.eulerAngles.y);
        byte[] zBytes = BitConverter.GetBytes(quaternion.eulerAngles.z);

        int offset = 0;
        Array.Copy(xBytes, 0, data, offset, xBytes.Length);
        offset += xBytes.Length;
        Array.Copy(yBytes, 0, data, offset, yBytes.Length);
        offset += yBytes.Length;
        Array.Copy(zBytes, 0, data, offset, zBytes.Length);
        offset += zBytes.Length;

        return data;
    }
    public static Vector3 DeSerialize(byte[] data)
    {
        byte[] xBytes = new byte[X_SIZE];
        byte[] yBytes = new byte[Y_SIZE];
        byte[] zBytes = new byte[Z_SIZE];

        int offset = 0;
        Array.Copy(data, offset, xBytes, 0, xBytes.Length);
        offset += xBytes.Length;
        Array.Copy(data, offset, yBytes, 0, yBytes.Length);
        offset += yBytes.Length;
        Array.Copy(data, offset, zBytes, 0, zBytes.Length);
        offset += zBytes.Length;

        float x = BitConverter.ToSingle(xBytes);
        float y = BitConverter.ToSingle(yBytes);
        float z = BitConverter.ToSingle(zBytes);

        return new Vector3(x, y, z);
    }

    public static UnityEngine.Vector3 ToVector3(byte[] data)
    {
        return ToVector3(DeSerialize(data));
    }
    public static UnityEngine.Vector3 ToVector3(Vector3 vec3)
    {
        return new UnityEngine.Vector3(vec3.x, vec3.y, vec3.z);
    }
    public static Quaternion ToQuaternion(byte[] data)
    {
        return ToQuaternion(DeSerialize(data));
    }
    public static Quaternion ToQuaternion(Vector3 vec3)
    {
        return Quaternion.Euler(vec3.x, vec3.y, vec3.z);
    }
}
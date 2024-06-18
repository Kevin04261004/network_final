using System.Text;

namespace DYUtil
{
    public static class MyEncoder
    {
        public static void Encode(string str, byte[] data, int offset)
        {
            Encoding.UTF8.GetBytes(str, 0, str.Length, data, offset);
        }

        public static void Encode(string str, byte[] data, int offset, int maxSize)
        {
            Encoding.UTF8.GetBytes(str, 0, Math.Min(maxSize, str.Length), data, offset);
        }
        public static void Encode(int i, byte[] data, int offset)
        {
            byte[] iByte = BitConverter.GetBytes(i);
            Array.Copy(iByte, 0, data, offset, iByte.Length);
        }
        public static void Encode(long l, byte[] data, int offset)
        {
            byte[] lByte = BitConverter.GetBytes(l);
            Array.Copy(lByte, 0, data, offset, lByte.Length);
        }
    }
}

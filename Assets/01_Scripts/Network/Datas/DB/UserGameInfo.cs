using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public struct UserGameInfo
{
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
    public string _nickName;
    [MarshalAs(UnmanagedType.I4)]
    public int _sumPoint;
    [MarshalAs(UnmanagedType.I4)]
    public int _maxPoint;
}
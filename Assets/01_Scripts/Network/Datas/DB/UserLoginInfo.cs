
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public struct UserLoginInfo
{
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
    public string _nickName;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
    public string _id;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
    public string _password;
}
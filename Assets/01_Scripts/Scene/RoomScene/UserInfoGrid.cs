using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserInfoGrid : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nickName;
    [SerializeField] private Image image;

    public enum EType
    {
        Host,
        Ready,
        NotReady,
    }
    
    public void SetNickName(string nickName)
    {
        this.nickName.text = nickName;
    }

    public void SetImage(EType type)
    {
        switch (type)
        {
            case EType.Host:

                break;
            default:
                break;
        }
    }
}

using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserInfoGrid : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nickName;
    [SerializeField] private Image image;

    public void SetNickName(string nickName)
    {
        this.nickName.text = nickName;
    }

    public void SetImage(bool isHost, bool isReady)
    {
        if (isHost)
        {
            // TODO: Setting Image Here
            return;
        }

        if (isReady)
        {
            // TODO: Setting Image Here 2222
        }
        else
        {
            
        }
    }
}

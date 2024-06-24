using System.Collections.Generic;
using GameLogicServer.Datas.Database;
using TMPro;
using UnityEngine;

public class RoomScene : MonoBehaviour
{
    [SerializeField] private GameObject UserInfoGrid_Prefab;
    [SerializeField] private Transform Panel;
    [SerializeField] private TextMeshProUGUI playerCount;
    public void SetPanel(Dictionary<DB_RoomUserInfo, DB_UserLoginInfo> dictionary)
    {
        RemoveAllGrid();
        foreach (var user in dictionary)
        {
            AddGrid(user.Value);
        }

        playerCount.text = $"{dictionary.Count} / 4";
    }

    private void AddGrid(DB_UserLoginInfo roomUserInfo)
    {
        MainThreadWorker.Instance.EnqueueJob(() =>
        {
            UserInfoGrid userInfoGrid = Instantiate(UserInfoGrid_Prefab, Vector3.zero, Quaternion.identity, Panel).GetComponent<UserInfoGrid>();
            userInfoGrid.SetNickName(roomUserInfo.NickName);
            userInfoGrid.SetImage(UserInfoGrid.EType.Host); 
        });
    }

    private void RemoveAllGrid()
    {
        MainThreadWorker.Instance.EnqueueJob(() =>
        {
            int temp = 100;
            while (Panel.childCount != 0)
            {
                temp--;
                if (temp < 0)
                {
                    break;
                }

                Destroy(Panel.GetChild(0).gameObject);

            }
        });
    }
}

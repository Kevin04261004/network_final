using System.Collections.Generic;
using GameLogicServer.Datas.Database;
using TMPro;
using UnityEngine;

public class RoomScene : MonoBehaviour
{
    [SerializeField] private GameObject UserInfoGrid_Prefab;
    [SerializeField] private Transform Panel;
    [SerializeField] private TextMeshProUGUI playerCount;
    public void SetPanel(List<NetworkPlayer> list)
    {
        RemoveAllGrid();
        foreach (var user in list)
        {
            AddGrid(user);
        }
        MainThreadWorker.Instance.EnqueueJob(() =>
        {
            playerCount.text = $"{list.Count} / 4"; // ? 바꿔야할 듯 (귀찮으니까 일단은 이렇게~ ㅋㅋ)
        });
    }

    private void AddGrid(NetworkPlayer networkPlayer)
    {
        MainThreadWorker.Instance.EnqueueJob(() =>
        {
            UserInfoGrid userInfoGrid = Instantiate(UserInfoGrid_Prefab, Vector3.zero, Quaternion.identity, Panel).GetComponent<UserInfoGrid>();
            userInfoGrid.SetNickName(networkPlayer.NickName);
            userInfoGrid.SetImage(networkPlayer.IsHost, networkPlayer.IsReady);
        });
    }

    private void RemoveAllGrid()
    {
        MainThreadWorker.Instance.EnqueueJob(() =>
        {
            foreach (Transform child in Panel)
            {
                Destroy(child.gameObject);
            }
        });
    }
}

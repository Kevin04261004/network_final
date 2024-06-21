using UnityEngine;

public class CurrentGameRoomData : MonoBehaviour
{
    public static CurrentGameRoomData Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public DB_GameRoom GameRoom { get; set; }
}

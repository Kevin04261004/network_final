using UnityEngine;

public class UserGameData : MonoBehaviour
{
    public static UserGameData Instance { get; private set; }

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

        GameData = new DB_UserGameData("",0,0);
    }
    
    public string NickName { get; set; }
    public DB_UserGameData GameData { get; set; }
}

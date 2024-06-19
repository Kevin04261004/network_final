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
    }
    
    public string NickName { get; set; }
    public string Id { get; set; }
    public long SumPoint { get; set; }
    public int MaxPoint { get; set; }
}

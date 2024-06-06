using UnityEngine;
using UnityEngine.Playables;
public class TitleSceneTimeLine : MonoBehaviour
{
    [SerializeField] private PlayableDirector lobbyToLogin; 
    [SerializeField] private PlayableDirector loginToLobby; 

    [ContextMenu("Lobby To Login")]
    public void LobbyToLogin()
    {
        lobbyToLogin.Play();
    }
    
    [ContextMenu("Login To Lobby")]
    public void LoginToLobby()
    {
        loginToLobby.Play();
    }
}

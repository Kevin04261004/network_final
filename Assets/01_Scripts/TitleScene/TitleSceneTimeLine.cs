using UnityEngine;
using UnityEngine.Playables;
public class TitleSceneTimeLine : MonoBehaviour
{
    [SerializeField] private PlayableDirector lobbyToLogin; 
    [SerializeField] private PlayableDirector loginToLobby; 
    [SerializeField] private PlayableDirector loginToNickName; 
    [SerializeField] private PlayableDirector nickNameToLogin; 

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
    [ContextMenu("Login To NickName")]
    public void LoginToNickName()
    {
        loginToNickName.Play();
    }
    [ContextMenu("NickName To Login")]
    public void NickNameToLogin()
    {
        nickNameToLogin.Play();
    }
}

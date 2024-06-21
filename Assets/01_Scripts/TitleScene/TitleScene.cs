using System;
using System.Collections;
using System.Net;
using System.Text;
using GameLogicServer.Datas.Database;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class TitleScene : MonoBehaviour
{
    private static readonly string ERROR_TOO_LONG_OR_SHORT_INPUT = "2-16자의 영문 소문자";
    private static readonly string ERROR_TOO_LONG_INPUT = "16자 이상의 영문 소문자";
    private static readonly string ERROR_INVALIDATE_INPUT = "숫자와 특수기호(_)만 사용이 가능합니다.";
    private static readonly string ERROR_CREATE_ACCOUNT_FAIL = "계정 생성에 실패하였습니다.";
    private static readonly string ERROR_ACCOUNT_CANT_EXIST = "아이디나 비밀번호가 존재하지 않습니다.";
    private static readonly string ERROR_ID_ALREADY_HAS = "아이디가 이미 존재합니다.";
    private static readonly string LOG_CREATE_ACCOUNT_SUCCESS = "계정 생성에 성공하였습니다.";
    private static readonly string ERROR_CREATE_ROOM_FAIL = "룸 이름이 이미 존재하거나, 서버(또는 DB)에 문제가 생겼습니다.";
    private static readonly string LOG_CREATE_ROOM_SUCCESS = "방 생성에 성공하였습니다.";
    private static readonly Color redFadeInColor = new Color(1, 0, 0, 0);

    [Header("Other")]
    [SerializeField] private BufferingImage _bufferingImage;
    
    [Header("LoginPanel")]
    [SerializeField] private TextMeshProUGUI _loginLogTMP;
    [SerializeField] private TMP_InputField idInputField;
    [SerializeField] private TMP_InputField pwInputField;

    [Header("LobbyPanel")]
    [SerializeField] private TextMeshProUGUI _lobbyNickNameTMP;
    [SerializeField] private TextMeshProUGUI _lobbyMyRanking;
    [SerializeField] private TextMeshProUGUI _lobbyMaxPoint;
    [SerializeField] private TextMeshProUGUI _lobbySumPoint;

    [Header("NickNamePanel")]
    [SerializeField] private TextMeshProUGUI _nickNameLogTMP;
    [SerializeField] private TMP_InputField nickNameInputField;

    [Header("RoomNamePanel")]
    [SerializeField] private TMP_InputField roomNameInputField;
    [SerializeField] private TextMeshProUGUI _roomNameLogTMP;
    
    private Coroutine fadeInCoroutine;
    private TitleSceneTimeLine _titleSceneTimeLine;

    private void Awake()
    {
        _titleSceneTimeLine = FindAnyObjectByType<TitleSceneTimeLine>();
        
        DatabasePacketHandler.Instance.SetHandler(PacketDataInfo.EDataBasePacketType.Server_LoginSuccess, LoginSuccess);
        DatabasePacketHandler.Instance.SetHandler(PacketDataInfo.EDataBasePacketType.Server_LoginFail, LoginFail);
        DatabasePacketHandler.Instance.SetHandler(PacketDataInfo.EDataBasePacketType.Server_SendUserGameData, SetLobbyPanel);
        DatabasePacketHandler.Instance.SetHandler(PacketDataInfo.EDataBasePacketType.Server_CanCreateAccount, CanCreateAccount);
        DatabasePacketHandler.Instance.SetHandler(PacketDataInfo.EDataBasePacketType.Server_CantCreateAccount, CantCreateAccount);
        DatabasePacketHandler.Instance.SetHandler(PacketDataInfo.EDataBasePacketType.Server_CreateAccountFail, CreateAccountFail);
        DatabasePacketHandler.Instance.SetHandler(PacketDataInfo.EDataBasePacketType.Server_CreateAccountSuccess, CreateAccountSuccess);
        DatabasePacketHandler.Instance.SetHandler(PacketDataInfo.EDataBasePacketType.Server_CreateRoomFail, CreateRoomFail);
        DatabasePacketHandler.Instance.SetHandler(PacketDataInfo.EDataBasePacketType.Server_CreateRoomSuccess, CreateRoomSuccess);
        DatabasePacketHandler.Instance.SetHandler(PacketDataInfo.EDataBasePacketType.Server_ClientEnterRoomSuccess, EnterRoomSuccess);
        DatabasePacketHandler.Instance.SetHandler(PacketDataInfo.EDataBasePacketType.Server_ClientEnterRoomFail, EnterRoomFail);
    }

    /* for Debug */
    [ContextMenu("BufferingImage OFF")]
    public void BufferingImageOff()
    {
        _bufferingImage.MinusCount();
    }
    /*  */
    
    public void TryLogin()
    {
        string id = idInputField.text;
        string pw = pwInputField.text;
        
        if (id.Length <= 2 || id.Length > 16 || pw.Length <= 2 || pw.Length > 16)
        {
            SetErrorCode(_loginLogTMP,ERROR_TOO_LONG_OR_SHORT_INPUT);
            return;
        }
        if (!IsValidInput(id) || !IsValidInput(pw))
        {
            SetErrorCode(_loginLogTMP,ERROR_INVALIDATE_INPUT);
            return;
        }
        
        DB_UserLoginInfo userLoginInfo = new DB_UserLoginInfo(id, pw, "TryLogin");
        byte[] data = DB_UserLoginInfoInfo.Serialize(userLoginInfo);
        var packetData = new PacketData<PacketDataInfo.EDataBasePacketType>(PacketDataInfo.EDataBasePacketType.Client_TryLogin, data);
        NetworkManager.Instance.SendToServer(ESendServerType.Database, packetData.ToPacket());
        MainThreadWorker.Instance.EnqueueJob(() =>
        {
            _bufferingImage.AddCount();
        });
    }
    public void TryCreateAccount()
    {
        string id = idInputField.text;
        string pw = pwInputField.text;
        
        if (id.Length <= 2 || id.Length > 16 || pw.Length <= 2 || pw.Length > 16)
        {
            SetErrorCode(_loginLogTMP,ERROR_TOO_LONG_OR_SHORT_INPUT);
            return;
        }
        if (!IsValidInput(id) || !IsValidInput(pw))
        {
            SetErrorCode(_loginLogTMP,ERROR_INVALIDATE_INPUT);
            return;
        }
        DB_UserLoginInfo userLoginInfo = new DB_UserLoginInfo(id, pw, "TryLogin");
        byte[] data = DB_UserLoginInfoInfo.Serialize(userLoginInfo);
        var packetData = new PacketData<PacketDataInfo.EDataBasePacketType>(PacketDataInfo.EDataBasePacketType.Client_RequireCheckHasID, data);
        NetworkManager.Instance.SendToServer(ESendServerType.Database, packetData.ToPacket());
        MainThreadWorker.Instance.EnqueueJob(() =>
        {
            _bufferingImage.AddCount();
        });
    }
    public void EnterRandomRoom()
    {
        var packetData =
            new PacketData<PacketDataInfo.EDataBasePacketType>(
                PacketDataInfo.EDataBasePacketType.Client_EnterRandomRoom);
        NetworkManager.Instance.SendToServer(ESendServerType.Database, packetData.ToPacket());
        MainThreadWorker.Instance.EnqueueJob(() =>
        {
            _bufferingImage.AddCount();
        });
    }
    public void CreateRoom()
    {
        string roomName = roomNameInputField.text;
        
        if (roomName.Length > 16)
        {
            SetErrorCode(_roomNameLogTMP, ERROR_TOO_LONG_INPUT);
            return;
        }
        if (!IsValidInput(roomName))
        {
            SetErrorCode(_roomNameLogTMP, ERROR_INVALIDATE_INPUT);
            return;
        }

        byte[] roomNameBytes = new byte[DB_GameRoomInfo.ROOM_NAME_SIZE];
        MyEncoder.Encode(roomName, roomNameBytes, 0, roomNameBytes.Length);
        var packetData =
            new PacketData<PacketDataInfo.EDataBasePacketType>(PacketDataInfo.EDataBasePacketType.Client_CreateRoom, roomNameBytes);
        NetworkManager.Instance.SendToServer(ESendServerType.Database, packetData.ToPacket());
        MainThreadWorker.Instance.EnqueueJob(() =>
        {
            _bufferingImage.AddCount();
        });
    }
    public void CreateAccount()
    {
        string id = idInputField.text;
        string pw = pwInputField.text;
        string nickName = nickNameInputField.text;
        
        if (id.Length <= 2 || id.Length > 16 || pw.Length <= 2 || pw.Length > 16 || nickName.Length <= 2 || nickName.Length > 16)
        {
            SetErrorCode(_nickNameLogTMP, ERROR_TOO_LONG_OR_SHORT_INPUT);
            return;
        }
        if (!IsValidInput(id) || !IsValidInput(pw) || !IsValidInput(nickName))
        {
            SetErrorCode(_nickNameLogTMP, ERROR_INVALIDATE_INPUT);
            return;
        }
        DB_UserLoginInfo userLoginInfo = new DB_UserLoginInfo(id, pw, nickName);
        byte[] data = DB_UserLoginInfoInfo.Serialize(userLoginInfo);
        var packetData = new PacketData<PacketDataInfo.EDataBasePacketType>(PacketDataInfo.EDataBasePacketType.Client_CreateAccount, data);
        NetworkManager.Instance.SendToServer(ESendServerType.Database, packetData.ToPacket());
        MainThreadWorker.Instance.EnqueueJob(() =>
        {
            _bufferingImage.AddCount();
            _bufferingImage.AddCount();
        });
    }
    private void CreateRoomFail(IPEndPoint endPoint, byte[] data)
    {
        MainThreadWorker.Instance.EnqueueJob(() =>
        {
            _bufferingImage.MinusCount();
            SetErrorCode(_roomNameLogTMP, ERROR_CREATE_ROOM_FAIL);
        });
    }
    private void CreateRoomSuccess(IPEndPoint endPoint, byte[] data)
    {
        string roomName = Encoding.UTF8.GetString(data);
        MainThreadWorker.Instance.EnqueueJob(() =>
        {
            _bufferingImage.MinusCount();
            SetErrorCode(_roomNameLogTMP, $"{LOG_CREATE_ROOM_SUCCESS} => {roomName}", 3f, Color.green);
        });
    }

    private void EnterRoomSuccess(IPEndPoint endPoint, byte[] data)
    {
        DB_GameRoom curGameRoom = DB_GameRoomInfo.DeSerialize(data);
        MainThreadWorker.Instance.EnqueueJob(() =>
        {
            _bufferingImage.MinusCount();
            CurrentGameRoomData.Instance.GameRoom = curGameRoom;
            SceneHandler.Instance.LoadScene(SceneHandler.RoomScene);
        });
        
    }

    private void EnterRoomFail(IPEndPoint endPoint, byte[] data)
    {
        DB_GameRoom curGameRoom = DB_GameRoomInfo.DeSerialize(data);
        MainThreadWorker.Instance.EnqueueJob(() =>
        {
            _bufferingImage.MinusCount();
            SetErrorCode(_roomNameLogTMP, $"{LOG_CREATE_ROOM_SUCCESS} => {curGameRoom.RoomName}", 3f, Color.green);
        });
    }
    private void LoginSuccess(IPEndPoint endPoint, byte[] data)
    {
        MainThreadWorker.Instance.EnqueueJob(() =>
        {
            _bufferingImage.MinusCount();
            _titleSceneTimeLine.LoginToLobby();
        });
        string nickName = Encoding.UTF8.GetString(data);
        UserGameData.Instance.NickName = nickName;
        MainThreadWorker.Instance.EnqueueJob(() =>
        {
            _lobbyNickNameTMP.text = nickName;
        });
    }
    private void LoginFail(IPEndPoint endPoint, byte[] data)
    {
        MainThreadWorker.Instance.EnqueueJob(() =>
        {
            _bufferingImage.MinusCount();
            SetErrorCode(_loginLogTMP, ERROR_ACCOUNT_CANT_EXIST);
        });
    }
    private void SetLobbyPanel(IPEndPoint endPoint, byte[] data)
    {
        DB_UserGameData userGameData = DB_UserGameDataInfo.Deserialize(data);
        MainThreadWorker.Instance.EnqueueJob(() =>
        {
            _lobbyMyRanking.text = "현재 랭킹: 업데이트 필요";
            _lobbySumPoint.text = $"합산 점수: {userGameData.SumPoint}";
            _lobbyMaxPoint.text = $"최대 점수: {userGameData.MaxPoint}";
            UserGameData.Instance.GameData.Id = userGameData.Id;
            UserGameData.Instance.GameData.SumPoint = userGameData.SumPoint;
            UserGameData.Instance.GameData.MaxPoint = userGameData.MaxPoint;
        });
    }
    private void CanCreateAccount(IPEndPoint endPoint, byte[] data)
    {
        MainThreadWorker.Instance.EnqueueJob(() =>
        {
            _bufferingImage.MinusCount();
            // 이름 
            _titleSceneTimeLine.LoginToNickName();
        });
    }
    private void CantCreateAccount(IPEndPoint endPoint, byte[] data)
    {
        MainThreadWorker.Instance.EnqueueJob(() =>
        {
            _bufferingImage.MinusCount();
            SetErrorCode(_loginLogTMP, ERROR_ID_ALREADY_HAS);
        });
    }
    private void CreateAccountFail(IPEndPoint endPoint, byte[] data)
    {
        MainThreadWorker.Instance.EnqueueJob(() =>
        {
            _bufferingImage.MinusCount();
            SetErrorCode(_nickNameLogTMP, ERROR_CREATE_ACCOUNT_FAIL);
        });
    }
    private void CreateAccountSuccess(IPEndPoint endPoint, byte[] data)
    {
        MainThreadWorker.Instance.EnqueueJob(() =>
        {
            _bufferingImage.MinusCount();
            _titleSceneTimeLine.NickNameToLogin();
            SetErrorCode(_loginLogTMP,LOG_CREATE_ACCOUNT_SUCCESS, 3f, Color.green);
        });
    }
    private bool IsValidInput(string input)
    {
        foreach (var c in input)
        {
            if (!char.IsLetterOrDigit(c) && c != '_')
            {
                return false;
            }
        }

        return true;
    }
    private void SetErrorCode(TextMeshProUGUI logTMP, string str, float time = 3, Color color = default(Color))
    {
        logTMP.gameObject.SetActive(true);
        logTMP.text = str;
        logTMP.color = (color == default(Color)) ? Color.red : color;
        if (fadeInCoroutine != null)
        {
            StopCoroutine(fadeInCoroutine);
            fadeInCoroutine = null;
        }
        fadeInCoroutine = StartCoroutine(ErrorCodeFadeIn(logTMP, time, color));
    }
    private IEnumerator ErrorCodeFadeIn(TextMeshProUGUI logTMP, float time, Color color = default(Color))
    {
        yield return new WaitForSeconds(time - 1);
        
        float elapsedTime = 0f;
        while (elapsedTime < 1)
        {
            float t = elapsedTime / 1;

            if (color == default(Color))
            {
                logTMP.color = Color.Lerp(Color.red, redFadeInColor, t);
            }
            else
            {
                Color fadeInColor = new Color(color.r, color.g, color.b, 0);
                logTMP.color = Color.Lerp(color, fadeInColor, t);   
            }
            
            elapsedTime += Time.deltaTime;
            
            yield return null;
        }
        
        logTMP.color = Color.clear;
    }
}

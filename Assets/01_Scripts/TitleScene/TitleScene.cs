using System.Collections;
using GameLogicServer.Datas.Database;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class TitleScene : MonoBehaviour
{
    private static readonly string ERROR_TOO_LONG_OR_SHORT_INPUT = "2-16자의 영문 소문자";
    private static readonly string ERROR_INVALIDATE_INPUT = "숫자와 특수기호(_)만 사용이 가능합니다.";
    private static readonly string ERROR_ACCOUNT_CANT_EXIST = "아이디나 비밀번호가 존재하지 않습니다.";
    private static readonly Color redFadeInColor = new Color(1, 0, 0, 0);


    [SerializeField] private BufferingImage _bufferingImage;
    [SerializeField] private TextMeshProUGUI _loginLogTMP;
    [SerializeField] private TMP_InputField idInputField;
    [SerializeField] private TMP_InputField pwInputField;
    
    private Coroutine fadeInCoroutine;
    public void TryLogin()
    {
        string id = idInputField.text;
        string pw = pwInputField.text;
        
        if (id.Length <= 2 || id.Length > 16 || pw.Length <= 2 || pw.Length > 16)
        {
            SetErrorCode(ERROR_TOO_LONG_OR_SHORT_INPUT);
            return;
        }
        if (!IsValidInput(id) || !IsValidInput(pw))
        {
            SetErrorCode(ERROR_INVALIDATE_INPUT);
            return;
        }
        
        DB_UserLoginInfo userLoginInfo = new DB_UserLoginInfo("TryLogin", id, pw);
        byte[] data = DB_UserLoginInfoInfo.Serialize(userLoginInfo);
        var packetData = new PacketData<PacketDataInfo.EDataBasePacketType>(PacketDataInfo.EDataBasePacketType.Client_TryLogin, data);
        NetworkManager.Instance.SendToServer(ESendServerType.Database, packetData.ToPacket());
        _bufferingImage.AddCount();
    }
    
    public void LoginSuccess()
    {
        _bufferingImage.MinusCount();
    }

    public void LoginFail()
    {
        _bufferingImage.MinusCount();
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
    private void SetErrorCode(string str, float time = 3, Color color = default(Color))
    {
        _loginLogTMP.text = str;
        _loginLogTMP.color = (color == default(Color)) ? Color.red : color;
        if (fadeInCoroutine != null)
        {
            StopCoroutine(fadeInCoroutine);
            fadeInCoroutine = null;
        }
        fadeInCoroutine = StartCoroutine(ErrorCodeFadeIn(time, color));
    }
    private IEnumerator ErrorCodeFadeIn(float time, Color color = default(Color))
    {
        yield return new WaitForSeconds(time - 1);
        
        float elapsedTime = 0f;
        while (elapsedTime < 1)
        {
            float t = elapsedTime / 1;

            if (color == default(Color))
            {
                _loginLogTMP.color = Color.Lerp(Color.red, redFadeInColor, t);
            }
            else
            {
                Color fadeInColor = new Color(color.r, color.g, color.b, 0);
                _loginLogTMP.color = Color.Lerp(color, fadeInColor, t);   
            }
            
            elapsedTime += Time.deltaTime;
            
            yield return null;
        }
        
        _loginLogTMP.color = Color.clear;
    }
}

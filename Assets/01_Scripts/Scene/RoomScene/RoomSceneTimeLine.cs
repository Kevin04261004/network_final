using System;
using UnityEngine;
using UnityEngine.Playables;

public class RoomSceneTimeLine : MonoBehaviour
{
    [SerializeField] private PlayableDirector userInfoTriggerTrue; 
    [SerializeField] private PlayableDirector userInfoTriggerFalse;

    [SerializeField] private RectTransform userInfoRectTransform;
    public bool IsUserInfoTriggered => (userInfoRectTransform.position.x == 0);

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            TriggerUserInfoPanel();
        }
    }

    public void TriggerUserInfoPanel()
    {
        if (IsUserInfoTriggered)
        {
            userInfoTriggerTrue.Play();
        }
        else
        {
            userInfoTriggerFalse.Play();
        }
    }
}

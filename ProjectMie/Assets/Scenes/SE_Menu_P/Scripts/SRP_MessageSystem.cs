using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SRP_MessageSystem : MonoBehaviour
{
    public static SRP_MessageSystem MS;
    void Start()
    {
        var List = FindObjectsOfType<SRP_MessageSystem>();
        if(List.Length > 1) Destroy(gameObject);
        MS = this;


    }
    
    public void SendMessage(bool isOK, string Message, float LifeTime = 2.0f)
    {
        GameObject.Find("MessageList/").GetComponent<SRP_MessageListManager>().CreateMessage(isOK, Message, LifeTime);
    }

    public void ClearAllMessage()
    {
        GameObject.Find("MessageList/").GetComponent<SRP_MessageListManager>().DoClear();
    }
}

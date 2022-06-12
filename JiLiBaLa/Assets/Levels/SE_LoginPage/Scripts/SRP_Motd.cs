using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class SRP_Motd : MonoBehaviour
{
    public Text MotdText;   //用于告知用户是否连接php服务器
    public Text VersionText;    //显示版本号
    public Image VersionBlocker;

    [Tooltip("必须拥有一致版本号才能进行游戏。")]
    public string LocalVersion = "1.0.0";

    void Start()
    {
        StartCoroutine(GetStringFromURL(SRP_DatabaseManager.Instance.sServerMainURL + "version.txt"));//版本号
        StartCoroutine(DelayCheckConnection());
    }
    IEnumerator GetStringFromURL(string url)
    {
        VersionText.text = "版本号：";
        UnityWebRequest request = UnityWebRequest.Get(url);
        request.timeout = 3;
        request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            yield break;
        }
        while (!request.isDone)
        {
            yield return 0;
        }
        if (request.isDone)
        {
            if(request.downloadHandler.text == LocalVersion) VersionText.text = "版本号：" + LocalVersion + "( " +request.downloadHandler.text + " )";
            else
            {
                if(request.downloadHandler.text == "")
                {
                    yield return new WaitForSeconds(3.0f);
                    StartCoroutine(GetStringFromURL(SRP_DatabaseManager.Instance.sServerMainURL + "version.txt"));
                }
                else
                {
                VersionText.color = Color.red;
                VersionBlocker.enabled = true;
                VersionText.text = "版本号：" + LocalVersion + "( " +request.downloadHandler.text + " ) 需要更新";
                }
            }
            
            yield break;
        }
    }
    IEnumerator DelayCheckConnection()
    {
        yield return new WaitForSeconds(3.1f);
        if (SRP_DatabaseManager.Instance.bServerConnection == false)
        {
            MotdText.text = "服务器连接已断开。";
            MotdText.color = Color.red;
        }
        else
        {
            MotdText.text = "";
            MotdText.color = Color.green;
        }

    }
    void FixedUpdate()
    {
        if(SRP_DatabaseManager.Instance.bServerConnection)
        {
            MotdText.text = "";
            MotdText.color = Color.green;
        }
        else
        {
            MotdText.text = "服务器连接已断开。";
            MotdText.color = Color.red;
        }
    }
}

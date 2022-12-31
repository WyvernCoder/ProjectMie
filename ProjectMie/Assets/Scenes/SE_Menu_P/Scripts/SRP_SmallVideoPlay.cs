using System;
using System.Net;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SRP_SmallVideoPlay : MonoBehaviour
{
    [Header("在这里输入视频地址")]
    public string VideoURL = "past ur url here";
    [Header("在这里输入制作人")]
    public string Author = @"在这里输入制作人";
    [Header("在这里输入介绍")]
    public string Describe = @"在这里输入描述";
    [Header("无法获取在线图片时的默认图片")]
    public Texture IMAGE;
    [Header("视频播放页面的index")]
    public int PageIndex = 5;
    [Header("视频封面原生IMAGE")]
    public UnityEngine.UI.RawImage _RawImage;
    public bool UseOfflineImage = false;

    void Start()
    {
        if (VideoURL == "past ur url here")//你啥也不填，那就是美式霸凌了！！！
        {
            VideoURL = @"";
            Author = @"咩咩学语";
            Describe = @"来自狗狗世界的美式霸凌！";
        }
        if(UseOfflineImage == false) StartCoroutine(GetVideoImage());
    }

    IEnumerator GetVideoImage()
    {
        if(_RawImage == null) yield break;

        string[] processingURL = VideoURL.Split('.');
        for(int i = 1; i<processingURL.Length;i++)
        {
            if(i != processingURL.Length - 1) processingURL[0] += "." + processingURL[i];
        }
        UnityWebRequest unityWebRequest = UnityWebRequestTexture.GetTexture(processingURL[0] + ".jpg");
        unityWebRequest.timeout = 0;
        unityWebRequest.SendWebRequest();

        float timer = 0;
        while (true)
        {
            if (unityWebRequest.isDone) break;
            if (timer >= 3.0) yield break;//3秒超时
            timer += Time.deltaTime;
            yield return null;
        }
        
        try
        {
            var TEX = DownloadHandlerTexture.GetContent(unityWebRequest);
            _RawImage.texture = TEX;
        }
        catch
        {
            _RawImage.texture = IMAGE;
        }
        yield break;
    }

    public void CONTROL_GoAndPlayVideo()
    {
        SRP_Menu_P_Manager MenuManager = GameObject.Find("Menu_P_Manager").GetComponent<SRP_Menu_P_Manager>();
        if (MenuManager == null)
        {
            print("严重错误，无法找到MenuManager！");
            return;
        }
        MenuManager.CONTROL_GoToVideoPlayPage(VideoURL,Author,Describe,PageIndex);
    }
}

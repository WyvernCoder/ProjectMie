using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using UnityEngine.UI;

public class SRP_PrivateCanvas : MonoBehaviour
{
    [Header("对象赋值部分")]

    [Tooltip("空间清理Canvas。")]
    public Canvas SpaceCanvas;

    [Tooltip("空间大小Text。")]
    public TMP_Text SpaceOccupyText;

    public void CONTROL_OpenSpaceWindow()
    {
        //!刷新存储空间列表内容
        foreach(Transform T in SpaceCanvas.GetComponent<SRP_SpaceManager>().DLCContent.transform)
        {
            Destroy(T.gameObject);
        }
        
        SpaceCanvas.enabled = true;
        SpaceCanvas.GetComponent<SRP_SpaceManager>().RefreshContent();
    }

    public void RefreshSpaceOccupy()
    {
        if (!Directory.Exists(Application.persistentDataPath + "/DownloadContent")) Directory.CreateDirectory(Application.persistentDataPath + "/DownloadContent"); //如果目录不存在就创建

        //刷新空间占用情况
        string unit = "MB";
        float result = GetDirectionSize(Application.persistentDataPath + "/DownloadContent");

        if(result >= 1024f) 
        {
            result = GetDirectionSize(Application.persistentDataPath + "/DownloadContent", "GB");
            unit = "GB";
        }
        SpaceOccupyText.text = result + unit;
    }

    public float GetDirectionSize(string path, string unit = "MB")
    {
        float result = 0;
        var dirSource = new DirectoryInfo(path);    //获取目录信息
        foreach (FileInfo FI in dirSource.GetFiles())    //从目录信息中读取文件信息
        {
            if(unit == "MB") result += Mathf.Round(FI.Length / 1000000f);
            if(unit == "GB") result += Mathf.Round(FI.Length / 1.0000E+9f);
        }
        return result;
    }

    void FixedUpdate()
    {
        RefreshSpaceOccupy();
    }
}

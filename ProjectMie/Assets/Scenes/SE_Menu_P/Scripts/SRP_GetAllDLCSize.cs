using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SRP_GetAllDLCSize : MonoBehaviour
{
    public TMPro.TMP_Text TEXT;

    void Start()
    {
        UpdateSize();//循环函数
    }

    void UpdateSize()
    {
        TEXT.text =  GetDirectionSize(Application.persistentDataPath + "/" + "DownloadableContent").ToString() + "MB";
        Invoke("UpdateSize", 1f);//隔1秒后再调用自己
    }

    /// <summary>
    /// 获取目录大小
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public int GetDirectionSize(string path)
    {
        int result = 0;
        if(Directory.Exists(path) == false) Directory.CreateDirectory(path);//查看目录是否存在，如果不存在就新建一个 
        var dirSource = new DirectoryInfo(path);    //获取目录信息
        foreach (FileInfo FI in dirSource.GetFiles())    //从目录信息中读取文件信息
        {
            result += (int)Mathf.Round(FI.Length / 1000000f);
        }
        return result;
    }
}

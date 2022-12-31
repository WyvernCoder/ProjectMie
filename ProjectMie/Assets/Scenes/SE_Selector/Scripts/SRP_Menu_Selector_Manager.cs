using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SRP_Menu_Selector_Manager : MonoBehaviour
{
    public List<UnityEngine.UI.Button> ButtonList = new List<UnityEngine.UI.Button>();
    void Start()
    {
        //禁用全部按钮
        foreach(UnityEngine.UI.Button BTN in ButtonList) DisableButton(BTN);

        APIManager.GENERATE_BODY();

        //初始化版本号
        if(APIManager.API.API_Local_GetCustomString("version") == "NONE") APIManager.API.API_Local_SetCustomString("version", "0");
        
        //下载版本号文件
        APIManager.API.DL_StartDownload(APIManager.API.HttpServer + "/version.txt", DownloadComplete);
    }

    void DownloadComplete(bool isOK, string FileFullPath)
    {
        if(isOK == false)
        {
            Debug.LogWarning("未知原因，版本号下载失败。");
            return;
        }

        int newVersion = int.Parse(WTool.ReadFile(FileFullPath, true, false));

        /* 如果版本号对不上就清理全部选关地图文件 */
        if(int.Parse(APIManager.API.API_Local_GetCustomString("version")) != newVersion)
        {
            string savePath = Application.persistentDataPath + "/" + "DownloadableContent";
            APIManager.API.API_DeleteFile(savePath + "/" + "SE_Menu_c1.ab", true);
            APIManager.API.API_DeleteFile(savePath + "/" + "SE_Menu_c2.ab", true);
            APIManager.API.API_DeleteFile(savePath + "/" + "SE_Menu_c3.ab", true);
            APIManager.API.API_DeleteFile(savePath + "/" + "SE_Menu_c4.ab", true);
            print("最新版本号："+newVersion + "，本地版本号："+APIManager.API.API_Local_GetCustomString("version")+"，旧版本地图清空完成。");
        }

        //提升版本号
        APIManager.API.API_Local_SetCustomString("version", newVersion.ToString());

        //启用全部按钮
        foreach(UnityEngine.UI.Button BTN in ButtonList) EnableButton(BTN);
    }

    /* 这些关卡不应该随着包一块发布，而是应该位于http服务器下的OnlineAssets文件夹内 */
    /* 不过为了方便测试...呵呵呵呵呵呵呵懒得搞 */
    public void CONTROL_PlayC1() => play(0, "SE_Menu_c1");
    public void CONTROL_PlayC2() => play(1, "SE_Menu_c2");
    public void CONTROL_PlayC3() => play(2, "SE_Menu_c3");
    public void CONTROL_PlayC4() => play(3, "SE_Menu_c4");
    public void CONTROL_PlayC5() => play(4, "SE_Menu_c5");

    void play(int index, string name)
    {
        APIManager.API.API_LoadScene_SetName(name);
        APIManager.API.API_LoadScene();
    }

    void DisableButton(UnityEngine.UI.Button BTN)
    {
        BTN.interactable = false;
        BTN.gameObject.GetComponent<UnityEngine.UI.Image>().color = Color.grey;
    }
    void EnableButton(UnityEngine.UI.Button BTN)
    {
        BTN.interactable = true;
        BTN.gameObject.GetComponent<UnityEngine.UI.Image>().color = Color.white;
    }
}

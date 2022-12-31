using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SRP_Sublevel : MonoBehaviour
{
    [HideInInspector]
    public string SublevelLevelName = "NONE";
    [HideInInspector]
    public int SublevelIndex = -2;//-1是主菜单的Index，避免冲突

    public TMP_Text SublevelText;//拖拽赋值
    public Image SublevelImage;//拖拽赋值
    public Button SublevelButton;//拖拽赋值
    public TMP_FontAsset DefaultFont;//拖拽赋值，作为默认字体：苹方常规体

    public void InitalSublevel(string SublevelTextName, Sprite SublevelImage, string SublevelLevelName, int SublevelIndex, TMP_FontAsset Fontt = null)
    {   
        //初始化Sublesson
        this.SublevelText.text = SublevelTextName;
        this.SublevelImage.sprite = SublevelImage;
        this.SublevelLevelName = SublevelLevelName;
        this.SublevelIndex = SublevelIndex;
        if(Fontt == null) this.SublevelText.font = DefaultFont;
        else this.SublevelText.font = Fontt;
    }

    // !Sublesson已死
    //当小课程被点击时（如“听”“说”“读”“写”）
    // public void CONTROL_OnSublevelClick()
    // {
    //     if(SublevelLevelName == "NONE" || SublevelIndex == -2)
    //     {
    //         print("该Sublesson未被正确初始化。");
    //         return;
    //     }

    //     if(APIManager.API == null || NetManager.NET == null)
    //     {
    //         print("重要组件丢失！");
    //         return;
    //     }
        
    //     APIManager.API.API_LessonIndexCollection_Set(-2, -2, SublevelIndex);//更新当前Sublevel下标
    //     APIManager.API.API_LoadScene_SetName(SublevelLevelName);//设置要载入的关卡名
    //     APIManager.API.API_LoadScene();//走起！
    // }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SRP_Learn_Collection : MonoBehaviour
{
    public TMP_Text TEXT;//拖拽赋值，作为大课程元素的标题文本引用
    public TMP_Text DESCRIBE_TEXT;//拖拽赋值，作为大课程元素的介绍文本引用
    public Image IMAGE;//拖拽赋值，作为大课程元素的图片文本引用
    private int iCollectionIndex = -1;//该大课对应的索引

    /// <summary>
    /// 初始化大课程元素
    /// 这个函数应该是在Menu_P_Manager中调用
    /// </summary>
    public void Inital(int INDEX, string TITLE, string DESCRIBE, Sprite IMAGE, TMP_FontAsset TITLEFONT = null, TMP_FontAsset DESCRIBEFont = null)
    {
        iCollectionIndex = INDEX;
        TEXT.text = TITLE;
        DESCRIBE_TEXT.text = DESCRIBE;
        this.IMAGE.sprite = IMAGE;

        if(TITLEFONT != null) 
        {
            TEXT.font = TITLEFONT;
        }

        if(DESCRIBEFont != null) 
        {
            DESCRIBE_TEXT.font = DESCRIBEFont;
        }
    }

    /// <summary>
    /// 点击大课程后
    /// </summary>
    public void CONTROL_OnClickkk()
    {
        if(iCollectionIndex == -1)
        {
            print("该大课元素未被正确初始化！");
            return;
        }

        //如果没登录，就跳转到登录页面
        if(NetManager.NET.IsLogin() == false)
        {   
            GameObject.Find("Menu_P_Manager").GetComponent<SRP_Menu_P_Manager>().CONTROL_GoToLoginPage();
            return;
        }

        APIManager.API.API_LessonIndexCollection_Set(iCollectionIndex, -2, -2);
        APIManager.API.API_BackToMenu();
    }
}

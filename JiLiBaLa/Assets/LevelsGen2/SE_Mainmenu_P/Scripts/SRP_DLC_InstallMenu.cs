using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;
using UnityEngine.Networking;


public class SRP_DLC_InstallMenu : MonoBehaviour
{
    [Header("对象赋值部分")]
    [Tooltip("大标题控件。")]
    public Text BigTitleText;

    //[Tooltip("标题控件。")]
    //public Text TitleText;

    [Tooltip("标题控件。")]
    public GameObject TitleTextMeshPro;

    [Tooltip("图标控件。")]
    public Image IconImage;

    // [Tooltip("介绍控件。")]
    // public Text DescribeText;
    [Tooltip("介绍控件。")]
    public GameObject DescribeText;

    [Tooltip("按钮文本控件。")]
    public Text ButtonText;

    [HideInInspector]
    public SRP_DLC DLC;

    [HideInInspector]
    public bool isUnistallMode = false;

    public void CONTROL_CloseWindow()
    {
        //Destroy(gameObject);焕新
        gameObject.GetComponent<Canvas>().sortingOrder = -1000;
    }
    
    public void CONTROL_InstallDLC()
    {
        //安装模式
        if (!isUnistallMode)
        {
            if(!DLC.isUpdateBundle) DLC.InstallDLC();
            else DLC.InstallUpdate();
        }
        else//卸载模式
        {
            DLC.UnistallDLC();
        }

        CONTROL_CloseWindow();
    }

    public void InitalContent(string titleText, Sprite icon, string describeText)
    {
        isUnistallMode = false;
        //TitleText.text = titleText;
        TitleTextMeshPro.GetComponent<TMP_Text>().text = titleText;
        IconImage.sprite = icon;
        DescribeText.GetComponent<TMP_Text>().text = describeText;
    }

    public void SetUnistallMode(string unistallDecribe)
    {
        isUnistallMode = true;
        BigTitleText.text = "确认卸载";
        DescribeText.GetComponent<TMP_Text>().text = unistallDecribe;
        ButtonText.text = "卸载";
    }

    

    public void SetInstallMode(string installDecribe)
    {
        isUnistallMode = false;
        BigTitleText.text = "确认下载";
        DescribeText.GetComponent<TMP_Text>().text = installDecribe;
        ButtonText.text = "下载并安装";
    }
}

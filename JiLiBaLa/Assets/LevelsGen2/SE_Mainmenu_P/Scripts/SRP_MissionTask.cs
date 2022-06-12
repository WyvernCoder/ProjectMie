using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SRP_MissionTask : MonoBehaviour
{
    [Header("对象赋值部分")]
    [Tooltip("游戏菜单Canvas。")]
    public Canvas GameMenuCanvas;
    [Tooltip("DLC菜单Canvas。")]
    public Canvas dlcMenuCanvas;
    [Tooltip("我的菜单Canvas。")]
    public Canvas PrivateCanvas;
    [Tooltip("主菜单Manager。")]
    public SRP_Mainmenu_P_Manager MainmenuManager;

    public void CONTROL_GoToGameMenu()
    {
        //显示游戏菜单Canvas，隐藏其他菜单
        GameMenuCanvas.enabled = true;
        dlcMenuCanvas.enabled = false;
        PrivateCanvas.enabled = false;
    }

    public void CONTROL_GoToDLCMenu()
    {
        //显示DLC菜单Canvas，隐藏其他菜单
        GameMenuCanvas.enabled = false;
        dlcMenuCanvas.enabled = true;
        PrivateCanvas.enabled = false;
        
        //刷新DLC内容
        MainmenuManager.RefreshDLCContent();
    }

    public void CONTROL_GoToPrivateMenu()
    {
        //显示DLC菜单Canvas，隐藏其他菜单
        GameMenuCanvas.enabled = false;
        dlcMenuCanvas.enabled = false;
        PrivateCanvas.enabled = true;
        
        //刷新DLC内容
        MainmenuManager.RefreshDLCContent();
    }
}

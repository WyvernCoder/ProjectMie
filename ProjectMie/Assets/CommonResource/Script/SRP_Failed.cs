using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SRP_Failed : MonoBehaviour
{
    //重新开始
    public void CONTROL_Restart()
    {
        APIManager.API.API_PlayButtonSound(0);
        APIManager.API.API_LoadScene();
    }

    //返回主菜单
    public void CONTROL_Leave()
    {
        APIManager.API.API_PlayButtonSound(0);
        APIManager.API.API_BackToMenu();
    }
}

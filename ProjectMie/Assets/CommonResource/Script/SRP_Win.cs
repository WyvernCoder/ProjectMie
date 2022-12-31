using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SRP_Win : MonoBehaviour
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
        APIManager.API.API_SceneGoBack(true);
    }
}

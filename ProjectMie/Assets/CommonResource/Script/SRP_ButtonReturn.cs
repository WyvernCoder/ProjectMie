using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// 通用返回按钮，它不能做的非常复杂，但可以解决大部分情况。
/// </summary>
public class SRP_ButtonReturn : MonoBehaviour
{
    public void CONTROL_BackToMenu()
    {
        APIManager.GENERATE_BODY();

        string CurrentScentName = SceneManager.GetActiveScene().name;

        if(CurrentScentName == "SE_Selector") APIManager.API.API_BackToMenu(true);
        if(CurrentScentName == "SE_Menu_c1") APIManager.API.API_BackToMenu(false);
        if(CurrentScentName.Contains("SE_c1")) APIManager.API.API_SceneGoBack(true);
        //! 在这里加入你自己的返回规则。
    }
}

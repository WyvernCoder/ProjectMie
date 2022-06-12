using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SRP_LostConnected : MonoBehaviour
{
    /// <summary>
    /// SRP_LostConnected
    /// 作用：当断网时实例化该Prefab，使用户返回登陆界面。
    /// </summary>
    

    /// <summary>
    /// CONTROLL_OnYesDown
    /// </summary>
    bool a = true;//使按钮功能只能执行一次
    public void CONTROLL_OnYesDown()
    {
        GameManager.Instance.PlayButtonSound();
        if (a == true)
        {
            Destroy(SRP_DatabaseManager.Instance.gameObject);
            Destroy(GameManager.Instance.gameObject);
            SceneManager.LoadScene("SE_LoginPage");
            Invoke("removeself", 0.5f);
            a = false;
        }
    }
    public void CONTROLL_OnNoDown()
    {
        Application.Quit();
    }
    private void removeself()
    {
        Destroy(gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SRP_Option_01_Login : MonoBehaviour
{
    public TMPro.TMP_Text TEXT;

    void FixedUpdate()
    {
        //如果已经登录了，就显示“退出登录”，反之显示“登录账号”
        TEXT.text = WTool.Selector_("退出登录", "登录账号", UserManager.isLogin); 
    }
}

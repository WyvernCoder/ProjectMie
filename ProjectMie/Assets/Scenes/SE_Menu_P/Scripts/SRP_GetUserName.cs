using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SRP_GetUserName : MonoBehaviour
{
    public TMPro.TMP_Text TEXT;

    void Start()
    {
        if(APIManager.API == null)
        {
            print("缺少重要组件，无法获取人名！");
            return;
        }

        UserManager.Event_OnUserLogin += UpdateName;
    }

    private void UpdateName(bool isLogin)
    {
        //如果登录了就显示“你好，XXX！”
        //如果没登录就显示“未登录”
        TEXT.text = WTool.Selector_("你好，" + UserManager.GetUserInfo(UserInfoField.nickname) + "！", "未登录", isLogin);
    }
}

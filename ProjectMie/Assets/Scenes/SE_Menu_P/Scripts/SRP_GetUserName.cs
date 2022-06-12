using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SRP_GetUserName : MonoBehaviour
{
    public TMPro.TMP_Text TEXT;

    void Start()
    {
        if(NetManager.NET == null || APIManager.API == null)
        {
            print("缺少重要组件，无法获取人名！");
            return;
        }

        UpdateName();//这是个循环函数
    }

    private void UpdateName()
    {
        //如果登录了就显示“你好，XXX！”
        //如果没登录就显示“未登录”
        TEXT.text = APIManager.API.Selector("你好，" + NetManager.NET.GetUserData().Name + "！", "未登录", NetManager.NET.IsLogin());
        Invoke("UpdateName", 1f);//每隔1秒就调用这个函数，实现自动更新的功能
    }
}

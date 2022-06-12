using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SRP_Signin : MonoBehaviour
{
    public InputField NameInput;//手机号输入框
    public InputField QQInput;//手机号输入框
    public InputField PhoneInput;//手机号输入框
    public InputField PasswordInput;//密码输入框
    public TMPro.TMP_Text HintText;//提示文本

    void Start()
    {
        HintText.text = "";//初始化提示信息
    }

    public void CONTROL_Signin()
    {
        HintText.text = "";//每次登录都初始化提示

        if(NetManager.NET.IsNetworkWell() == false)
        {
            HintText.color = Color.red;
            HintText.text = "请检查是否已有网络连接";
            return;
        }

        if(PhoneInput.text == "" || PasswordInput.text == "" || QQInput.text == "" || NameInput.text == "")
        {
            HintText.color = Color.red;
            HintText.text = "凭据输入不全";
            return;
        }

        APIManager.API.API_Local_SetPhoneNumber(PhoneInput.text);//把输入框的手机号填入本地存档
        NetManager.NET.UserData_Download();//从数据库获取对应手机号的Document...没错，是直接获取整个DOCUMENT，就像直接登录了一样....非常危险吧！

        //看看是否成功下载DOCUMENT
        if (NetManager.NET.IsLogin() == true)
        {
            //账号不存在
            HintText.color = Color.red;
            HintText.text = "手机号已存在";
            NetManager.NET.UserData_Logout();//清空客户端接收的DOCUMENT
            return;
        }

        //设置本地账号数据
        APIManager.API.API_Local_SetUserName(NameInput.text);
        APIManager.API.API_Local_SetQQNumber(QQInput.text);
        APIManager.API.API_Local_SetPhoneNumber(PhoneInput.text);
        APIManager.API.API_Local_SetPassword(PasswordInput.text);
        //使用本地账号数据进行创建账号
        NetManager.NET.UserData_Create();

        //验证账号是否创建完成
        NetManager.NET.UserData_Download();
        HintText.text = APIManager.API.Selector("注册成功", "注册失败", NetManager.NET.IsLogin());

        //无论是否创建完成，都清空本地记录的数据
        APIManager.API.API_Local_SetUserName("NONE");
        APIManager.API.API_Local_SetQQNumber("NONE");
        APIManager.API.API_Local_SetPhoneNumber("NONE");
        APIManager.API.API_Local_SetPassword("NONE");

    }

    public void CONTROL_Close()
    {
        GameObject.Find("Menu_P_Manager").GetComponent<SRP_Menu_P_Manager>().CONTROL_GoToLoginPage();
    }
}

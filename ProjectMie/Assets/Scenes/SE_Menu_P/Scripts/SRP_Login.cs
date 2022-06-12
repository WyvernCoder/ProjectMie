using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SRP_Login : MonoBehaviour
{
    public InputField PhoneInput;//手机号输入框
    public InputField PasswordInput;//密码输入框
    public Image AutologinButton;//自动登录按钮
    public TMPro.TMP_Text HintText;//提示文本

    public Sprite AutoModeIsON;//自动登录图片
    public Sprite AutoModeIsOFF;//不自动登录的图片

    private bool bAutologin = false;

    void Start()
    {
        HintText.text = "";//初始化提示文本

        //初始化自动登录按钮，只有当autologin数据等于OK的时候，才会true
        bAutologin = APIManager.API.Selector(true, false, APIManager.API.API_Local_GetCustomString("autologin") == "OK");
        //初始化自动登录按钮的图片
        AutologinButton.sprite = APIManager.API.Selector(AutoModeIsON, AutoModeIsOFF, bAutologin == true);

        //自动登录
        if (bAutologin == true)
        {
            //自动填写输入框，如果本地没有数据，就啥也不填写，如果本地有数据，就填写本地数据
            PhoneInput.text = APIManager.API.Selector("", APIManager.API.API_Local_GetPhoneNumber(), APIManager.API.API_Local_GetPhoneNumber() == "NONE");
            PasswordInput.text = APIManager.API.Selector("", APIManager.API.API_Local_GetPassword(), APIManager.API.API_Local_GetPassword() == "NONE");
            Invoke("CONTROL_Login", 1f);//填写完后，过1秒执行登录。要延时，是因为网络默认是不连接的，ping通一次才算连接。
        }
    }

    public void CONTROL_Login()
    {
        HintText.text = "";//每次登录都初始化提示

        if(NetManager.NET.IsNetworkWell() == false)
        {
            HintText.color = Color.red;
            HintText.text = "请检查是否已有网络连接";
            return;
        }

        if(PhoneInput.text == "" || PasswordInput.text == "")
        {
            HintText.color = Color.red;
            HintText.text = "请输入账号或密码";
            return;
        }

        APIManager.API.API_Local_SetPhoneNumber(PhoneInput.text);//把输入框的手机号填入本地存档
        NetManager.NET.UserData_Download();//从数据库获取对应手机号的Document...没错，是直接获取整个DOCUMENT，就像直接登录了一样....非常危险吧！

        //看看是否成功下载DOCUMENT
        if (NetManager.NET.IsLogin() == false)
        {
            //账号不存在
            HintText.color = Color.red;
            HintText.text = "账号不存在";
            return;
        }

        //看看输入的密码和上面下载DOCUMENT里的密码是否相同
        if (PasswordInput.text != NetManager.NET.GetUserData().Password)
        {
            //密码错误
            HintText.color = Color.red;
            HintText.text = "猜的好，现在请输入正确的密码";
            NetManager.NET.UserData_Logout();//密码不对，就退出账号
            APIManager.API.API_Local_SetPhoneNumber("NONE");//重置本地保存的手机和密码
            APIManager.API.API_Local_SetPassword("NONE");
            return;
        }

        //登录成功
        if(bAutologin == true)//如果设置了自动登录，就把账号密码保存到本地
        {
            APIManager.API.API_Local_SetPhoneNumber(PhoneInput.text);
            APIManager.API.API_Local_SetPassword(PasswordInput.text);
        }

        //如果登录成功，就返回“我的”页面
        //只有当前位于登录页面时才会自动返回“我的”页面。不然每次开始游戏，都会因为自动登录而强制跳转到“我的”页面
        if(GameObject.Find("Menu_P_Manager").GetComponent<SRP_Menu_P_Manager>().MenuP_PageIndex() == 3) CONTROL_Close();//返回“我的”页面

        NetManager.NET.UserData_UpdateLoginTime();//刷新登录时间
    }

    public void CONTROL_Close()
    {
        GameObject.Find("Menu_P_Manager").GetComponent<SRP_Menu_P_Manager>().CONTROL_ToggleNavBar(true);//显示导航栏
        GameObject.Find("Menu_P_Manager").GetComponent<SRP_Menu_P_Manager>().CONTROL_GoToMePage();//返回“我的”页面
    }

    public void CONTROL_ToggleAutoLogin()
    {
        bAutologin = !bAutologin;

        //根据bAutologin设置图片
        AutologinButton.sprite = APIManager.API.Selector(AutoModeIsON, AutoModeIsOFF, bAutologin == true);

        //根据bAutologin设置本地自动登录标记
        APIManager.API.API_Local_SetCustomString("autologin", APIManager.API.Selector("OK", "NONE", bAutologin == true));
    }
}

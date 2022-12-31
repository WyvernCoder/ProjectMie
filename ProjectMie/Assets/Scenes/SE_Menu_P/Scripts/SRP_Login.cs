using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SRP_Login : MonoBehaviour
{
    public InputField PhoneInput;//手机号输入框     !其实是账号输入框，只是还没做支持
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
        bAutologin = WTool.Selector_(true, false, APIManager.API.API_Local_GetCustomString("autologin") == "OK");
        //初始化自动登录按钮的图片
        AutologinButton.sprite = WTool.Selector_(AutoModeIsON, AutoModeIsOFF, bAutologin == true);

        //自动登录
        if (bAutologin == true)
        {
            //自动填写输入框，如果本地没有数据，就啥也不填写，如果本地有数据，就填写本地数据
            PhoneInput.text = WTool.Selector_("", APIManager.API.API_Local_GetPhoneNumber(), APIManager.API.API_Local_GetPhoneNumber() == "NONE");
            PasswordInput.text = WTool.Selector_("", APIManager.API.API_Local_GetPassword(), APIManager.API.API_Local_GetPassword() == "NONE");
            Invoke("CONTROL_Login", 1f);//填写完后，过1秒执行登录。要延时，是因为网络默认是不连接的，ping通一次才算连接。
        }
    }

    public void CONTROL_Login()
    {
        HintText.text = "";//每次登录都初始化提示

        if(PhoneInput.text == "" || PasswordInput.text == "")
        {
            HintText.color = Color.red;
            HintText.text = "请输入账号或密码";
            return;
        }
        
        HintText.color = Color.gray;
        HintText.text = "正在登录";

        /* 把输入框的账号填入本地存档，方便下次自动登录 */
        APIManager.API.API_Local_SetPhoneNumber(PhoneInput.text);
        APIManager.API.API_Local_SetPassword(PasswordInput.text);

        /* 用本地存档里的账号密码去登录 */
        UserManager.Login(APIManager.API.API_Local_GetPhoneNumber(), APIManager.API.API_Local_GetPassword());




        /* 看看是否成功登录 */
        /* 这里其实可以用事件 Event_OnUserLogin 来判断是否登录成功的，只是没必要，太麻烦 */
        if (UserManager.isLogin == false)
        {
            HintText.color = Color.red;
            HintText.text = "我也不知道你的账号密码对不对。";

            /* 错误后就清空自动登录数据 */
            APIManager.API.API_Local_SetPhoneNumber("NONE");
            APIManager.API.API_Local_SetPassword("NONE");

            return;
        }

        /* 如果没有勾选自动登录，就再把刚刚存在本地存档里的账号密码清空 */
        if(bAutologin == false)
        {
            APIManager.API.API_Local_SetPhoneNumber("NONE");
            APIManager.API.API_Local_SetPassword("NONE");
        }

        //如果登录成功，就返回“我的”页面
        //只有当前位于登录页面时才会自动返回，不然每次启动软件时，都会因为“自动登录”而强制跳转到“我的”页面
        if(GameObject.Find("Menu_P_Manager").GetComponent<SRP_Menu_P_Manager>().MenuP_PageIndex() == 3) CONTROL_Close();//返回“我的”页面
        
        /* 登录成功后就初始化 hint text */
        HintText.color = Color.gray;
        HintText.text = "请输入账号密码。";
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
        AutologinButton.sprite = WTool.Selector_(AutoModeIsON, AutoModeIsOFF, bAutologin == true);

        //根据bAutologin设置本地自动登录标记
        APIManager.API.API_Local_SetCustomString("autologin", WTool.Selector_("OK", "NONE", bAutologin == true));
    }
}

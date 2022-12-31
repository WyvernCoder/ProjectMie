using System;
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

        //TODO 检查网络连接

        // if(PhoneInput.text == "" || PasswordInput.text == "" || QQInput.text == "" || NameInput.text == "")
        if(PhoneInput.text == "" || PasswordInput.text == "" || NameInput.text == "")
        {
            HintText.color = Color.red;
            HintText.text = "凭据输入不全";
            return;
        }

        if (UserManager.FindSameUser(UserInfoField.username, PhoneInput.text) != 0)
        {
            HintText.color = Color.red;
            HintText.text = "手机号已存在";
            return;
        }

        // 创建账号
        try
        {
            UserManager.CreateUser(PhoneInput.text, NameInput.text, PasswordInput.text);
        }
        catch(Exception ex)
        {
            HintText.text = "注册失败，原因是" + ex.Message;
            HintText.color = Color.red;
            return;
        }

        HintText.text = "注册成功";
        HintText.color = Color.green;
        return;
    }

    public void CONTROL_Close()
    {
        GameObject.Find("Menu_P_Manager").GetComponent<SRP_Menu_P_Manager>().CONTROL_GoToLoginPage();
    }
}

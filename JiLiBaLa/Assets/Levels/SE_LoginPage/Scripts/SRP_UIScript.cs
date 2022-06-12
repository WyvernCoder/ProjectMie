using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SRP_UIScript : MonoBehaviour
{
    [Header("登录部分对象引用")]

    [Tooltip("包含InputField组件的GameObject，用于输入QQ号或密码。")]
    public InputField NumberInput;

    [Tooltip("包含InputField组件的GameObject，用于在注册/登录界面输入玩家密码。")]
    public InputField PassInput;

    [Tooltip("包含Toggle组件的GameObject，用于决定是否记住玩家的登录账号和密码。")]
    public Toggle RemToggle;

    [Tooltip("包含Button组件的GameObject，用于登录按钮。")]
    public Button button;

    [Tooltip("包含Text组件的GameObject，用于显示“注册账号”和“登录账号”按钮文字。")]
    public Text changePageBtnText;
    
    [Tooltip("包含Text组件的GameObject，用于显示提示信息，比如“账号烂了，无法登录”或“正在载入”。")]
    public Text hint;

    [Tooltip("包含Canvas组件的GameObject，用于显示登录面坂UI。")]
    public Canvas canvas;

    [Tooltip("包含Canvas组件的GameObject，用于显示注册面坂UI。")]
    public Canvas RegisterCanvas;

    private bool isLoading = false;//此时是否处于载入状态？
    private bool onLoginPage = true;//此时是在登录页面还是注册页面？

    private void Start()
    {
        if (PlayerPrefs.GetInt("REM", 0) == 1)//判断是否需要自动输入账号密码
        {
            GameManager.Instance.GetInformation();//从GameManager中获取本地账号和密码，这个函数会为sNum和sPassword赋值
            NumberInput.text = GameManager.Instance.sNum;//为输入框赋值
            PassInput.text = GameManager.Instance.sPassword;//为输入框赋值
        }

        if (PlayerPrefs.GetInt("REM", 0) == 0) RemToggle.isOn = false;//自动调整“保存密码”勾选
        else RemToggle.isOn = true;
    }

    public void CONTROLL_OnToggleChanged()
    {
        int b = 0;//保存勾选状态
        if (RemToggle.isOn)b = 1;//bool转换int
        PlayerPrefs.SetInt("REM",b);
        PlayerPrefs.Save();
    }

    public void CONTROLL_OnLoginButtonPress()
    {
        GameManager.Instance.PlayButtonSound();
        if (PlayerPrefs.GetInt("REM", 0) == 1)//如果有保存密码勾选，就保存信息，反之清空信息
        {
            GameManager.Instance.sNum = NumberInput.text;
            GameManager.Instance.sPassword = PassInput.text;
            GameManager.Instance.SaveInformation();
        }
        else
        {
            GameManager.Instance.sNum = null;
            GameManager.Instance.sPassword = null;
            GameManager.Instance.SaveInformation();
        }

        //登录按钮
        bool isFind;
        isFind = false;
        SRP_DatabaseManager.Instance.UserData_Download(NumberInput.text, false);//把输入值当手机号找一遍   
        if (SRP_DatabaseManager.Instance.UserAccount.sName != null) isFind = true;
        else SRP_DatabaseManager.Instance.UserData_Download(NumberInput.text, true);//把输入值当QQ号找一遍   
        if (SRP_DatabaseManager.Instance.UserAccount.sName != null)
        {
            SRP_DatabaseManager.Instance.bUsingQQNum = true;
            isFind = true;
        }

        if (!isFind) hint.text = "账号不存在";
        else if (PassInput.text == SRP_DatabaseManager.Instance.UserAccount.sPassword) StartCoroutine(AsyncChangeToMainmenu());//登陆成功
        else
        {
            hint.text = "密码错误";
        }
    }


    private float fakeprocess = 0f;
    private float targetprocess = 0f;
    IEnumerator AsyncChangeToMainmenu()//开始加载主菜单
    {
        isLoading = true;
        SRP_DatabaseManager.Instance.UserData_UpdateLoginTime();
        SRP_DatabaseManager.Instance.UserData_Download(NumberInput.text,SRP_DatabaseManager.Instance.bUsingQQNum);
        AsyncOperation loadingScene = SceneManager.LoadSceneAsync("SE_Mainmenu");//异步加载主菜单关卡
        loadingScene.allowSceneActivation = false;
        while (loadingScene.progress <= .89)
        {
            hint.text = "加载中...";
            yield return null;
        }
        targetprocess = 1f;
        while(fakeprocess < 0.98f)
        {
            hint.text = "加载中：" + fakeprocess*100 + "%";
            yield return null;
        }
        loadingScene.allowSceneActivation = true;
        SRP_DatabaseManager.Instance.networkCanCauseEvent = true;
        yield break;
    }

    private void FixedUpdate()
    {
        fakeprocess = Mathf.Lerp(fakeprocess,targetprocess,0.1f);
        if (isLoading || SRP_DatabaseManager.Instance.bServerConnection)//出现网络问题或载入情况就禁用操作
        {
            button.interactable = false;
            NumberInput.interactable = false;
            PassInput.interactable = false;
            RemToggle.interactable = false;

            QQinputField.interactable = false;
            phoneinputField.interactable = false;
            nameinputField.interactable = false;
            passinputField.interactable = false;
            agreeToggle.interactable = false;
            registerButton.interactable = false;
        }
        else
        {
            button.interactable = true;
            NumberInput.interactable = true;
            PassInput.interactable = true;
            RemToggle.interactable = true;
            
            QQinputField.interactable = true;
            phoneinputField.interactable = true;
            nameinputField.interactable = true;
            passinputField.interactable = true;
            agreeToggle.interactable = true;
            registerButton.interactable = true;
        }
    }

    public void CONTROLL_ChangePage()//切换登录、注册界面
    {
        if(onLoginPage)
        {
            changePageBtnText.text = "去登录";
            gameObject.GetComponent<Animation>().PlayQueued("Anim_ToRegister");
            // canvas.enabled = false;
            // RegisterCanvas.enabled = true;
        }
        else
        {
            changePageBtnText.text = "去注册";
            gameObject.GetComponent<Animation>().PlayQueued("Anim_ToLogin");
            // canvas.enabled = true;
            // RegisterCanvas.enabled = false;
        }
        onLoginPage = !onLoginPage;
    }



    /// <summary>
    /// 注册部分
    /// </summary>
    [Header("注册部分对象引用")]

    [Tooltip("包含InputField组件的GameObject，用于输入QQ号。")]
    public InputField QQinputField;
    
    [Tooltip("包含InputField组件的GameObject，用于输入手机号。")]
    public InputField phoneinputField;
    
    [Tooltip("包含InputField组件的GameObject，用于输入游戏名称。")]
    public InputField nameinputField;
    
    [Tooltip("包含InputField组件的GameObject，用于输入密码。")]
    public InputField passinputField;
    
    [Tooltip("包含Toggle组件的GameObject，用于强迫用户同意协议。")]
    public Toggle agreeToggle;
    
    [Tooltip("包含Text组件的GameObject，用于提示密码错误等信息。")]
    public Text hintTexxt;
    
    [Tooltip("包含Button组件的GameObject，用于点击注册账号。")]
    public Button registerButton;

    private string nname;//为下列功能服务
    private string qqnum;//为下列功能服务
    private string phonenum;//为下列功能服务
    private string passnum;//为下列功能服务
    public void CONTROLL_OnNameInputChange() => nname = nameinputField.text;//把输入的值存储进变量
    public void CONTROLL_OnQQInputChange() => qqnum = QQinputField.text; //把输入的值存储进变量
    public void CONTROLL_OnPhoneInputChange() => phonenum = phoneinputField.text; //把输入的值存储进变量
    public void CONTROLL_OnPassInputChange() => passnum = passinputField.text; //把输入的值存储进变量
    public void CONTROLL_OnRegisterClicked()//点击注册按钮
    {
        if(nname==null || qqnum==null || phonenum==null || passnum==null)
        {
            hintTexxt.color = Color.red;
            hintTexxt.text = "请填写全部信息";
            return;
        }

        if(agreeToggle.isOn == false)
        {
            hintTexxt.color = Color.red;
            hintTexxt.text = "请同意测试协议";
            return;
        }

        registerButton.interactable = false;//防止多次注册
        hintTexxt.color = Color.black;
        hintTexxt.text = "正在注册";

        SRP_DatabaseManager.Instance.UserData_Download(phonenum);//从手机号下载一遍
        if(SRP_DatabaseManager.Instance.UserAccount.sName == null)//如果不存在
        {
            SRP_DatabaseManager.Instance.UserData_Download(qqnum,true);//从QQ下载一遍
            if(SRP_DatabaseManager.Instance.UserAccount.sName == null)//如果不存在
            {
                SRP_DatabaseManager.Instance.UserData_Create(nname,qqnum,phonenum,passnum);//注册
                SRP_DatabaseManager.Instance.UserData_Download(phonenum);//从手机号下载一遍
                if(SRP_DatabaseManager.Instance.UserAccount.sName == null)
                {
                    registerButton.interactable = true;
                    hintTexxt.color = Color.red;
                    hintTexxt.text = "注册失败，请重试";
                    SRP_DatabaseManager.Instance.UserAccount.sName = null;
                }
                else
                {
                    registerButton.interactable = true;
                    hintTexxt.color = Color.green;
                    hintTexxt.text = "注册成功";
                    SRP_DatabaseManager.Instance.UserAccount.sName = null;
                }
            }
            else
            {
                registerButton.interactable = true;
                hintTexxt.color = Color.red;
                hintTexxt.text = "QQ账号已存在";
                SRP_DatabaseManager.Instance.UserAccount.sName = null;
            }
        }
        else
        {
            registerButton.interactable = true;
            hintTexxt.color = Color.red;
            hintTexxt.text = "手机号已存在";
            SRP_DatabaseManager.Instance.UserAccount.sName = null;
        }
    }
}

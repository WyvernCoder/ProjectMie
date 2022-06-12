using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SRP_Menu_P_Manager : MonoBehaviour
{
    public GameObject CanvasRoot;//拖拽赋值，我们将会遍历这个GO下的所有子GO，这些子GO必须带有Canvas组件，这么做是为了方便制作切换页面
    public GameObject Learn_SVContent;//拖拽赋值，程序化生成的关卡组件将会刷在这里
    public GameObject PRB_Learn_Collection;//拖拽赋值，程序将会使用这个Prefab作为程序化生成的元素
    public GameObject NAVBar;//拖拽赋值，一些页面不需要显示导航栏
    private int iCurrentPageIndex = 0;//页码索引
    private List<GameObject> CanvasList = new List<GameObject>();//存放每个页的Canvas GameObject的List

    public List<CollectionData> CollectionDataList = new List<CollectionData>();//用于存放序列化数据的List

    [System.Serializable]
    public struct CollectionData//序列化数据结构
    {
        [Header("名称")]
        [Header("大课程设置")]

        public string Name;
        [Header("介绍")]
        public string Describe;
        [Header("图标")]
        public Sprite Icon;
        [Header("大标题字体，留空为苹方常规体")]
        public TMPro.TMP_FontAsset TitleFont;
        [Header("简介字体，留空为苹方常规体")]
        public TMPro.TMP_FontAsset DescribeFont;
    }

    void Start()
    {
        //初始化CanvasList，把所有Canvas都加进去，便于控制
        foreach(Transform TM in CanvasRoot.transform) CanvasList.Add(TM.gameObject);
        if(CanvasList.Count == 0)
        {
            print("未找到任何页！");
            return;
        }

        MenuP_PageIndex(MenuP_PageIndex());//开屏首页

        foreach(var DATA in CollectionDataList)
        {
            var GO = Instantiate(PRB_Learn_Collection);
            GO.GetComponent<SRP_Learn_Collection>().Inital(CollectionDataList.IndexOf(DATA), DATA.Name, DATA.Describe, DATA.Icon, DATA.TitleFont, DATA.DescribeFont);
            GO.transform.SetParent(Learn_SVContent.transform, true);
        }
    }

    /// <summary>
    /// 获取当前页的索引
    /// </summary>
    /// <returns></returns>
    public int MenuP_PageIndex()
    {
        return iCurrentPageIndex;
    }

    /// <summary>
    /// 翻页至参数索引
    /// 规定：0为首页，1为学习页，2为我的，3为注册，4为登录
    /// </summary>
    /// <param name="INDEX"></param>
    public void MenuP_PageIndex(int INDEX)
    {
        if(INDEX > CanvasList.Count - 1)
        {
            print("欲翻的页的索引超出CanvasList大小，请考虑是否少页。");
            return;
        }

        iCurrentPageIndex = INDEX;//更新页码索引
        foreach(GameObject GO in CanvasList) GO.GetComponent<Canvas>().enabled = false;//隐藏全部页
        CanvasList[INDEX].GetComponent<Canvas>().enabled = true;//显示索引指定的页
    }

    public void CONTROL_ClearDLC()
    {
        string path = Application.persistentDataPath + "/" + "DownloadableContent";
        if(Directory.Exists(path) == false) return;//查看目录是否存在，如果不存在就完事
        var dirSource = new DirectoryInfo(path);    //获取目录信息
        foreach (FileInfo FI in dirSource.GetFiles())    //从目录信息中读取文件信息
        {
            File.Delete(FI.FullName);
        }

    }

    /// <summary>
    /// 切换隐藏显示导航栏
    /// </summary>
    public void CONTROL_ToggleNavBar()
    {
        NAVBar.GetComponent<Canvas>().enabled = !NAVBar.GetComponent<Canvas>().enabled;
    }

    /// <summary>
    /// 显示或隐藏导航栏
    /// </summary>
    /// <param name="Visible"></param>
    public void CONTROL_ToggleNavBar(bool Visible)
    {
        NAVBar.GetComponent<Canvas>().enabled = Visible;
    }










    /// <summary>
    /// 0是首页，1是学习页，2是我的，3是登录页
    /// </summary>
    
    public void CONTROL_GoToHandPage()
    {
        MenuP_PageIndex(0);
    }

    public void CONTROL_GoToLearnPage()
    {
        MenuP_PageIndex(1);
    }

    public void CONTROL_GoToMePage()
    {
        MenuP_PageIndex(2);
    }

    public void CONTROL_GoToLoginPage()
    {
        if(NetManager.NET.IsLogin() == true)
        {   
            //如果现在已经登录，点击按钮后就会退出登录
            NetManager.NET.UserData_Logout();
        }
        else
        {
            //如果现在没有登录，点击后就会跳转到登录页面
            CONTROL_ToggleNavBar(false);//登录页隐藏导航栏
            MenuP_PageIndex(3);
        }
    }

    public void CONTROL_GoToSigninPage()
    {
        MenuP_PageIndex(4);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SRP_SingleLesson : MonoBehaviour
{
    public Canvas SublevelCanvas;//拖拽赋值，这个Canvas用于显示整个Sublevel菜单。
    public GameObject ScrollViewContent;//拖拽赋值，这个GameObject用于存放所有Sublesson（SL）按钮元素。
    public GameObject PRB_Sublevel;//拖拽赋值，必须是PRB_Sublevel，不能是别的

    [HideInInspector]
    public int SingleLessonIndex = -2;//-2表示初始值，下面要用它来判断是否被正确初始化。这个变量的初始化是在SRP_Menu_L_Manager里进行赋值的。
    private bool isLoaded = false;//只有在打开Sublevel目录时才会创建Sublevel目录里的东西，这个是用来判断是否已经打开过的

    [Header("在这里设置Sublesson目录的背景图片。")]
    public Sprite SublevelBGImage;
    [Header("在这里输入Sublesson课程数据。")]
    public List<SublevelData> Sublevels;

    [System.Serializable]//可序列化标签，标记这个标签后，就可以在Unity右侧Inspector面板进行加减操作了
    public struct SublevelData
    {
        [Header("Sublesson的名称，如“听”“说”“读”“写”")]
        public string TitleText;
        [Header("Sublesson对应的关卡名，如“c1a1”")]
        public string LevelName;
        [Header("Sublesson的竖图片。")]
        public Sprite Image;
        [Header("字体，留空为苹方常规体。")]
        public TMPro.TMP_FontAsset Fontt;
    }

    void Start()
    {
        ToggleSublevelActive();
    }


    //Toggle的意思是切换
    //就是执行一次它就会打开，再执行一次它就会关闭，很智能吧
    public void ToggleSublevelActive()
    {
        SublevelCanvas.enabled = !SublevelCanvas.enabled;
    }

    /// <summary>
    /// 切换解锁自己
    /// </summary>
    public void ToggleUnlockMe()
    {
        gameObject.GetComponent<BoxCollider2D>().enabled = !gameObject.GetComponent<BoxCollider2D>().enabled;
        if(gameObject.GetComponent<BoxCollider2D>().enabled == false) gameObject.GetComponent<SpriteRenderer>().color = Color.gray;
        else gameObject.GetComponent<SpriteRenderer>().color = Color.white;
    }

    //当课程被点击时（如“字母a”）
    //这里采用Toggle模式，很方便吧。
    public void CONTROL_ToggleSublessonMenu()
    {
        if(SingleLessonIndex == -2)
        {
            print("该SingleLesson的index未被正确设置，请检查SRP_Menu_L_Manager中的遍历部分是否正确！");
            return;
        }

        //判断动作是滑动还是点击，如果是滑动，就啥也不干
        //因为鼠标松开时会触发OnMouseUp，所以玩家滑动的时候松开也会触发，这里加个判断条件看看玩家究竟是在滑动还是点击。
        if(Mathf.Abs(APIManager.API.API_GetTouchMover().MoveLength) > 1) return;

        ToggleSublevelActive();//显示或关闭Sublevel菜单

        APIManager.API.API_GetTouchMover().CanMoveCamera = !APIManager.API.API_GetTouchMover().CanMoveCamera;//禁止或不禁止移动相机，否则玩家在子级菜单也会影响主菜单相机的移动

        //如果当前SingleLesson索引和该API那边的索引相同，说明现在已经打开Sublesson菜单，那么要做的就是关闭Sublesson，即：设置Sublesson索引为-1，就是没打开的意思。
        if(APIManager.API.API_LessonIndexCollection_Get().y == SingleLessonIndex) APIManager.API.API_LessonIndexCollection_Set(-2, -1, -1);
        else APIManager.API.API_LessonIndexCollection_Set(-2, SingleLessonIndex, -1);//不相同，说明现在没打开，那么要做的就是打开，就把API的索引设为SingleLessonIndex

        //只有在打开时才加载UI里面的东西
        if(isLoaded == false)
        {
            isLoaded = true;
            //遍历Sublevel，有几个课程就生成几个Sublevel按钮
            foreach(SublevelData DATA in Sublevels)
            {
                //实例化一个Sublevel按钮
                var SL = Instantiate(PRB_Sublevel).GetComponent<SRP_Sublevel>();

                //把这个按钮塞到ScrollView的Content里。
                //ScrollView就是列表，把东西塞进它的Content里它就会自动排序
                SL.transform.SetParent(ScrollViewContent.transform);

                //初始化Sublevel
                //IndexOf函数作用是获取一个东西在它的list里的index，就像搜索功能
                SL.InitalSublevel(DATA.TitleText, DATA.Image, DATA.LevelName, Sublevels.IndexOf(DATA), DATA.Fontt);                
                
                //如果数据库那边是false，就把这个Sublevel禁用并设置图片灰色
                if(NetManager.NET.GetUserData().ClassData[APIManager.API.API_LessonIndexCollection_Get().x].LessonList[APIManager.API.API_LessonIndexCollection_Get().y].isSublevelUnlock[Sublevels.IndexOf(DATA)] == false)
                {
                    SL.SublevelButton.enabled = false;
                    SL.SublevelImage.gameObject.GetComponent<Image>().color = Color.gray;
                }
            }
        }
    }

    
}

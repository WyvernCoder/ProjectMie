using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SRP_Menu_L_Manager : MonoBehaviour
{
    public GameObject BackgroundGameObject;//背景图片GameObject

    [Tooltip("调试模式下的大类课程索引，0是白泽识字，1是古诗学习，2是山海经涂鸦，3是图书馆")]
    public int DebugCollectionIndex = 0;//如果直接从横主菜单开始游戏，会跳过设置大课程索引的过程，所以这里必须为DEBUG模式做点什么

    [Header("请确保选择图片为1080P，不要大不要小，影响适配")]
    [Header("在这里按照大课顺序添加背景图，第一个图片将会作为Fallback。")]
    public List<Sprite> CollectionBackgroundImages = new List<Sprite>();//背景图片List

    [Header("在这里按照大课顺序拖拽大课Collection的根GameObject，开始游戏后会按照当前所选的大课而隐藏其他未选的大课。")]
    public List<GameObject> CollectionRootGameObjects = new List<GameObject>();//大课根GameObject的List

    void Start()
    {
        if(APIManager.API == null || NetManager.NET == null)
        {
            print("缺少重要组件！");
            return;
        }

        //debug模式自动选择第一大类课程，可以自行修改
        if(NetManager.NET.DebugMode == true) APIManager.API.API_LessonIndexCollection_Set(DebugCollectionIndex, -2, -2);

        //*检测本地大课数量和数据库大课数量是否相同
        if(CollectionRootGameObjects.Count != NetManager.NET.GetUserData().ClassData.Count)
        {   
            //如果数据库大课数量小于本地大课数量，就为数据库增加条目，否则多出来的课程没办法存储数据
            //我们只要保证本地大课程数量小于等于数据库大课程数量就可以了
            if(NetManager.NET.GetUserData().ClassData.Count < CollectionRootGameObjects.Count)
            {
                //计算出相差的条目数量
                int FixAmount = CollectionRootGameObjects.Count - NetManager.NET.GetUserData().ClassData.Count;
                //添加FixAmount个大类课程
                NetManager.NET.UserData_AddElement(0, FixAmount);
            }
        }

        //*检测已选择的本地大课中的SingleLesson的数量和数据库中的量是否相同
        if (CollectionRootGameObjects[APIManager.API.API_LessonIndexCollection_Get().x].transform.Find("SingleLessonList/").childCount != NetManager.NET.GetUserData().ClassData[APIManager.API.API_LessonIndexCollection_Get().x].LessonList.Count)
        {
            //如果数据库数量小于本地数量
            if (NetManager.NET.GetUserData().ClassData[APIManager.API.API_LessonIndexCollection_Get().x].LessonList.Count < CollectionRootGameObjects[APIManager.API.API_LessonIndexCollection_Get().x].transform.Find("SingleLessonList/").childCount)
            {
                //计算出相差的条目数量
                int FixAmount = CollectionRootGameObjects[APIManager.API.API_LessonIndexCollection_Get().x].transform.Find("SingleLessonList/").childCount - NetManager.NET.GetUserData().ClassData[APIManager.API.API_LessonIndexCollection_Get().x].LessonList.Count;
                //添加FixAmount个SingleLesson
                NetManager.NET.UserData_AddElement(1, FixAmount, -3);
            }
        }

        //初始化背景图片
        if(APIManager.API.API_LessonIndexCollection_Get().x + 1 > CollectionBackgroundImages.Count - 1)
        {
            print("课程图片List下标溢出，请考虑添加更多图片，不会影响程序进行。");
            BackgroundGameObject.GetComponent<SpriteRenderer>().sprite = CollectionBackgroundImages[0];
        }
        else BackgroundGameObject.GetComponent<SpriteRenderer>().sprite = CollectionBackgroundImages[APIManager.API.API_LessonIndexCollection_Get().x + 1];

        //初始化大类课程列表
        //先把所有大类课程列表（Collection）都隐藏，然后再根据API里的LessonIndex显示出来
        foreach(GameObject GO in CollectionRootGameObjects) GO.SetActive(false);
        if(APIManager.API.API_LessonIndexCollection_Get().x > CollectionRootGameObjects.Count - 1)
        {
            print("缺少所选大课的Root GameObject，程序停止。请检查CollectionRootGameObjects List的数量是否够量！");
            Destroy(gameObject);
        }
        else CollectionRootGameObjects[APIManager.API.API_LessonIndexCollection_Get().x].SetActive(true);

        //解锁课程，如“字母a”、“字母b”。注意，字母课程下还有一层小课程，如“拼”“说”“读”“写”
        //Find函数既可以用来查询，也可以用来访问一个GameObject下的子GameObject，这里是用Find去获取并遍历下面的GameObject，然后根据数据库数据去设置状态
        //别问我为什么 “遍历GameObject的transform就是遍历这个GameObject下的所有子GameObject”，这个是Unity规定的，一开始我也疑惑。。。。
        int forIndex = 0;//要用它去为每个SingleLesson的Index去赋值，这样每个SingleLesson就能知道自己的index了
        foreach(Transform TM in CollectionRootGameObjects[APIManager.API.API_LessonIndexCollection_Get().x].transform.Find("SingleLessonList/").transform)
        {
            TM.gameObject.GetComponent<SRP_SingleLesson>().SingleLessonIndex = forIndex;//告诉SingleLesson它的index

            //*检测本地Sublevel数量和数据库Sublevel数量是否相同
            if(TM.gameObject.GetComponent<SRP_SingleLesson>().Sublevels.Count != NetManager.NET.GetUserData().ClassData[APIManager.API.API_LessonIndexCollection_Get().x].LessonList[forIndex].isSublevelUnlock.Count)//因为玩家在这个阶段还没有选择SingleLesson，不能用API提供的入口获取(会返回-1)，所以只能用forIndex拿到属于本次遍历的SingleLesson下标
            {
                //如果数据库数量小于本地数量
                if(NetManager.NET.GetUserData().ClassData[APIManager.API.API_LessonIndexCollection_Get().x].LessonList[forIndex].isSublevelUnlock.Count < TM.gameObject.GetComponent<SRP_SingleLesson>().Sublevels.Count)
                {
                    //计算出相差的条目数量
                    int FixAmount = TM.gameObject.GetComponent<SRP_SingleLesson>().Sublevels.Count - NetManager.NET.GetUserData().ClassData[APIManager.API.API_LessonIndexCollection_Get().x].LessonList[forIndex].isSublevelUnlock.Count;
                    //添加FixAmount个大类课程到当前所选
                    NetManager.NET.UserData_AddElement(2, FixAmount, -3, forIndex);
                }
            }

            //如果这个SingleLesson的第一个Sublevel是解锁的，那么就解锁这个SingleLesson
            //为什么这样做？因为如果这个章节里的第一关被解锁，说明已经玩到这个章节了
            TM.gameObject.GetComponent<SRP_SingleLesson>().ToggleUnlockMe();//锁定
            if(NetManager.NET.GetUserData().ClassData[APIManager.API.API_LessonIndexCollection_Get().x].LessonList[forIndex].isSublevelUnlock[0] == true)
            {
                TM.gameObject.GetComponent<SRP_SingleLesson>().ToggleUnlockMe();//解锁当前SingleLesson
            }

            //初始化StarSystem
            if(TM.Find("StarSystem/").GetComponent<SRP_StarSystem>() != null) 
            {
                TM.Find("StarSystem/").GetComponent<SRP_StarSystem>().iSingleLessonIndex = forIndex;//告诉星星系统它的SingleLesson index
                TM.Find("StarSystem/").GetComponent<SRP_StarSystem>().InitalStarSystem();//让StarSystem生成Star
            }
            
            forIndex++;
        }
    }

    public void CONTROL_ReturnMenuP()
    {
        APIManager.API.API_BackToMenu(true);
    }
}
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using LitJson;

//MongoDB
using MongoDB;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Core.Operations;
using MongoDB.Libmongocrypt;

public class NetManager : MonoBehaviour
{
    [HideInInspector]
    public static NetManager NET;   //静态类型变量，便于访问NetManager

    [Header("MongoDB 设置部分")]

    [Tooltip("如果为true，就会自动登录奇怪账号")]
    public bool DebugMode = false;

    [Tooltip("MongoDB连接地址，例：mongodb://localhost:27017。")]
    public string MongoDB_URL = "mongodb://localhost:27017";
    
    [Tooltip("MongoDB中的数据库名称。")]
    public string MongoDB_DataBaseName = "JiLiBaLaDB";

    [Tooltip("数据库中的集合名称。")]
    public string MongoDB_CollectionName = "UserCO";
    
    [Header("php服务器 设置部分")]
    
    [Tooltip("php服务器的IP部分URL，例：http://localhost:27017/")]
    public string phpURL = "http://localhost:2021/";

    [Tooltip("程序会通过ping此IP以知晓服务器连接状况。")]
    public string PingAddress = "localhost";

    private IMongoCollection<BsonDocument> DBCollection;//用于存储数据库当前Collection的变量
    private UserDocument UserAccount = new UserDocument();//该变量用于存储从数据库接收的数据，也用于发送
    private bool bNetworkWell = false;//用于判断ping是否能ping通


    void Awake()
    {
        if(NET != null) Destroy(gameObject);    //去重
        NET = this;
        DontDestroyOnLoad(this);
        SetMongoDBAddress(MongoDB_URL, MongoDB_DataBaseName, MongoDB_CollectionName);//获取MongoDB的Collection数据到本地
    }

    void Start()
    {
        if(DebugMode) 
        {
            APIManager.API.API_Local_SetPhoneNumber("phone");
            NetManager.NET.UserData_Download();
        }
        StartCoroutine(DoPing_IE());//开启ping协程，这个协程是无限循环的，协程放进Awake会出毛病





    }










    /// <summary>
    /// MongoDB 序列化部分，对应的是数据库里的结构类型
    /// </summary>
    public class UserDocument//用户数据类
    {
        public ObjectId _id;//用户的识别标识符
        public string Name = "NONE";//用户名称
        public string QQ;//用户QQ号
        public string Phone;//用户电话号
        public string Password;//用户密码
        public List<int> SignDay = new List<int>();//注册日期
        public List<int> LoginDay = new List<int>();//登录日期

        //大类课程，比如“学拼音”、“学英语”、“学成语”的存档是分开放的，元素0规定为拼音，元素1规定为古诗，元素2为涂鸦
        public List<ClassCollection> ClassData = new List<ClassCollection>();
    }
    //ClassData->大类ClassCollection->单一课程LessonList->小课程
    public class ClassCollection
    {
        public List<SingleLesson> LessonList = new List<SingleLesson>();//存放每节课程的数据
    }

    public class SingleLesson
    {
        public bool isTotalUnlock = false;//是否已解锁全部小课程
        public List<bool> isSublevelUnlock = new List<bool>();//小课程解锁情况，比“对话系统”“汉字解说”这些阶段的解锁情况
        public List<int> SublevelScore = new List<int>();//小课程得分情况
        public List<int> LegacyInt = new List<int>();//遗留系
        public List<string> LegacyString = new List<string>();//遗留系
    }

















    // !作用类函数


    /// <summary>
    /// 创建账号
    /// </summary>
    public void UserData_Create()
    {
        if(APIManager.API == null)
        {
            print("未检测到APIManager的存在，终止操作。");
            return;
        }

        BsonDocument newDocument = BsonDocument.Parse(GetJsonFileText("UserSetting"));//直接把Json解析成Bson
        UserDocument Cache = BsonSerializer.Deserialize<UserDocument>(newDocument);//将Bson反序列化成UserDocument类对象
        Cache._id = new ObjectId(Random24DigitString());//ObjectID是Mongo规定的类，这里为ObjectID生成一个随机的24位字符
        Cache.Name = APIManager.API.API_Local_GetUserName();//为Cache对象的各项赋值，以本地数据为主
        Cache.QQ = APIManager.API.API_Local_GetQQNumber();
        Cache.Phone = APIManager.API.API_Local_GetPhoneNumber();
        Cache.Password = APIManager.API.API_Local_GetPassword();
        Cache.SignDay[0] = DateTime.Now.Year;//注册时间
        Cache.SignDay[1] = DateTime.Now.Month;
        Cache.SignDay[2] = DateTime.Now.Day;
        //Cache.ClassData.Add(new ClassCollection());//在ClassData里添加一个大类，如“学拼音”“学成语”
        //Cache.ClassData[0].LessonList.Add(new SingleLesson());//在ClassData里添加一个课程
        //↑由于JSON里已经有了，所以上面这两行就不必了↑
        Cache.ClassData[0].LessonList[0].isSublevelUnlock.Add(true);//在课程里添加一个关卡并解锁第一关
        Cache.ClassData[0].LessonList[0].SublevelScore.Add(0);  //在课程里添加一个得分元素并赋予0分
        Cache.ClassData[0].LessonList[0].LegacyInt.Add(0);//在课程里添加一个遗留整形元素，并赋值0
        Cache.ClassData[0].LessonList[0].LegacyString.Add("NONE");//在课程里添加一个遗留字符串元素，并赋值0
        string CacheJson = Newtonsoft.Json.JsonConvert.SerializeObject(Cache);//将Cache反序列化成string
        newDocument = BsonDocument.Parse(CacheJson);//将string解析成Document
        DBCollection.InsertOne(newDocument);//将Document插入Collection
    }

    /// <summary>
    /// 更新数据库数据
    /// 这个不会更新账号信息，只会更新课程数据
    /// </summary>
    public void UserData_Update()
    {
        if(!IsLogin())
        {
            print("未登录账号，终止操作。");
            return;
        }

        //定义Update，使得这个Update会使用UserAccount里的ClassData去替换数据库里的ClassData
        UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update.Set("ClassData", UserAccount.ClassData);

        //更新，条件是数据库的Phone值等于本地Phone值
        DBCollection.UpdateOne(Builders<BsonDocument>.Filter.Eq("Phone", APIManager.API.API_Local_GetPhoneNumber()), update);
    }

    /// <summary>
    /// 从数据库下载用户数据
    /// </summary>
    public void UserData_Download()
    {
        //定义Find过滤器并使用该过滤器从本地DBCollection变量中进行过滤；userdata里是一组用户数据，但根据这个过滤条件，我们肯定会拿到只有一个元素的userdata
        IFindFluent<BsonDocument, BsonDocument> userdata = DBCollection.Find(Builders<BsonDocument>.Filter.Eq("Phone", APIManager.API.API_Local_GetPhoneNumber()));
        foreach (var DOC in userdata.ToEnumerable())
        {
            UserAccount = BsonSerializer.Deserialize<UserDocument>(DOC);//将DOCUMENT反序列化成自己定义的UserAccount类并为其赋值，完成更新
        }
    }

    /// <summary>
    /// 更新登录时间
    /// </summary>
    public void UserData_UpdateLoginTime()
    {
        if(!IsLogin())
        {
            print("未登录账号，终止操作。");
            return;
        }
        
        UserAccount.LoginDay[0] = DateTime.Now.Year;
        UserAccount.LoginDay[1] = DateTime.Now.Month;
        UserAccount.LoginDay[2] = DateTime.Now.Day;
        
        //单独更新一次LoginDay
        UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update.Set("LoginDay", UserAccount.LoginDay);
        DBCollection.UpdateOne(Builders<BsonDocument>.Filter.Eq("Phone", APIManager.API.API_Local_GetPhoneNumber()), update);
    }

    /// <summary>
    /// 为数据库添加条目
    /// </summary>
    /// <param name="EleType">0表示要添加课程大类，1表示添加SingleLesson，2表示添加Sublevel。</param>
    /// <param name="Amount">要添加的数量。</param>
    /// <param name="ClassCollectionIndex">课程大类索引，SingleLesson会被添加至此课程大类中。如不需要设置，参数留空即可。如需设置为当前所选SingleLesson的下标则将其设为-3即可。</param>
    /// <param name="SingleLessonIndex">SingleLesson索引，Sublevel会被添加至此SingleLesson中。如不需要设置，参数留空即可。如需设置为当前所选SingleLesson的下标则将其设为-3即可。</param>
    public void UserData_AddElement(int EleType, int Amount, int ClassCollectionIndex = -1, int SingleLessonIndex = -1)
    {
        if(Amount < 0) { print("添加数量不合法，添加中断。"); return; }
        if(Amount == 0) return;

        if(ClassCollectionIndex == -3) ClassCollectionIndex = APIManager.API.API_LessonIndexCollection_Get().x;
        if(SingleLessonIndex == -3) SingleLessonIndex = APIManager.API.API_LessonIndexCollection_Get().y;

        switch(EleType)
        {
            case 0:
                {
                    for (int i = 0; i < Amount; i++)
                    {
                        UserAccount.ClassData.Add(new ClassCollection());//在ClassData中添加一个元素

                    }
                    break;
                }
            case 1:
                {
                    //判断一下前置条件的情况
                    if(ClassCollectionIndex == -1)
                    {
                        print("函数未设置大类索引，添加中断！");
                        return;
                    }

                    for (int i = 0; i < Amount; i++)
                    {
                        UserAccount.ClassData[ClassCollectionIndex].LessonList.Add(new SingleLesson());
                    }
                    break;
                }
            case 2:
                {
                    //判断一下前置条件的情况
                    if(ClassCollectionIndex == -1)
                    {
                        print("函数未设置大类索引，添加中断！");
                        return;
                    }
                    if(SingleLessonIndex == -1)
                    {
                        print("函数未设置SingleLesson索引，添加中断！");
                        return;
                    }
                    
                    for (int i = 0; i < Amount; i++)
                    {
                        UserAccount.ClassData[ClassCollectionIndex].LessonList[SingleLessonIndex].isSublevelUnlock.Add(APIManager.API.Selector<bool>(true, false, SingleLessonIndex == 0 && i == 0));//添加Sublevel，如果这个Sublevel是这个大课里的第一个Sublevel（注意，是“大课”里的第一个Sublevel），那么就解锁它，不然玩家没法玩这个大课
                        UserAccount.ClassData[ClassCollectionIndex].LessonList[SingleLessonIndex].SublevelScore.Add(-1);//这些都是一个Sublevel的附属物
                        UserAccount.ClassData[ClassCollectionIndex].LessonList[SingleLessonIndex].LegacyInt.Add(-1);
                        UserAccount.ClassData[ClassCollectionIndex].LessonList[SingleLessonIndex].LegacyString.Add("NONE");
                    }
                    break;
                }
            default: return;
        }
        UserData_Update();
    }

    /// <summary>
    /// 设置MongoDB地址
    /// </summary>
    /// <param name="URL"></param>
    /// <returns></returns>
    public string SetMongoDBAddress(string URL, string DBName, string DBCollectionName)
    {
        //初始化数据库Collection
        DBCollection = new MongoClient(URL).GetDatabase(DBName).GetCollection<BsonDocument>(DBCollectionName);
        return URL;
    }

    /// <summary>
    /// 获取MongoDB数据库地址
    /// </summary>
    /// <returns></returns>
    public string GetMongoDBAddress()
    {
        return MongoDB_URL;
    }
    
    /// <summary>
    /// 设置PHP服务器地址
    /// </summary>
    /// <param name="URL"></param>
    /// <returns></returns>
    public string SetPHPAddress(string URL)
    {
        phpURL = URL;
        return URL;
    }
    
    /// <summary>
    /// 获取php服务器地址
    /// </summary>
    /// <returns></returns>
    public string GetPHPAddress()
    {
        return phpURL;
    }

    /// <summary>
    /// 设置Ping地址，不要端口号
    /// </summary>
    /// <param name="URL"></param>
    /// <returns></returns>
    public string SetPingAddress(string URL)
    {
        PingAddress = URL;
        return URL;
    }
    
    /// <summary>
    /// 获取当前ping的地址，该地址负责判断是否联网
    /// </summary>
    /// <returns></returns>
    public string GetPingPAddress()
    {
        return PingAddress;
    }
    
    /// <summary>
    /// 判断是否登录
    /// </summary>
    /// <returns></returns>
    public bool IsLogin()
    {
        return UserAccount.Name != "NONE";
    }

    /// <summary>
    /// 判断是否已联网
    /// </summary>
    /// <returns></returns>
    public bool IsNetworkWell()
    {
        return bNetworkWell;
    }

    /// <summary>
    /// 获取已下载至本地的用户数据
    /// </summary>
    /// <returns></returns>
    public UserDocument GetUserData()
    {
        return UserAccount;
    }

    /// <summary>
    /// 登出账号
    /// </summary>
    public void UserData_Logout()
    {
        UserAccount = new UserDocument();
    }

    IEnumerator DoPing_IE()
    {   //该协程为永远循环，会对bNetworkWell变量进行终生维护
        Ping ping = new Ping(PingAddress);
        float timer = 0f;//计时器，单位是毫秒(deltaTime)

        //TimeOut时间3秒，如果在这时间里能ping通，那就OK；ping不通则计为寄了。
        while (timer <= 3f && !ping.isDone)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        
        //判断是否ping通
        if(ping.isDone) 
        {
            bNetworkWell = true;
            if(timer > 2f) 
            {   //如果上次ping时间大于2则直接再ping，网络状况不佳
                StartCoroutine(DoPing_IE());
            }
            else 
            {
                //如果时间小于2则等10秒再ping，网络状况良好
                yield return new WaitForSeconds(10f);
                StartCoroutine(DoPing_IE());
            }
        }
        else 
        {
            bNetworkWell = false;
            yield return new WaitForSeconds(2f);
            StartCoroutine(DoPing_IE());
        }

        ping.DestroyPing();
        yield break;
    }

    






















    // !工具类函数






    /// <summary>
    /// 将JSON内容映射到对象
    /// 用法：参数为Assets/CommonResource/Json下的Json文件名，不需要带后缀名；T为JSON映射的类。
    /// 举例：JsonFileConvertToObject<UserDocument>("UserSetting")  将UserSetting.json映射至UserDocument类。
    /// </summary>
    public T JsonFileConvertToObject<T>(string JsonFileName = "UserSetting")
    {
        StreamReader STREAMREADER = new StreamReader(Application.dataPath + "/CommonResource/Json/" + JsonFileName + ".json");//读取数据，转换成数据流
        JsonReader JSR = new JsonReader(STREAMREADER);//将数据流转换成JS数据
        return JsonMapper.ToObject<T>(JSR);//将JsonReader转换成Object
    }

    /// <summary>
    /// 将类对象的内容转换为Json字符串，理论上任何形式的对象都可以转换成Json内容
    /// </summary>
    public string ObjectConvertToJSONString(object obj)
    {   
        //ObjectID类是Mongo规定用于区分用户的类
        //return JsonMapper.ToJson(obj);这个不支持转换MongoDB的ObjectId类，不能使用
        return Newtonsoft.Json.JsonConvert.SerializeObject(obj);
    }

    /// <summary>
    /// 将JSON字符串转换成BSON
    /// 用法：参数为JSON字符串，会返回BSON
    /// </summary>
    public BsonDocument JsonStringToBson(string JsonString) 
    {   //把string JS反序列化成BSON Document
        return MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonDocument>(JsonString);
    }

    /// <summary>
    /// 获取指定Json文件的字符串
    /// </summary>
    public string GetJsonFileText(string JsonFileName = "UserSetting")
    {
        StreamReader SR = File.OpenText(Application.dataPath + "/CommonResource/Json/" + JsonFileName + ".json");
        return SR.ReadToEnd();
    }

    /// <summary>
    /// 生成随机24位字符
    /// </summary>
    public string Random24DigitString()
    {
        string Result = "";
        List<string> EveryDigit = new List<string>();//48~57数字，65~90大写，97~122小写
        for(int i = 0; i < 24; i++)
        {
           switch (UnityEngine.Random.Range(0,3))//盲盒三选一
           {
               case 0://ObjectID只接受16进制数，因此字母应当在97~103之间
                   EveryDigit.Add(((char)UnityEngine.Random.Range(48,58)).ToString());
                   break;
               case 1:
                   EveryDigit.Add(((char)UnityEngine.Random.Range(97,103)).ToString());
                   break;
               case 2:
                   EveryDigit.Add(((char)UnityEngine.Random.Range(97,103)).ToString());
                   break;
           }
        }
        foreach(string STR in EveryDigit) Result += STR;
        return Result;
    }
}
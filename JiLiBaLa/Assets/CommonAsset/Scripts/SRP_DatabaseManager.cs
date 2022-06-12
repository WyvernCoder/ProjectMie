using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Bson.Serialization;
using MongoDB.Driver.Core.Operations;
using LitJson;
using UnityEngine.Networking;
/// <summary>
/// SRP_DataManager
/// 作用：为 全局游戏 提供数据库连接功能，如查询、更新MongoDB数据库数据等。
/// </summary>

public class SRP_DatabaseManager : MonoBehaviour
{
    private static SRP_DatabaseManager _instance;
    public static SRP_DatabaseManager Instance
    {
        get
        {
            if (_instance == null) _instance = FindObjectOfType<SRP_DatabaseManager>() as SRP_DatabaseManager;//去重
            return _instance;
        }
    }

    [Header("MongoDB 设置部分")]

    [Tooltip("MongoDB连接地址，例：mongodb://localhost:27017。")]
    public string MongoDB_URL = "mongodb://82.157.140.63:27018";
    
    [Tooltip("MongoDB中的数据库名称。")]
    public string MongoDB_DataBaseName = "JiLiBaLaDB";

    [Tooltip("数据库中的集合名称。")]
    public string MongoDB_CollectionName = "UserCO";
    


    [Header("php服务器 设置部分")]
    
    [Tooltip("php服务器的IP部分URL，例：http://localhost:27017/")]
    public string sServerMainURL = "http://82.157.140.63/";

    [Tooltip("程序会通过ping此IP以知晓服务器连接状况，必须只有IP并且不要带端口。")]
    public string sPingAddress = "82.157.140.63";
    
    [Tooltip("php服务器的连接问题是否能影响到游戏的进行。")]
    public bool networkCanCauseEvent = true;

    [HideInInspector]
    public bool bServerConnection = true;//当前情况下是否已经连接至php服务器
    // [HideInInspector]
    // public bool bDataBaseConnection = false;//当前情况下是否连接至数据库
    // [HideInInspector]
    // public bool bNetworkIssue = true;//当前情况下，php或MongoDB连接，只要有一个出了问题，这东西就是true
    [HideInInspector]
    public bool bUsingQQNum = false;//当前情况下，如为false，说明用户使用的是电话号，如为true，说明是用QQ号
    


    [Header("对象引用部分")]
    
    [Tooltip("在无法连接至php服务器时创建的Prefab，通常是询问用户是否要退出游戏或返回登陆界面。")]
    public GameObject NetworkErrorPrefab;

    [HideInInspector]
    public int iNetworkVersion = -1;

    //[Header("用户赋值部分")]

    // [Tooltip("在线 AssetBundle 所下载的版本号。")]
    // public uint iDownloadVersion = 5;


    /// <summary>
    /// MongoDB 序列化部分，对应的是MongoDB的结构类型
    /// </summary>
    public class UserDocument//用户数据类
    {
        public ObjectId _id;//用户的识别标识符
        public string sName = null;//用户名称
        public string sQQNum;//用户QQ号
        public string sPhoneNum;//用户电话号
        public string sPassword;//用户密码
        public int sAccountVersion;//账号版本
        public List<int> SignDay = new List<int>();//注册日期
        public List<int> LoginDay = new List<int>();//登录日期
        public List<Island> ClassData = new List<Island>();//Island元素数组
    }
    public class Island//对应每一个岛屿
    {
        public List<bool> SMUnlock = new List<bool>();//岛屿上的SubIsland的解锁情况
        public List<int> Score = new List<int>();//SubIsland的得分情况
        public List<Level_> Level = new List<Level_>();//Level元素数组
    }
    public class Level_//对应SubMainmenu中的Level
    {
        public List<bool> LUnlock = new List<bool>();//Level解锁情况
        public List<int> Score = new List<int>();//Level得分情况
    }


    private IMongoCollection<BsonDocument> collection; //数据库的Collection
    void Start()
    {
        DontDestroyOnLoad(this);
        collection = new MongoClient(MongoDB_URL).GetDatabase(MongoDB_DataBaseName).GetCollection<BsonDocument>(MongoDB_CollectionName);//获取数据库Collection
        //UserData_Create("CY", "2373002368", "13020390530", "12345");
        UserData_Download("13020390530");
        StartCoroutine(CheckServerConnection());   //启动php服务器状态检测
        //StartCoroutine(CheckDataBaseConnection(UserAccount.sPhoneNum, false));    //启动MongoDB连接状态检测
        StartCoroutine(_UpdateNetworkVersion());


        //bNetworkIssue = true;
    }
    private void Awake()
    {
        if (_instance != null) Destroy(gameObject);//去重，防止有多个PRB_DatabaseManager的存在
    }


    /// <summary>
    /// 协程类
    /// </summary>
    public void UpdateNetworkVersion() => StartCoroutine(_UpdateNetworkVersion());
    IEnumerator _UpdateNetworkVersion()
    {
        UnityWebRequest request = UnityWebRequest.Get(sServerMainURL + "version.txt");
        request.SendWebRequest();
        while (!request.isDone)
        {
            yield return 0;
        }
        int.TryParse(request.downloadHandler.text, out iNetworkVersion);
        yield break;
    }
    IEnumerator CheckServerConnection()//检测php服务器连接状态
    {
        Ping ping = new Ping(sPingAddress); //ping实例

        float timer = 0f;
        while (timer <= 3f && !ping.isDone)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        
        if(ping.isDone) 
        {
            bServerConnection = true;
            if(timer > 2f) 
            {   //如果上次ping时间大于1.5ms则直接再ping
                StartCoroutine(CheckServerConnection());
            }
            else 
            {
                //如果时间小于1.5s则等1.5秒再ping
                yield return new WaitForSeconds(1.5f);
                StartCoroutine(CheckServerConnection());
            }
        }
        else 
        {
            bServerConnection = false;
            print("你他妈傻逼吧自己都ping不通");
        }
        ping.DestroyPing();
        yield break;
        // string cache = null;//初始化用于验证的文档
        // UnityWebRequest request = UnityWebRequest.Get(url);//声明URL连接请求
        // request.timeout = 3;//设置timeout时间为3秒
        // request.SendWebRequest();//发送URL连接请求
        // while (!request.isDone) //等待文档接收完成
        // {
        //     yield return null;
        // }

        // cache = request.downloadHandler.text;//为cache赋值接收的文档的内容
        // if (cache == "freeman you FOOLISH!") bServerConnection = true;//如果文档内容等于该内容，说明有连接
        // else bServerConnection = false;//如果文档内容不等于标识，说明连接不正常或无法连接
        // yield return new WaitForSeconds(3);//间歇3秒
        // StartCoroutine(CheckServerConnection(url));//开启下一次检测
        // yield break;//退出协程
    }

    // IEnumerator CheckDataBaseConnection(string Num, bool UseQQNum = false)
    // {   //由于当MongoDB无法连接时，游戏将直接卡死若干秒，非常影响用户体验，所以不如依靠php服务器检测网络状态。
    //     //MongoDB没有给出ping相关的指令，下面代码是重新连接至MongoDB并试图下载对应的用户，如果没有用户则说明MongoDB连接断开。
    //     IFindFluent<BsonDocument, BsonDocument> userdata;
    //     List<UserDocument> users = new List<UserDocument>();//声明一个userList
    //     if (UseQQNum) userdata = collection.Find(Builders<BsonDocument>.Filter.Eq("sQQNum", Num));//根据QQ号查询，若一个号码有多个用户结果，会取最后面的用户
    //     else userdata = collection.Find(Builders<BsonDocument>.Filter.Eq("sPhoneNum", Num));//根据手机号查询用户
    //     foreach (var DOC in userdata.ToEnumerable())//将查询到的DOCUMENT反序列化成自己定义的UserAccount类
    //     {
    //         users.Add(BsonSerializer.Deserialize<UserDocument>(DOC));
    //     }
    //     if (users.Count == 0) bDataBaseConnection = false;//如果List里没有元素，就说明MongoDB连接断开了
    //     else bDataBaseConnection = true;

    //     yield return new WaitForSeconds(3);//等三秒
    //     StartCoroutine(CheckDataBaseConnection(Num, UseQQNum));//再次检测
    //     yield break;
    // }

    // IEnumerator NetworkIssueCheck()//网络问题检测
    // {
    //     yield return new WaitForSeconds(3);//先等三秒，让检测服务器和数据库的东西执行完
    //     //if (!bServerConnection || !bDataBaseConnection )
    //     if (!bServerConnection)
    //     {
    //         bNetworkIssue = true;//如果上面那俩检测有一个有问题，那就是有问题了
    //         if (GameObject.Find("PRB_LostConnected(Clone)") == null && networkCanCauseEvent) Instantiate(NetworkErrorPrefab);//实例化网络错误Prefab
    //     }
    //     else if (SceneManager.GetActiveScene().name == "SE_LoginPage") bNetworkIssue = false;
    //     StartCoroutine(NetworkIssueCheck());//bNetworkIssue在必要的时候需要手动设为false，比如重新连接后
    //     yield break;
    // }

    [HideInInspector]
    public UserDocument UserAccount=new UserDocument();//该变量表示当前用户数据，这个类的结构和数据库的结构是对应的
    /// <summary>
    /// UserData 相关部分
    /// </summary>
    /// <param name="Num"></param>
    /// <param name="UseQQNum"></param>
    public bool UserData_Download(string Num, bool UseQQNum = false)//更新本地用户数据
    {   //TODO删除注释
        if (!bServerConnection)
        {
            print("网络已断开，无法下载账号数据。");
            return false;
        }
        IFindFluent<BsonDocument, BsonDocument> userdata;
        if(UseQQNum) userdata = collection.Find(Builders<BsonDocument>.Filter.Eq("sQQNum", Num));//根据QQ号查询，若一个号码有多个用户结果，会取最后面的用户
        else userdata = collection.Find(Builders<BsonDocument>.Filter.Eq("sPhoneNum", Num));//根据手机号查询
        foreach (var DOC in userdata.ToEnumerable())//将查询到的DOCUMENT反序列化成自己定义的UserAccount类
        {
            UserAccount = BsonSerializer.Deserialize<UserDocument>(DOC);//只会有一个用户
        }
        UserData_UpdateLoginTime();
        return true;
    }
    public bool UserData_Upload(string Num, bool UseQQNum = false)//更新MongoDB中的用户数据
    {   //TODO删除注释
        // if(!bServerConnection)
        // {
        //     print("网络已断开，无法完成任务。");
        //     return false;
        // }

        UpdateDefinition<BsonDocument> update;//声明Update定义
        string NumType;
        if (UseQQNum) NumType = "sQQNum";
        else NumType = "sPhoneNum";

        // update = Builders<BsonDocument>.Update.Set("sName", UserAccount.sName);
        // collection.UpdateOne(Builders<BsonDocument>.Filter.Eq(NumType, Num), update);

        // update = Builders<BsonDocument>.Update.Set("sQQNum", UserAccount.sQQNum);
        // collection.UpdateOne(Builders<BsonDocument>.Filter.Eq(NumType, Num), update);

        // update = Builders<BsonDocument>.Update.Set("sPhoneNum", UserAccount.sPhoneNum);
        // collection.UpdateOne(Builders<BsonDocument>.Filter.Eq(NumType, Num), update);

        // update = Builders<BsonDocument>.Update.Set("sPassword", UserAccount.sPassword);
        // collection.UpdateOne(Builders<BsonDocument>.Filter.Eq(NumType, Num), update);

        update = Builders<BsonDocument>.Update.Set("ClassData", UserAccount.ClassData);
        collection.UpdateOne(Builders<BsonDocument>.Filter.Eq(NumType, Num), update);
        return true;
    }
    public bool UserData_Create(string Name, string QQ, string Phone, string Password)//新建MongoDB Document，即新建账号
    {
        if(!bServerConnection)
        {
            print("网络已断开，无法创建账号。");
            return false;
        }

        BsonDocument newDocument = BsonDocument.Parse(GetJsonFileText("UserSetting"));//直接把Json解析成Bson
        UserDocument Cache = BsonSerializer.Deserialize<UserDocument>(newDocument);//将Bson反序列化成对象
        Cache.sName = Name;
        Cache.sQQNum = QQ;
        Cache.sPhoneNum = Phone;
        Cache.sPassword = Password;
        Cache.sAccountVersion = PlayerPrefs.GetInt("version",-1);
        Cache._id = new ObjectId(Random24DigitString());
        Cache.SignDay[0] = DateTime.Now.Year;
        Cache.SignDay[1] = DateTime.Now.Month;
        Cache.SignDay[2] = DateTime.Now.Day;
        Cache.ClassData[0].SMUnlock.Add(true);
        Cache.ClassData[0].Score.Add(0);
        Cache.ClassData[0].Level.Add(new Level_());
        Cache.ClassData[0].Level[0].Score.Add(0);
        Cache.ClassData[0].Level[0].LUnlock.Add(true);
        string CacheJson = Newtonsoft.Json.JsonConvert.SerializeObject(Cache);
        newDocument = BsonDocument.Parse(CacheJson);
        collection.InsertOne(newDocument);//将Documet插入Collection
        return true;

        // var update = Builders<BsonDocument>.Update.Set("sPhoneNum", Phone);
        // collection.UpdateOne(Builders<BsonDocument>.Filter.Eq("sPhoneNum", Phone), update);

        // UserData_Upload(Phone);
        // BsonDocument NEWDocument = new BsonDocument     //https://docs.mongodb.com/manual/tutorial/query-arrays/
        // {
        //         { "sName",Name},
        //         { "sQQNum",QQ},
        //         {"sPhoneNum",Phone},
        //         {"sPassword",Password},                             //第一关是要解锁的，所以bIsComplete1默认值是true
        //         { "ClassData", new BsonArray { new BsonDocument { { "bIsComplete1", true }, { "bIsComplete2", false }, { "bIsComplete3", false }, { "iLegacy1", -1 }, { "iLegacy2", -1 }, { "iLegacy3", -1 }, { "iLegacy4", -1 }, { "iLegacy5", -1 } }, new BsonDocument { { "bIsComplete1", false }, { "bIsComplete2", false }, { "bIsComplete3", false }, { "iLegacy1", -1 }, { "iLegacy2", -1 }, { "iLegacy3", -1 }, { "iLegacy4", -1 }, { "iLegacy5", -1 } },new BsonDocument { { "bIsComplete1", false }, { "bIsComplete2", false }, { "bIsComplete3", false }, { "iLegacy1", -1 }, { "iLegacy2", -1 }, { "iLegacy3", -1 }, { "iLegacy4", -1 }, { "iLegacy5", -1 } } } },
        // };
        //collection.InsertOne(NEWDocument);//把上面的Document插入到Collection里
    }

    
    /// <summary>
    /// 在数据库中添加新的车厢数据
    /// 用法：参数为Assets/CommonAsset/Json下的Json文件名，不需要带后缀名；T为JSON映射的类。
    /// 举例：JsonFileConvertToObject<UserDocument>("UserSetting")  将UserSetting.json映射至UserDocument类。
    /// </summary>
    // public bool UserData_InsertNewTrain(string Num, bool UseQQNum = false)
    // {
    //     if(!bServerConnection)
    //     {
    //         print("网络已断开，无法完成任务。");
    //         return false;
    //     

    /// <summary>
    /// 更新账号登录时间
    /// 用法：
    /// 举例：
    /// </summary>
    public bool UserData_UpdateLoginTime()
    {
        if(!bServerConnection)
        {
            print("网络已断开，无法刷新登录时间。");
            return false;
        }

        UserAccount.LoginDay[0] = DateTime.Now.Year;
        UserAccount.LoginDay[1] = DateTime.Now.Month;
        UserAccount.LoginDay[2] = DateTime.Now.Day;
        UserData_Upload(UserAccount.sPhoneNum);
        return true;
    }

    /// <summary>
    /// 将JSON内容映射到对象
    /// 用法：参数为Assets/CommonAsset/Json下的Json文件名，不需要带后缀名；T为JSON映射的类。
    /// 举例：JsonFileConvertToObject<UserDocument>("UserSetting")  将UserSetting.json映射至UserDocument类。
    /// </summary>
    public T JsonFileConvertToObject<T>(string JsonFileName = "UserSetting")
    {
        StreamReader STREAMREADER = new StreamReader(Application.dataPath + "/CommonAsset/Json/" + JsonFileName + ".json");//读取数据，转换成数据流
        JsonReader JSR = new JsonReader(STREAMREADER);//将数据流转换成JS数据
        return JsonMapper.ToObject<T>(JSR);//将JsonReader转换成Object
    }

    /// <summary>
    /// 将类对象的内容转换为Json字符串，理论上任何形式的对象都可以转换成Json内容
    /// 用法：
    /// 举例：
    /// </summary>
    public string ObjectConvertToJSONString(object obj)
    {
        //return JsonMapper.ToJson(obj);这个不支持MongoDB的ObjectId类，不能使用
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
    /// 用法：
    /// 举例：
    /// </summary>
    public string GetJsonFileText(string JsonFileName = "UserSetting")
    {
        StreamReader SR = File.OpenText(Application.dataPath + "/CommonAsset/Json/" + JsonFileName + ".json");
        return SR.ReadToEnd();
    }

    /// <summary>
    /// 生成随机24位字符
    /// 用法：
    /// 举例：
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

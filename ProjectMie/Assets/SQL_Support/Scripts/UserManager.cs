using System.Threading;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//!可更新
/* 读取用户数据时使用的枚举，它和创建用户的枚举是分开的，因为有一些变量是数据库自动给赋值，不需要这边赋值的 */
/* 至于有哪些枚举是被自动赋值的，是你数据库里设置的 */
public enum UserInfoField
{/* 枚举名          枚举在SQL那边的字段ID */
    id,             //1
    uuid,           //2
    username,       //3
    password,       //4
    nickname,       //5
    avatar_url,     //6
    YDDH,           //7
    XXDM,           //8
    status,         //9
    lastlogin_time, //10
    create_time,    //11
    update_time,    //12
    delete_time,    //13
    subscribe,      //14
    temp,           //15
    neuinfo,        //16
    END             //遍历枚举时要用到这个，算是一个小聪明做法吧。
}

//!可更新
/* 创建用户时使用的枚举 */
public enum UserInfoField_Create
{/* 枚举名          枚举在SQL那边的字段ID */
    uuid,           //2
    username,       //3
    password,       //4
    nickname,       //5
    subscribe,      //14
    END
}

/* 登录状态枚举，用以方便输出登录结果 */
public enum LoginResult
{
    Pass,   //登录成功
    Wrong,  //密码或账号错误或不存在
    Banned, //已封禁
}

public class UserManager : SQLManager
{
    public delegate void D_NoReturnOneStringParam(string PARAM);
    public delegate void D_NoReturnNoParam();
    public static event D_NoReturnOneBoolParam Event_OnUserLogin;   // 参数是是否登录成功
    public static event D_NoReturnOneBoolParam Event_OnUserModify;  // 参数是是否修改成功
    public static event D_NoReturnOneBoolParam Event_OnUserCreate;  // 参数是是否注册成功


    /* 本地存储的用户数据，你可以通过GetUserInfo函数访问它，它会在Login函数中被更新 */
    private static List<StructMap<UserInfoField, string>> UserData = new List<StructMap<UserInfoField, string>>();

    /* 它是用来获取全部用户数据枚举的string数组 */
    /* 用于作为用户搜索条件，代替手动输入全部枚举 */
    protected static UserInfoField[] UserInfoField_FullEnum
    {
        get
        {
            var FE = new UserInfoField[(int)UserInfoField.END];
            for(int i=0;i<(int)UserInfoField.END;i++) FE[i] = (UserInfoField)Enum.Parse(typeof(UserInfoField), Enum.GetName(typeof(UserInfoField), (object)i));
            return FE;
        }
    }
    
    /* 它是用来获取已登录用户的WHERE，便于快速填写需要已登录用户的情景。 */
    protected static string WhereLogin
    {
        get
        {
            return "`username`='"+GetUserInfo(UserInfoField.username)+"'";
        }
    }

    /* 一个 键-值 对应结构，用于更直观地通过枚举获取想要的数据。具体效果可以看 GetUserInfo 函数的参数，很方便吧。 */
    public struct StructMap<K, V>
    {
        public StructMap(K KEY, V VALUE)
        {
            Key = KEY;
            Value = VALUE;
        }

        public K Key;
        public V Value;
    }

    /* 用户的 Subscribe JSON对象 */
    [Serializable]
    public class SubscribeClass
    {
        public List<SubClass> Class;

        [Serializable]
        public class SubClass
        {
            public string ID;
            public bool Status;
        }
    }



    /// <summary>
    /// 创建用户。
    /// 该函数是 InsertInto 的封装版本，用于快速操作数据库。
    /// </summary>
    /// <param name="username">登录账号</param>
    /// <param name="password">密码</param>
    /// <param name="nickname">用户名称</param>
    public static bool CreateUser(string username, string nickname, string password)
    {
        if(FindSameUser(UserInfoField.username, username) != 0)
        {
            Debug.LogWarning("username已被注册，请换一个！");
            Event_OnUserCreate?.Invoke(false);
            return false;
        }

        string[] fieldCache = new string[(int)UserInfoField_Create.END];
        for (int i = 0; i < fieldCache.Length; i++) fieldCache[i] = Enum.GetName(typeof(UserInfoField_Create), i);

        SQLD_InsertInto("userinfo", CombineField(fieldCache, false), CombineField(new string[]
        {   /* 创建用户时设置的一些默认值 */
            WTool.GetUidWithField(),    //uuid
            username,
            password,
            nickname,
            //"{\"Class\": [{\"ID\": \"category01_class01\", \"Status\": false}]}", /* 注册账号成功后，设置用户的第一个关卡为true */
            "JSON_OBJECT()",
        }, false));

        bool result = FindSameUser(UserInfoField.username, username) >= 1;

        if(result) SQLJ_Insert("userinfo","subscribe",new string[]{"Class"},new string[]{"JSON_ARRAY(0,true)"}, WhereLogin);

        Event_OnUserCreate?.Invoke(result);
        return result;
    }

    /// <summary>
    /// 获取用户数据库信息
    /// 根据参数所选择的寻找类型和返回类型来返回用户的某一字段的值。
    /// </summary>
    /// <param name="Field">要寻找的类型。</param>
    /// <param name="Value">寻找类型的目标值。</param>
    /// <param name="ReturnField">返回的类型。</param>
    /// <returns>找到的值，若找不到则为NULL。</returns>
    public static string DownloadUserInfo(UserInfoField Field, string Value, UserInfoField ReturnField)
    {
        return SQLD_SelectFrom("userinfo", new string[] { ReturnField.ToString() }, "`" + Field.ToString() + "`='" + Value + "'")[0];
    }
    public static List<string> DownloadUserInfo(UserInfoField Field, string Value, UserInfoField[] ReturnField)
    {
        string[] cache = new string[ReturnField.Length];
        for(int i = 0; i < ReturnField.Length; i++) cache[i] = ReturnField[i].ToString();
        return SQLD_SelectFrom("userinfo", cache, "`" + Field.ToString() + "`='" + Value + "'");
    }

    /// <summary>
    /// 获取指定条件下的用户数量。
    /// 如密码都是AAA或都同名的用户数量。
    /// </summary>
    /// <param name="Field"></param>
    /// <param name="Value"></param>
    /// <returns></returns>
    public static int FindSameUser(UserInfoField Field, string Value)
    {
        return SQLD_SelectCount("userinfo", "1", "`" + Field + "`='" + Value + "'");
    }

    /// <summary>
    /// 删除用户。
    /// 根据参数寻找要删除的用户。
    /// </summary>
    /// <param name="Field">要寻找的类型。</param>
    /// <param name="Value">寻找类型的目标值。</param>
    /// <returns>是否删除成功</returns>
    public static bool DeleteUser(UserInfoField Field, string Value)
    {
        return SQLD_Delete("userinfo", "`" + Field.ToString() + "` = '" + Value + "'");
    }

    /// <summary>
    /// 修改用户数据库信息。
    /// 函数会根据 SearchField 和 SearchValue 找人，
    /// 一旦找到后，就会根据 Field 和 Value 修改人的信息。
    /// </summary>
    /// <param name="SearchField">搜索Field，如"nickname"。</param>
    /// <param name="SearchValue">搜索Value，如【小明】。</param>
    /// <param name="Field">要修改的Field。</param>
    /// <param name="Value">修改成的值。，如【12345】</param>
    /// <returns>是否成功修改。</returns>
    public static bool ModifyUserInfo(UserInfoField SearchField, string SearchValue, UserInfoField Field, string Value)
    {
        bool result = true;

        try
        {
            SQLD_Update("userinfo", Field.ToString(), Value, "`" + SearchField.ToString() + "` = '" + SearchValue + "'");
        }
        catch
        {
            result = false;
        }

        Event_OnUserModify?.Invoke(result);
        return result;
    }
    public static bool ModifyUserInfo(UserInfoField SearchField, string SearchValue, UserInfoField Field, SQLTimeObject TimeObject)
    {
        return ModifyUserInfo(SearchField, SearchValue, Field, TimeObject.Read());
    }
    protected static bool UpdateLoginTime()
    {
        if(isLogin == false) return false;
        return ModifyUserInfo(UserInfoField.username, GetUserInfo(UserInfoField.username), UserInfoField.lastlogin_time, new SQLTimeObject().Read());
    }    
    protected static bool UpdateUpdateTime()
    {
        if(isLogin == false) return false;
        return ModifyUserInfo(UserInfoField.username, GetUserInfo(UserInfoField.username), UserInfoField.update_time, new SQLTimeObject().Read());
    }

    /// <summary>
    /// 登录。
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password">如【'PASSWORD'】</param>
    /// <returns></returns>
    public static LoginResult Login(string username, string password)
    {
        /* 判断账号密码错误或不存在 */
        if (SQLD_SelectCount("userinfo", "1", string.Format("`username`='{0}' AND `password`='{1}'", username, password)) != 1)
        {
            Event_OnUserLogin?.Invoke(false);
            return LoginResult.Wrong;
        }

        /* 登出 */
        Logout();

        /* 从 MySQL 接收用户数据 */
        List<string> DownloadedInfo = DownloadUserInfo(UserInfoField.username, username, UserInfoField_FullEnum);

        /* 反序列化用户数据到本地对象 */
        int index = 0;
        foreach (string E in Enum.GetNames(typeof(UserInfoField)))
        {
            if(index == DownloadedInfo.Count) break;
            UserData.Add(new StructMap<UserInfoField, string>((UserInfoField)Enum.Parse(typeof(UserInfoField), E), DownloadedInfo[index].ToString()));
            index++;
        }

        if(GetUserInfo(UserInfoField.status) == "0")
        {
            UserData.Clear();
            return LoginResult.Banned;
        }

        /* 如果subscribe为空，就初始化它使其成为JSON字段。 */
        if(GetUserInfo(UserInfoField.subscribe) == "") SQLJ_Initial("userinfo","subscribe", WhereLogin);

        UpdateLoginTime();
        Event_OnUserLogin?.Invoke(true);

        return LoginResult.Pass;
    }

    /// <summary>
    /// 登出账号。
    /// </summary>
    public static void Logout()
    {
        Event_OnUserLogin?.Invoke(false);
        UserData.Clear();
    }

    /* 判断是否登录 */
    public static bool isLogin { get { return UserData.Count != 0; } }

    /// <summary>
    /// 获取用户本地数据。
    /// </summary>
    /// <param name="Type">信息类型</param>
    /// <returns>用户数据</returns>
    public static string GetUserInfo(UserInfoField Type)
    {
        if(isLogin == false) return "NULL";
        if((int)Type >= UserData.Count) return "ERROR";
        return UserData[(int)Type].Value;
    }

    /// <summary>
    /// 设置用户本地数据。
    /// </summary>
    /// <param name="Type"></param>
    /// <param name="Value"></param>
    public static void SetUserInfo(UserInfoField Type, string Value)
    {
        if(isLogin == false) return;
        if((int)Type >= UserData.Count) return;
        UserData[(int)Type] = new StructMap<UserInfoField, string>(Type, Value);
    }

    /// <summary>
    /// 用本地账号的数据去替换在线账号的数据。
    /// </summary>
    public static void UploadUserInfo()
    {
        if(isLogin == false) return;
        foreach(var S in UserInfoField_FullEnum)
        {
            ModifyUserInfo(UserInfoField.username, GetUserInfo(UserInfoField.username), S, UserData[(int)S].Value);
        }
    }
    
    /// <summary>
    /// 获取反序列化后的用户Subscribe Json。
    /// 该方法会检测并修复MySQL的Json字段，会初始化Json字段为适合该项目的样子，比如创建Class数组。
    /// </summary>
    /// <returns></returns>
    protected static SubscribeClass GetUserSubscribeObject()
    {
        if(isLogin == false)
        {
            Debug.LogError("获取用户 Subscribe 失败，原因 未登录。");
            return null;
        }

        /* 用于判断是否需要更新本地的subscribe */
        bool needUpdate = false;

        /* 如果subscribe为NULL，就初始化它使其成为JSON字段。 */
        if(GetUserInfo(UserInfoField.subscribe) == "")
        {
            Debug.LogWarning("获取用户 Subscribe 错误，原因 该用户的subscribe字段为null。已为其初始化。");
            SQLJ_Initial("userinfo","subscribe", WhereLogin);
            needUpdate = true;
        }

        //print(GetUserInfo(UserInfoField.subscribe));
        SubscribeClass Obj = JsonUtility.FromJson<SubscribeClass>(GetUserInfo(UserInfoField.subscribe));
        
        /* 若subscribe里啥也没有，会实例化不出来东西 */
        if(Obj == null)
        {
            Debug.LogWarning("获取用户 Subscribe 有危险，原因 该Subscirbe内不存在任何元素，应当是全新账号。已为该用户创建JSON对象并加入了Class对象。");
            Obj = new SubscribeClass();
            Obj.Class = new List<SubscribeClass.SubClass>();
            SQLJ_Insert("userinfo","subscribe",new string[]{"Class"},new string[]{"CAST('[]' AS JSON)"},WhereLogin);
            needUpdate = true;
        }

        /* unity的反序列化JSON似乎会将指定class下的所有变量都实例化，所以不能判null，只能判数量。。无解 */
        if(Obj.Class.Count == 0)
        {
            //Debug.LogWarning("获取用户 Subscribe 有危险，原因 该Subscirbe内不存在Class，应当是错误账号。已为该用户加入Class对象。");
            Obj.Class = new List<SubscribeClass.SubClass>();
            SQLJ_Insert("userinfo","subscribe",new string[]{"Class"},new string[]{"CAST('[]' AS JSON)"},WhereLogin,true);
            needUpdate = true;
        }

        /* 从SQL重新接收subscribe以更新本地subscribe */
        if(needUpdate)
        {
            SetUserInfo(UserInfoField.subscribe, SQLD_SelectFrom("userinfo", "subscribe", WhereLogin)[0]);
        }

        return Obj;
    }

    /// <summary>
    /// 为Class数组增加元素至指定数量。
    /// 仅在输入数量大于当前数量时才会进行添加元素操作。
    /// 一般用于维护Class数组的访问安全。
    /// 默认值(-1, false)。
    /// </summary>
    /// <param name="Count"></param>
    protected static void FixSubscribeClassElement(int index)
    {
        /* 检测登录 */
        if(isLogin == false)
        {
            Debug.LogError("获取用户课程状态失败，原因 未登录。");
            return;
        }

        if(GetUserSubscribeObject().Class.Count <= index)
        {
            /* 缺几个就补几个，这个是补的在线数据 */
            for(int i=0;i<=index-GetUserSubscribeObject().Class.Count;i++)
                    SQLJ_ArrayAppend("userinfo","subscribe", new string[]{"Class"},new string[]{"JSON_OBJECT('ID','-1','Status',false)"}, WhereLogin);
            
            /* 补完后更新一下本地subscribe数据 */
            SetUserInfo(UserInfoField.subscribe, SQLD_SelectFrom("userinfo", "subscribe", WhereLogin)[0]);
        }
    }

    /// <summary>
    /// 获取用户的课程解锁状态。
    /// </summary>
    /// <param name="ClassIndex">数组下标</param>
    /// <returns>指定下标是否解锁</returns>
    public static bool GetUserSubscribeClassBool(int ClassIndex)
    {
        /* 检测登录 */
        if(isLogin == false)
        {
            Debug.LogError("获取用户课程状态失败，原因 未登录。");
            return false;
        }

        /* 看看要访问的元素在不在，如果不在就补上去，防止访问出现错误 */
        FixSubscribeClassElement(ClassIndex);

        return GetUserSubscribeObject().Class[ClassIndex].Status;
    }

    /// <summary>
    /// 获取用户的课程解锁状态。
    /// </summary>
    /// <param name="ID">ID名称</param>
    /// <returns>指定下标是否解锁</returns>
    public static bool GetUserSubscribeClassBool(string ID)
    {
        /* 检测登录 */
        if(isLogin == false)
        {
            Debug.LogError("获取用户课程状态失败，原因 未登录。");
            return false;
        }

        SubscribeClass.SubClass result;

        result = GetUserSubscribeObject().Class.Find( (SubscribeClass.SubClass C) => {return C.ID == ID;} );
        
        /* 如果要找的没有就自动创建一个 */
        if(result == null)
        {
            Debug.LogWarning("获取用户课程状态失败，原因 ID不存在。已为该ID创建元素，Status默认值为false。");
            CreateUserSubscribeClassElement(ID, false);
            result = GetUserSubscribeObject().Class.Find( (SubscribeClass.SubClass C) => {return C.ID == ID;} );
        }

        return WTool.Selector<bool>(result != null, result.Status, false);
    }
    /// <summary>
    /// 设置用户的课程解锁状态。
    /// </summary>
    /// <param name="ClassIndex">下标</param>
    /// <param name="Status"></param>
    public static void SetUserSubscribeClassBool(int ClassIndex, bool Status)
    {
        /* 检测登录 */
        if(isLogin == false)
        {
            Debug.LogError("设置用户课程状态失败，原因 未登录。");
            return;
        }

        /* 看看要访问的元素在不在，如果不在就补上去，防止访问出现错误 */
        FixSubscribeClassElement(ClassIndex);
        
        SQLJ_Insert("userinfo", "subscribe", new string[]{"Class["+ClassIndex+"]"}, new string[]{"JSON_OBJECT('ID','"+ClassIndex+"','Status',"+Status.ToString()+")"},WhereLogin);
        SetUserInfo(UserInfoField.subscribe, SQLD_SelectFrom("userinfo", "subscribe", WhereLogin)[0]);
    }
    
    /// <summary>
    /// 设置用户的课程解锁状态。
    /// </summary>
    /// <param name="ID"></param>
    /// <param name="Status"></param>
    public static void SetUserSubscribeClassBool(string ID, bool Status)
    {
        /* 检测登录 */
        if(isLogin == false)
        {
            Debug.LogError("设置用户课程状态失败，原因 未登录。");
            return;
        }

        int result = GetUserSubscribeObject().Class.FindIndex( (SubscribeClass.SubClass C) => {return C.ID == ID;} );
        
        /* 如果要找的没有就自动创建一个 */
        if(result == -1)
        {
            Debug.LogWarning("获取用户课程状态失败，原因 ID不存在。已为该ID创建元素。");
            CreateUserSubscribeClassElement(ID, Status);
            result = GetUserSubscribeObject().Class.FindIndex( (SubscribeClass.SubClass C) => {return C.ID == ID;} );
        }

        SQLJ_Insert("userinfo", "subscribe", new string[]{"Class["+result+"]"}, new string[]{"JSON_OBJECT('ID','"+ID+"','Status',"+Status.ToString()+")"},WhereLogin);
        SetUserInfo(UserInfoField.subscribe, SQLD_SelectFrom("userinfo", "subscribe", WhereLogin)[0]);
    }

    /// <summary>
    /// 修改用户的课程ID。
    /// </summary>
    /// <param name="ID"></param>
    /// <param name="newID">要修改成的ID</param>
    public static void SetUserSubscribeClassID(string ID, string newID)
    {
        /* 检测登录 */
        if(isLogin == false)
        {
            Debug.LogError("设置用户课程状态失败，原因 未登录。");
            return;
        }

        int result = GetUserSubscribeObject().Class.FindIndex( (SubscribeClass.SubClass C) => {return C.ID == ID;} );
   
        if(result == -1)
        {
            Debug.LogError("设置用户课程状态失败，原因 找不到课程。已自动创建newID课程。");
            CreateUserSubscribeClassElement(newID, false);
            return;
        }

        SQLJ_Insert("userinfo", "subscribe", new string[]{"Class["+result+"]"}, new string[]{"JSON_OBJECT('ID','"+newID+"','Status',"+GetUserSubscribeClassBool(result)+")"},WhereLogin);
        SetUserInfo(UserInfoField.subscribe, SQLD_SelectFrom("userinfo", "subscribe", WhereLogin)[0]);
    }

    /// <summary>
    /// 创建用户课程。
    /// </summary>
    /// <param name="ID"></param>
    /// <param name="Status"></param>
    /// <returns>新建对象在Class数组的index</returns>
    protected static int CreateUserSubscribeClassElement(string ID, bool Status)
    {
        /* 检测登录 */
        if(isLogin == false)
        {
            Debug.LogError("设置用户课程状态失败，原因 未登录。");
            return -1;
        }

        /* 检测一下有没有必要的Class数组，它可以解决这个问题。 */
        GetUserSubscribeObject();

        SQLJ_ArrayAppend("userinfo","subscribe",new string[]{"Class"}, new string[]{"JSON_OBJECT('ID','"+ID+"','Status',"+Status.ToString()+")"}, WhereLogin);
        SetUserInfo(UserInfoField.subscribe, SQLD_SelectFrom("userinfo", "subscribe", WhereLogin)[0]);

        return GetUserSubscribeObject().Class.FindIndex( (SubscribeClass.SubClass C) => {return C.ID == ID;} );
    }
}
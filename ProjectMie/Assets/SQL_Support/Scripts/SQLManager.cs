using System.Threading;
using System.Linq;
using System.Data;
using System.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MySqlConnector;

/* 用于折腾时间格式的class */
/* 不用该class，直接手敲string输入也是可以的，但你怎么知道你输入的格式就一定正确？ */
/* 调用该类的 Read() 函数即可拿到正确格式的string。 */
public class SQLTimeObject
{
    string timespan;

    /* 无参构造即获取当前时间 */
    public SQLTimeObject()
    {
        timespan = DateTime.Now.Year.ToString() + "/" + DateTime.Now.Month.ToString() + "/" + DateTime.Now.Day.ToString();
        timespan += " ";
        timespan += DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Minute.ToString() + ":" + DateTime.Now.Second.ToString();
    }

    /* Date格式为 年/月/日，Time格式为 时:分:秒 */
    public SQLTimeObject(string Date, string Time)
    {   // 2022/8/29 0:37:16
        timespan = Date + " " + Time;
    }

    public string Read()
    {
        return timespan;
    }
}

public class SQLManager : MonoBehaviour
{
    public static string ServerAddress = "bj-cynosdbmysql-grp-4xukozna.sql.tencentcdb.com";
    public static string ServerPort = "22544";
    public static string ServerDB = "advancedschedule";
    public static string ServerUser = "user";
    public static string ServerUserPassword = "password";

    private static MySqlConnection SQL_ConnectionObject;
    public static bool SQL_HasConnection { get { return hasConnection; } }
    private static bool hasConnection = false;
    public static bool SQL_DontPing = false;

    public delegate void D_NoReturnOneBoolParam(bool BOOL);
    public static event D_NoReturnOneBoolParam Event_OnSQLStatueChanged;

    /// <summary>
    /// 连接至服务器。
    /// </summary>
    /// <returns>是否连接成功</returns>
    public static bool SQL_Open()
    {
        if (hasConnection)
        {
            Debug.Log("SQL已连接，请不要重复连接。");
            return hasConnection;
        }

        /* 初始化SQL连接Object */
        SQL_ConnectionObject = new MySqlConnection(@"Server=" + ServerAddress + "; User=" + ServerUser + "; Database=" + ServerDB + "; Port=" + ServerPort + "; Password=" + ServerUserPassword + "; SSL Mode=None");

        try
        {
            //print("正在建立 SQL 连接 ...");
            SQL_ConnectionObject.Open();
        }
        catch (Exception ex)
        {
            Debug.LogError("SQL 无法连接：" + ex.Message);
            hasConnection = false;
        }

        hasConnection = true;
        Event_OnSQLStatueChanged?.Invoke(hasConnection);
        return SQL_HasConnection;
    }

    /// <summary>
    /// 断开与数据库的连接，
    /// 该操作会影响 hasConnection 变量。
    /// </summary>
    public static void SQL_Close()
    {
        Debug.LogWarning("SQL连接被关闭。");
        hasConnection = false;
        SQL_ConnectionObject.Close();
        Event_OnSQLStatueChanged?.Invoke(hasConnection);
    }

    /// <summary>
    /// 析构函数
    /// </summary>
    void OnDestroy()
    {
        if (hasConnection == true)
            SQL_Close();
    }

    /// <summary>
    /// MySQL 的 SLECT 指令。
    /// 若什么都找不到，则会返回NULL字符串。
    /// </summary>
    /// <param name="Table">数据表名，如直接输入【"UserInfo"】。</param>
    /// <param name="Field">多个字段名，如直接输入【"name","age","password"】。</param>
    /// <param name="Where">寻找条件，可留空。如直接输入【"`name`='XiaoMing'"】</param>
    /// <returns>查获的数据列表</returns>
    public static List<string> SQLD_SelectFrom(string Table, string[] Field, string Where = "")
    {
        MySqlDataReader DATA;
        List<string> result = new List<string>();
        string sql_string = @"SELECT " + CombineField(Field, true, "`") + " FROM " + Table + WTool.Selector<string>(Where == "", "", " WHERE " + Where);

        try
        {
            DATA = SQLD_Command_Query(sql_string);
        }
        catch (Exception ex)
        {
            Debug.LogWarning("SELECT错误：" + ex.Message);
            result.Add("NULL");
            return result;
        }

        /* 阅读数据并判断是否能读 */
        if(DATA.Read() == false)
        {
            print("找不到任何数据！");
            DATA.Close();
            return result;
        }


        /* 如果能读，就往外折腾数据 */
        int index = 0;
        while (index < Field.Count())   //不建议用 Read() 作为while参数，因为Read一次之后，Read()就变false停止循环了。
        {
            result.Add(DATA[index].ToString());
            index++;
        }
        DATA.Close();
        return result;
    }

    /// <summary>
    /// MySQL 的 SLECT 指令。
    /// 若什么都找不到，则会返回NULL字符串。
    /// </summary>
    /// <param name="Table">数据表名，如直接输入【"UserInfo"】。</param>
    /// <param name="Field">多个字段名，如直接输入【"name","age","password"】。</param>
    /// <param name="Where">寻找条件，可留空。如直接输入【"`name`='XiaoMing'"】</param>
    /// <returns>查获的数据列表</returns>
    public static List<string> SQLD_SelectFrom(string Table, string Field, string Where = "")
    {
        return SQLD_SelectFrom(Table, new string[] { Field }, Where);
    }

    /// <summary>
    /// 使用 SELECT 指令并打印结果。
    /// </summary>
    /// <param name="Table">数据表名，如直接输入【"UserInfo"】。</param>
    /// <param name="Field">多个字段名，如直接输入【"name","age","password"】。</param>
    /// <param name="Where">寻找条件，可留空。如直接输入【"`name`='XiaoMing'"】</param>
    /// <returns>查获的数据列表</returns>
    public static List<string> SQLD_SelectFromPrint(string Table, string[] Field, string Where = "")
    {
        List<string> result = SQLD_SelectFrom(Table, Field, Where);
        foreach (string S in result) print(S);
        return result;
    }

    /// <summary>
    /// 使用 SELECT 指令并打印结果。
    /// </summary>
    /// <param name="Table">数据表名，如直接输入【"UserInfo"】。</param>
    /// <param name="Field">多个字段名，如直接输入【"name","age","password"】。</param>
    /// <param name="Where">寻找条件，可留空。如直接输入【"`name`='XiaoMing'"】</param>
    /// <returns>查获的数据列表</returns>
    public static List<string> SQLD_SelectFromPrint(string Table, string Field, string Where = "")
    {
        return SQLD_SelectFromPrint(Table, new string[] { Field }, Where);
    }

    /// <summary>
    /// MySQL 中的 Exists。
    /// 可以让两个表中的变量互相比对，最后返回比对成功的 TableA 行。
    /// 由于不经常使用，所以没有经过测试。
    /// </summary>
    /// <param name="TableA">A表。</param>
    /// <param name="TableB">B表。</param>
    /// <param name="FieldA">A表中的字段。</param>
    /// <param name="FieldB">B表中的字段。</param>
    /// <returns>A表中满足条件的行。</returns>
    [Obsolete]
    public static List<string> SQLD_SelectExists(string TableA, string TableB, string FieldA, string FieldB)
    {
        MySqlDataReader DATA;
        List<string> result = new List<string>();
        string sql_string = @"select * from " + TableA + " where exists( select * from " + TableB + " where " + TableA + "." + FieldA + " = " + TableB + "." + FieldB + ")";
        print(sql_string);
        try
        {
            DATA = SQLD_Command_Query(sql_string);
        }
        catch (Exception ex)
        {
            Debug.LogWarning("SELECT错误：" + ex.Message);
            result.Add("NULL");
            return result;
        }

        while (DATA.Read())
        {
            result.Add(DATA[0].ToString());
        }

        return result;
    }

    /// <summary>
    /// MySQL 的 INSERT INTO 指令。
    /// </summary>
    /// <param name="Table">数据表名，如直接输入【"UserInfo"】。</param>
    /// <param name="Field">多个字段名，如直接输入【"name","age","password"】。</param>
    /// <param name="Value">多个值，如直接输入【XiaoMing, 12, 123456】。</param>
    public static void SQLD_InsertInto(string Table, string[] Field, string[] Value)
    {
        string sql_string = @"INSERT INTO `" + Table + "` (" + CombineField(Field, false) + ") VALUES (" + CombineField(Value, true, "'") + ")";
        
        try
        {
            SQLD_Command_Modify(sql_string);
        }
        catch (Exception ex)
        {
            string result = ex.Message;
            if (ex.Message.Contains("Duplicate")) result = "数据重复。";
            if (ex.Message == "Connection property must be non-null.") result = "未连接服务器";
            Debug.LogError("INSERT 错误：" + result);
        }
    }

    /// <summary>
    /// MySQL 的 INSERT INTO 指令。
    /// 单句分解版，可根据逗号拆分成段。
    /// </summary>
    /// <param name="Table">数据表名，如直接输入【"UserInfo"】。</param>
    /// <param name="Field">单句多个字段名，如直接输入【"name, age, password"】。</param>
    /// <param name="Value">单句多个值，如直接输入【"XiaoMing, 12, 123456"】。</param>
    public static void SQLD_InsertInto(string Table, string Field, string Value)
    {
        List<string> field_cache = new List<string>();
        List<string> value_cache = new List<string>();
        foreach (string S in Field.Split(',')) field_cache.Add(S);
        foreach (string S in Value.Split(',')) value_cache.Add(S);

        string[] field_array = new string[field_cache.Count];
        string[] value_array = new string[value_cache.Count];

        for (int i = 0; i < field_cache.Count; i++) field_array[i] = field_cache[i];
        for (int i = 0; i < value_cache.Count; i++) value_array[i] = value_cache[i];

        SQLD_InsertInto(Table, field_array, value_array);
    }

    /// <summary>
    /// MySQL 的 Update。
    /// </summary>
    /// <param name="Table">数据表名，如直接输入【"UserInfo"】。</param>
    /// <param name="Field">单个字段名，如直接输入【"name"】。</param>
    /// <param name="Value">单个值，如直接输入【XiaoMing】。</param>
    /// <param name="Where">寻找条件，可留空。如直接输入【"`name`='XiaoMing'"】</param>
    public static void SQLD_Update(string Table, string Field, string Value, string Where = "")
    {
        string sql_string = @"UPDATE " + Table + " SET `" + Field + "` = '" + Value + "' " + WTool.Selector<string>(Where == "", "", "WHERE " + Where);
        
        try
        {
            SQLD_Command_Modify(sql_string);
        }
        catch (Exception ex)
        {
            Debug.LogWarning("UPDATE 错误：" + ex.Message);
        }
    }

    /// <summary>
    /// MySQL 的 Update。
    /// </summary>
    /// <param name="Table">数据表名，如直接输入【"UserInfo"】。</param>
    /// <param name="Field">多个字段名，如直接输入【"name","age","password"】。</param>
    /// <param name="Value">多个字段名，如直接输入【"XiaoMing","12","mima"】。</param>
    /// <param name="Where">寻找条件，可留空。如直接输入【"`name`='XiaoMing'"】</param>
    public static void SQLD_Update(string Table, string[] Field, string[] Value, string Where = "")
    {
        if (Field.Length != Value.Length)
        {
            Debug.LogError("UPDATE 失败：Field 和 Value 数量不相同。");
            return;
        }

        string prefix_sql = @"UPDATE " + Table + " SET ";
        string suffix_sql = WTool.Selector<string>(Where == "", "", "WHERE " + Where);
        string middle = "";
        for (int i = 0; i < Field.Length; i++)
        {
            middle += Field[i] + " = '" + Value[i] + "',";
        }
        middle = middle.Remove(middle.Length - 1, 1);
        string sql_string = prefix_sql + middle + suffix_sql;

        try
        {
            SQLD_Command_Modify(sql_string);
        }
        catch (Exception ex)
        {
            Debug.LogWarning("UPDATE 错误：" + ex.Message);
        }
    }

    /// <summary>
    /// MySQL 的 Delete。
    /// 注意， Where 留空时是会把整个表删了。
    /// </summary>
    /// <param name="Table"></param>
    /// <param name="Where"></param>
    public static bool SQLD_Delete(string Table, string Where = "")
    {
        bool result = false;
        string sql_string = @"DELETE FROM " + Table + WTool.Selector<string>(Where == "", "", " WHERE " + Where);

        if (Where == "")
        {
            Debug.LogError("卧槽，兄弟你差点把整个表的数据都给删了，这东西试试就逝世啊...别以为我自己没试过！");
            return result;
        }

        try
        {
            SQLD_Command_Modify(sql_string);
            result = true;
        }
        catch (Exception ex)
        {
            Debug.LogWarning("DELETE 错误：" + ex.Message);
            result = false;
        }

        return result;
    }

    /// <summary>
    /// SELECT COUNT 语句。
    /// 用于查询表中数据数量。
    /// </summary>
    /// <param name="Table">表名。</param>
    /// <param name="Count">Count参数，为 * 或 1 时返回所有行数量，为 字段名 时也会返回所有行数量但不会返回 字段名 下为null的行。</param>
    /// <param name="Where"></param>
    /// <returns></returns>
    protected static int SQLD_SelectCount(string Table, string Count, string Where = "")
    {
        int result = -1;
        string sql_string = @"SELECT COUNT(" + Count + ") FROM " + Table + WTool.Selector(Where == "", "", " WHERE " + Where);

        try
        {
            result = SQLD_Command_Scalar(sql_string);
        }
        catch (Exception ex)
        {
            print("COUNT 错误：" + ex.Message);
            result = -1;
        }

        return result;
    }

    /// <summary>
    /// 在Json中插入或修改Json对象。
    /// </summary>
    /// <param name="Table"></param>
    /// <param name="Field"></param>
    /// <param name="Key">固定String类型，不可以数字开头或纯数字。</param>
    /// <param name="Value">非固定类型，如输入 \"A\" 表示字符串A，CAST('[1,2,3]' AS JSON)表示数组。</param>
    /// <param name="Value">是否使用INSERT代替SET以提高安全性？SET会替换已有的值，而INSERT不会。</param>
    /// <param name="Where"></param>
    public static void SQLJ_Insert(string Table, string Field, string[] Key, string[] Value, string Where, bool SafeUpdate = false)
    {
        if (Key.Length != Value.Length)
        {
            Debug.LogError("Key与Value数量不等，无法组合。");
            return;
        }

        // if(SQLD_SelectFrom(Table, Field, Where)[0] == "") 
        // {
        //     SQLJ_Initial(Table, Field, Where);
        // }

        string KeyValue = "";
        for (int i = 0; i < Key.Length; i++)
        {
            KeyValue += string.Format("'$.{0}', {1},", Key[i], Value[i]);
        }

        string sql_string = string.Format("UPDATE {0} SET {1} = {4}( {1}, {2} ) {3}", Table, Field, WTool.StringChopInline(ref KeyValue, 1), WTool.Selector(Where=="","","WHERE "+Where), WTool.Selector<string>(SafeUpdate,"JSON_INSERT","JSON_SET"));
        //print(sql_string);
        try
        {
            SQLD_Command_Modify(sql_string);
        }
        catch(Exception ex)
        {
            Debug.LogError("JSON INSERT 错误：" + ex);
        }
    }

    /// <summary>
    /// 在Json中移除键。
    /// </summary>
    /// <param name="Table"></param>
    /// <param name="Field"></param>
    /// <param name="Key">如【class】【class.grade[2][3]】</param>
    /// <param name="Where"></param>
    public static void SQLJ_Remove(string Table, string Field, string[] Key, string Where)
    {
        /* TODO:真的需要判断吗，sql自己会不会处理这个问题？ */
        if(SQLD_SelectFrom(Table, Field, Where)[0] == "") 
        {
            Debug.Log("JSON REMOVE 错误：目标键值不存在。");
            return;
        }

        string KeyValue = "";
        for (int i = 0; i < Key.Length; i++)
        {
            KeyValue += string.Format("'$.{0}',", Key[i]);
        }

        string sql_string = string.Format("UPDATE {0} SET {1} = JSON_REMOVE( {1}, {2} ) {3}", Table, Field, WTool.StringChopInline(ref KeyValue, 1), WTool.Selector(Where=="","","WHERE "+Where));
        //print(sql_string);
        try
        {
            SQLD_Command_Modify(sql_string);
        }
        catch(Exception ex)
        {
            Debug.LogError("JSON REMOVE 错误：" + ex);
        }
    }

    /// <summary>
    /// MySQL的JSON ARRAY APPEND操作。
    /// </summary>
    /// <param name="Table"></param>
    /// <param name="Field"></param>
    /// <param name="Key">如【class】或【class.grade】，留空时表示不在任何对象里。注意，该函数不能自动新建Key对象，若指定Key对象不存在，则JSON不会有任何变化。</param>
    /// <param name="Value">如【1】【CAST('[1,2]' AS JSON)】【\"三班\"】</param>
    /// <param name="Where"></param>
    public static void SQLJ_ArrayAppend(string Table, string Field, string[] Key, string[] Value, string Where)
    {
        if (Key.Length != Value.Length)
        {
            Debug.LogError("Key与Value数量不等，无法组合。");
            return;
        }

        if(SQLD_SelectFrom(Table, Field, Where)[0] == "") 
        {
            Debug.LogError("JSON ARRAY APPEND 错误：函数所指定格为NULL。");
            return;
        }

        string KeyValue = "";
        for (int i = 0; i < Key.Length; i++)
        {
            KeyValue += string.Format("'$.{0}', {1},", Key[i], Value[i]);
        }
        KeyValue = KeyValue.Replace("'$.'","'$'");
        string sql_string = string.Format("UPDATE {0} SET {1} = JSON_ARRAY_APPEND( {1}, {2} ) {3}", Table, Field, WTool.StringChopInline(ref KeyValue, 1), WTool.Selector(Where=="","","WHERE "+Where));
        //print(sql_string);
        try
        {
            SQLD_Command_Modify(sql_string);
        }
        catch(Exception ex)
        {
            Debug.LogError("JSON ARRAY APPEND 错误：" + ex);
        }
    }

    /// <summary>
    /// 将目标字段初始化成Json格式。
    /// Json能够进行操作的前提是目标字段是有“{}”的，这个函数就是使目标字段变成“{}”，以便使其能接受其他的Json操作。
    /// </summary>
    /// <param name="Table"></param>
    /// <param name="Field"></param>
    /// <param name="Where"></param>
    public static void SQLJ_Initial(string Table, string Field, string Where)
    {
        SQLD_Update(Table, Field, "JSON_OBJECT()", Where);
    }
































    private static MySqlDataReader SQLD_Command_Query(string SQL_String)
    {
        CheckFormal(ref SQL_String);
        MySqlCommand SCMD = new MySqlCommand(SQL_String, SQL_ConnectionObject);
        return SCMD.ExecuteReader();
    }

    private static int SQLD_Command_Modify(string SQL_String)
    {
        CheckFormal(ref SQL_String);
        MySqlCommand SCMD = new MySqlCommand(SQL_String, SQL_ConnectionObject);
        return SCMD.ExecuteNonQuery();
    }

    private static int SQLD_Command_Scalar(string SQL_String)
    {
        CheckFormal(ref SQL_String);
        MySqlCommand SCMD = new MySqlCommand(SQL_String, SQL_ConnectionObject);
        return Convert.ToInt32(SCMD.ExecuteScalar());
    }

    /// <summary>
    /// 将参数数组中的所有string都合并成一个string。
    /// </summary>
    /// <param name="args">string数组</param>
    /// <param name="UseIsolation">是否隔离每句话？</param>
    /// <param name="Sign">设置隔离符号，如“,”或“'”；只支持一个字符。</param>
    /// <returns></returns>
    protected static string CombineField(string[] args, bool UseIsolation = false, string Sign = "`")
    {
        string result = "";
        for (int i = 0; i < args.Count(); i++)
        {
            args[i] = args[i].Trim();
            if (UseIsolation) args[i] = Sign + args[i] + Sign;
            result += args[i] + ", ";
        }
        result = result.Remove(result.Length - 2, 2);
        return result;
    }

    /// <summary>
    /// 检查sql_string中的语法，
    /// 避免出现无法为SQL变量赋值NULL的问题。
    /// </summary>
    /// <param name="sql_string"></param>
    protected static void CheckFormal(ref string sql_string)
    {
        sql_string = sql_string.Replace("'NULL'", "NULL");
        sql_string = sql_string.Replace("'now()'", "now()");
        sql_string = sql_string.Replace("'JSON_OBJECT()'", "JSON_OBJECT()");
    }
}

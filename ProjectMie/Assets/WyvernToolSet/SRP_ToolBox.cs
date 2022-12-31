using System.Threading;
using System.Net;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

#region tool
/// <summary>
/// 常用工具类
/// </summary>
public static class WTool
{
    /// <summary>
    /// 选择语句，true时返回A，false时返回B。
    /// </summary>
    /// <param name="isA"></param>
    /// <param name="A"></param>
    /// <param name="B"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T Selector<T>(bool isA, T A, T B)
    {
        if (isA) return A;
        else return B;
    }
    /// <summary>
    /// 选择语句，true时返回A，false时返回B。
    /// 方法名称中的下斜线是为了防止二义性情况发生的。
    /// </summary>
    /// <param name="A"></param>
    /// <param name="B"></param>
    /// <param name="isA"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T Selector_<T>(T A, T B, bool isA) => Selector<T>(isA, A, B);

    /// <summary>
    /// 获取Uid
    /// </summary>
    /// <returns></returns>
    public static string GetUidWithField() => Guid.NewGuid().ToString();

    /// <summary>
    /// 从27个英文字母和十个数字中随机获取一个字。
    /// </summary>
    /// <returns></returns>
    public static char RandomChar()
    {
        const string list = "abcdefghijklmnopqrstuvwxyz0123456789";
        return list[UnityEngine.Random.Range(0, list.Length - 1)];
    }

    /// <summary>
    /// 获取指定长度的随机字符串。
    /// </summary>
    /// <param name="length"></param>
    /// <returns></returns>
    public static string RandomString(int length)
    {
        string result = "";

        for (int i = 0; i < length; i++)
        {
            result += RandomChar();
        }

        return result;
    }


    /// <summary>
    /// 切割String。
    /// </summary>
    /// <param name="STRING">被切割的String</param>
    /// <param name="isRight">向右还是向左</param>
    /// <param name="count">切割数量</param>
    public static string StringChop(string STRING, int count, bool isRight = false)
    {
        string cache = STRING;
        return StringChopInline(ref cache, count, isRight);
    }
    public static string StringChopInline(ref string STRING, int count, bool isRight = false)
    {
        if(STRING.Length < count)
        {
            Debug.LogError("长度爆炸，无法切割。");
            return STRING;
        }
        if(isRight) STRING = STRING.Remove(0, count);
        else STRING = STRING.Remove(STRING.Length - count, count);
        return STRING;
    }

    /// <summary>
    /// 将string拆分进数组。
    /// </summary>
    /// <param name="STR"></param>
    /// <param name="SplitChar"></param>
    /// <returns></returns>
    public static List<string> ParseStringIntoArray(string STR, char SplitChar)
    {
        List<string> result = new List<string>();
        foreach(var S in STR.Split(SplitChar)) result.Add(S);
        return result;
    }

    /// <summary>
    /// 读取本地文件。
    /// </summary>
    /// <param name="path">Local模式下，Windows平台基础路径是Assets。安卓平台基础路径是Assets/StreamingAssets。非Local模式下，即全局模式。</param>
    /// <param name="OneLine">是否剔除换行符和制表符，使其成为标准的单行字符串？</param>
    /// <param name="isLocalFile">是否启用Local模式？</param>
    /// <returns>目标文件的string。</returns>
    public static string ReadFile(string path, bool OneLine = false, bool isLocalFile = true)
    {
        string path_ = "";

        if(isLocalFile)
        {
            /* Windows平台 */
            if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
                path_ = Application.dataPath + "/" + path;

            /* Android平台 */
            if (Application.platform == RuntimePlatform.Android)
                path_ = Application.streamingAssetsPath + "/" + path;
        }
        else path_ = path;

        Debug.Log(path_);

        if(path_ == "")
        {
            Debug.LogError("读取文件失败，文件路径不合法。");
            return "";
        }

        if(File.Exists(path_) == false)
        {
            Debug.LogError("读取文件失败，无法找到文件：" + path_);
            return "";
        }


        if(isLocalFile == false)
            if(Application.platform == RuntimePlatform.Android) path_ = "file://" + path_;
            
        using(UnityWebRequest WWW = UnityWebRequest.Get(path_))
        {
            WWW.SendWebRequest();
            while(WWW.isDone == false) {};
            return Selector<string>(OneLine ,WWW.downloadHandler.text.Replace("\n","").Replace("\t",""), WWW.downloadHandler.text);
        }
    }
}
#endregion

#region math
/// <summary>
/// 常用数学类
/// </summary>
public static class WMath
{
    /// <summary>
    /// 平面圆公式。
    /// 以(1,0)（水平向右）方向为正方向，以(0,1)方向为上方向。
    /// </summary>
    /// <param name="Origin">原点位置</param>
    /// <param name="Radius">半径</param>
    /// <param name="Degree">角度</param>
    /// <returns>平面圆上的位置。</returns>
    static Vector2 Path_Circle(Vector2 Origin, float Radius, float Degree)
    {   
        Degree = Degree % 360f; /* 使角度始终限制在360度内 */
        return new Vector2(Radius * Mathf.Cos(Degree * (Mathf.PI / 180f)), Radius * Mathf.Sin(Degree * (Mathf.PI / 180f))) + Origin;
    }
    
    /// <summary>
    /// 计算两向量的夹角。
    /// 以(1,0)为正方向，返回0~360度。
    /// </summary>
    /// <param name="A"></param>
    /// <param name="B"></param>
    /// <returns>角度，非弧度</returns>
    static float GetRotationBetweenVector_360(Vector2 A, Vector2 B)
    {   
        //叉乘<0时朝上，叉乘>0时朝下
        if(Cross(Normalize(Vector2to3(A)), Normalize(Vector2to3(B))).z > 0) 
        {
            return 360 - GetRotationBetweenVector(A, B);
        }
        else return GetRotationBetweenVector(A, B);
    }
    
    /// <summary>
    /// 计算两向量的夹角。
    /// 最基础的反三角公式，只能处理0~180度以内的角。
    /// </summary>
    /// <param name="A"></param>
    /// <param name="B"></param>
    /// <returns>角度，非弧度</returns>
    static float GetRotationBetweenVector(Vector2 A, Vector2 B)//Acos只能输出0~180度
    {   //Acos返回的是弧度，后面乘数转成角度
        return (float)Mathf.Acos(Mathf.Clamp(Dot(Normalize(A), Normalize(B)), -1f, 1f)) * (180f / Mathf.PI);
    }

    /// <summary>
    /// 计算二维向量的模长。
    /// </summary>
    /// <param name="VECTOR"></param>
    /// <param name="abs">是否使用绝对值。</param>
    /// <returns>模长。</returns>
    static float GetVectorLength(Vector2 VECTOR, bool abs = false)
    {
        if(abs == true) return Mathf.Abs(Mathf.Sqrt(Mathf.Pow(VECTOR.x, 2) + Mathf.Pow(VECTOR.y, 2)));
        else return Mathf.Sqrt(Mathf.Pow(VECTOR.x, 2) + Mathf.Pow(VECTOR.y, 2));
    }
    
    /// <summary>
    /// 计算三维向量的模长。
    /// </summary>
    /// <param name="VECTOR"></param>
    /// <param name="useVec2">是否抛弃第三维度，将其当作一个二维向量计算？</param>
    /// <param name="abs">是否使用绝对值。</param>
    /// <returns>模长。</returns>
    static float GetVectorLength(Vector3 VECTOR, bool useVec2 = false, bool abs = false)
    {
        if (useVec2 == false)
        {
            if(abs == true) return Mathf.Abs(Mathf.Sqrt(Mathf.Pow(VECTOR.x, 2) + Mathf.Pow(VECTOR.y, 2) + Mathf.Pow(VECTOR.z, 2)));
            else return Mathf.Sqrt(Mathf.Pow(VECTOR.x, 2) + Mathf.Pow(VECTOR.y, 2) + Mathf.Pow(VECTOR.z, 2));
        }
        else
        {
            if(abs == true) return Mathf.Abs(Mathf.Sqrt(Mathf.Pow(VECTOR.x, 2) + Mathf.Pow(VECTOR.y, 2)));
            else return Mathf.Sqrt(Mathf.Pow(VECTOR.x, 2) + Mathf.Pow(VECTOR.y, 2));
        }
    }
    
    /// <summary>
    /// 二维向量点乘。
    /// </summary>
    /// <param name="A"></param>
    /// <param name="B"></param>
    /// <returns></returns>
    static float Dot(Vector2 A, Vector2 B)
    {
        return A.x * B.x + A.y * B.y;
    }
    
    /// <summary>
    /// 二维向量叉乘。
    /// </summary>
    /// <param name="A"></param>
    /// <param name="B"></param>
    /// <returns></returns>
    static float Cross(Vector2 A, Vector2 B)
    {
        return (A.x * B.y - A.y * B.x);
    }
    
    /// <summary>
    /// 三维向量叉乘。
    /// </summary>
    /// <param name="A"></param>
    /// <param name="B"></param>
    /// <returns></returns>
    static Vector3 Cross(Vector3 A, Vector3 B)
    {
        return new Vector3(A.y * B.z - A.z * B.y, A.z * B.x - B.z * A.x, A.x * B.y - A.y * B.x);
    }
    
    /// <summary>
    /// 二维向量转换三维向量。
    /// </summary>
    /// <param name="A"></param>
    /// <param name="dim">第三维度的值。</param>
    /// <returns></returns>
    static Vector3 Vector2to3(Vector2 A, float dim = 0f)
    {
        return new Vector3(A.x, A.y, dim);
    }
    
    /// <summary>
    /// 三维向量转换二维向量。
    /// 第三维度会被抛弃。
    /// </summary>
    /// <param name="A"></param>
    /// <returns></returns>
    static Vector2 Vector3to2(Vector3 A)
    {
        return new Vector2(A.x, A.y);
    }
    
    /// <summary>
    /// 单位化向量。
    /// </summary>
    /// <param name="A"></param>
    /// <returns></returns>
    static Vector3 Normalize(Vector3 A)
    {
        if(A.x == 0) A.x = Mathf.Epsilon;
        if(A.y == 0) A.y = Mathf.Epsilon;
        if(A.z == 0) A.z = Mathf.Epsilon;
        return new Vector3(A.x / GetVectorLength(A, false, true), A.y / GetVectorLength(A, false, true), A.z / GetVectorLength(A, false, true));
    }
    
    /// <summary>
    /// 单位化向量。
    /// </summary>
    /// <param name="A"></param>
    /// <returns></returns>
    static Vector2 Normalize(Vector2 A)
    {
        if(A.x == 0) A.x = Mathf.Epsilon;
        if(A.y == 0) A.y = Mathf.Epsilon;
        return new Vector2(A.x / GetVectorLength(A, true), A.y / GetVectorLength(A, true));
    }
    
    /// <summary>
    /// 三维向量插值。
    /// </summary>
    /// <param name="A"></param>
    /// <param name="B"></param>
    /// <param name="C">速度</param>
    /// <returns></returns>
    static Vector3 LerpVector(Vector3 A, Vector3 B, float C)
    {
        return new Vector3(Mathf.Lerp(A.x, B.x, C), Mathf.Lerp(A.y, B.y, C), Mathf.Lerp(A.z, B.z, C));
    }
    
    /// <summary>
    /// 二维向量插值。
    /// </summary>
    /// <param name="A"></param>
    /// <param name="B"></param>
    /// <param name="C">速度</param>
    /// <returns></returns>
    static Vector2 LerpVector(Vector2 A, Vector2 B, float C)
    {
        return new Vector3(Mathf.Lerp(A.x, B.x, C), Mathf.Lerp(A.y, B.y, C));
    }
    
    /// <summary>
    /// 三维向量常量插值。
    /// </summary>
    /// <param name="A"></param>
    /// <param name="B"></param>
    /// <param name="C">速度</param>
    /// <returns></returns>
    static Vector3 MoveVector(Vector3 A, Vector3 B, float C)
    {
        return new Vector3(Mathf.MoveTowards(A.x, B.x, C), Mathf.MoveTowards(A.y, B.y, C), Mathf.MoveTowards(A.z, B.z, C));
    }
    
    /// <summary>
    /// 二维向量常量插值。
    /// </summary>
    /// <param name="A"></param>
    /// <param name="B"></param>
    /// <param name="C"></param>
    /// <returns>速度</returns>
    static Vector2 MoveVector(Vector2 A, Vector2 B, float C)
    {
        return new Vector2(Mathf.MoveTowards(A.x, B.x, C), Mathf.MoveTowards(A.y, B.y, C));
    }
    
    /// <summary>
    /// 三维向量渐变插值。
    /// </summary>
    /// <param name="A"></param>
    /// <param name="B"></param>
    /// <param name="MoveSpeed">应当小于等于LerpSpeed，否则效果不明显。</param>
    /// <param name="LerpSpeed">应当大于等于LerpSpeed，否则效果不明显。</param>
    /// <returns></returns>
    static Vector3 GradVector(Vector3 A, Vector3 B, float MoveSpeed, float LerpSpeed)
    {
        return MoveVector(A, LerpVector(A, B, LerpSpeed), MoveSpeed);
    }
    
    /// <summary>
    /// 二维向量渐变插值。
    /// </summary>
    /// <param name="A"></param>
    /// <param name="B"></param>
    /// <param name="MoveSpeed">应当小于等于LerpSpeed，否则效果不明显。</param>
    /// <param name="LerpSpeed">应当大于等于LerpSpeed，否则效果不明显。</param>
    /// <returns></returns>
    static Vector2 GradVector(Vector2 A, Vector2 B, float MoveSpeed, float LerpSpeed)
    {
        return MoveVector(A, LerpVector(A, B, LerpSpeed), MoveSpeed);
    }

    //TODO:TransformLocation和Direction
}
#endregion
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SRP_FanpaiManager : MonoBehaviour
{
    public static SRP_FanpaiManager FanpaiManagerInstance;

    [HideInInspector]
    public static int ForcusingIndex = -1;
    [HideInInspector]
    public bool isMoveing = false;//水果是否正在移动
    [HideInInspector]
    public static GameObject ForcusingPrefab = null;
    [HideInInspector]
    public static List<Vector3> ForcusingPrefabOriginalLocList = new List<Vector3>();
    [HideInInspector]
    public static List<Vector3> ForcusingPrefabOriginalScaleList = new List<Vector3>();
    [HideInInspector]
    public static bool isWatchingFruit = false;
    [HideInInspector]
    private SpriteRenderer LowLightImg;
    private int TrueCount = 0;
    private int FalseCount = 0;

    [Tooltip("中心点位置Offset")]
    public Vector3 CenterLocOffset;
    [Tooltip("水果缩放速度")]
    public float ScaleSpeed = 0.1f;
    [Tooltip("水果缩放")]
    public float TargetScale = 2f;
    [Tooltip("失败音频")]
    public AudioClip FailedSound;
    [Tooltip("成功音频")]
    public AudioClip WinSound;

    [Tooltip("将场景中的水果Prefab们放到这里以注册。")]
    public List<GameObject> FruitPrefabList = new List<GameObject>();

    void Awake()
    {
        //初始化static变量
        FanpaiManagerInstance = this;

        ForcusingPrefabOriginalLocList.Clear();

        ForcusingPrefabOriginalScaleList.Clear();

        isWatchingFruit = false;

        LowLightImg = transform.Find("LowLight/").GetComponent<SpriteRenderer>();

        //为每个fruit设置一个独一无二的index
        foreach(var GO in FruitPrefabList)
        {
            GO.GetComponent<SRP_SingleCard>().FruitIndex = FruitPrefabList.IndexOf(GO);
            ForcusingPrefabOriginalLocList.Add(GO.transform.position);
            ForcusingPrefabOriginalScaleList.Add(GO.transform.localScale);
        }
        
        gameObject.AddComponent<AudioSource>();

        Init();
    }

    public void Init()
    {
        //初始化变量
        ForcusingPrefab = null;
        ForcusingIndex = -1;
        CONTROL_GetCi();

    }

    //把水果移动到脸上
    public void MoveForcusingPrefabToFace(GameObject FruitPrefab, int index,bool isWin = false, string textt = "", bool noGetCi = false) => StartCoroutine(MFPTF(FruitPrefab, isWin, index, textt, noGetCi));
    IEnumerator MFPTF(GameObject FruitPrefab, bool isWin,int index, string textt ,bool noGetCi)
    {
        if(isMoveing == true) yield break;//如果正在移动，就不执行任何事情
        if(ForcusingPrefab != null && ForcusingPrefab != FruitPrefab) yield break;//如果已经有正观看的水果，就不执行任何事情

        transform.Find("Yinbiao/").GetComponent<TMPro.TMP_Text>().text = "";

        isMoveing = true;

        if(isWatchingFruit)
        {
            if(ForcusingPrefab == null || ForcusingPrefab.transform.Find("Square/") == null) 
            {
                isMoveing = false;
                yield break;
            }
            ForcusingPrefab.transform.Find("Square/").GetComponent<SpriteRenderer>().sortingOrder = 2;

            //设置ForcusingPrefab
            ForcusingPrefab = null;
            
            transform.Find("Yinbiao/").GetComponent<TMPro.TMP_Text>().text = "";

            //缩放小
            while(GetVector3Length(ForcusingPrefabOriginalLocList[index] - FruitPrefabList[index].transform.position) >= 0.1f)
            {
                FruitPrefabList[index].transform.position = Vector3.Lerp(FruitPrefabList[index].transform.position,     new Vector3(ForcusingPrefabOriginalLocList[index].x,    ForcusingPrefabOriginalLocList[index].y,    FruitPrefabList[index].transform.position.z),ScaleSpeed);
                FruitPrefabList[index].transform.localScale = Vector3.Lerp(FruitPrefabList[index].transform.localScale,    ForcusingPrefabOriginalScaleList[index],ScaleSpeed);
                LowLightImg.color = new Color(1,1,1,Mathf.Lerp(LowLightImg.color.a,0f,ScaleSpeed));
                yield return null;
            }

            //Lerp动画纠正偏移
            FruitPrefabList[index].transform.position = new Vector3(ForcusingPrefabOriginalLocList[index].x, ForcusingPrefabOriginalLocList[index].y, FruitPrefabList[index].transform.position.z);
            FruitPrefabList[index].transform.localScale = ForcusingPrefabOriginalScaleList[index];
            FruitPrefabList[index].transform.Find("Square/").GetComponent<SRP_FloatingEffect>().enabled = true;
            LowLightImg.color = new Color(1,1,1,0f);


            isWatchingFruit = false;

            yield return null;
        }
        else
        {
            //设置ForcusingPrefab
            ForcusingPrefab = FruitPrefabList[index];

            ForcusingPrefab.transform.Find("Square/").GetComponent<SpriteRenderer>().sortingOrder = 5;
            ForcusingPrefab.transform.Find("Square/").GetComponent<SRP_FloatingEffect>().enabled = false;

            //缩放大
            while(GetVector3Length(CenterLocOffset - FruitPrefabList[index].transform.position) >= 0.1f)
            {
                FruitPrefabList[index].transform.position = Vector3.Lerp(FruitPrefabList[index].transform.position,     new Vector3(0,0,FruitPrefabList[index].transform.position.z),ScaleSpeed);
                FruitPrefabList[index].transform.localScale = Vector3.Lerp(FruitPrefabList[index].transform.localScale,     ForcusingPrefabOriginalScaleList[index] * TargetScale ,ScaleSpeed);
                LowLightImg.color = new Color(1,1,1,Mathf.Lerp(LowLightImg.color.a,0.6f,ScaleSpeed));
                yield return null;
            }

            //Lerp动画纠正偏移
            FruitPrefabList[index].transform.position = Vector3.zero;
            FruitPrefabList[index].transform.localScale = ForcusingPrefabOriginalScaleList[index] * TargetScale;
            LowLightImg.color = new Color(1,1,1,0.6f);

            isWatchingFruit = true;

            //transform.Find("Yinbiao/").GetComponent<TMPro.TMP_Text>().text = textt;

            yield return null;
        }

        //弹出对错号
        if(ForcusingPrefab != null)
        if(isWin == true) 
        {
            GetComponent<AudioSource>().PlayOneShot(WinSound);
            SRP_Checker.DoRight();
            TrueCount++;
            if(TrueCount == 3) 
            {
                isWatchingFruit = false;
                GameManager.Instance.WinTheMission();
            }
        }
        else 
        {
            GetComponent<AudioSource>().PlayOneShot(FailedSound);
            SRP_Checker.DoFalse();
            FalseCount--;
            if(FalseCount == -3) 
            {
                GameManager.Instance.FailedTheMission();
                yield break;
            }
        }
        
        //等待动画完成
        yield return new WaitForSeconds(1.7f);
        

            transform.Find("Yinbiao/").GetComponent<TMPro.TMP_Text>().text = textt;
             //!缩放左
            while(GetVector3Length(CenterLocOffset - new Vector3(3.51f,0,0) - FruitPrefabList[index].transform.position) >= 0.1f)
            {
                FruitPrefabList[index].transform.position = Vector3.Lerp(FruitPrefabList[index].transform.position,     new Vector3(-3.51f,0,0),ScaleSpeed * 0.5f);
                yield return null;
            }

        //等待动画完成
        yield return new WaitForSeconds(2.5f);
        isMoveing = false;
        
        if (isWatchingFruit) 
        {
            ForcusingPrefab.transform.Find("Square/").GetComponent<SpriteRenderer>().sortingOrder = 2;

            //设置ForcusingPrefab
            ForcusingPrefab = null;
            
            transform.Find("Yinbiao/").GetComponent<TMPro.TMP_Text>().text = "";

            //缩放小
            while(GetVector3Length(ForcusingPrefabOriginalLocList[index] - FruitPrefabList[index].transform.position) >= 0.1f)
            {
                FruitPrefabList[index].transform.position = Vector3.Lerp(FruitPrefabList[index].transform.position,     new Vector3(ForcusingPrefabOriginalLocList[index].x,    ForcusingPrefabOriginalLocList[index].y,    FruitPrefabList[index].transform.position.z),ScaleSpeed);
                FruitPrefabList[index].transform.localScale = Vector3.Lerp(FruitPrefabList[index].transform.localScale,    ForcusingPrefabOriginalScaleList[index],ScaleSpeed);
                LowLightImg.color = new Color(1,1,1,Mathf.Lerp(LowLightImg.color.a,0f,ScaleSpeed));
                yield return null;
            }

            //Lerp动画纠正偏移
            FruitPrefabList[index].transform.position = new Vector3(ForcusingPrefabOriginalLocList[index].x, ForcusingPrefabOriginalLocList[index].y, FruitPrefabList[index].transform.position.z);
            FruitPrefabList[index].transform.localScale = ForcusingPrefabOriginalScaleList[index];
            FruitPrefabList[index].transform.Find("Square/").GetComponent<SRP_FloatingEffect>().enabled = true;
            LowLightImg.color = new Color(1,1,1,0f);


            isWatchingFruit = false;
        }

        if(isWin && noGetCi == false) CONTROL_GetCi();
        yield break;
    }

    float GetVector3Length(Vector3 vec)
    {
        //放弃深度
        return Mathf.Sqrt(Mathf.Pow(vec.x,2)+Mathf.Pow(vec.y,2));
    }

    public void CONTROL_BackToMenu()
    {
        GameManager.Instance.BackToMainmenu();
    }

    bool isGetting = false;
    public void CONTROL_GetCi(bool noVoice = false) => StartCoroutine(GetCi(noVoice));
    IEnumerator GetCi(bool noVoice)
    {
        if(isGetting == true) yield break;//正在取词
        isGetting = true;
        
        //随机抽取
        ForcusingIndex = Random.Range(0,FruitPrefabList.Count);

        yield return new WaitForSeconds(1f);
        if(noVoice == false)
        FruitPrefabList[ForcusingIndex].GetComponent<SRP_SingleCard>().PlayVoice();
        isGetting = false;
        yield break;
    }
}

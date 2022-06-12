using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SRP_ZBK_Manager : MonoBehaviour
{
    public static SRP_ZBK_Manager ZBKManagerInstance;

    [HideInInspector]
    public static List<GameObject> SelectedPrefabList = new List<GameObject>();

    [Tooltip("将场景中的水果Prefab们放到这里以注册。")]
    public List<GameObject> ItemList = new List<GameObject>();

    public List<GameObject> LockerGOList = new List<GameObject>();

    [HideInInspector]
    public int CurrentPlaceIndex = 0;//当前需要被填充的位置index

    [Tooltip("失败音频")]
    public AudioClip FailedSound;
    [Tooltip("成功音频")]
    public AudioClip WinSound;

    void Awake()
    {
        //初始化static变量
        ZBKManagerInstance = this;
        SelectedPrefabList.Clear();

        //为每个fruit设置一个独一无二的index
        foreach(var GO in ItemList)
        {
            GO.GetComponent<SRP_ZBK_Item>().ItemIndex = ItemList.IndexOf(GO);
        }

        gameObject.AddComponent<AudioSource>();
    }

    List<int> KindList = new List<int>();
    public void AddedMessage()
    {
        if(SelectedPrefabList.Count != 3) return;
        KindList.Clear();
        foreach(var GO in SelectedPrefabList) KindList.Add(GO.GetComponent<SRP_ZBK_Item>().KindIndex);
        if(KindList[0] + KindList[1] + KindList[2] == KindList[0]*3) 
        Invoke("win",1f);
    }

    private void win()
    {
        GameManager.Instance.WinTheMission();
    }

    //把水果移动到脸上
    public void MoveForcusingPrefabToFace() => StartCoroutine(MFPTF());
    IEnumerator MFPTF()
    {
        //Item
        //OriginalPos
        //ToggleMove()
        //KindIndex
        //ItemIndex
        //CONTROL_Click()

        //Manager
        //FruitPrefabList
        //SelectedPrefabList
        //CurrentPlaceIndex
        //CheckWin()


        yield break;
    }

    








    




    public void CONTROL_BackToMenu()
    {
        GameManager.Instance.BackToMainmenu();
    }
}

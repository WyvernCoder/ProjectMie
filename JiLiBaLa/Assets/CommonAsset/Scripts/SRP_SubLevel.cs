using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SRP_SubLevel : MonoBehaviour
{
    [Tooltip("关卡Prefab List")]
    public List<GameObject> LevelPrefabList = new List<GameObject>();
    private List<GameObject> LevelPrefabList_ = new List<GameObject>();//存储已实例化的关卡Prefab
    
    [HideInInspector]
    public Canvas SelectCanvas;//选择关卡的节面
    [HideInInspector]
    public int SubLevelIndex = 0;//通过按钮事件赋值的关卡index

    void Awake()
    {
        SelectCanvas = GameObject.Find("SelectCanvas/").gameObject.GetComponent<Canvas>();
        if(SelectCanvas == null) print("找不到关卡选择Canvas！");
    }

    void Start()
    {
        //进入场景的黑窗切出动画
        if(GameManager.Instance != null) GameManager.Instance.Anim_BlackOut();
    }

    public void PlaySubLevel()
    {
        if(SubLevelIndex > LevelPrefabList.Count - 1) 
        {
            print("关卡index溢出啦！");
            return;
        }

        LevelPrefabList_.Add(Instantiate(LevelPrefabList[SubLevelIndex]));
        SelectCanvas.enabled = false;//隐藏掉关卡选择节面
    }
}

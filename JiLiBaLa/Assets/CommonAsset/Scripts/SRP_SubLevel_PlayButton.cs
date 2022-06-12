using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SRP_SubLevel_PlayButton : MonoBehaviour
{
    [Tooltip("按钮对应的关卡Prefab index")]
    public int ButtonIndex = 0;
    private SRP_SubLevel SubLevelSelecterScript;
    
    void Start()
    {
        SubLevelSelecterScript = FindObjectOfType<SRP_SubLevel>();
        if(SubLevelSelecterScript == null) print("无法找到SRP_SubLevel，严重错误！");
    }
    public void CONTROL_PlaySubLevel()
    {
        SubLevelSelecterScript.SubLevelIndex = ButtonIndex;
        SubLevelSelecterScript.PlaySubLevel();
    }
}

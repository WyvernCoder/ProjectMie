using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SRP_SubIsland : MonoBehaviour
{
    [HideInInspector]
    public SRP_Island IslandBase;

    [HideInInspector]
    public int subIslandIndex = -1;

    [Tooltip("启用自动清理，仅用于调试，不会影响游戏性。")]
    public bool EnableAutoClean = true;

    private float originalY;

    void Start()
    {
        if(Application.isPlaying == false && EnableAutoClean) StartCoroutine(AutoRemoveAfterOneFrame());
        originalY = transform.position.y;
        
    }

    void Update()
    {
        if(Application.isPlaying == true) gameObject.transform.position = new Vector3(gameObject.transform.position.x, originalY + 0.2f*Mathf.Sin(Time.fixedTime),gameObject.transform.position.z);
    }

    public void CONTROL_OpenSubMainmenu()
    {
        //关闭Submenu的逻辑在SRP_Control里，这里只负责打开
        if (IslandBase.TouchMover.MoveLength > 1f) return;//防止误触
        IslandBase.TouchMover.CanMoveCamera = false;//菜单打开后禁止移动相机
        IslandBase.SubMainmenuObjectList[subIslandIndex].GetComponent<Canvas>().enabled = true;
        IslandBase.MainmenuController.CurrentSubMenu = IslandBase.SubMainmenuObjectList[subIslandIndex];//为Control里的当前菜单变量赋值便于返回选关菜单
        //关闭菜单时要把CurrentSubMenu设null
    }

    IEnumerator AutoRemoveAfterOneFrame()
    {
        yield return null;
        if(IslandBase.Enable == true)
        DestroyImmediate(gameObject);
        yield break;
    }
}

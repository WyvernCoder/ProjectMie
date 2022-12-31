using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SRP_CanvasTransformGetter : MonoBehaviour
{
    public GameObject CanvasTransform;
    
    void Start()
    {
        if(CanvasTransform == null)
        {
            print("该脚本未设置CanvasTransform，可能无法使用水平移动切屏模式。");
            Destroy(this);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class SRP_SpriteFitter : MonoBehaviour
{
///把我附加在带有方形Sprite组件的GameObject上，我会让它的大小适配屏幕大小
/// Sprite必须是方形且Scale是1,1,1！

    private Camera CAM;
    public Vector2 Offset = new Vector2(1f,1f);
    private Vector3 originalSize;
    void Start()
    {
        if(Camera.main == null) CAM = GameObject.Find("Main Camera").GetComponent<Camera>();
        else CAM = Camera.main;
        if(CAM == null)
        {
            print("未找到可用相机，适配进程终止！");
            return;
        }
        
    }
    void Awake()
    {
        originalSize = transform.localScale;
    }
    void Update()
    {
        transform.position = Camera.main.transform.position + new Vector3(0,0,10);
        transform.localScale = new Vector3((float)Screen.width / (float)Screen.height * CAM.orthographicSize / 9.6f * Offset.x, originalSize.y * Offset.y , 1f);
    }
}

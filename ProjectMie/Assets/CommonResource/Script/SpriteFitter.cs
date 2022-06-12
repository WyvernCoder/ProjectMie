using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]//!该语句确保脚本能在编辑器状态运行以实时查看效果
public class SpriteFitter : MonoBehaviour
{
    [Header("请观察视口并调整ScaleOffset以达到合适的值")]
    [Header("挂载该脚本的GameObject会和屏幕大小等比缩放")]
    [Tooltip("缩放偏移")]
    public Vector2 ScaleOffset = new Vector2(1f,1f);
    private Camera CAM;//当前相机

    //Awake要早于Update，所以这里不能使用Start
    void Awake()
    {   
        //寻找当前相机
        if(Camera.main == null) CAM = GameObject.Find("Main Camera").GetComponent<Camera>();
        else CAM = Camera.main;//按照Unity的标准，主要相机应当自带“MainCamera”这个标签，这个函数就是用来获取这个带有MainCamera标签的相机的
    }

    void Update()
    {   
        if(CAM == null)
        {
            print("未找到可用相机，任务终止！");
            Destroy(this);
        }
        
        //使被适配对象永远跟随相机
        transform.position = Camera.main.transform.position + new Vector3(0,0,10);

        //使被适配对象缩放至屏幕大小；算法的话，线索：①一定和分辨率长宽比有关 ②UNITY定义屏幕中心到屏幕上下侧的距离是orthographicSize个单位，用这俩线索再乘以一个固定常数以得到正确的结果。关于Y：Y只和orthographicSize有关，和长宽比无关，所以Y不用乘长宽比。
        transform.localScale = new Vector3((float)Screen.width / (float)Screen.height * CAM.orthographicSize * 0.2f * ScaleOffset.x, ScaleOffset.y * CAM.orthographicSize * 0.2f , 1f);
    }
}


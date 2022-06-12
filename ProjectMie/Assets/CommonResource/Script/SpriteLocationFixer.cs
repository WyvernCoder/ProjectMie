using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]//!该语句确保脚本能在编辑器状态运行
public class SpriteLocationFixer : MonoBehaviour
{
    private Camera CAM;
    private Vector3 FinalLoc;//因为要根据枚举去选择位置，所以这个就表示最终位置

    [Tooltip("屏幕锚点")]
    public E_TheAnchor Anchor;//允许用户选择屏幕锚点，很人性化吧？

    [Tooltip("位置偏移")]
    public Vector2 Offset;//这里还给了偏移，很好吧？

    public enum E_TheAnchor //枚举
    {
        TopLeft,
        TopCenter,
        TopRight,
        MiddleLeft,
        MiddleCenter,
        MiddleRight,
        BottomLeft,
        BottomCenter,
        BottomRight
    }

    void Awake()
    {
        //寻找相机
        if (Camera.main == null) CAM = GameObject.Find("Main Camera").GetComponent<Camera>();
        else CAM = Camera.main;
        if (CAM == null)
        {
            print("未找到可用相机，停止运行！");
            Destroy(this);
        }
    }

    void Update()
    {
        //算法：既然UNITY已经告诉我们屏幕中心点到屏幕上下侧的距离了，那么我们就可以使用分辨率长宽比得出屏幕中心点到屏幕左右侧的距离，以此确定不同锚点的位置。
        switch (Anchor)
        {
            case E_TheAnchor.TopLeft: FinalLoc = new Vector3((float)Screen.width / (float)Screen.height * CAM.orthographicSize * -1f + Offset.x + CAM.transform.position.x, CAM.orthographicSize * 1f + Offset.y + CAM.transform.position.y, transform.position.z); break;
            case E_TheAnchor.TopCenter: FinalLoc = new Vector3(0 + Offset.x + CAM.transform.position.x, CAM.orthographicSize * 1f + Offset.y + CAM.transform.position.y, 0f + transform.position.z); break;
            case E_TheAnchor.TopRight: FinalLoc = new Vector3((float)Screen.width / (float)Screen.height * CAM.orthographicSize * 1f + Offset.x + CAM.transform.position.x, CAM.orthographicSize * 1f + Offset.y + CAM.transform.position.y, 0f + transform.position.z); break;
            case E_TheAnchor.MiddleLeft: FinalLoc = new Vector3((float)Screen.width / (float)Screen.height * CAM.orthographicSize * -1f + Offset.x + CAM.transform.position.x, Offset.y + CAM.transform.position.y, 0f + transform.position.z); break;
            case E_TheAnchor.MiddleCenter: FinalLoc = new Vector3(0 + Offset.x + CAM.transform.position.x, 0f + Offset.y + CAM.transform.position.y, 0f + transform.position.z); break;
            case E_TheAnchor.MiddleRight: FinalLoc = new Vector3((float)Screen.width / (float)Screen.height * CAM.orthographicSize * 1f + Offset.x + CAM.transform.position.x, Offset.y + CAM.transform.position.y, 0f + transform.position.z); break;
            case E_TheAnchor.BottomLeft: FinalLoc = new Vector3((float)Screen.width / (float)Screen.height * CAM.orthographicSize * -1f + Offset.x + CAM.transform.position.x, CAM.orthographicSize * -1f + Offset.y + CAM.transform.position.y, 0f + transform.position.z); break;
            case E_TheAnchor.BottomCenter: FinalLoc = new Vector3(0 + Offset.x + CAM.transform.position.x, CAM.orthographicSize * -1f + Offset.y + CAM.transform.position.y, 0f + transform.position.z); break;
            case E_TheAnchor.BottomRight: FinalLoc = new Vector3((float)Screen.width / (float)Screen.height * CAM.orthographicSize * 1f + Offset.x + CAM.transform.position.x, CAM.orthographicSize * -1f + Offset.y + CAM.transform.position.y, 0f + transform.position.z); break;
        }
        transform.position = FinalLoc;
    }
}

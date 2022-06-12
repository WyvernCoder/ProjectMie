using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class SRP_SpriteLocFixer : MonoBehaviour
{
    ///把我附加在需要锚定的GameObject上，然后选择锚点即可！

    private Camera CAM;

    private Vector3 FinalLoc;

    [Tooltip("启动对齐锚点Script")]
    public bool Enable = true;

    [Tooltip("锚点")]
    public E_TheAnchor Anchor;

    [Tooltip("位置偏移")]
    public Vector2 Offset;

    public enum E_TheAnchor
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

    void Start()
    {
        if (Camera.main == null) CAM = GameObject.Find("Main Camera").GetComponent<Camera>();
        else CAM = Camera.main;
        if (CAM == null)
        {
            print("未找到可用相机，适配进程终止！");
            Destroy(gameObject);
        }


        if (Enable && Application.isPlaying)
        {
            switch (Anchor)
            {
                case E_TheAnchor.TopLeft: FinalLoc = new Vector3((float)Screen.width / (float)Screen.height * CAM.orthographicSize * -1f + Offset.x, CAM.orthographicSize * 1f + Offset.y, 0f); break;
                case E_TheAnchor.TopCenter: FinalLoc = new Vector3(0 + Offset.x, CAM.orthographicSize * 1f + Offset.y, 0f); break;
                case E_TheAnchor.TopRight: FinalLoc = new Vector3((float)Screen.width / (float)Screen.height * CAM.orthographicSize * 1f + Offset.x, CAM.orthographicSize * 1f + Offset.y, 0f); break;
                case E_TheAnchor.MiddleLeft: FinalLoc = new Vector3((float)Screen.width / (float)Screen.height * CAM.orthographicSize * -1f + Offset.x, Offset.y, 0f); break;
                case E_TheAnchor.MiddleCenter: FinalLoc = new Vector3(0 + Offset.x, 0f + Offset.y, 0f); break;
                case E_TheAnchor.MiddleRight: FinalLoc = new Vector3((float)Screen.width / (float)Screen.height * CAM.orthographicSize * 1f + Offset.x, Offset.y, 0f); break;
                case E_TheAnchor.BottomLeft: FinalLoc = new Vector3((float)Screen.width / (float)Screen.height * CAM.orthographicSize * -1f + Offset.x, CAM.orthographicSize * -1f + Offset.y, 0f); break;
                case E_TheAnchor.BottomCenter: FinalLoc = new Vector3(0 + Offset.x, CAM.orthographicSize * -1f + Offset.y, 0f); break;
                case E_TheAnchor.BottomRight: FinalLoc = new Vector3((float)Screen.width / (float)Screen.height * CAM.orthographicSize * 1f + Offset.x, CAM.orthographicSize * -1f + Offset.y, 0f); break;
            }
            transform.position = FinalLoc;
        }
    }

    void Update()
    {
        if (Enable)
        {
            switch (Anchor)
            {
                case E_TheAnchor.TopLeft: FinalLoc = new Vector3((float)Screen.width / (float)Screen.height * CAM.orthographicSize * -1f + Offset.x, CAM.orthographicSize * 1f + Offset.y, 0f); break;
                case E_TheAnchor.TopCenter: FinalLoc = new Vector3(0 + Offset.x, CAM.orthographicSize * 1f + Offset.y, 0f); break;
                case E_TheAnchor.TopRight: FinalLoc = new Vector3((float)Screen.width / (float)Screen.height * CAM.orthographicSize * 1f + Offset.x, CAM.orthographicSize * 1f + Offset.y, 0f); break;
                case E_TheAnchor.MiddleLeft: FinalLoc = new Vector3((float)Screen.width / (float)Screen.height * CAM.orthographicSize * -1f + Offset.x, Offset.y, 0f); break;
                case E_TheAnchor.MiddleCenter: FinalLoc = new Vector3(0 + Offset.x, 0f + Offset.y, 0f); break;
                case E_TheAnchor.MiddleRight: FinalLoc = new Vector3((float)Screen.width / (float)Screen.height * CAM.orthographicSize * 1f + Offset.x, Offset.y, 0f); break;
                case E_TheAnchor.BottomLeft: FinalLoc = new Vector3((float)Screen.width / (float)Screen.height * CAM.orthographicSize * -1f + Offset.x, CAM.orthographicSize * -1f + Offset.y, 0f); break;
                case E_TheAnchor.BottomCenter: FinalLoc = new Vector3(0 + Offset.x, CAM.orthographicSize * -1f + Offset.y, 0f); break;
                case E_TheAnchor.BottomRight: FinalLoc = new Vector3((float)Screen.width / (float)Screen.height * CAM.orthographicSize * 1f + Offset.x, CAM.orthographicSize * -1f + Offset.y, 0f); break;
            }
            transform.position = FinalLoc;
        }
    }
}

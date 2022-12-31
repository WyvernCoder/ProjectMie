using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
//using Function;
//using PlaneManager;

//namespace RankManager

public class UIDragThis : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    #region 参数
    [Header("是否精确拖拽")]
    internal bool m_isPrecision;
    //internal类型(Type)和成员(member)的访问修饰符
    //存储图片中心点与鼠标点的偏移量
    private Vector3 m_offset;
    //存储当前拖拽图片的RectTransform组件
    private RectTransform m_rt;
    //是否可以拖动
    internal bool canMove = true;
    //自身起始位置
    public Vector3 thisPosition;
    //自身的id
    public int thisID = 0;

    public Transform otherThis;

    //拖动组件父物体
    public Transform content;
    //碰撞物体数组
    public RankColliderManager[] rankColliderManager;
    #endregion
    #region 常规方法
    void Start()
    {
        //初始化
        m_rt = gameObject.GetComponent<RectTransform>();
    }
    #endregion

    #region 公共方法
    public void OnBeginDrag(PointerEventData eventData)
    {
        ////this.transform.parent = otherThis;
        //建议改用SetParent方法，将worldPositionStays参数设置为false。这将保留局部方向和比例，而不是世界方向和比例，这可以防止常见的UI缩放问题。
        this.transform.SetParent(otherThis);
        if (canMove)
        {
            //如果精确拖拽则进行计算偏移量操作
            if (m_isPrecision)
            {
                //存储点击时的鼠标坐标
                Vector3 tWorldPos;
                //UI屏幕坐标转换为世界坐标
                RectTransformUtility.ScreenPointToWorldPointInRectangle(m_rt, eventData.position, eventData.pressEventCamera, out tWorldPos);
                //计算偏移量
                m_offset = transform.position - tWorldPos;

            }
            //否则，默认偏移量为0
            else
            {
                m_offset = Vector3.zero;
            }
            SetDraggedPosition(eventData);
        }
    }

    /// <summary>
    /// 拖拽过程中触发
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(PointerEventData eventData)
    {
        if (canMove)
        {

            SetDraggedPosition(eventData);
        }
    }

    /// <summary>
    /// 结束拖拽触发
    /// </summary>
    /// <param name="eventData"></param>
    public void OnEndDrag(PointerEventData eventData)
    {
        if (canMove)
        {
            SetDraggedPosition(eventData);
            ChangePosition();
        }
    }
    #endregion

    #region 私有方法
    /// <summary>
    /// 设置图片位置方法
    /// </summary>
    /// <param name="eventData"></param>
    private void SetDraggedPosition(PointerEventData eventData)
    {
        //存储当前鼠标所在的位置
        Vector3 globalMousePos;
        //UI屏幕坐标转换为世界坐标
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(m_rt, eventData.position, eventData.pressEventCamera, out globalMousePos))
        {
            m_rt.position = m_offset + globalMousePos;
        }
    }

    /// <summary>
    /// 判断改变自身位置
    /// </summary>
    void ChangePosition()
    {
        if (thisID == 0)
        {
            if (rankColliderManager[0].isHaveCard[0] && !rankColliderManager[0].isHaveCard[1] &&
                !rankColliderManager[0].isHaveCard[2] && !rankColliderManager[0].isHaveCard[3] &&
                !rankColliderManager[0].isHaveCard[4] && !rankColliderManager[0].isHaveCard[5] &&
                !rankColliderManager[0].isHaveCard[5])
            {
                m_rt.localPosition = new Vector3(0f, 220.0f, 0f);
            }
            else if (rankColliderManager[1].isHaveCard[0] && !rankColliderManager[1].isHaveCard[1] &&
                !rankColliderManager[1].isHaveCard[2] && !rankColliderManager[1].isHaveCard[3] &&
                !rankColliderManager[1].isHaveCard[4] && !rankColliderManager[1].isHaveCard[5] &&
                !rankColliderManager[1].isHaveCard[5])
            {
                m_rt.localPosition = new Vector3(0f, 150.0f, 0f);
            }
            else if (rankColliderManager[2].isHaveCard[0] && !rankColliderManager[2].isHaveCard[1] &&
                !rankColliderManager[2].isHaveCard[2] && !rankColliderManager[2].isHaveCard[3] &&
                !rankColliderManager[2].isHaveCard[4] && !rankColliderManager[2].isHaveCard[5] &&
                !rankColliderManager[2].isHaveCard[5])
            {
                m_rt.localPosition = new Vector3(0f, 80.0f, 0f);
            }
            else
            {
                ToRecoverPosition();
            }

        }
        if (thisID == 1)
        {
            if (!rankColliderManager[0].isHaveCard[0] && rankColliderManager[0].isHaveCard[1] &&
                !rankColliderManager[0].isHaveCard[2] && !rankColliderManager[0].isHaveCard[3] &&
                !rankColliderManager[0].isHaveCard[4] && !rankColliderManager[0].isHaveCard[5] &&
                !rankColliderManager[0].isHaveCard[6])
            {
                m_rt.localPosition = new Vector3(0f, 220.0f, 0f);
            }
            else if (!rankColliderManager[1].isHaveCard[0] && rankColliderManager[1].isHaveCard[1] &&
                !rankColliderManager[1].isHaveCard[2] && !rankColliderManager[1].isHaveCard[3] &&
                !rankColliderManager[1].isHaveCard[4] && !rankColliderManager[1].isHaveCard[5] &&
                !rankColliderManager[1].isHaveCard[6])
            {
                m_rt.localPosition = new Vector3(0f, 150.0f, 0f);
            }
            else if (!rankColliderManager[2].isHaveCard[0] && rankColliderManager[2].isHaveCard[1] &&
                !rankColliderManager[2].isHaveCard[2] && !rankColliderManager[2].isHaveCard[3] &&
                !rankColliderManager[2].isHaveCard[4] && !rankColliderManager[2].isHaveCard[5] &&
                !rankColliderManager[2].isHaveCard[6])
            {
                m_rt.localPosition = new Vector3(0f, 80.0f, 0f);
            }
            else
            {
                ToRecoverPosition();
            }
        }
        if (thisID == 2)
        {
            if (!rankColliderManager[0].isHaveCard[0] && !rankColliderManager[0].isHaveCard[1] &&
                rankColliderManager[0].isHaveCard[2] && !rankColliderManager[0].isHaveCard[3] &&
                !rankColliderManager[0].isHaveCard[4] && !rankColliderManager[0].isHaveCard[5] &&
                !rankColliderManager[0].isHaveCard[6])
            {
                m_rt.localPosition = new Vector3(0f, 220.0f, 0f);
            }
            else if (!rankColliderManager[1].isHaveCard[0] && !rankColliderManager[1].isHaveCard[1] &&
                rankColliderManager[1].isHaveCard[2] && !rankColliderManager[1].isHaveCard[3] &&
                !rankColliderManager[1].isHaveCard[4] && !rankColliderManager[1].isHaveCard[5] &&
                !rankColliderManager[1].isHaveCard[6])
            {
                m_rt.localPosition = new Vector3(0f, 150.0f, 0f);
            }
            else if (!rankColliderManager[2].isHaveCard[0] && !rankColliderManager[2].isHaveCard[1] &&
                rankColliderManager[2].isHaveCard[2] && !rankColliderManager[2].isHaveCard[3] &&
                !rankColliderManager[2].isHaveCard[4] && !rankColliderManager[2].isHaveCard[5] &&
                !rankColliderManager[2].isHaveCard[6])
            {
                m_rt.localPosition = new Vector3(0f, 80.0f, 0f);
            }
            else
            {
                ToRecoverPosition();
            }
        }
        if (thisID == 3)
        {
            if (!rankColliderManager[0].isHaveCard[0] && !rankColliderManager[0].isHaveCard[1] &&
                !rankColliderManager[0].isHaveCard[2] && rankColliderManager[0].isHaveCard[3] &&
                !rankColliderManager[0].isHaveCard[4] && !rankColliderManager[0].isHaveCard[5] &&
                !rankColliderManager[0].isHaveCard[6])
            {
                m_rt.localPosition = new Vector3(0f, 220.0f, 0f);
            }
            else if (!rankColliderManager[1].isHaveCard[0] && !rankColliderManager[1].isHaveCard[1] &&
                !rankColliderManager[1].isHaveCard[2] && rankColliderManager[1].isHaveCard[3] &&
                !rankColliderManager[1].isHaveCard[4] && !rankColliderManager[1].isHaveCard[5] &&
                !rankColliderManager[1].isHaveCard[6])
            {
                m_rt.localPosition = new Vector3(0f, 150.0f, 0f);
            }
            else if (!rankColliderManager[2].isHaveCard[0] && !rankColliderManager[2].isHaveCard[1] &&
                !rankColliderManager[2].isHaveCard[2] && rankColliderManager[2].isHaveCard[3] &&
                !rankColliderManager[2].isHaveCard[4] && !rankColliderManager[2].isHaveCard[5] &&
                !rankColliderManager[2].isHaveCard[6])
            {
                m_rt.localPosition = new Vector3(0f, 80.0f, 0f);
            }
            else
            {
                ToRecoverPosition();
            }
        }
        if (thisID == 4)
        {
            if (!rankColliderManager[0].isHaveCard[0] && !rankColliderManager[0].isHaveCard[1] &&
                !rankColliderManager[0].isHaveCard[2] && !rankColliderManager[0].isHaveCard[3] &&
                rankColliderManager[0].isHaveCard[4] && !rankColliderManager[0].isHaveCard[5] &&
                !rankColliderManager[0].isHaveCard[6])
            {
                m_rt.localPosition = new Vector3(0f, 220.0f, 0f);
            }
            else if (!rankColliderManager[1].isHaveCard[0] && !rankColliderManager[1].isHaveCard[1] &&
                !rankColliderManager[1].isHaveCard[2] && !rankColliderManager[1].isHaveCard[3] &&
                rankColliderManager[1].isHaveCard[4] && !rankColliderManager[1].isHaveCard[5] &&
                !rankColliderManager[1].isHaveCard[6])
            {
                m_rt.localPosition = new Vector3(0f, 150.0f, 0f);
            }
            else if (!rankColliderManager[2].isHaveCard[0] && !rankColliderManager[2].isHaveCard[1] &&
                !rankColliderManager[2].isHaveCard[2] && !rankColliderManager[2].isHaveCard[3] &&
                rankColliderManager[2].isHaveCard[4] && !rankColliderManager[2].isHaveCard[5] &&
                !rankColliderManager[2].isHaveCard[6])
            {
                m_rt.localPosition = new Vector3(0f, 80.0f, 0f);
            }
            else
            {
                ToRecoverPosition();
            }
        }
        if (thisID == 5)
        {
            if (!rankColliderManager[0].isHaveCard[0] && !rankColliderManager[0].isHaveCard[1] &&
                !rankColliderManager[0].isHaveCard[2] && !rankColliderManager[0].isHaveCard[3] &&
                !rankColliderManager[0].isHaveCard[4] && rankColliderManager[0].isHaveCard[5] &&
                !rankColliderManager[0].isHaveCard[6])
            {
                m_rt.localPosition = new Vector3(0f, 220.0f, 0f);
            }
            else if (!rankColliderManager[1].isHaveCard[0] && !rankColliderManager[1].isHaveCard[1] &&
                !rankColliderManager[1].isHaveCard[2] && !rankColliderManager[1].isHaveCard[3] &&
                !rankColliderManager[1].isHaveCard[4] && rankColliderManager[1].isHaveCard[5] &&
                !rankColliderManager[1].isHaveCard[6])
            {
                m_rt.localPosition = new Vector3(0f, 150.0f, 0f);
            }
            else if (!rankColliderManager[2].isHaveCard[0] && !rankColliderManager[2].isHaveCard[1] &&
                !rankColliderManager[2].isHaveCard[2] && !rankColliderManager[2].isHaveCard[3] &&
                !rankColliderManager[2].isHaveCard[4] && rankColliderManager[2].isHaveCard[5] &&
                !rankColliderManager[2].isHaveCard[6])
            {
                m_rt.localPosition = new Vector3(0f, 80.0f, 0f);
            }
            else
            {
                ToRecoverPosition();
            }
        }
        if (thisID == 6)
        {
            if (!rankColliderManager[0].isHaveCard[0] && !rankColliderManager[0].isHaveCard[1] &&
                !rankColliderManager[0].isHaveCard[2] && !rankColliderManager[0].isHaveCard[3] &&
                !rankColliderManager[0].isHaveCard[4] && !rankColliderManager[0].isHaveCard[5] &&
                rankColliderManager[0].isHaveCard[6])
            {
                m_rt.localPosition = new Vector3(0f, 220.0f, 0f);
            }
            else if (!rankColliderManager[1].isHaveCard[0] && !rankColliderManager[1].isHaveCard[1] &&
                       !rankColliderManager[1].isHaveCard[2] && !rankColliderManager[1].isHaveCard[3] &&
                       !rankColliderManager[1].isHaveCard[4] && !rankColliderManager[1].isHaveCard[5] &&
                       rankColliderManager[1].isHaveCard[6])
            {
                m_rt.localPosition = new Vector3(0f, 150.0f, 0f);
            }
            else if (!rankColliderManager[2].isHaveCard[0] && !rankColliderManager[2].isHaveCard[1] &&
                !rankColliderManager[2].isHaveCard[2] && !rankColliderManager[2].isHaveCard[3] &&
                !rankColliderManager[2].isHaveCard[4] && !rankColliderManager[0].isHaveCard[5] &&
                rankColliderManager[2].isHaveCard[6])
            {
                m_rt.localPosition = new Vector3(0f, 80.0f, 0f);
            }
            else
            {
                ToRecoverPosition();
            }
        }
        /// <summary>
        /// 恢复原始位置
        /// </summary>
        void ToRecoverPosition()
        {
            m_rt.localPosition = thisPosition;
            canMove = true;
        }

        #endregion
    }
}

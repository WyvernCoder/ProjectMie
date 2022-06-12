using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SRP_TouchMover : MonoBehaviour
{
    [Tooltip("需要移动的相机")]
    public Camera CameraNeedToMove;//场景中可能存在多个Camera，这些Camera不一定都要有触控移动的功能，所以必须在这指定一个Camera

    [Tooltip("加速度系数。")]
    public float SpeedUpFactor = 1;

    [Tooltip("减速度系数。")]
    public float SpeedDownFactor = 0.2f;

    [HideInInspector]
    public float MaxDistance = 200;//可移动距离，默认200。可以试着根据课程数量去动态修改它。

    private Vector3 TouchX;//手指按下时的位置，也就是初始位置

    [HideInInspector]
    public float MoveLength;//触摸不放时的手指当前位置与初始位置的距离，它被用作速度指标，就是说，距离越远速度就越快。

    [HideInInspector]
    public bool CanMoveCamera = true;//作用如其名
    
    private Vector3 FinalLocation;//相机最终移到的位置，要注意z默认为-10


    void Awake()
    {
        FinalLocation = CameraNeedToMove.transform.position;//初始化相机最终位置；这里让最终位置等于当前位置，所以相机不会移动
    }

    void Update()
    {
        //当鼠标按下的一瞬间，为TouchX赋值当前鼠标位置
        if (Input.GetMouseButtonDown(0) && CanMoveCamera)
        {   //要注意，↑↑↑是“ButtonDown”，是只会执行一次的，所以用它来记住最初位置是最合适的
            TouchX = Input.mousePosition;
        }

        //按住鼠标时
        if (Input.GetMouseButton(0) && CanMoveCamera)
        {   //若当前鼠标位置在最初鼠标位置的右侧，那么MoveLength就是正数，就会向右移动
            //若在左侧，MoveLength就是负数，就会向左移动
            //这里使用if去选择正负
            if(TouchX.x > Input.mousePosition.x) MoveLength = fGetVector3Length(TouchX-Input.mousePosition);
            else MoveLength = fGetVector3Length(TouchX - Input.mousePosition) * -1f;
        }
    }

    //这个函数是用来获取向量长度的，已知两点求距离用的
    float fGetVector3Length(Vector3 A)
    {
        return Mathf.Sqrt(Mathf.Pow(A.x, 2f) + Mathf.Pow(A.y, 2) + Mathf.Pow(A.z, 2));
    }
    
    //为什么这里要使用FixedUpdate？
    //与Update不同的是FixedUpdate不是按帧数去执行的，它是按固定时间间隔去执行的
    //所以把相机移动逻辑写在这里，移动速度就不会受帧数影响
    //打个比方，如果写在Update里，那么这相机每帧都去移动Δx，如果帧数越高，相机移动的就越快，造成了不同设备体验不同的情况
    //写在FixedUpdate里，每Δt都去移动Δx，这个Δt几乎不受任何影响，是固定的，所以这相机在任何设备上的移动速度都近乎相同
    //（Δ=delta，是非常非常小的数，趋近于0）
    //Lerp是做什么用的？Lerp要求输入两点位置和一个0~1之间的值，Lerp会返回两点之间在0%~100%的位置，比如，(A, B, 0.5f)返回的就是A和B两点中间的位置，(A, B, 1f)返回的就是B点位置。
    //另外，这家伙的中文名叫“线性插值”
    void FixedUpdate()
    {
        //如果Camera的X大于最大距离，就给它拉回来
        if (CameraNeedToMove.transform.position.x > MaxDistance)
        {
            //向左移动0.01个单位。注意这里是FixedUpdate，每隔Δt都会执行一次，所以这儿应该是一段连贯的动画
            CameraNeedToMove.transform.position -= new Vector3(0.01f, 0, 0);
            FinalLocation = CameraNeedToMove.transform.position;//让最终位置等于自己本身的位置，就是说，相机不会继续向右移动了
            MoveLength = 0;//使相机骤停，没有减速度也没有加速度
        }
        //和上面同理
        if (CameraNeedToMove.transform.position.x < 0)
        {
            CameraNeedToMove.transform.position += new Vector3(0.01f, 0, 0);
            FinalLocation = CameraNeedToMove.transform.position;
            MoveLength = 0;
        }



        //如果按住鼠标了，就让鼠标按下的最初始位置向鼠标当前位置以每次靠近10%的速度接近。
        //因为MoveLength是上述两点的距离，也被当做相机移动速度的指标，所以只要让这两点慢慢靠近，MoveLength就会越来越小，相机移动速度就会越来越慢
        //这样，就实现了向右滑动越远，相机移动速度就越快，然后速度会慢慢下降(因为MoveLength变小了)
        if (Input.GetMouseButton(0)) TouchX = Vector3.Lerp(TouchX,Input.mousePosition,0.1f);
        else MoveLength = Mathf.Lerp(MoveLength, 0f, 0.05f * SpeedDownFactor);//松开鼠标左键则让MoveLength慢慢归零

        



        //让相机向着FinalPosition以平滑的速度前进！
        if(CanMoveCamera) CameraNeedToMove.transform.position = Vector3.Lerp(CameraNeedToMove.transform.position, FinalLocation, 0.1f*SpeedUpFactor);
        else
        {
            //立刻马上停止移动
            MoveLength = 0f;
            FinalLocation = CameraNeedToMove.transform.position;
        }
        

        //让FinalPosition的X加上MoveLength
        //人类手指控制的是MoveLength大小，所以MoveLength为0时(就是鼠标初始位置等于鼠标当前位置)，相机就不会移动了。
        //简单来说，就是人类手指控制的MoveLength影响了Camera的最终位置FinalPosition。
        FinalLocation = new Vector3(FinalLocation.x + MoveLength*0.001f,FinalLocation.y,FinalLocation.z);
    }
}

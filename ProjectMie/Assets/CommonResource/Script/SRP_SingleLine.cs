using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SRP_SingleLine : MonoBehaviour
{
    [HideInInspector]
    public SRP_LineDrawer lineDrawer;
    [HideInInspector]
    public float depth = 1f;

    //LineRenderer
    [HideInInspector]
    public LineRenderer lineRenderer;
    //定义一个Vector3,用来存储鼠标点击的位置
    private Vector3 position;
    //用来索引端点
    private int index = 0;
    //端点数
    private int LengthOfLineRenderer = 0;
    //用来判断当前线条是否画完
    private bool workDone = false;

    void Awake()
    {
        //添加LineRenderer组件
        if(GetComponent<LineRenderer>() == null) lineRenderer = gameObject.AddComponent<LineRenderer>();
        else lineRenderer = GetComponent<LineRenderer>();
    }

    public void InitalLine(Material DrawMaterial,Color StartColor, Color EndColor, float StartWidth, float EndWidth, float depth)
    {
        //设置材质
        lineRenderer.material = DrawMaterial;
        //设置颜色
        lineRenderer.startColor = StartColor;
        lineRenderer.endColor = EndColor;
        //设置宽度
        lineRenderer.startWidth = StartWidth;
        lineRenderer.endWidth = EndWidth;
        //设置深度
        this.depth = depth;
    }

    void Update()
    {
        if(Input.GetMouseButton(0) && workDone == false)
        {
            AddPoint();
        }

        //连续绘制线段
        while (index < LengthOfLineRenderer)
        {
            //两点确定一条直线，所以我们依次绘制点就可以形成线段了
            lineRenderer.SetPosition(index, position);
            index++;
        }

        //换一条线
        if(Input.GetMouseButtonUp(0) && workDone == false)
        {
            workDone = true;
        }
    }

    public void AddPoint()
    {
            //预设置顶点位置
            position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, depth));

            //端点数+1
            LengthOfLineRenderer++;

            //设置线段的端点数
            lineRenderer.positionCount = LengthOfLineRenderer;
    }
}


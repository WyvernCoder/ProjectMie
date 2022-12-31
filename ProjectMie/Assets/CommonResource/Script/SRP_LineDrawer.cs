using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SRP_LineDrawer : MonoBehaviour
{
    public GameObject SingleLine;
    public float StartWidth = 0.02f;
    public float EndWidth = 0.01f;
    public Color StartColor = Color.red;
    public Color EndColor = Color.yellow;
    public Material PenMaterial = null;
    public float PenDepth = 10f;//深度为10时代表0

    [HideInInspector]
    public List<GameObject> LineList = new List<GameObject>();//线条GO的List
    
    private SRP_SingleLine cache_;

    void Update()
    {
        if(Input.GetMouseButtonDown(0)) 
        {
            //创建新线条
            cache_ = Instantiate(SingleLine).GetComponent<SRP_SingleLine>();

            //初始化线条
            cache_.InitalLine(PenMaterial,StartColor,EndColor,StartWidth,EndWidth,PenDepth);

            cache_.lineDrawer = this;

            //添加进列表
            LineList.Add(cache_.gameObject);
        }



        if(Input.GetMouseButtonUp(0))
        {
            cache_ = null;
        }
    }
}

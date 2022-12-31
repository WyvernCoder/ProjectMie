using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SRP_ShowTime : MonoBehaviour
{
    private TMP_Text TEXT;
    // Start is called before the first frame update
    void Awake()
    {
        if(gameObject.GetComponent<TMP_Text>() == null) Destroy(this);//如果被绑定的gameobject没有字组件，就自毁
        else TEXT = gameObject.GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        TEXT.text = System.DateTime.Now.Hour.ToString() + ":" + WTool.Selector(System.DateTime.Now.Minute<10,"0"+System.DateTime.Now.Minute.ToString(),System.DateTime.Now.Minute.ToString());
    }
}
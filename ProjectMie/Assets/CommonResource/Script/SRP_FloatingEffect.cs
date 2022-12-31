using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteAlways]
public class SRP_FloatingEffect : MonoBehaviour
///把我加到要移动的对象的父层级
{
    Vector3 originalpos = new Vector3();

    [Tooltip("需要漂浮的Component")]
    public GameObject floatingObject;
    
    [Tooltip("漂浮速度因数")]
    public float floatSpeed = 1f;

    [Tooltip("漂浮波峰因数")]
    public float floatA = 1f;

    float randomValue;

    void Awake()
    {
        if(floatingObject != null)
        originalpos = floatingObject.transform.position;
        randomValue = Random.Range(1,5);
    }

    void FixedUpdate()
    {
        if(floatingObject != null)
        floatingObject.transform.position = new Vector3(originalpos.x, originalpos.y + floatA * Mathf.Sin(floatSpeed*Time.fixedTime + randomValue), originalpos.z);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SRP_ShakingEffect : MonoBehaviour
{
    public bool enable = true;
    private Vector2 oLocation;
    public float shakeAmount = 1f;
    public float shakeIncrement = .5f;//震撼增量，调用CONTROL_MoreShake或CONTROL_LessShake后会为shakeAmount增加相应的增量。
    void Start()
    {
        oLocation = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(enable == false) return;
        transform.position = new Vector3(oLocation.x + UnityEngine.Random.Range(-1*shakeAmount, shakeAmount), oLocation.y + UnityEngine.Random.Range(-1*shakeAmount, shakeAmount), transform.position.z);
    }

    public void CONTROL_Shake_Enable()
    {
        enable = true;
    }

    public void CONTROL_Shake_Disable()
    {
        enable = false;
    }

    public void CONTROL_Shake_MoreShake()
    {
        shakeAmount += shakeIncrement;
    }

    public void CONTROL_Shake_LessShake()
    {
        shakeAmount -= shakeIncrement;
    }
}

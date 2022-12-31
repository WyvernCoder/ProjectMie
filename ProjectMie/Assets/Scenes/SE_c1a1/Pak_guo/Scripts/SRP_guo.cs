using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SRP_guo : MonoBehaviour
{
    public Transform rootTransform;
    protected AudioSource AS;
    protected void Awake()
    {
        AS = gameObject.AddComponent<AudioSource>();
        AS.loop = false;
        AS.playOnAwake = false;
    }
    public void CONTROL_End()
    {
        Destroy(rootTransform.gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SRP_Checker : MonoBehaviour
{
    [HideInInspector]
    public static Animation AnimC;


    void Awake()
    {
        AnimC = GetComponent<Animation>();
    }

    public static void DoRight()
    {
        AnimC.Play("doright");
    }

    public static void DoFalse()
    {
        AnimC.Play("dofalse");
    }
}

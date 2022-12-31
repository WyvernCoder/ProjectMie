using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SRP_guo3 : SRP_guo
{
    public AudioClip AC_word;
    public GameObject DrawGO;

    void FixedUpdate()
    {
        if(DrawGO == null) CONTROL_End();
    }
    public void CONTROL_PressButton()
    {
        AS.PlayOneShot(AC_word);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SRP_guo4_receiver : SRP_guo
{
    public SRP_guo4_character character;
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.GetComponent<SRP_guo4_fish>() == null)
        {
            return;
        }

        if(other.gameObject.GetComponent<SRP_guo4_fish>().wordIndex == 4) 
        {
            CONTROL_End();
        }
        else character.shaking = 1f;
    }
}

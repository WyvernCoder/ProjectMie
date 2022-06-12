using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SRP_CloudMoveSystem : MonoBehaviour
{
    public GameObject HeadScript;

    void FixedUpdate()
    {
        if(gameObject.transform.TransformPoint(gameObject.transform.position).x <= -20) 
        {
            transform.position = new Vector3(HeadScript.GetComponent<SRP_CloudGenerator>().MaxLength, gameObject.transform.position.y, gameObject.transform.position.z);
        
        }
        else gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position,new Vector3(-20f,gameObject.transform.position.y,gameObject.transform.position.z),0.1f);
    }
}
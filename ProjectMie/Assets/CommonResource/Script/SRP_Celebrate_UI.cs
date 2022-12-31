using System;
using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SRP_Celebrate_UI : MonoBehaviour
{
    public float force = 3000f;
    public float torque = 600f;
    public float gravity = 50f;
    public int wireOrder = 20;
    public RectTransform wireCollection;

    void Start()
    {
        Vector2 cacheForce;
        gameObject.GetComponent<Canvas>().sortingOrder = wireOrder;

        foreach(Transform T in wireCollection)
        {
            var PhysX = T.gameObject.AddComponent<Rigidbody2D>();
            PhysX.gravityScale = gravity;
            cacheForce = new Vector2(force * UnityEngine.Random.Range(-1f, 1f), force * UnityEngine.Random.Range(2f, 8f));
            PhysX.AddRelativeForce(cacheForce);
            PhysX.AddTorque(UnityEngine.Random.Range(torque * -1f, torque));
        }
    }

    int RandomInNOneAndOne()
    {
        switch(UnityEngine.Random.Range(-1, 2))
        {
            case -1: return -1;
            case 0: return -1;
            case 1: return 1;
            default: return 1;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SRP_c1a2Bullet : MonoBehaviour
{
    public GameObject Mesh;
    public bool Nstopmove = false;
    public float MoveSpeed = 0.1f;
    // Start is called before the first frame update
    void Awaked()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(!Nstopmove)
        transform.position = new Vector3(Mathf.MoveTowards(transform.position.x, 11f, MoveSpeed), transform.position.y, 0);
        if(transform.position.y < -7f)Destroy(gameObject);
        if(transform.position.x > 10f)Destroy(gameObject);
    }
    

}

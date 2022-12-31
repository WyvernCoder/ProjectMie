using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SRP_ElectronicTrick : MonoBehaviour
{
    public float fRotateSpeed = 0f;
    public GameObject Toy;
    public float ToySpeedScale = 0.05f;
    public GameObject Enemy;

    void Update()
    {
        if(Toy == null) return;
        Toy.transform.localScale = new Vector3(1f+0.5f*Mathf.Sin(Time.fixedTime*fRotateSpeed*0.7f), 1f, 1f);
        Camera.main.gameObject.transform.position = new Vector3(Toy.transform.position.x, Camera.main.gameObject.transform.position.y, Camera.main.gameObject.transform.position.z);
    }

    void FixedUpdate()
    {
        if(Toy == null) return;
        fRotateSpeed = Mathf.Lerp(fRotateSpeed, 0f, 0.05f);
        Toy.transform.position = Vector3.Lerp(Toy.transform.position, new Vector3(Toy.transform.position.x + fRotateSpeed, Toy.transform.position.y, 0f), ToySpeedScale);
        if(Enemy.GetComponent<SRP_SpeedTracker>().seekermode == true) Toy.transform.position = Vector3.Lerp(Toy.transform.position, new Vector3(Toy.transform.position.x, 0f, Toy.transform.position.z), 0.5f);
    }

    public void CONTROL_GiveThumb()
    {
        if(Toy == null) return;
        fRotateSpeed++;
    }


}

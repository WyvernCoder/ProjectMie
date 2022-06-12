using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SRP_HorrorEgg : MonoBehaviour
{
    //自身Scale必须为1
    public AudioClip AC;
    public Sprite IMG;
    public int ACNum = 5;
    private AudioSource AS;
    void Start()
    {
        AS = gameObject.AddComponent<AudioSource>();
    }

    public void CONTROL_Click()
    {
        var SR = gameObject.AddComponent<SpriteRenderer>();
        gameObject.AddComponent<SRP_SpriteFitter>();
        SR.sprite = IMG;
        SR.sortingOrder = 999;
        for(int i=0;i<ACNum;i++) AS.PlayOneShot(AC);
        Destroy(gameObject,2.5f);
    }
}

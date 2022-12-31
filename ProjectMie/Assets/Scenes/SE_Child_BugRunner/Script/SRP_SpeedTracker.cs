using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SRP_SpeedTracker : MonoBehaviour
{
    private float Speed;
    private float difficlut = 1f;
    private float anim = 1f;
    public GameObject playerr;
    public SpriteRenderer background;
    public bool seekermode = false;
    public AudioClip horn;
    public AudioClip die_music;
    public AudioClip nemmusic;
    public AudioClip die;
    public AudioClip bye;
    public AudioClip come;
    void Start()
    {

    }

    void FixedUpdate()
    {
        if(playerr == null) return;
        transform.localScale = new Vector3(1f+0.5f*Mathf.Sin(Time.fixedTime*5f*difficlut*anim), 1f, 1f);
        transform.position += new Vector3(Time.deltaTime*5*difficlut, 0f);
        if(seekermode == false) 
        {
            difficlut = Mathf.Clamp(playerr.transform.position.x - transform.position.x, 1f, 1.1f);
            if(difficlut >= 1.05f) BYE();
        }
        else 
        {
            HORN();
            StartCoroutine(ScaryTime());
            transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, 0f, transform.position.z), 0.5f);
            difficlut = 1.8f;
            anim = 10f;
            if(playerr.transform.position.x - transform.position.x < 8f) 
            {
                Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, 3.2f, 0.1f);
                toggleBackColor();
                COME();
            }
            else Camera.main.orthographicSize = 5.4f;
            if(playerr.transform.position.x - transform.position.x < 1f) 
            {
                DIEE();
                Destroy(playerr);
            }
        }
        if(playerr.transform.position.x - transform.position.x > 10f) seekermode = true;

    }

    public void toggleBackColor()
    {
        if(background.color == Color.black) background.color = Color.red;
        else background.color = Color.black;
    }

    bool doonce = false;
    public void BYE()
    {
        if(doonce) return;
        gameObject.GetComponent<AudioSource>().PlayOneShot(bye);
        doonce = true;
    }

    bool doonce1 = false;
    public void HORN()
    {
        if(doonce1) return;
        gameObject.GetComponent<AudioSource>().PlayOneShot(horn);
        doonce1 = true;
    }

    bool doonce2 = false;
    public void DIEE()
    {
        if(doonce2) return;
        doonce2 = true;
        APIManager.API.API_Toy_Celebrate();
        gameObject.GetComponent<AudioSource>().PlayOneShot(die);
        Invoke("BBBBB",2f);
    }

    bool doonce3 = false;
    public void COME()
    {
        if(doonce3) return;
        gameObject.GetComponent<AudioSource>().PlayOneShot(come);
        doonce3 = true;
    }

    public void BBBBB()
    {
        
        gameObject.GetComponent<AudioSource>().PlayOneShot(die_music);
        //APIManager.API.API_LoadScene_SetName("SE_c1a1");
        APIManager.API.API_FailTheGame();
    }

    bool aaaaa = false;
    IEnumerator ScaryTime()
    {
        if(aaaaa == true) yield break;
        aaaaa = true;
        int timer = 0;
        while(timer<=10)
        {
            timer++;
            yield return new WaitForSeconds(1);
        }
        difficlut = 0;
        //APIManager.API.API_LoadScene_SetName("SE_c1a1");
        APIManager.API.API_WinTheGame();
        yield break;
    }
}
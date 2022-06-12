using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Monkee的动画背景，这里没屌用
/// </summary>
public class SRP_GameTitleController : MonoBehaviour
{
    public GameObject bg;
    public GameObject monkey;
    public GameObject blackBG;
    public GameObject anim;
    private void Start()
    {
        gameObject.GetComponent<Animation>().Play();
        gameObject.GetComponent<Animation>().PlayQueued("FadeAnimation2");
        Invoke("RemoveSelf", 6.8f);
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        anim.transform.position = new Vector3(Random.Range(-.1f, .1f), Random.Range(-.1f, .1f)) + monkey.transform.position;
        //anim.transform.localScale = new Vector3()
    }

    private void RemoveSelf()
    {
        //gameObject.GetComponent<Animation>().Stop();
        monkey.transform.position = new Vector3(2.62f,0,0);
    }
}

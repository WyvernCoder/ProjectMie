using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SRP_ShowWifiStatue : MonoBehaviour
{
    public List<Sprite> StatueImage = new List<Sprite>();
    private Image IMAGE;
    
    // Start is called before the first frame update
    void Awake()
    {
        if(gameObject.GetComponent<Image>() == null) Destroy(this);//如果被绑定的gameobject没有组件，就自毁
        else IMAGE = gameObject.GetComponent<Image>();

        if(StatueImage.Count != 4) Destroy(this);//必须四张图片，写死的
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(CheckServerConnection());
    }

    IEnumerator CheckServerConnection()//检测php服务器连接状态
    {
        Ping ping = new Ping(WTool.StringChop(WTool.ParseStringIntoArray(APIManager.API.HttpServer,':')[1],2,true)); //ping实例
        float timer = 0f;
        while (timer <= 3f && !ping.isDone)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        if(ping.isDone == false || ping.time == -1)
        {
            print("连接超时。" + ping);
            IMAGE.sprite = StatueImage[StatueImage.Count - 1];
        }
        else UpdateImage(timer);
        
        ping.DestroyPing();
        yield return new WaitForSeconds(5f);
        StartCoroutine(CheckServerConnection());


        yield break;
    }

    void UpdateImage(float PingInSecond)
    {
        // 0.05以下BEST
        // 0.3以下GOOD
        // 3以下BAD
        // 超时NO
        if(PingInSecond <= 0.05f) IMAGE.sprite = StatueImage[0];
        if(PingInSecond <= 0.3f && PingInSecond > 0.05f) IMAGE.sprite = StatueImage[1];
        if(PingInSecond <= 3.0f && PingInSecond > 0.3f) IMAGE.sprite = StatueImage[2];
        if(PingInSecond > 3.0f) IMAGE.sprite = StatueImage[3];
    }
}

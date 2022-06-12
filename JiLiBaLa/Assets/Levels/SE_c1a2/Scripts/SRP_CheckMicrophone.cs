using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SRP_CheckMicrophone : MonoBehaviour
{
    public Image micimage;
    public Text statuetextt;
    public Image backgroundi;
    public Button returntomenubtn;
    public Button recheckmicbtn;
    private Vector3 originalloc;
    public GameObject SceneManager;
    public AudioClip AChao;
    private bool isok = false;
    void Start()
    {
        originalloc = micimage.transform.position;
        StartCoroutine(Starttt());
        if(GameManager.Instance != null) Instantiate(GameManager.Instance.BlackOut);
    }
    /// <summary>
    /// 1 month 29 days , seele wont COLOR COLOUR
    /// </summary>
    /// <returns></returns>
    IEnumerator Starttt()
    {
        yield return new WaitForSeconds(2f);
        if (Microphone.devices.Length == 0)
        {
            statuetextt.text = "未检测到任何麦克风设备";
            isok = false;
            yield break;
        }
        statuetextt.text = "已检测麦克风设备：" + Microphone.devices[0];
        GetComponent<AudioSource>().PlayOneShot(AChao);
        isok = true;
        yield return new WaitForSeconds(2f);
        if(GameManager.Instance != null) Instantiate(GameManager.Instance.BlackOut);
        SceneManager.SetActive(true);
        SceneManager.GetComponent<SRP_c1a2Manager>().sMicrophoneName = Microphone.devices[0];
        Destroy(gameObject);
    }

    public void CONTROLL_BackToMenu()
    {
        GameManager.Instance.PlayButtonSound();
        if(GameManager.Instance != null) GameManager.Instance.BackToMainmenu();
    }

    public void CONTROLL_RecheckMic()
    {
        GameManager.Instance.PlayButtonSound();
        isok = false;
        statuetextt.text = "正在检测可用麦克风设备...";
        StopAllCoroutines();
        StartCoroutine(Starttt());
    }

    // Update is called once per frame
    void Update()
    {
        if(!isok)
        {
            micimage.transform.position = new Vector3(originalloc.x+Random.Range(-20, 20), originalloc.y+Random.Range(-20, 20), micimage.transform.position.z);
            micimage.transform.localScale = new Vector3(1, 1, 1);
        }

        if (isok)
        {
            micimage.transform.position = originalloc;
            micimage.transform.localScale = new Vector3(Random.Range(0.5f, 1.5f), Random.Range(0.5f, 1.5f), micimage.transform.position.z);
        }
    }
}

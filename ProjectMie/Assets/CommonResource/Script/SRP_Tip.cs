using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SRP_Tip : MonoBehaviour
{
    public TMPro.TMP_Text TEXT;
    public GameObject RootComponent;
    public CanvasGroup CG_IMAGE;
    public string Text = "字幕";
    public float TextDelay = 0.1f;
    public AudioClip AC_Voice;
    private AudioSource AS;
    public bool bHideImage = true;
    [HideInInspector]public bool bShouldEndWhenNextHide = false;
    public float FadeFactor = 0.5f;
    

    void Awake()
    {
        if(TEXT == null || CG_IMAGE == null || RootComponent == null)
        {
            print("必备组件未选择，停止指引。");
            Destroy(this);
        }

        TEXT.text = "";//初始化对话框文本
        if(bHideImage == true)
        {
            CG_IMAGE.alpha = 0;
            bHideImage = false;
        }

        if(AC_Voice != null) AS = gameObject.AddComponent<AudioSource>();
    }

    void Start()
    {
        StartCoroutine(Print_IE());
        StartCoroutine(CG_IE());
    }

    IEnumerator Print_IE()
    {
        List<string> SingleCharList = new List<string>();

        foreach(char C in Text)SingleCharList.Add(C.ToString());

        if(AS != null && AC_Voice != null)
        {
            AS.clip = AC_Voice;
            AS.Play();
        }

        foreach(string S in SingleCharList)
        {
            TEXT.text += S;
            yield return new WaitForSeconds(TextDelay);
        }

        yield return new WaitForSeconds(5f);

        TEXT.text = "";
        bHideImage = true;
        bShouldEndWhenNextHide = true;

        yield break;
    }

    IEnumerator CG_IE()
    {
        while(true)
        {
            if(bHideImage == true) 
            {
                CG_IMAGE.alpha -= Time.deltaTime * FadeFactor;
                if(bHideImage == true && bShouldEndWhenNextHide == true && CG_IMAGE.alpha <= 0.05f) Destroy(RootComponent, AS.time);
            }
            else CG_IMAGE.alpha += Time.deltaTime * FadeFactor;

            yield return null;
        }
    }
}

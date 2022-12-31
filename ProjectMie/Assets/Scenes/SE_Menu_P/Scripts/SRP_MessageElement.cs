using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SRP_MessageElement : MonoBehaviour
{
    public Sprite YES;
    public Sprite NO;
    public Color MessageColor = new Color(255f,233f,213f);
    public float LifeTime = 2f;
    public Image ICON;
    public TMPro.TMP_Text TEXT;
    private bool isClear = false;//如果为true就会开始强制消失

    void Start()
    {
        if(ICON == null || TEXT == null)
        {
            print("元素组件不完整。");
            Destroy(this);
        }

        GetComponent<CanvasGroup>().alpha = 0f;
        //InitalMessage(true, "???", 10f);  测试用
    }

    public void IAmClear() => isClear = true;

    public void InitalMessage(bool isOK, string message, float LifeTime = 2f)
    {
        if(isOK) 
        {
            ICON.color = new Color(0.4470588f,1,0.4470588f);
            ICON.sprite = YES;
        }
        else 
        {
            ICON.color = new Color(1,0.4470588f,0.4470588f);
            ICON.sprite = NO;
        }
        
        TEXT.text = message;
        this.LifeTime = LifeTime;
        StartCoroutine(Go());
    }

    IEnumerator Go()
    {
        yield return null;
        var CG = gameObject.GetComponent<CanvasGroup>();

        bool isOpening = true;
        float timer = 0f;
        while(true)
        {
            if(isOpening) CG.alpha += 0.1f;
            else CG.alpha -= 0.1f;

            if(timer >= LifeTime/2f || isClear) isOpening = false;

            if(CG.alpha < 0.1f) break;

            timer += Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
        }
        Destroy(gameObject);
        yield break;
    }
    
}

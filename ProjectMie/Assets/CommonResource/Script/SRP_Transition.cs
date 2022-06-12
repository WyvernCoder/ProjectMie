using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SRP_Transition : MonoBehaviour
{
    private Animation aAnimationComponent;
    private float lifetimer = 0f;
    private Canvas animCanvas;
    public AudioClip Sounddd;//拖拽赋值，开始转场时会放这个声音

    void Awake()
    {
        aAnimationComponent = GetComponent<Animation>();
        DontDestroyOnLoad(this);
        animCanvas = gameObject.GetComponent<Canvas>();
        animCanvas.enabled = false;//彩虹的初始位置太难看了，等开始播放再显示出来
        StartCoroutine(Timer_IE());//启动计时器
        gameObject.AddComponent<AudioSource>();//添加一个AudioSource用于播放声音
    }

    IEnumerator Timer_IE()//Start和End动画都是0.5秒，如果Start没有播放完成就去播放End动画，那么这里会计算出Start动画的剩余时间，这样就能准确算出End应该在几秒后播放了，实现流畅的动画效果
    {
        while(true)
        {
            yield return new WaitForSeconds(0.1f);
            lifetimer += 0.1f;
        }
    }

    public GameObject StartTrans()
    {
        gameObject.GetComponent<AudioSource>().PlayOneShot(Sounddd);
        aAnimationComponent.Play("transition");
        animCanvas.enabled = true;
        lifetimer = 0f;//EndTrans要的是Start之后的秒数所以要归零
        return gameObject;
    }

    public GameObject EndTrans()
    {
        if(lifetimer < 0.5f)//如果Start动画正在播放，就延后几秒再调用这个函数
        {
            Invoke("EndTrans",0.5f - lifetimer);
            return gameObject;
        }
        aAnimationComponent.Play("transition_end");
        animCanvas.enabled = true;
        Destroy(gameObject,0.55f);
        return gameObject;
    }
}

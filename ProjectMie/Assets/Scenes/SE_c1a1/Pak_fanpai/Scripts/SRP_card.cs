using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SRP_card : MonoBehaviour
{
    public Sprite ok;
    public Sprite no;
    public Sprite forwardCard;
    public Sprite backwardCard;
    public Image gradeIMG;
    public CanvasGroup gradeCG;
    public float rotateSpeed = 0.5f;
    public int cardType = -1;
    [HideInInspector] public int cardIndex = -1;
    [HideInInspector] public SRP_cardManager Master;//记录卡牌管理器以便于和它交流卡牌选取信息
    private Image selfImage;
    private float targetRotate = 0f;
    private float targetAlpha = 1f;//控制gradeCG的透明度，当它被设为0时，就会显示成绩

    void Start()
    {
        selfImage = gameObject.GetComponent<Image>();//确定Image组件

        if(selfImage == null)//如果被绑定到的gameObject没有Image组件
        {
            print("你把我放错地方了，傻逼。");
            Destroy(gameObject);//把整个gameObject都移除以表示愤怒！！
        }

        if(gradeCG == null)
        {
            print("没有拖拽赋值成绩结果图片的CG！");
            Destroy(gameObject);
        }

        StartCoroutine(rotate_IE());
        StartCoroutine(cgUpdate_IE());
    }

    IEnumerator rotate_IE()
    {
        float selfRotate;//我们旋转图片，只需要转transform的Pitch，也就是该rotation的Y值。

        while(true)
        {
            selfRotate = transform.rotation.eulerAngles.y;//更新当前Pitch旋转度。

            //如果当前Pitch旋转度小于90，就把Image图片设为背面图片；大于90就设为正面图片。
            if(selfRotate <= 90f) selfImage.sprite = backwardCard;
            else selfImage.sprite = forwardCard;

            //旋转gameObject
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, Mathf.Lerp(selfRotate, targetRotate, rotateSpeed * Time.deltaTime ), transform.rotation.eulerAngles.z);

            //等待1帧。如果不等待就变成死循环卡死游戏了。
            yield return null;
        }
    }

    IEnumerator cgUpdate_IE()
    {
        while(true)
        {
            if(targetAlpha < 1) targetAlpha += 1f * Time.deltaTime;
            gradeCG.alpha = Mathf.Lerp(0, 1, 2*Mathf.Sin(Mathf.PI*targetAlpha));
            yield return null;
        }
    }

    //该协程用于测试，效果是每3秒换一面
    IEnumerator test_IE()
    {
        TurnToForward();
        yield return new WaitForSeconds(3f);
        TurnToBackward();
        yield return new WaitForSeconds(3f);
        StartCoroutine(test_IE());
    }

    //转到正面
    public void TurnToForward()
    {
        targetRotate = 180f;
    }

    //转到背面
    public void TurnToBackward()
    {
        targetRotate = 0f;
    }

    public void CONTROL_Click()
    {
        if(Master.secondSelect != -1) return;//如果第二张卡牌序号已被赋值，说明正在翻牌，就直接return

        if(Master.firstSelect == cardIndex) return;//解决掉自己选自己也能识别成正确的情况

        if(Master.canSelect == false) return;//当canSelect为false时就拒绝点击，用于等待 比对结果动画 播放完成。

        if(Master.firstSelect == -1) Master.firstSelect = cardIndex;
        else if(Master.secondSelect == -1) Master.secondSelect = cardIndex;

        TurnToForward();
    }

    public void OK() => StartCoroutine(OK_IE());

    public void NO() => StartCoroutine(NO_IE());

    IEnumerator OK_IE()
    {
        print("比对成功，YES！");
        targetAlpha = 0f;
        gradeIMG.sprite = ok;
        yield return new WaitForSeconds(1.5f);
        Master.canSelect = true;
        Master.correctCount++;
        yield break;
    }

    IEnumerator NO_IE()
    {
        print("比对失败，傻逼！");
        targetAlpha = 0f;
        gradeIMG.sprite = no;
        yield return new WaitForSeconds(1.5f);
        TurnToBackward();
        Master.canSelect = true;
        yield break;
    }
}

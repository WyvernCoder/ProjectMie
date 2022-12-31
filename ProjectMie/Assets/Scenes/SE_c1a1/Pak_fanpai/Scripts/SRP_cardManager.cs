using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SRP_cardManager : MonoBehaviour
{
    public Transform rootTransform;
    [HideInInspector] public List<SRP_card> cardList = new List<SRP_card>();//记录卡牌的列表
    [HideInInspector] public int firstSelect = -1;//记录第一次翻开的卡牌
    [HideInInspector] public int secondSelect = -1;//记录第二次翻开的卡牌
    [HideInInspector] public bool canSelect = true;//判断当前是否能够点击下一张卡牌
    [HideInInspector] public int correctCount = 0;//判断当前是否能够点击下一张卡牌

    void Start()
    {
        //初始化卡牌列表数组
        SRP_card cache;
        int index = 0;

        foreach(Transform T in transform)
        {
            cache = null;
            cache = T.gameObject.GetComponent<SRP_card>();

            if(cache == null) 
            {
                print("卡牌Collection混入了非卡牌，你倒是好好管管啊，傻逼。");
                Destroy(gameObject);//摧毁整个卡牌Collection以示愤怒！！
            }

            cache.Master = this;//初始化卡牌的Master
            cache.cardIndex = index;//初始化卡牌的index
            cardList.Add(cache);//把卡牌加进去
            index++;
        }

        //随机卡牌位置
        for(int i = 0; i < cardList.Count; i++)
        {
            SweepGOLocation(cardList[UnityEngine.Random.Range(0, cardList.Count)].gameObject, cardList[UnityEngine.Random.Range(0, cardList.Count)].gameObject);
        }

        //启动卡牌对比协程
        StartCoroutine(compareCard());
        StartCoroutine(checkWin());
    }

    IEnumerator checkWin()
    {
        while(true)
        {
            if(correctCount >= cardList.Count) break;
            yield return new WaitForFixedUpdate();
        }

        if(APIManager.API != null) APIManager.API.API_Toy_Celebrate();
        print("游戏胜利！");

        //物理特效
        float force = 10000f;
        foreach(var G in cardList)
        {
            var PhysX = G.gameObject.AddComponent<Rigidbody2D>();
            PhysX.gravityScale = 80f;
            PhysX.AddTorque(360f*UnityEngine.Random.Range(-1f,1f));
            PhysX.AddForce(new Vector2(force*UnityEngine.Random.Range(-1f,1f), force*UnityEngine.Random.Range(0.5f,1f)));
        }

        yield return new WaitForSeconds(3f);
        Destroy(rootTransform.gameObject);

        yield break;
    }

    IEnumerator compareCard()
    {
        while(true)
        {
            //未选择first和second或canSelect为false时就什么都不执行
            if(firstSelect == -1 || secondSelect == -1 || canSelect == false) 
            {
                yield return new WaitForFixedUpdate();
                continue;
            }

            canSelect = false;//禁止选择其他卡牌，因为需要等待比对动画完成。

            if(cardList[firstSelect].cardType == cardList[secondSelect].cardType)
            {
                //正确
                cardList[firstSelect].OK();
                cardList[secondSelect].OK();
            }
            else
            {
                //错误
                cardList[firstSelect].NO();
                cardList[secondSelect].NO();
            }

            //比对完成后，初始化。
            firstSelect = -1;
            secondSelect = -1;

            yield return new WaitForFixedUpdate();
        }
    }

    public void CONTROL_Cheat()
    {
        for(int i=0; i<cardList.Count; i++)
        {
            cardList[i].TurnToForward();
            cardList[i].OK();
        }

        canSelect = false;
    }

    /// <summary>
    /// 交换A、B的位置
    /// </summary>
    /// <param name="A"></param>
    /// <param name="B"></param>
    void SweepGOLocation(GameObject A, GameObject B)
    {
        Vector3 cache = A.transform.position;
        A.transform.position = B.transform.position;
        B.transform.position = cache;
    }
}

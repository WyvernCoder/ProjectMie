using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using Function;

public class RankColliderManager : MonoBehaviour
{
    public int maxCount;
    #region 参数
    /// <summary>
    /// 存储拖拽类
    /// </summary>
    public bool[] isHaveCard;
    //public List<bool> isHaveCard = new List<bool>();
    public GameObject rootGameObject;
    #endregion


    #region 私有方法
    /// <summary>
    /// 碰撞检测
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerEnter2D(Collider2D other)//碰撞器
    {

        if (!isHaveCard[0])
        {
            if (other.gameObject.name == "frameToImage")
            {
                isHaveCard[0] = true;
            }
        }
        if (!isHaveCard[1])
        {
            if (other.gameObject.name == "frameToImage0")
            {
                isHaveCard[1] = true;
            }
        }
        if (!isHaveCard[2])
        {
            if (other.gameObject.name == "frameToImage1")
            {
                isHaveCard[2] = true;
            }
        }
        if (!isHaveCard[3])
        {
            if (other.gameObject.name == "frameToImage2")
            {
                isHaveCard[3] = true;
            }
        }
        if (!isHaveCard[4])
        {
            if (other.gameObject.name == "frameToImage3")
            {
                isHaveCard[4] = true;
            }
        }
        if (!isHaveCard[5])
        {
            if (other.gameObject.name == "frameToImage4")
            {
                isHaveCard[5] = true;
            }
        }
        if (!isHaveCard[6])
        {
            if (other.gameObject.name == "frameToImage5")
            {
                isHaveCard[6] = true;
            }
        }
    }

    /// <summary>
    /// 离开检测
    /// </summary>
    void OnTriggerExit2D(Collider2D other)
    {
        if (isHaveCard[0])
        {
            if (other.gameObject.name == "frameToImage")
            {
                isHaveCard[0] = false;
            }
        }
        if (isHaveCard[1])
        {
            if (other.gameObject.name == "frameToImage0")
            {
                isHaveCard[1] = false;
            }
        }
        if (isHaveCard[2])
        {
            if (other.gameObject.name == "frameToImage1")
            {
                isHaveCard[2] = false;
            }
        }
        if (isHaveCard[3])
        {
            if (other.gameObject.name == "frameToImage2")
            {
                isHaveCard[3] = false;
            }
        }
        if (isHaveCard[4])
        {
            if (other.gameObject.name == "frameToImage3")
            {
                isHaveCard[4] = false;
            }
        }
        if (isHaveCard[5])
        {
            if (other.gameObject.name == "frameToImage4")
            {
                isHaveCard[5] = false;
            }
        }
        if (isHaveCard[6])
        {
            if (other.gameObject.name == "frameToImage5")
            {
                isHaveCard[6] = false;
            }
        }
    }
    void Update()
    {
        var imgList1 = FindObjectOfType<RankColliderManager>().isHaveCard[1];
        var imgList4 = FindObjectOfType<RankColliderManager>().isHaveCard[4];
        //print(imgList1+"=imgList1");
        //print(imgList4+"=imgList4");
        //print(isHaveCard[1]+"=isHaveCard[1]");
        //print(isHaveCard[4]+"=isHaveCard[4]");
        if (isHaveCard[1] == true & isHaveCard[4] == true)
        {
            print("1");
            Destroy(rootGameObject);
                
        }
        else
        {
            //print("0");
        }
    }
   
    #endregion
}

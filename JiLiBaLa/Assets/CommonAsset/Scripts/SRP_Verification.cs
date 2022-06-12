using System.Net;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.IO;

public class SRP_Verification : MonoBehaviour
{
    public GameObject AnimalHead;
    public GameObject TargetLocation;
    public float ForcePower = 100f;

    private bool goDrag = false;
    
    [HideInInspector]
    public static bool superCore = false;

    private bool doonce = false;

    void FixedUpdate()
    {
        //设置位置为鼠标位置
        //if(goDrag) AnimalHead.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition + new Vector3(0,0,10)); //暴力位移
        if(goDrag || superCore) AnimalHead.GetComponent<Rigidbody2D>().AddForce(GetVectorDirection(Camera.main.ScreenToWorldPoint(Input.mousePosition) - AnimalHead.transform.position) * ForcePower); 
        
        //到达TargetPosition后就司马
        if(GetVector3Length(AnimalHead.transform.position - TargetLocation.transform.position) <= 0.5f && AnimalHead.transform.rotation.eulerAngles.z <= 200f &&AnimalHead.transform.rotation.eulerAngles.z >= 165f) 
        {
            //!胜利后执行的内容
            if(doonce == false)
            {
                doonce = true;
                //StartCoroutine(GoToMenu());
                GameManager.Instance.WinTheMission();
            }
        }

        //掉出屏幕外就重来
        if(AnimalHead.transform.position.y >= Camera.main.orthographicSize ) 
        {
            AnimalHead.transform.position = new Vector3(AnimalHead.transform.position.x, Camera.main.orthographicSize * -1f + 0.05f, 0f);
            CONTROL_StopDragHead();
        }

        if(AnimalHead.transform.position.y <= Camera.main.orthographicSize * -1 )
        {
            AnimalHead.transform.position = new Vector3(AnimalHead.transform.position.x, Camera.main.orthographicSize - 0.05f, 0f);
            CONTROL_StopDragHead();
        } 
        
        if(AnimalHead.transform.position.x >= Camera.main.orthographicSize * Screen.width/Screen.height ) 
        {
            AnimalHead.transform.position = new Vector3(AnimalHead.transform.position.x - Camera.main.orthographicSize * Screen.width/Screen.height * 2 + 0.05f, AnimalHead.transform.position.y, 0f);
            CONTROL_StopDragHead();
        }

        if(AnimalHead.transform.position.x <= Camera.main.orthographicSize * Screen.width/Screen.height * -1 ) 
        {
            AnimalHead.transform.position = new Vector3(AnimalHead.transform.position.x + Camera.main.orthographicSize * Screen.width/Screen.height * 2 - 0.05f, AnimalHead.transform.position.y, 0f);
            CONTROL_StopDragHead();
        }
    }

    private float GetVector3Length(Vector3 vec)
    {
        return Mathf.Sqrt(Mathf.Pow(vec.x,2f)+Mathf.Pow(vec.y,2f)+Mathf.Pow(vec.z,2f));
    }

    private Vector3 GetVectorDirection(Vector3 vec)
    {
        float length = GetVector3Length(vec);//求模
        return new Vector3(vec.x/length, vec.y/length, vec.z/length);
    }

    public void CONTROL_DragHead()
    {
        goDrag = true;
    }

    public void CONTROL_StopDragHead()
    {
        goDrag = false;
        superCore = false;
    }

    public void CONTROL_DragAllHead()
    {
        if(superCore == true) superCore = false;
        else superCore = true;
    }


    IEnumerator GoToMenu()
    {
        var L = GameManager.Instance.DoLoading();
        var SM = SceneManager.LoadSceneAsync("SE_Mainmenu_P",LoadSceneMode.Additive);
        while(SM.progress < 0.9) yield return null;
        yield return new WaitForSeconds(1f);
        Destroy(L);
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        yield break;
    }
}
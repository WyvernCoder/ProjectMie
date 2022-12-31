using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SRP_videoSelectElement : MonoBehaviour
{
    public string URL;
    public string Title = "标题";
    public string Describe = "简介";

    public void CONTROL_JoinIntoDetail()
    {
        FindObjectOfType<SRP_Menu_P_Manager>().CONTROL_C_VideoDetailPage(URL, Title, Describe);
    }
}

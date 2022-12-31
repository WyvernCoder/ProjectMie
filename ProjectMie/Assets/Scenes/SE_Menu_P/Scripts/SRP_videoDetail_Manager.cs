using System.Security.Cryptography;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SRP_videoDetail_Manager : MonoBehaviour
{
    public TMPro.TMP_Text TITLE;
    public TMPro.TMP_Text DESCRIBE;

    public void Init(string URL, string Title, string Describe)
    {
        TITLE.text = Title;
        DESCRIBE.text = Describe;
        FindObjectOfType<SRP_Menu_P_Manager>().CONTROL_GoToVideoPlayPage(URL,"NULL","NULL", 12);
    }
}

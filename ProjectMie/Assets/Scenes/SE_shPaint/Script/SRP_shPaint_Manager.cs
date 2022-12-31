using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SRP_shPaint_Manager : MonoBehaviour
{
    void Start()
    {
        APIManager.API.API_RotateScreen(true);
    }
    public void CONTROL_Return()
    {
        APIManager.API.API_BackToMenu(true);
    }
}

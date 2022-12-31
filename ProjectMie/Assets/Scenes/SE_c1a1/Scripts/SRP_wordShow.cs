using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SRP_wordShow : MonoBehaviour
{
    public void CONTROL_BackToMenu()
    {
        APIManager.API.API_BackToMenu();
    }
    public void CONTROL_OK()
    {
        Destroy(gameObject);
    }
}

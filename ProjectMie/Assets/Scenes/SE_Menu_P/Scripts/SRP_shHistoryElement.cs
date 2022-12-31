using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SRP_shHistoryElement : MonoBehaviour
{
    public void CONTROL_Click()
    {
        FindObjectOfType<SRP_Menu_P_Manager>().CONTROL_FunctionNotReady();
    }
}

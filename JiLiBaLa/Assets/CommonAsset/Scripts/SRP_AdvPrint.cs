using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SRP_AdvPrint : MonoBehaviour
{
    public Text text;
    public void AdvPrint(string A) => text.text = A;
    void Start()
    {
        Destroy(gameObject,100f);
    }
}

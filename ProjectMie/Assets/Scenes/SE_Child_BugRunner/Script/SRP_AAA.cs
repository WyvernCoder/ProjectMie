using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SRP_AAA : MonoBehaviour
{
    public GameObject ENEMY;
    public Text HELP;

    void Start()
    {
        ENEMY.SetActive(false);
        ENEMY.GetComponents<AudioSource>()[1].Pause();
    }

    bool doonce = false;
    public void CONTROL_PLAY()
    {
        if(doonce) return;
        doonce = true;
        ENEMY.SetActive(true);
        HELP.enabled = false;
        ENEMY.GetComponents<AudioSource>()[1].Play();
    }
}

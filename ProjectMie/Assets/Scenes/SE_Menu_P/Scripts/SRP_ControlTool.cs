using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SRP_ControlTool : MonoBehaviour
{
    public Image PLAYControl;
    public Slider SLIDER;
    public Sprite play_img;
    public Sprite pause_img;
    private UnityEngine.Video.VideoPlayer VP;
    private CanvasGroup CG;
    private bool isFadeing = false;

    void Start()
    {
        CG = GetComponent<CanvasGroup>();
        VP = GameObject.Find("Menu_P_Manager").GetComponent<UnityEngine.Video.VideoPlayer>();
        if(VP == null) Destroy(this);
        StartCoroutine(UpdateSlider());
        StartCoroutine(UpdateAnim());
    }

    IEnumerator UpdateSlider()
    {
        if(VP == null) 
        {
            yield return new WaitForFixedUpdate();
            StartCoroutine(UpdateSlider());
        }
        SLIDER.value = (float)VP.frame/(float)VP.frameCount;
        
        yield return new WaitForFixedUpdate();
        StartCoroutine(UpdateSlider());
    }

    IEnumerator UpdateAnim()
    {
        while(true)
        {
            if(isFadeing) CG.alpha -= 5f * Time.deltaTime;
            else CG.alpha += 5f * Time.deltaTime;
            yield return null;
        }
    }

    public void CONTROL_TogglePlayVideo()
    {
        if(VP.isPaused == true) 
        {
            PLAYControl.sprite = play_img;
            VP.Play();
        }
        else
        {
            PLAYControl.sprite = pause_img;
            VP.Pause();
        }
    }

    public void CONTROL_ToggleFade()
    {
        isFadeing = !isFadeing;
    }
}

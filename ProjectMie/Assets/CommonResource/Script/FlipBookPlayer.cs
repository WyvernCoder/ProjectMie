using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipBookPlayer : MonoBehaviour
{
    public float speed = 0.1f;
    public bool playOnAwake = true;
    public bool loopPlay = false;
    public List<Sprite> TextureList = new List<Sprite>();
    private SpriteRenderer SR;
    private UnityEngine.UI.Image IR;
    private bool isPlay = true;

    void Start()
    {
        SR = GetComponent<SpriteRenderer>();
        if(SR == null) IR = GetComponent<UnityEngine.UI.Image>();
        if(SR == null && IR == null) Destroy(this);
        if(playOnAwake == true) isPlay = true;
        else isPlay = false;
        StartCoroutine(Player());
    }

    IEnumerator Player()
    {
        int index = 0;

        while(true)
        {
            if(SR == null) break;

            if(isPlay == false)
            {
                yield return null;
                continue;
            }

            if(index < TextureList.Count)
                SR.sprite = TextureList[index];
            index++;
            if(loopPlay == false)
            {
                if (index == TextureList.Count) TogglePlay(false);
            }
            else index %= TextureList.Count;
            yield return new WaitForSeconds(speed);
        }

        while(true)
        {
            if(IR == null) break;

            if(isPlay == false)
            {
                yield return null;
                continue;
            }

            if(index < TextureList.Count)
                IR.sprite = TextureList[index];
            index++;
            if(loopPlay == false)
            {
                if (index == TextureList.Count) TogglePlay(false);
            }
            else index %= TextureList.Count;
            yield return new WaitForSeconds(speed);
        }
    }

    public void TogglePlay(bool isPlay)
    {
        this.isPlay = isPlay;
        if(isPlay) StartCoroutine(Player());
    }

    public void ResetImage()
    {
        TogglePlay(false);
        if(SR != null)
        SR.sprite = TextureList[0];
        if(IR != null)
        IR.sprite = TextureList[0];
    }
}

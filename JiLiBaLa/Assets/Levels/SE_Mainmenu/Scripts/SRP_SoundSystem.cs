using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SRP_SoundSystem : MonoBehaviour
{
    public List<AudioClip> AlertList = new List<AudioClip>();
    public List<AudioClip> WindList = new List<AudioClip>();

    void Awake()
    {
        StartCoroutine("Wind");
        StartCoroutine("Alert");
    }

    IEnumerator Wind()
    {
        while(true)
        {
            PlayClip(WindList[Random.Range(0,WindList.Count)]);
            yield return new WaitForSeconds(Random.Range(5f,10f));
        }
    }

    IEnumerator Alert()
    {
        while(true)
        {
            PlayClip(AlertList[Random.Range(0,AlertList.Count)]);
            yield return new WaitForSeconds(Random.Range(12f,20f));
        }
    }









    private void PlayClip(AudioClip AC) => GetComponent<AudioSource>().PlayOneShot(AC);
}

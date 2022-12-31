using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SRP_guo2 : SRP_guo
{
    public GameObject WordCollection;
    public AudioClip AC_WordSound;
    public UnityEngine.UI.Image character;

    new void Awake()
    {
        base.Awake();
        WordCollection.SetActive(false);
    }

    public void CONTROL_PressCharacter()
    {
        StartCoroutine(PC());
    }

    bool rolling = false;
    IEnumerator PC()
    {
        AS.PlayOneShot(AC_WordSound);

        if(WordCollection.activeSelf == true) yield break;
        WordCollection.SetActive(true);
        rolling = true;
        yield return new WaitForSeconds(2.0f);
        rolling = false;
        WordCollection.SetActive(false);
    }

    void FixedUpdate()
    {
        if(rolling) character.rectTransform.RotateAround(character.transform.position, new Vector3(0,0,1), UnityEngine.Random.Range(-45f,45f));
        else character.rectTransform.rotation = Quaternion.Euler(0,0,0);
    }
}

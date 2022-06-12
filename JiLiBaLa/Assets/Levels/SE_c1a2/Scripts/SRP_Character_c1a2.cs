using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SRP_Character_c1a2 : MonoBehaviour
{
    //-5.92,1.09,0
    //-8.11,-0.85,0
    //-7.18,-2.64
    public GameObject Mesh;
    public GameObject Firepoint;
    public GameObject projectionObject;
    public GameObject fireeffectObject;
    private Animator animator;
    public float firedelay = 2f;
    private bool canfire = true;
    private Vector3 originalloc;
    private float shakefactor = 0.1f;
    public float ShakePower = 1;
    private bool isdead = false;
    public AudioClip ACdie;
    public AudioClip ACfire;



    private void Awake()
    {
        animator = Mesh.GetComponent<Animator>();
        originalloc = Mesh.transform.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<SRP_eCharacter_c1a2>() != null)
        {
            gameObject.transform.localScale = new Vector3(1,0.2f,1);
            if(!isdead)
            {
                isdead = !isdead;
                originalloc -= new Vector3(0,1.02f,0);
                if(gameObject.name == "PRB_Character_Pander") GameObject.Find("SceneManager").GetComponent<SRP_c1a2Manager>().PanderTigerStatus -= new Vector2Int(1,0);
                if(gameObject.name == "PRB_Character_Tiger") GameObject.Find("SceneManager").GetComponent<SRP_c1a2Manager>().PanderTigerStatus -= new Vector2Int(0,1);
                GetComponent<AudioSource>().PlayOneShot(ACdie);
            }
        }
    }
    private void Update()
    {

        Mesh.transform.position = new Vector3(originalloc.x + Random.Range(-ShakePower * shakefactor, ShakePower * shakefactor), originalloc.y + Random.Range(-ShakePower * shakefactor, ShakePower * shakefactor), originalloc.z);
    }

    void FixedUpdate()
    {
        shakefactor = Mathf.Lerp(shakefactor, 0.1f, 0.1f);
    }

    public void FIRE_ONCE()
    {
        if(!isdead)
        StartCoroutine(IE_FIRE());
        GetComponent<AudioSource>().PlayOneShot(ACfire);
    }
    IEnumerator AUTO_FIRE(float Delay = 5)
    {
        while(true)
        {
            FIRE_ONCE();
            yield return new WaitForSeconds(Delay);
        }
    }
    IEnumerator IE_FIRE()
    {
        if(!canfire) yield break;
        canfire = !canfire;
        animator.SetBool("roar", true);
        GameObject proj, proj2;
        if (projectionObject != null) 
        {
            proj = Instantiate(projectionObject);
            proj.transform.position = Firepoint.transform.position;
        }
        if (fireeffectObject != null)
        {
            proj2 = Instantiate(fireeffectObject);
            proj2.transform.position = Firepoint.transform.position;
        }
        shakefactor = 1;
        yield return new WaitForSeconds(firedelay);
        animator.SetBool("roar", false);
        canfire = !canfire;
        yield break;
    }


}

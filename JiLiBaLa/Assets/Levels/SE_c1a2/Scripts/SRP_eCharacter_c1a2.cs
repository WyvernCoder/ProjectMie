using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SRP_eCharacter_c1a2 : MonoBehaviour
{
    public GameObject Mesh;
    public GameObject c1a2Manager;
    public Vector3 TargetLocation;
    private Vector3 originallocalloc;
    private float shakefactor = 0;
    public float ShakePower = 1;
    private float reversefactor = 1;
    private int shakemode;
    public AudioClip ACdie;
    public AudioClip AChey;
    public List<AudioClip> ACfresh = new List<AudioClip>();
    void Start()
    {
        GetComponent<AudioSource>().PlayOneShot(AChey,Random.Range(0f,0.5f));
    }
    private void Awake()
    {
        c1a2Manager = GameObject.Find("SceneManager");
        originallocalloc = Mesh.transform.localPosition;
        shakemode = Random.Range(0,2);
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.GetComponent<SRP_c1a2Bullet>() != null)//如果碰撞体是子弹
        {
            BeingKilled();//调用死亡函数
            other.gameObject.GetComponent<Rigidbody2D>().gravityScale = 5f;
            other.gameObject.GetComponent<SRP_c1a2Bullet>().Nstopmove = true;
            other.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(-500f,1000f));
            GameObject.Find("SceneManager").GetComponent<SRP_c1a2Manager>().AddScore(1);
        }
    }
    
    

    public void BeingKilled()
    {
        Destroy(GetComponent<BoxCollider2D>());
        Destroy(GetComponent<Rigidbody2D>());
        //TargetLocation = new Vector3(12f,TargetLocation.y,TargetLocation.z);
        TargetLocation = transform.position;
        //shakefactor = 1;
        reversefactor = -1;
        StartCoroutine("DIE_Anim");
        Invoke("DestroySelf",4f);
        GetComponent<AudioSource>().PlayOneShot(ACdie);
        GetComponent<AudioSource>().PlayOneShot(ACfresh[Random.Range(0,ACfresh.Count)]);
    }
    void FixedUpdate()
    {
        Mesh.transform.position = new Vector3(transform.position.x + originallocalloc.x + Random.Range(-ShakePower * shakefactor, ShakePower * shakefactor), transform.position.y + originallocalloc.y + Random.Range(-ShakePower * shakefactor, ShakePower * shakefactor), 0f);
        //shakefactor = Mathf.Lerp(shakefactor, 0, 0.1f);       //震动因数缓慢下降
        if (shakemode == 0)
        {
            if (reversefactor != 1f)
            {
                Mesh.transform.localRotation = Quaternion.AngleAxis(Mathf.Sin(Time.time * 10f) * 100f, new Vector3(0, 0, 1));
                Mesh.transform.localScale = Vector3.Lerp(Mesh.transform.localScale, new Vector3(0, 0, 0), 0.01f);
            }
            else Mesh.transform.localRotation = Quaternion.AngleAxis(Time.time * 100 * reversefactor, new Vector3(0, 0, 1));
        }
        if (shakemode == 1)
        {
            if (reversefactor != 1f)
            {
                //Mesh.transform.localRotation = Quaternion.AngleAxis(Mathf.Sin(Time.time * 10f) * 100f, new Vector3(0, 0, 1));
                Mesh.transform.localScale = new Vector3(Random.Range(-3f,3f),Random.Range(-shakefactor,shakefactor),1f);
            }
            else Mesh.transform.localScale = new Vector3(Random.Range(0.1f,1f),Random.Range(0.5f,1f),1f);
        }

        transform.position = new Vector3(Mathf.MoveTowards(transform.position.x, TargetLocation.x, c1a2Manager.GetComponent<SRP_c1a2Manager>().EnemySpeed),transform.position.y,transform.position.z);
    }

    IEnumerator DIE_Anim()
    {
        while (Mesh.transform.localScale.x > 0.01f)
        {
            shakefactor += 0.001f;
            yield return null;
        }
        yield break;
    }
    public void DestroySelf() => Destroy(gameObject);
}

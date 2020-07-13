using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicJumpSlide : MonoBehaviour {
    Rigidbody2D rigid;
    public Animator anim;
    BoxCollider2D[] box;
    public int jumpCount = 0;
    public int hp = 100;
    public int AddReward = 1;
    public BasicJumpSlide singleton;


    public GameObject[] Objects;
	// Use this for initialization
	void Start () {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        box = GetComponentsInChildren<BoxCollider2D>();
        singleton = this;
        Objects = GameObject.FindGameObjectsWithTag("block");
        Debug.Log((Objects.Length+ 2) + " 총 갯수 + 2");
    }

    // Update is called once per frame
    void Update () {
        
        if (hp <= 0)
        {
            anim.SetInteger("CurrentState", 4);
            return;
        }
        if (Input.GetKeyDown(KeyCode.Space) && jumpCount == 0)
        {
            rigid.velocity = Vector3.up * 2.3f;
            anim.SetInteger("CurrentState", 1);
            jumpCount++;
        }
        if (Input.GetKeyDown(KeyCode.Z) && jumpCount == 0)
        {
            anim.SetInteger("CurrentState", 3);
            box[0].enabled = false;
            box[1].enabled = true;
            transform.position = new Vector3(-1.59f, -0.717f, 0);            
            
        }
        if (Input.GetKeyUp(KeyCode.Z))
        {
            anim.SetInteger("CurrentState", 0);
            box[0].enabled = true;
            box[1].enabled = false;
            transform.position = new Vector3(-1.59f, -0.5375f, 0);
        }
       
    }
    //void checkAnim()
    //{
    //    anim.SetInteger("CurrentState", 0);
    //}
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.tag == "ground")
        {
            if (jumpCount == 1)
            {
                //anim.SetInteger("CurrentState", 2);
                jumpCount =0;
                //Invoke("checkAnim", 0.3f);
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.transform.tag == "block")
        {
            hp -= 100;
        }
        if(collision.transform.tag == "jelly")
        {
            collision.gameObject.SetActive(false);
            AddReward++;
        }
        if (collision.transform.tag == "coin")
        {
            collision.gameObject.SetActive(false);
            AddReward++;
        }
    }
}

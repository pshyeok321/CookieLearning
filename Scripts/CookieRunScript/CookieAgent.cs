using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using UnityEngine.UI;
public class CookieAgent : Agent
{
    Text hpText;

    private Rigidbody2D _Rigid;
    private Animator _Anim;
    BoxCollider2D[] Boxs;

    public int jumpCount = 0;

    //GameObject[] jellys;
    //GameObject[] Coins;
    //public GameObject[] Objects;
    //public GameObject[] Jellys;
    Vector3 StartTrans = new Vector3(-1.59f, -0.5375f, 0);


    private const int Jump = 1;
    private const int Down = 2;
    private const int None = 0;
    public GameObject Map;
    public Camera mainCamera;
    // Use this for initialization
   public int playerHP= 100;

    List<GameObject> jelly = new List<GameObject>();
    List<GameObject> block = new List<GameObject>();
    List<GameObject> coin = new List<GameObject>();

    AudioSource audioSource;
    AudioClip bgm;
    Image GameResultImage;
    private void Awake()
    {
        

        jelly.Clear();
        block.Clear();
        coin.Clear();
        _Rigid = GetComponent<Rigidbody2D>();
        _Anim = GetComponent<Animator>();
        Boxs = GetComponentsInChildren<BoxCollider2D>();

        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        audioSource = mainCamera.GetComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.Play();

        jelly.AddRange(GameObject.FindGameObjectsWithTag("jelly"));
        block.AddRange(GameObject.FindGameObjectsWithTag("block"));
        coin.AddRange(GameObject.FindGameObjectsWithTag("coin"));
        hpText = GameObject.FindGameObjectWithTag("Text").GetComponent<Text>();
        GameResultImage = GameObject.Find("GameResultImage").GetComponent<Image>();
        GameResultImage.gameObject.SetActive(false);
        //Objects = GameObject.FindGameObjectsWithTag("block");
        //jellys = GameObject.FindGameObjectsWithTag("jelly");
        //Coins = GameObject.FindGameObjectsWithTag("coin");

        //Debug.Log((Objects.Length + jellys.Length + Coins.Length) + "총 갯수 ");
        Sort();


    }
    void Sort()
    {
        jelly.Sort(delegate (GameObject a, GameObject b)
        {
            return Vector3.Distance(transform.position, a.transform.position).CompareTo(Vector3.Distance(transform.position, b.transform.position));
        });
        block.Sort(delegate (GameObject a, GameObject b)
        {
            return Vector3.Distance(transform.position, a.transform.position).CompareTo(Vector3.Distance(transform.position, b.transform.position));
        });
        coin.Sort(delegate (GameObject a, GameObject b)
        {
            return Vector3.Distance(transform.position, a.transform.position).CompareTo(Vector3.Distance(transform.position, b.transform.position));
        });
    }
    void ResetJellyAndCoin()
    {
        for (int i = 0; i < jelly.Count; i++)
        {
            jelly[i].gameObject.SetActive(true);
        }
        for (int i = 0; i < coin.Count; i++)
        {
            coin[i].gameObject.SetActive(true);
        }
    }

    public override void AgentReset()
    {
        transform.position = StartTrans;
        transform.position = new Vector3(-1.591f, -0.57f, 0);
        mainCamera.transform.position = new Vector3(0, 0, -1f);
        playerHP = 100;
        ResetJellyAndCoin();
    }
    
    public override void CollectObservations()
    {
        for (int i = 0; i < block.Count; i++)
        {
            Vector3 dis = block[i].transform.position - transform.position;
            AddVectorObs(dis.x);
            AddVectorObs(dis.y);
        }
        for (int i = 0; i < jelly.Count; i++)
        {
            Vector3 dis = jelly[i].transform.position - transform.position;
            AddVectorObs(dis.x);
            AddVectorObs(dis.y);
        }

        for (int i = 0; i < coin.Count; i++)
        {
            Vector3 dis = coin[i].transform.position - transform.position;
            AddVectorObs(dis.x);
            AddVectorObs(dis.y);
        }
        AddVectorObs(transform.position.x);
        AddVectorObs(transform.position.y);
    }
    public float _time=0;
    public float _sTime = 0;

    bool isSlide = false;
    public float slideTimer = 0f;

    bool isBasic = false;
    public float basicTimer = 0f;

    public bool isInvicible = false;
    public float invicibleTimer = 0f;
    float invicibleTime = 0.3f;
    private void Update()
    {
        hpText.text = "CurrentHP : " + playerHP;
        if (isSlide)
        {
            slideTimer += Time.deltaTime;

            _Anim.SetInteger("CurrentState", 3);
            Boxs[0].enabled = false;
            Boxs[1].enabled = true;
            transform.position = new Vector3(transform.position.x, -0.717f, 0);
            if (slideTimer >= 0.5f)
            {
                slideTimer = 0;
                isSlide = false;
            }
        }
        if (isBasic && !isInvicible)
        {
            basicTimer += Time.deltaTime;

            _Anim.SetInteger("CurrentState", 0);
            Boxs[0].enabled = true;
            Boxs[1].enabled = false;
            transform.position = new Vector3(transform.position.x, -0.5375f, 0);
            if (basicTimer >= 0.3f)
            {
                basicTimer = 0;
                isBasic = false;
            }
        }
        if (transform.position.x >= 21f)
        {
            GameResultImage.gameObject.SetActive(true);
            audioSource.Stop();
            Time.timeScale = 0;
        }

        if (isInvicible)
        {
            invicibleTimer += Time.deltaTime;
            if(invicibleTimer >= invicibleTime)
            {
                invicibleTimer = 0;
                isInvicible = false;
                _Anim.SetInteger("CurrentState", 0);
            }
        }
    }
    public override void AgentAction(float[] vectorAction, string textAction)
    {
        //AddReward(-0.01f);

        var action = (int)vectorAction[0];  

        switch (action)
        {
            case None:
                if(isBasic)//if(jumpCount !=0 || isSlide)
                {
                    break;
                }
                if (jumpCount == 0 && !isSlide && !isBasic)
                {
                    isBasic = true;
                    break;
                }
                break; ;
            case Jump:
                if (isSlide || isBasic)
                {
                    break;
                }
                if (jumpCount == 0 && !isSlide && !isBasic)
                {
                    _Rigid.velocity = Vector3.up * 2.3f;
                    _Anim.SetInteger("CurrentState", 1);
                    Boxs[0].enabled = true;
                    Boxs[1].enabled = false;
                    jumpCount = 1;
                    GameObject.Find("JumpSound").GetComponent<AudioSource>().Play();

                    break;
                }
                break; ;
            case Down:
                if (isSlide)
                {
                    break;
                }
                if (jumpCount == 0 && !isSlide && !isBasic)
                {                  
                    isSlide = true;
                    GameObject.Find("SlideSound").GetComponent<AudioSource>().Play();

                    break;
                }
                break;
        }



        for(int i=0; i<jelly.Count; i++)
        {
            if (!jelly[i].activeSelf)
            {
                AddReward(1.0f);
            }
        }
        for (int i = 0; i < coin.Count; i++)
        {
            if (!coin[i].activeSelf)
                AddReward(1.0f);
        }
        if (playerHP <= 0)
        {
            
            onOffTimer += Time.deltaTime;
            if(onOffTimer > 0.1f)
            {
                OnOffBox.SetActive(true);
            }
            if (onOffTimer > 1f)
            {
                OnOffBox.SetActive(false);
                onOffTimer = 0;
                AddReward(-10.0f);
                Done();
            }
            
        }
        if(transform.position.x >= 25f)
        {
            AddReward(100f);
            Done();
        }
    }
    public GameObject OnOffBox;
    float onOffTimer;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag == "ground")
        {
            if (jumpCount == 1)
            {
                jumpCount = 0;
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isInvicible)
        {
            if (collision.transform.tag == "block")
            {
                _Anim.SetInteger("CurrentState", 4);
                playerHP -= 50;
                isInvicible = true;
                StartCoroutine(Shake.instance.ShakeCamera(0.2f, 0.2f, 20f));             
            }
        }
        if (collision.transform.tag == "jelly")
        {
            collision.gameObject.SetActive(false);
            GameObject.Find("EatSound").GetComponent<AudioSource>().Play();

        }
        if (collision.transform.tag == "coin")
        {
            collision.gameObject.SetActive(false);
            GameObject.Find("EatSound").GetComponent<AudioSource>().Play();

        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveMap : MonoBehaviour
{
    public CookieAgent player;
    public Camera mainCamera;
    // Use this for initialization
    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<CookieAgent>();
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player.transform.position.x >= 21f)
        {
            Time.timeScale = 0;            
        }
        else
        {
            player.transform.Translate(0.01f, 0, 0);
            mainCamera.transform.Translate(0.01f, 0, 0);
        }
            
    }
}

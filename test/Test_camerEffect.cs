using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_camerEffect : MonoBehaviour
{
    public Transform transform;
    Camera_PlayerTracker tk;
    Camera_Effecter temp;

    private void Awake()
    {
        //GetComponent<EventCenter>().Init();
    }
    // Start is called before the first frame update
    void Start()
    {
        temp = GetComponent<Camera_Effecter>();
        temp.Shake(1, 10, DIR.front);
         //tk = GetComponent<Camera_PlayerTracker>();
        //tk.Track(transform);
    }

    DIR dir = DIR.front;
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))//切视角
        {
           // tk.changeViewDir(90);
        }
        if(Input.GetKeyDown(KeyCode.R))//震动屏幕
        {
            temp.Shake(1, 10, dir);
            if (dir == DIR.front) { dir = DIR.left; }
            else if (dir == DIR.left) { dir = DIR.up; }
            else if (dir == DIR.up) { dir = DIR.front; }
        }
    }
}
/*
 * 除震动效果不太好，视角切换太生硬外没什么问题
 * 
 * 
 * 
 */ 
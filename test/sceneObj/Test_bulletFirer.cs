using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_bulletFirer : MonoBehaviour
{
    public Damage damage;
    //public GameObject bullet;
    public float speed;
    public Ticker t;

    BulletParm parm=new BulletParm();

    //BaseObjectPool pool ;
    private void Awake()
    {

        
        
    }
    // Update is called once per frame
    void Update()
    {BulletParm bp = new BulletParm();
        bp.speed = 1;
        bp.pos = Vector3.zero;
        bp.spreadRadius = 5;
        bp.targetPos = new Vector3(0, 2, 1);
        Debug.DrawLine(bp.pos, bp.targetPos, Color.blue);
        Debug.Log(bp.GetSpeed());
        Debug.DrawLine(bp.targetPos, bp.GetSpeed(),Color.red);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_NPCView : MonoBehaviour
{
    public GameObject allnpcBG, npcSelectedBG;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("A");
            Animator aa = allnpcBG.GetComponent<Animator>();
            //aa.SetFloat("rever", -1);
            aa.Play("allNPCbgup");
            aa.speed = -1;

            aa = npcSelectedBG.GetComponent<Animator>();
            //aa.SetFloat("rever", -1);
            aa.Play("selectNPCup");
            aa.speed = -1;
        }
        else if(Input.GetKeyDown(KeyCode.D))
        {
            Debug.Log("D");
            Animator aa = allnpcBG.GetComponent<Animator>();
            //aa.SetFloat("rever", 1);
            aa.Play("allNPCbgup");
            aa.speed = 1;

            aa = npcSelectedBG.GetComponent<Animator>();
            //aa.SetFloat("rever", 1);
            aa.Play("selectNPCup");
            aa.speed = 1;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCDataShower_3D : MonoBehaviour
{
    PlayerHand hand;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    

   public void Show(NpcData nd)
    {
        if (nd == null)//��������
        { Debug.Log("��"); }
        else gameObject.SetActive(true);
    }
}

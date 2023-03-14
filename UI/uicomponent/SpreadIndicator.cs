using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpreadIndicator : Base_UIComponent
{
    RawImage img;
    Ray r;
    RaycastHit rt;
    // Start is called before the first frame update
    void Start()
    {
        img = GetComponent<RawImage>();
    }
    private void Update()
    {
        r = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(r,out rt))
        {
            img.transform.position = rt.point;
            EventCenter e = EventCenter.WorldCenter.GetParm<EventCenter>(nameof(EventNames.GetLocalPlayer));
            if (e != null) img.transform.LookAt(e.transform);
        }
    }
    public void OnQuitInGameScene()
    {
        gameObject.SetActive(false);
    }

    public void SetSpread(float value)
    {
        img.rectTransform.sizeDelta=new Vector3(value,value,0);

    }
}

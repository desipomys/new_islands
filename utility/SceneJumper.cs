using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneJumper : MonoBehaviour,IEventRegister
{
    protected EventCenter center;
    public void OnEventRegist(EventCenter e)
    {
        center = e;
        e.ListenEvent<string>(nameof(EventNames.JumpToScene),JumpTo);
    }
    public void AfterEventRegist()
    {

    }

    public void JumpTo(string name)
    {
        StartCoroutine(jumpSingle(name));
    }
    public void ArriveScene(string name)
    {
        
    }
    IEnumerator jumpSingle(string name)
    {
        if(name==ConstantValue.startSceneName)
            GetComponent<GameLifeCycle>().OnQuitInGameScene();
            else GetComponent<GameLifeCycle>().OnQuitStartScene();

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(name);
        while (!asyncLoad.isDone)
        {
            yield return null;
            loadingProcess(asyncLoad);
        }
        if(name== ConstantValue.ingameSceneName)
            GetComponent<GameLifeCycle>().OnArriveInGameScene();
            else GetComponent<GameLifeCycle>().OnArriveStartScene();
    }
     IEnumerator jumpMulti(string name)
    {
        if(name== ConstantValue.startSceneName)
            GetComponent<GameLifeCycle>().OnQuitInGameScene();
            else GetComponent<GameLifeCycle>().OnQuitStartScene();
         AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(name);//应该用photon的跳转场景
         while (!asyncLoad.isDone)
        {
            yield return null;
        }
        if(name== ConstantValue.ingameSceneName)
            GetComponent<GameLifeCycle>().OnArriveInGameScene();
            else GetComponent<GameLifeCycle>().OnArriveStartScene();
    }
    Ticker tick=new Ticker(0.125f);//每0.125S刷新一次场景加载条
    void loadingProcess(AsyncOperation oper)
    {
        if(tick.IsReady())
        {
            EventCenter.WorldCenter.SendEvent<float>(nameof(EventNames.LoadSceneProcessGo),oper.progress);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerBuilderEventName
{
    OnShowerHit
}
public class PlayerBuilder_UIController : BaseUIController
{
    bool OnChosePos = false;//是否正在选择位置
    DIR dir;
    BuildAble_Data currentBuilding;
    SketchBlock buildingShadowObj;

    PlaceEBlockParm parm=new PlaceEBlockParm();

    public override void OnEventRegist(EventCenter e)
    {
        base.OnEventRegist(e);
        e.ListenEvent<BuildAble_Data, int>(nameof(PlayerBuilderEventName.OnShowerHit), OnShowerHit);
    }

    /// <summary>
    /// 当buildshower被点击
    /// </summary>
    /// <param name="bud"></param>
    /// <param name="stat"></param>
    public void OnShowerHit(BuildAble_Data bud,int stat)
    {
        /// <summary>
        /// 按下按钮暂时隐藏UI，进入虚像模式：生成对应建筑虚像，跟随鼠标在方块上移动，虚像按R旋转，按shift长按左键连续放置，按esc退出虚像模式
        /// </summary>   
        if (stat == 0)
        {
            Debug.Log("点击建筑");
            currentBuilding = bud;
            if (buildingShadowObj != null) Destroy(buildingShadowObj);
            buildingShadowObj = GetBuildingShadow(currentBuilding);
            SetOnChosePos(true);
        }
    }
    
    public void Update()
    {
        if(OnChosePos)
        {
            if(Input.GetKeyDown(KeyCode.Mouse1))//取消选择建造
            {
                SetOnChosePos(false);
                return;
            }
            Vector3 pointAt = EventCenter.WorldCenter.GetParm<Vector3>(nameof(EventNames.GetMouseLookAt));
            //Debug.Log(pointAt);
            pointAt.y += 0.01f;
            Vector3Int temp = Chunk.WorldPosToBlockPos(pointAt);
            int[] size = GetBuildingSize(currentBuilding);
            temp.x -= size[0] / 2;
            temp.z -= size[1] / 2;
            buildingShadowObj.transform.position =Chunk.BlockPosToWorldPos(temp) - new Vector3(Chunk.BlockSize / 2, 0, Chunk.BlockSize / 2);//shadow跟随鼠标指向
            //
            bool canPlaceAt = TestCanPlaceAt(buildingShadowObj.transform.position,currentBuilding);

            if(canPlaceAt&&Vector3.Magnitude(buildingShadowObj.transform.position-model.transform.position)<=5f)
            {
                buildingShadowObj.SetAvalible(true);
                if ( Input.GetKeyDown(KeyCode.Mouse0))//选定位置
                    {
                    Debug.Log("建造于" + buildingShadowObj.transform.position);

                    StartBuild(buildingShadowObj.transform.position);
                    }
            }
            else//将shadow设为不可用
            {
                buildingShadowObj.SetAvalible(false);
            }
            
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                view.uiCenter.CloseCurrentView();
            }
        }
        
    }

    FindItemParm par = new FindItemParm();
    void StartBuild(Vector3 pos)
    {//要求建筑底下必须有tblock/eblock格子
        if (currentBuilding.isBBlock)
        {

        }
        else
        {
            parm.pos = Chunk.WorldPosToBlockPos(pos);
            parm.initData = "";
            parm.typ = currentBuilding.eblockTyp.typ;

            EventCenter e= EventCenter.WorldCenter.GetParm<PlaceEBlockParm,EventCenter>(nameof(EventNames.SetEBlock), parm);
            e.GetComponent<BuildAbleEBlock>().Init();
        }
        //扣除物资
        for (int i = 0; i < currentBuilding.mats.Length; i++)
        {
            par.item = currentBuilding.mats[i];
            par.mode = ItemCompareMode.excludeNum;
            par.page = 0;
            model.GetParm<FindItemParm, int>(nameof(PlayerEventName.takeItem),par);
        }
       
    }
    
    int[] GetBuildingSize(BuildAble_Data bd)
    {
        if (bd.isBBlock) { return new int[] { 1, 1, 1 }; }
        else
        {
            int xyh = EventCenter.WorldCenter.GetParm<int, int>(nameof(EventNames.GetEBlockSizeByID), bd.eblockTyp.typ);
            return xyh.GetCoord3();
        }
        
    }

    void SetOnChosePos(bool stat)
    {
        OnChosePos = stat;
        if(OnChosePos)
        {
            ((PlayerBuilder_View)view).SetScrollViewVisable(false);
        }
        else
        {
            buildingShadowObj.gameObject.SetActive(false);
            ((PlayerBuilder_View)view).SetScrollViewVisable(true);
        }
    }
   
    bool TestCanPlaceAt(Vector3 pos, BuildAble_Data bd)
    {
        if(bd.isBBlock)
        {
            return false;
        }
        else
        {
            Entity_Block temp = new Entity_Block(Chunk.WorldPosToBlockPos(pos), bd.eblockTyp.typ);
           return EventCenter.WorldCenter.GetParm<Entity_Block, bool>(nameof(EventNames.EBlockCanPlaceAt),temp);
        }
    }

    SketchBlock GetBuildingShadow(BuildAble_Data bd)
    {
        if(bd.isBBlock)
        {
            return null;//bblock在场景中是用gobj还是合并的mesh形式表示？
        }
        else
        {
            GameObject g = EventCenter.WorldCenter.GetParm<int, GameObject>(nameof(EventNames.GetEBlockObjByID),bd.eblockTyp.typ);
            SketchBlock sk= g.AddComponent<SketchBlock>();
            sk.Init();
            return sk;
        }
    }
}

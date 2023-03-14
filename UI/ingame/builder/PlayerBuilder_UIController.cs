using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerBuilderEventName
{
    OnShowerHit
}
public class PlayerBuilder_UIController : BaseUIController
{
    bool OnChosePos = false;//�Ƿ�����ѡ��λ��
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
    /// ��buildshower�����
    /// </summary>
    /// <param name="bud"></param>
    /// <param name="stat"></param>
    public void OnShowerHit(BuildAble_Data bud,int stat)
    {
        /// <summary>
        /// ���°�ť��ʱ����UI����������ģʽ�����ɶ�Ӧ�������񣬸�������ڷ������ƶ�������R��ת����shift��������������ã���esc�˳�����ģʽ
        /// </summary>   
        if (stat == 0)
        {
            Debug.Log("�������");
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
            if(Input.GetKeyDown(KeyCode.Mouse1))//ȡ��ѡ����
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
            buildingShadowObj.transform.position =Chunk.BlockPosToWorldPos(temp) - new Vector3(Chunk.BlockSize / 2, 0, Chunk.BlockSize / 2);//shadow�������ָ��
            //
            bool canPlaceAt = TestCanPlaceAt(buildingShadowObj.transform.position,currentBuilding);

            if(canPlaceAt&&Vector3.Magnitude(buildingShadowObj.transform.position-model.transform.position)<=5f)
            {
                buildingShadowObj.SetAvalible(true);
                if ( Input.GetKeyDown(KeyCode.Mouse0))//ѡ��λ��
                    {
                    Debug.Log("������" + buildingShadowObj.transform.position);

                    StartBuild(buildingShadowObj.transform.position);
                    }
            }
            else//��shadow��Ϊ������
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
    {//Ҫ�������±�����tblock/eblock����
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
        //�۳�����
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
            return null;//bblock�ڳ���������gobj���Ǻϲ���mesh��ʽ��ʾ��
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

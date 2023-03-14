using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base_BBlockBehavious
{
    public B_Material mat=B_Material.none;
    public virtual void Init()
    {

    }
    public virtual void NeighborBlockUpdateTB(B_Block self,Vector3Int pos, Vector3Int target, T_Block tb)
    {
        
    }
    public virtual void NeighborBlockUpdateBB(B_Block self, Vector3Int pos, Vector3Int target, B_Block bb)
    {
       
    }
    public virtual void NeighborBlockUpdateEB(B_Block self, Vector3Int pos, Vector3Int target, Entity_Block eb)
    {

    }

}
public class BBlockBehavious_liqud:Base_BBlockBehavious
{
    public override void Init()
    {
        mat = B_Material.water;
    }
    public override void NeighborBlockUpdateTB(B_Block self, Vector3Int pos, Vector3Int target, T_Block tb)
    {
        //����·��Ƿ�Ϊ�գ�Ϊ�����Լ���ղ�����һ����Ϊ�Լ�
    }
    public override void NeighborBlockUpdateBB(B_Block self, Vector3Int pos, Vector3Int target, B_Block bb)
    {
        //����·��Ƿ�Ϊ�գ�Ϊ�����Լ���ղ�����һ����Ϊ�Լ�
        //�������ǰ���Ƿ�Ϊ�գ�Ϊ����Һ���һ���ָ������
    }
    public override void NeighborBlockUpdateEB(B_Block self, Vector3Int pos, Vector3Int target, Entity_Block eb)
    {

    }
}
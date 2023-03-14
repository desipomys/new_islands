using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public enum EntityBlockEventNames
{
    saveStr,loadStr,ebinit, geteblock
}
//Ҫʹ�����ʽ�ű�ʵ��eblock���������ܣ������ѵ����˹����ǵ�����һ���ű����
//���桢��ȡʹ��entitydatacollector
public class BaseEntityBlock :MonoBehaviour,IEventRegister
{

    public Entity_Block block;
    EventCenter center;
    public void OnEventRegist(EventCenter e)
    {
        center =e;
        center.RegistFunc<Entity_Block>(nameof(EntityBlockEventNames.geteblock), () => { return block; });
        //center.RegistFunc<string>(nameof(EntityBlockEventNames.saveStr), ToString);//��objdatacontainer�ռ����ݣ���eblock�ڵ���������ͨ��objdatacontainer����
        //center.ListenEvent<string>(nameof(EntityBlockEventNames.loadStr), FromString);
        center.ListenEvent<Entity_Block>(nameof(EntityBlockEventNames.ebinit), OnEBlockInit);
    }
    public void AfterEventRegist()
    {

    }
    public virtual void OnEBlockInit(Entity_Block eb)
    {
        block = eb;
        center.UUID = eb.UUID;
        Vector3Int bpos = new Vector3Int(eb.posX, eb.posH, eb.posY);
        transform.position = Chunk.BlockPosToWorldPos(bpos)-new Vector3(Chunk.BlockSize/2,0,Chunk.BlockSize/2);

        DIR[] ds = eb.dir.UnPack();
        Vector3 temp = Vector3.zero;
        foreach (var item in ds)
        {
            switch (item)
            {
                case DIR.none:
                    break;
                case DIR.front:
                    temp.x += 1;
                    break;
                case DIR.back:
                    temp.x -= 1;
                    break;
                case DIR.up:
                    temp.y += 1;
                    break;
                case DIR.down:
                    temp.y -= 1;
                    break;
                case DIR.left:
                    temp.z += 1;
                    break;
                case DIR.right:
                    temp.z -= 1;
                    break;
                default:
                    break;
            }
        }
        transform.GetChild(0).LookAt(temp + transform.GetChild(0).position);
    }

    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InteractType
{
    open,
    AIopen
}
/// <summary>
/// �����ӿڣ���������Ʒ����û��evc������ֱ���ýӿ�
/// 
/// </summary>
public interface IInterectable
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="type"></param>
    /// <returns>����0������Ч��1�ɹ���2��ʰȡ�ɹ���3��ʰȡ�ɹ�</returns>
    int OnInterect(EventCenter source, InteractType type);
    
}
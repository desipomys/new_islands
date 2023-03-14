using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ThreeKeyDictionary<K1, K2,K3, V> : IEnumerable<KeyValuePair<K1, Dictionary<K2, Dictionary<K3,V>>>>
{
    public Dictionary<K1, Dictionary<K2, Dictionary<K3,V>>> dict = new Dictionary<K1, Dictionary<K2, Dictionary<K3,V>>>();

    public void Add(K1 k1, K2 k2,K3 k3, V v)
    {
        if (dict.ContainsKey(k1))
        {
            if (dict[k1].ContainsKey(k2))
            {
                if(!dict[k1][k2].ContainsKey(k3))//只有没有这个key3的情况下才会加,已存在则无效果
                {
                    dict[k1][k2].Add(k3,v);
                }
            }
            else
            {
                Dictionary<K3, V> temp1 = new Dictionary<K3, V>();
                temp1.Add(k3,v);
                dict[k1].Add(k2, temp1);
            }
        }
        else
        {
            Dictionary<K2,Dictionary<K3, V>> temp=new Dictionary<K2,Dictionary<K3, V>>();
            Dictionary<K3, V> temp1 = new Dictionary<K3, V>();
            temp1.Add(k3,v);
            temp.Add(k2, temp1);
            dict.Add(k1, temp);
        }
    }
    public IEnumerator<KeyValuePair<K1, Dictionary<K2, Dictionary<K3,V>>>> GetEnumerator()
    {
        return dict.GetEnumerator();
    }
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
    public bool ContainsKey(K1 k)
    {
       
        return dict.ContainsKey(k);
    }
    public bool Remove(K1 k)
    {
        return dict.Remove(k);
    }
    public bool TryGetValue(K1 d,out Dictionary<K2, Dictionary<K3,V>> o)
    {
        return dict.TryGetValue(d,out o);
    }
    public void Clear()
    {
        dict.Clear();
    }
    public bool ContainsValue(Dictionary<K2, Dictionary<K3,V>> d){
        return dict.ContainsValue(d);
    }

    public int Count
    {
        get{return dict.Count;}

    }
    public Dictionary<K2, Dictionary<K3,V>> this[K1 k]
    {
        get
        {
            return dict[k];
        }
        set
        {
            dict[k] = value;
        }
    }

}

    
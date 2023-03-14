using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TwoKeyDictionary<K1, K2, V> : IEnumerable<KeyValuePair<K1, Dictionary<K2, V>>>
{
    public Dictionary<K1, Dictionary<K2, V>> dict = new Dictionary<K1, Dictionary<K2, V>>();

    public void Add(K1 k1, K2 k2, V v)
    {
        if (dict.ContainsKey(k1))
        {
            if (!dict[k1].ContainsKey(k2))
            {
                dict[k1].Add(k2, v);
            }
            else
            {
                dict[k1][k2] = v;
            }
        }
        else
        {
            Dictionary<K2, V> temp = new Dictionary<K2, V>();
            temp.Add(k2, v);
            dict.Add(k1, temp);
        }
    }
    public IEnumerator<KeyValuePair<K1, Dictionary<K2, V>>> GetEnumerator()
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
    public bool TryGetValue(K1 d, out Dictionary<K2, V> o)
    {
        return dict.TryGetValue(d, out o);
    }
    public void Clear()
    {
        dict.Clear();
    }
    public bool ContainsValue(Dictionary<K2, V> d)
    {
        return dict.ContainsValue(d);
    }

    public int Count
    {
        get { return dict.Count; }

    }
    public Dictionary<K2, V> this[K1 k]
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

    /// <summary>
    /// 通过第二个KEY找其下所有的值
    /// </summary>
    /// <param name="k"></param>
    /// <returns></returns>
    public List<V> GetValuesByK2(K2 k)
    {
        List<V> ans = new List<V>();
        foreach (var item in dict)
        {
            foreach (var i in item.Value)
            {
                if (k.Equals(i.Key))
                    ans.Add(i.Value);
            }
        }
        return ans;
    }
    //可通过K1值找出其下的所有[k2,v]对
    //或通过K2的值找出其下的所有[V]
}

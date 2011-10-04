using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebKit.Server.Misc
{
    public class MultiArray<K, V>
    {
        public List<K> Keys     { get; set; }
        public List<V> Values   { get; set; }

        public MultiArray()
        {
            Keys    = new List<K>();
            Values   = new List<V>();
        }

        public void Add(K key, V val)
        {
            Keys.Add(key);
            Values.Add(val);
        }

        public void Remove(int index)
        {
            Keys    .RemoveAt(index);
            Values   .RemoveAt(index);
        }

        public K GetKey(int index)
        {
            return Keys.ToArray()[index];
        }

        public V GetValue(int index)
        {
            return Values.ToArray()[index];
        }

        public bool ContainsKey(K key)
        {
            return Keys.Contains(key);
        }

        public bool ContainsValue(V val)
        {
            return Values.Contains(val);
        }
    }
}

// Project:      TDSM WebKit
// Contributors: DeathCradle
// 
using System;
using System.Collections.Generic;

namespace WebKit.Server.Misc
{
    public class MultiArray<K, V>
    {
        public List<K> Keys     { get; set; }
        public List<V> Values   { get; set; }

		public MultiArray()
		{
			this.Keys = new List<K>();
			this.Values = new List<V>();
		}

		public void Add(K key, V val)
		{
			this.Keys.Add(key);
			this.Values.Add(val);
		}

		public void Remove(int index)
		{
			this.Keys.RemoveAt(index);
			this.Values.RemoveAt(index);
		}

		public K GetKey(int index)
		{
			return this.Keys.ToArray()[index];
		}

		public V GetValue(int index)
		{
			return this.Values.ToArray()[index];
		}

		public bool ContainsKey(K key)
		{
			return this.Keys.Contains(key);
		}

		public bool ContainsValue(V val)
		{
			return this.Values.Contains(val);
		}
    }
}

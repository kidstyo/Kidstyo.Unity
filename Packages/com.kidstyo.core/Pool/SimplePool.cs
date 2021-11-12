using System.Collections.Generic;
using Orvnge;
using UnityEngine;

public class SimplePool : MonoSingleton<SimplePool>
{
    public class Pool
    {
        private readonly GameObject _origin;
        private readonly Queue<GameObject> Childs;

        public Pool(GameObject origin)
        {
            _origin = origin;
            Childs = new Queue<GameObject>();
        }

        public GameObject Spawn()
        {
            return Childs.Count > 0 ? Childs.Dequeue() : Instantiate(_origin);
        }

        public void Recycle(GameObject obj)
        {
            Childs.Enqueue(obj);
            obj.SetActive(false);
        }
    }
    
    public int PoolCount = 0;
    private readonly Dictionary<string, Pool> _pools = new();

    public void InitPool(GameObject item, string poolName)
    {
        if (string.IsNullOrEmpty(poolName))
        {
            Debug.LogError("PoolName is null! " + item.name);
            return;
        }

        if (_pools.ContainsKey(poolName))
        {
            // Debug.LogError("Pool Already Exist! " + poolName);
            return;
        }

        PoolCount++;
        _pools.Add(poolName, new Pool(item));
    }

    public GameObject GetItemFromPool(string poolName)
    {
        Debug.Log("GetItemFromPool:" + poolName);

        if (_pools.TryGetValue(poolName, out var pool))
        {
            return pool.Spawn();
        }

        Debug.LogError("Pool is null! " + poolName);
        return null;
    }

    public void Recycle(string poolName, GameObject item)
    {
        Debug.Log("Recycle:" + poolName);
        if (_pools.TryGetValue(poolName, out var pool))
        {
            pool.Recycle(item);
        }
        else
        {
            Debug.LogError("Pool is null! " + poolName);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace YPools
{
    [System.Serializable]
    public class ObjectPool
    {
        [TooltipAttribute("seconds time for auto free pools overflow.")]
        public float clearTime = 120;
        public float clrDelay = 0;
        [TooltipAttribute("size for auto free pools overflow.")]
        public int miniSize = 0;
        public GameObject prefab;
        public bool clr_flag = false;
        public Queue<GameObject> poolQ = new Queue<GameObject>();
    }

    public class ObjectPoolsMgr : MonoBehaviour
    {
        public List<ObjectPool> pools = new List<ObjectPool>(); //as dict not Serializable save
        public static ObjectPoolsMgr instance;
        void Awake()
        {
            instance = this;
            for (int idx = 0; idx < pools.Count; idx++)
            {
                if (pools[idx].prefab == null)
                {
                    pools.RemoveAt(idx);
                    idx--;
                    continue;
                }
            }
            //add pool with code here
        }

        void Start()
        {            
            for (int idx = 0; idx < pools.Count; idx++)
            {
                ObjectPool pool = pools[idx];
                GameObject subContain = Instantiate(new GameObject(pool.prefab.name),gameObject.transform);
                for (int i = 0; i < pool.miniSize; i++)
                {
                    var obj = Instantiate(pool.prefab,subContain.transform);
                    obj.SetActive(false);
                    pool.poolQ.Enqueue(obj);
                }

            }
        }
        void FixedUpdate()
        {
            foreach (var pool in pools)
            {
                if (pool.poolQ.Count > pool.miniSize)
                {
                    pool.clr_flag = true;
                    pool.clrDelay += Time.deltaTime;
                }
                else
                {
                    pool.clr_flag = false;
                    pool.clrDelay = 0;
                }
                if ((pool.clrDelay > pool.clearTime) && (pool.clr_flag == true))
                {
                    pool.clr_flag = false;
                    ClearPool(pool.prefab.name, pool.poolQ.Count - pool.miniSize);
                }
            }
        }
        public ObjectPool FindPool(string prefabName)
        {
            ObjectPool objectPool = pools.Find(pool => pool.prefab.name == prefabName);
            if (objectPool == null)
                Debug.LogWarning("YPools: pools not contain " + prefabName, gameObject);
            return objectPool;
        }
        public void ClearPool(string prefabName, int num)
        {
            ObjectPool objectPool = pools.Find(pool => pool.prefab.name == prefabName);
            if (objectPool == null)
            {
                Debug.LogWarning("YPools: pools not contain " + prefabName, gameObject);
                return;
            }
            for (int i = num; i > 0; i--)
            {
                GameObject obj = objectPool.poolQ.Dequeue();
                Destroy(obj);
            }
        }
        public void RemovePool(string prefabName)
        {
            ObjectPool objectPool = pools.Find(pool => pool.prefab.name == prefabName);
            if (objectPool == null)
            {
                Debug.LogWarning("YPools: pools not contain " + prefabName, gameObject);
                return;
            }
            for (int i = objectPool.poolQ.Count - 1; i > 0; i--)
            {
                GameObject obj = objectPool.poolQ.Dequeue();
                Destroy(obj);
            }
            pools.Remove(objectPool);
        }
        public void NewPool(ObjectPool objectPool)
        {
            if (objectPool.prefab == null)
            {
                Debug.LogWarning("YPools: pool prefab is null");
                return;
            }
            if (pools.Exists(item => item.prefab.name == objectPool.prefab.name))
            {
                Debug.LogWarning("YPools: pools had contain pool:" + objectPool.prefab.name);
                return;
            }
            pools.Add(objectPool);
        }
        public void BackPool(string prefabName, GameObject obj)
        {
            ObjectPool objectPool = pools.Find(pool => pool.prefab.name == prefabName);
            if (objectPool == null)
            {
                Debug.LogWarning("YPools: pools not contain " + prefabName, gameObject);
                return;
            }
            objectPool.poolQ.Enqueue(obj);
            obj.SetActive(false);
        }
        public GameObject Spawn(string prefabName)
        {
            ObjectPool objectPool = pools.Find(pool => pool.prefab.name == prefabName);
            if (objectPool == null)
            {
                Debug.LogWarning("YPools: pools not contain " + prefabName, gameObject);
                return null;
            }
            GameObject obj = null;
            if ((objectPool.poolQ.Count == 0) && (objectPool.prefab != null))
            {
                obj = Instantiate(objectPool.prefab);
                obj.transform.parent = gameObject.transform;
            }
            else
            {
                obj = objectPool.poolQ.Dequeue();
            }
            obj.SetActive(true);
            return obj;
        }
        public List<GameObject> Spawns(string prefabName, int num)
        {
            ObjectPool objectPool = pools.Find(pool => pool.prefab.name == prefabName);
            if (objectPool == null)
            {
                Debug.LogWarning("YPools: pools not contain " + prefabName, gameObject);
                return null;
            }
            List<GameObject> objs = new List<GameObject>();
            GameObject obj = null;
            for (int i = 0; i < num; i++)
            {
                if (objectPool.poolQ.Count == 0)
                {
                    obj = Instantiate(objectPool.prefab);
                    obj.transform.parent = gameObject.transform;
                }
                else
                {
                    obj = objectPool.poolQ.Dequeue();
                }
                obj.SetActive(true);
                objs.Add(obj);
            }
            return objs;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace YPools
{
    [System.Serializable]
    public class ObjectPool
    {
        [TooltipAttribute("tag for index pool")]
        public string tag;
        [TooltipAttribute("seconds time for auto free pools overflow.")]
        public float clearTime = 120;
        public float clrDelay = 0;
        [TooltipAttribute("size for auto free pools overflow.")]
        public int miniSize = 0;
        public GameObject prefab;
        public bool clr_flag = false;
        public Queue<GameObject> pool = new Queue<GameObject>();
    }

    public class ObjectPoolsMgr : MonoBehaviour
    {
        // dict {tag, pool}
        public Dictionary<string, ObjectPool> poolsDict = new Dictionary<string, ObjectPool>();
        public List<ObjectPool> poolsSet = new List<ObjectPool>(); // for ui set,as dict not save
        public static ObjectPoolsMgr instance;
        void Awake()
        {
            instance = this;
            foreach (ObjectPool item in poolsSet)
            {
                poolsDict.Add(item.tag, item);
            }
        }
        // init dict here
        void Start()
        {
            //add pools with code
            foreach (KeyValuePair<string, ObjectPool> kvp in poolsDict)
            {
                for (int i = 0; i < kvp.Value.miniSize; i++)
                {
                    if (kvp.Value.prefab != null)
                    {
                        GameObject obj = GameObject.Instantiate(kvp.Value.prefab);
                        obj.SetActive(false);
                        obj.transform.parent = gameObject.transform;
                        kvp.Value.pool.Enqueue(obj);
                    }
                }
            }
        }
        void FixedUpdate()
        {
            foreach (KeyValuePair<string, ObjectPool> kvp in poolsDict)
            {
                string tag = kvp.Key;
                ObjectPool objectPool = kvp.Value;
                if (objectPool.pool.Count > objectPool.miniSize)
                {
                    objectPool.clr_flag = true;
                    objectPool.clrDelay += Time.deltaTime;
                }
                else
                {
                    objectPool.clr_flag = false;
                    objectPool.clrDelay = 0;
                }
                if ((objectPool.clrDelay > objectPool.clearTime) && (objectPool.clr_flag == true))
                {

                    objectPool.clr_flag = false;
                    ClearPool(tag, objectPool.pool.Count - objectPool.miniSize);
                }
            }
        }
        public void ClearPool(string tag, int num)
        {
            if (!poolsDict.ContainsKey(tag))
            {
                Debug.LogWarning("poolsDict not contain " + tag, gameObject);
                return;
            }
            ObjectPool objectPool = poolsDict[tag];
            for (int i = num; i > 0; i--)
            {
                GameObject obj = objectPool.pool.Dequeue();
                Destroy(obj);
            }
        }
        public void RemovePool(string tag)
        {
            if (!poolsDict.ContainsKey(tag))
            {
                Debug.LogWarning("poolsDict not contain " + tag, gameObject);
                return;
            }
            ObjectPool objectPool = poolsDict[tag];
            for (int i = objectPool.pool.Count - 1; i > 0; i--)
            {
                GameObject obj = objectPool.pool.Dequeue();
                Destroy(obj);
            }
            poolsDict.Remove(tag);
            poolsSet.Remove(poolsSet.Find(item => item.tag == tag));
        }
        public void NewPool(ObjectPool objectPool)
        {
            if (!poolsDict.ContainsKey(tag))
            {
                Debug.LogWarning("poolsDict has contain " + tag, gameObject);
                return;
            }
            poolsDict.Add(objectPool.tag, objectPool);
            poolsSet.Add(objectPool);
        }
        public void BackPool(string tag, GameObject obj)
        {
            if (!poolsDict.ContainsKey(tag))
            {
                Debug.LogWarning("poolsDict not contain " + tag, gameObject);
                return;
            }
            ObjectPool objectPool = poolsDict[tag];
            objectPool.pool.Enqueue(obj);
            obj.SetActive(false);
        }
        public GameObject Spawn(string tag)
        {
            if (!poolsDict.ContainsKey(tag))
            {
                Debug.LogWarning("poolsDict not contain " + tag, gameObject);
                return null;
            }
            ObjectPool objectPool = poolsDict[tag];
            if (objectPool.prefab == null)
            {
                Debug.LogWarning("pools prefab is null ");
                return null;
            }
            GameObject obj = null;
            if ((objectPool.pool.Count == 0) && (objectPool.prefab != null))
            {
                obj = Instantiate(objectPool.prefab);
                obj.transform.parent = gameObject.transform;
            }
            else
            {
                obj = objectPool.pool.Dequeue();
            }
            obj.SetActive(true);
            return obj;
        }
        public List<GameObject> Spawns(string tag, int num)
        {
            if (!poolsDict.ContainsKey(tag))
            {
                Debug.LogWarning("poolsDict not contain " + tag, gameObject);
                return null;
            }
            ObjectPool objectPool = poolsDict[tag];
            if (objectPool.prefab == null)
            {
                Debug.LogWarning("pools prefab is null ");
                return null;
            }
            List<GameObject> objs = new List<GameObject>();
            GameObject obj = null;
            for (int i = 0; i < num; i++)
            {
                if (objectPool.pool.Count == 0)
                {
                    obj = Instantiate(objectPool.prefab);
                    obj.transform.parent = gameObject.transform;
                }
                else
                {
                    obj = objectPool.pool.Dequeue();
                }
                obj.SetActive(true);
                objs.Add(obj);
            }

            return objs;
        }
    }
}
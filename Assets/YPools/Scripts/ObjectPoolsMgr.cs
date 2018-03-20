using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//todo
// 1. use ijob
namespace YPools
{
    public class ObjectPoolsMgr : MonoBehaviour
    {
        // dict {tag, pool}
        public Dictionary<string, ObjectPool> poolsDict = new Dictionary<string, ObjectPool>();
        public List<ObjectPool> poolsSet = new List<ObjectPool>(); // for ui set
        [System.Serializable]
        public class ObjectPool
        {
            public string tag;
            public float clearTime = 120; //second
            public float clrDelay = 0;
            public int miniSize = 0; // mini size for auto free
            public GameObject prefab;
            public bool clr_flag = false;
            public Queue<GameObject> pool = new Queue<GameObject>();
        }

        void Awake()
        {
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
                    GameObject obj = GameObject.Instantiate(kvp.Value.prefab);
                    obj.SetActive(false);
                    obj.transform.parent = gameObject.transform;
                    kvp.Value.pool.Enqueue(obj);
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
        public void ClearPool(string tag)
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
        public GameObject SpawnFromPool(string tag)
        {
            if (!poolsDict.ContainsKey(tag))
            {
                Debug.LogWarning("poolsDict not contain " + tag, gameObject);
                return null;
            }
            ObjectPool objectPool = poolsDict[tag];
            GameObject obj = null;
            if (objectPool.pool.Count == 0)
            {
                obj = Instantiate(objectPool.prefab);
            }
            else
            {
                obj = objectPool.pool.Dequeue();
            }
            obj.SetActive(true);
            return obj;
        }



    }
}
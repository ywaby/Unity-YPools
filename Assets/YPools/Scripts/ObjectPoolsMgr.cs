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
        public List< ObjectPool> poolsSet = new List< ObjectPool>(); // for ui set
        
        // init dict here
        void Start(){
            foreach (ObjectPool item in poolsSet){
                    poolsDict.Add(item.tag,item);
            }
        }
        [System.Serializable]
        public class ObjectPool
        {
            public string tag ;
            public float clearTime = 120; //s
            private float clrDelay = 0;
            public int miniSize = 0; // mini size for auto free
            public GameObject prefab;
            private bool clr_flag = false;
            Queue<GameObject> pool = new Queue<GameObject>();
            void Start()
            {
                for (int i = 0; i < miniSize; i++)
                {
                    GameObject obj = Instantiate(prefab);
                    obj.SetActive(false);
                    pool.Enqueue(obj);

                }
            }
            void FixedUpdate()
            {
                if (pool.Count > miniSize)
                {
                    clr_flag = true;
                    clrDelay += Time.deltaTime;
                }
                else
                {
                    clr_flag = false;
                    clrDelay = 0;

                }
                if ((clrDelay > clearTime) && (clr_flag == true))
                {

                    clr_flag = false;
                    ClearPool(pool.Count - miniSize);
                }

            }

            public void ClearPool(int num)
            {
                for (int i = num; i > 0; i--)
                {
                    GameObject obj = pool.Dequeue();
                    Destroy(obj);
                }
            }
            public void ClearPool()
            {
                for (int i = pool.Count - 1; i > 0; i--)
                {
                    GameObject obj = pool.Dequeue();
                    Destroy(obj);
                }
                pool = null;
            }
            public void BackPool(GameObject obj)
            {
                pool.Enqueue(obj);
                obj.SetActive(false);
            }
            public GameObject SpawnFromPool()
            {
                GameObject obj = null;
                if (pool.Count == 0)
                {
                    obj = Instantiate(prefab);
                }
                else
                {
                    obj = pool.Dequeue();
                }
                obj.SetActive(true);
                return obj;
            }
        }

    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using YPools;

public class ListView : MonoBehaviour
{

    // Use this for initialization
	public GameObject itemPrefab;
	public int scale = 10;
    private ObjectPoolsMgr poolsMgr;
    void Start()
    {
        poolsMgr = ObjectPoolsMgr.instance;
        foreach (KeyValuePair<string, ObjectPool> kvp in poolsMgr.poolsDict)
        {
			GameObject item = Instantiate(itemPrefab);
            item.transform.SetParent(transform);
			ListItem itemCtl= item.GetComponent<ListItem>();
			itemCtl.refPool = kvp.Value;
			itemCtl.listview = this;
            
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using YPools;

public class ListView : MonoBehaviour
{

    // Use this for initialization
	public GameObject listViewItemPrefab;
	public int scale = 10;
    private ObjectPoolsMgr poolsMgr;
    void Start()
    {
        poolsMgr = ObjectPoolsMgr.instance;
        foreach (var pool in poolsMgr.pools)
        {
			GameObject listViewItem = Instantiate(listViewItemPrefab);
            listViewItem.transform.SetParent(transform);
			ListViewItem listItemCls= listViewItem.GetComponent<ListViewItem>();
			listItemCls.refPool = pool;
			listItemCls.listview = this;
            
        }
    }
}

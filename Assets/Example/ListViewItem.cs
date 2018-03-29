using UnityEngine;
using UnityEngine.UI;
using YPools;
using System.Collections.Generic;

public class ListViewItem : MonoBehaviour
{

    [HideInInspector]
    public ObjectPool refPool;
    [HideInInspector]    
    public ListView listview;
    private InputField spwanNumInput;
    private ObjectPoolsMgr poolsMgr;

    void Start()
    {
        spwanNumInput = gameObject.GetComponentInChildren<InputField>();
        Button btn_sprawm = gameObject.GetComponentInChildren<Button>();
        btn_sprawm.onClick.AddListener(OnBtnSpawn);
        poolsMgr = ObjectPoolsMgr.instance;
        Text tag = transform.Find("tag").GetComponent<Text>();
        tag.text = refPool.prefab.name;
    }
    void OnBtnSpawn()
    {
        poolsMgr.Spawns(refPool.prefab.name, int.Parse(spwanNumInput.text));
    }
}

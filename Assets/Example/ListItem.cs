using UnityEngine;
using UnityEngine.UI;
using YPools;

public class ListItem : MonoBehaviour
{

    // Use this for initialization
    [HideInInspector]
    public ObjectPool refPool;
    [HideInInspector]    
    public ListView listview;
    private int outsidePoolNum = 0;//todo
    private InputField spwanNumInput;
    private ObjectPoolsMgr poolsMgr;

    void Start()
    {
        spwanNumInput = gameObject.GetComponentInChildren<InputField>();
        Button btn_sprawm = gameObject.GetComponentInChildren<Button>();
        btn_sprawm.onClick.AddListener(OnBtnSpawn);
        poolsMgr = ObjectPoolsMgr.instance;
        Text tag = transform.Find("tag").GetComponent<Text>();
        tag.text = refPool.tag;
    }
    void OnBtnSpawn()
    {
        poolsMgr.Spawns(refPool.tag, int.Parse(spwanNumInput.text));
    }
}

using UnityEngine;
using System.Collections;
using YPools;

class PrefabInPool : MonoBehaviour
{
    [Tooltip("secend")]
    public float autoDestoryTime = 20;
    public float upforce = 1f;
    public float sideforce = -1f;
    private ObjectPoolsMgr poolsMgr;
    private string tag;
    void Start()
    {
        poolsMgr = ObjectPoolsMgr.instance;
        transform.position = new Vector3(0, 0, 0);
        float xforce = Random.Range(-sideforce, sideforce);
        float yforce = Random.Range(upforce / 2f, upforce);
        float zforce = Random.Range(-sideforce, sideforce);
        Vector3 force = new Vector3(xforce, yforce, zforce);
        GetComponent<Rigidbody>().velocity = force;
        StartCoroutine(AutoDestory());

    }


    IEnumerator AutoDestory()
    {
        yield return new WaitForSeconds(autoDestoryTime);
        poolsMgr.BackPool(tag, gameObject);
    }

}
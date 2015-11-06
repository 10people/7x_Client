using UnityEngine;
using System.Collections;

public class EnterFriend : MonoBehaviour {

    public GameObject m_LoadGameobject;
	void Start () 
    {
	
	}
    void OnClick()
    { 
         WWW p_www = null;
         BaiZhanLoadCallback(ref p_www, "", m_LoadGameobject);
    }
    public void BaiZhanLoadCallback(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject baizhanRoot = Instantiate(p_object) as GameObject;

        baizhanRoot.SetActive(true);

        baizhanRoot.name = "BaiZhan";

        baizhanRoot.transform.localPosition = new Vector3(0, 800, 0);

        baizhanRoot.transform.localScale = Vector3.one;
    }
}

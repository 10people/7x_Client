using UnityEngine;
using System.Collections;

public class EnterWorship : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

    void OnClick()
    {
        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.WORSHIP_MAIN_LAYER),
                                           LoadCallback);
    }

    public void LoadCallback(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject tempObject = (GameObject)Instantiate(p_object);

        tempObject.transform.localPosition = new Vector3(100, 100, 0);

        tempObject.transform.localScale = Vector3.one;
    }
}

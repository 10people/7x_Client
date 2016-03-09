using UnityEngine;
using System.Collections;

public class EnterHuangyebtn : MonoBehaviour {

	//public GameObject HuangYe;

	public static EnterHuangyebtn HYMapData;
	public static EnterHuangyebtn Instance()
	{
		if (!HYMapData)
		{
			HYMapData = (EnterHuangyebtn)GameObject.FindObjectOfType (typeof(EnterHuangyebtn));
		}
		
		return HYMapData;
	}

	void Start () {
	
	}

	void OnDestroy(){
		HYMapData = null;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void loodBoxBack(ref WWW p_www,string p_path, Object p_object)
	{

		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		uibox.setBox("等级不足",null, "联盟等级不足，请先升级联盟等级",null,"确定",null,null,null,null);
	}
	void loodBoxBack2(ref WWW p_www,string p_path, Object p_object)
	{
		
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		uibox.setBox("提示",null, "请先加入一个联盟才能进入荒野求生！",null,"确定",null,null,null,null);
	}
	private GameObject HY_Map;
	void LoadHY_Map(ref WWW p_www,string p_path, Object p_object)
	{
		if(HY_Map)
		{
			return;
		}
		HY_Map = Instantiate (p_object) as GameObject;
		//WildnessManager mWildnessManager = mobg.GetComponent<WildnessManager>();
		HY_Map.transform.localPosition = new Vector3 (-200, 200, 0);
		HY_Map.transform.localScale = Vector3.one;
        MainCityUI.TryAddToObjectList(HY_Map);
		HY_UIManager mWildnessManager = HY_Map.GetComponent<HY_UIManager>(); 
		mWildnessManager.init ();
	}
//	void OnClick()
//	{
//
//		InitHYMap ();
//	}
	public void InitHYMap()
	{
		if(AllianceData.Instance.g_UnionInfo == null)
		{
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),loodBoxBack2);
			return;
		}
		if(AllianceData.Instance.g_UnionInfo.level < 2)
		{
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),loodBoxBack);
			return;
		}
		Debug.Log ("弹出UI。 荒野的");
		
		Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.HY_MAP ),LoadHY_Map);

	}
}
